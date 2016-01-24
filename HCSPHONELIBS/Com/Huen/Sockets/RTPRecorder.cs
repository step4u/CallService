using System;
using System.Collections.Generic;
using System.Linq;

using Com.Huen.Libs;
using Com.Huen.DataModel;

using System.IO;
using NAudio.Wave;

namespace Com.Huen.Sockets
{
    public class RTPRecorder
    {
        private HUDPClient client = null;

        private WaveFormat mulawFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.MuLaw, 8000, 1, 8000, 1, 8);
        private WaveFormat pcmFormat = new WaveFormat(8000, 16, 1);

        private string seqnum = string.Empty;
        private List<RcvData> rcvqueList = new List<RcvData>();
        private List<RecInfos> lExtension0 = new List<RecInfos>();
        private List<RecInfos> lExtension1 = new List<RecInfos>();

        public RTPRecorder()
            : this(MsgKinds.RecordInfo, 21010)
        {
        }

        public RTPRecorder(MsgKinds msgkinds, int port)
        {
            client = new HUDPClient();
            client.UDPClientEventReceiveMessage += client_UDPClientEventReceiveMessage;

            client.SocketMsgKinds = msgkinds;
            client.ServerPort = port;
            client.StartServer();
        }

        public void StartServer()
        {
            client.StartServer();
        }

        public void StopServer()
        {
            client.Stop();
        }

        void client_UDPClientEventReceiveMessage(object sender, object e)
        {
            switch (client.SocketMsgKinds)
            {
                case MsgKinds.CdrRequest:
                    CdrRequest_t cdr_req = (CdrRequest_t)e;
                    CdrList cdrlist = (CdrList)util.GetObject<CdrList>(cdr_req.data);

                    HUDPClient cl = (HUDPClient)sender;
                    cl.Send(2, MsgKinds.CdrResponse, cdr_req);
                    break;
                case MsgKinds.RecordInfo:
                    RecordInfo_t recInfo = (RecordInfo_t)e;
                    //if (recInfo.isExtension == 0)
                    //    return;

                    //string filename = string.Empty;
                    //byte[] voiceSrc = recInfo.voice;
                    int nDataSize = recInfo.size - 12;

                    if (nDataSize != 80 && nDataSize != 160 && nDataSize != 240 && nDataSize != -12) break;

                    //this.Rtp2Wav(recInfo, nDataSize);
                    this.Rtp2Wav2(recInfo, nDataSize);
                    //this.Rtp2Binary(recInfo, nDataSize);
                    break;
            }
        }

        private void DoItFromSocketMessage(object sender, object e)
        {
            switch (client.SocketMsgKinds)
            {
                case MsgKinds.CdrRequest:
                    CdrRequest_t cdr_req = (CdrRequest_t)e;
                    CdrList cdrlist = (CdrList)util.GetObject<CdrList>(cdr_req.data);

                    HUDPClient cl = (HUDPClient)sender;
                    cl.Send(2, MsgKinds.CdrResponse, cdr_req);
                    break;
                case MsgKinds.RecordInfo:
                    RecordInfo_t recInfo = (RecordInfo_t)e;
                    //if (recInfo.isExtension == 0)
                    //    return;

                    //string filename = string.Empty;
                    //byte[] voiceSrc = recInfo.voice;
                    int nDataSize = recInfo.size - 12;

                    if (nDataSize != 80 && nDataSize != 160 && nDataSize != 240 && nDataSize != -12) break;

                    //this.Rtp2Wav(recInfo, nDataSize);
                    this.Rtp2Wav2(recInfo, nDataSize);
                    //this.Rtp2Binary(recInfo, nDataSize);
                    break;
            }
        }

        public void Rtp2Wav2(RecordInfo_t recordInfo, int dataSize)
        {
            string fn = string.Empty;

            RcvData ingData = null;
            ingData = rcvqueList.Find(
                delegate(RcvData qlist)
                {
                    return qlist.extension == recordInfo.extension && qlist.peernumber == recordInfo.peer_number;
                });

            if (ingData == null)
            {
                DateTime now = DateTime.Now;
                int yyyy = now.Year;
                int mm = now.Month;
                int dd = now.Day;
                int hh = now.Hour;
                int m = now.Minute;
                int sec = now.Second;
                int ms = now.Millisecond;

                seqnum = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:000}", yyyy, mm, dd, hh, m, sec, ms);

                ingData = new RcvData() { extension = recordInfo.extension, peernumber = recordInfo.peer_number, seqnum = seqnum };
                rcvqueList.Add(ingData);
            }

            if (dataSize == -12)
            {
                // MP3로 저장할 경우 이 곳에서 마지막에 wav를 mp3로 변환

                ProcessMixing2(ingData, dataSize);

                lock (rcvqueList)
                {
                    rcvqueList.Remove(ingData);
                }

                return;
            }

            RecInfos recinfo = new RecInfos()
            {
                rcvData = ingData
                ,
                isExtension = recordInfo.isExtension
                ,
                seq = recordInfo.seq
                ,
                size = recordInfo.size
                ,
                voice = recordInfo.voice
            };

            if (recordInfo.isExtension == 0)
            {
                lock (lExtension0)
                {
                    lExtension0.Add(recinfo);
                }
            }

            if (recordInfo.isExtension == 1)
            {
                lock (lExtension1)
                {
                    lExtension1.Add(recinfo);
                }
            }

            int list0 = lExtension0.Count(
                    delegate(RecInfos list)
                    {
                        return list.rcvData.Equals(ingData) && list.isExtension == 0;
                    });

            int list1 = lExtension1.Count(
                    delegate(RecInfos list)
                    {
                        return list.rcvData.Equals(ingData) && list.isExtension == 1;
                    });

            if (list0 >= 20 && list1 >= 20)
            {
                ProcessMixing2(ingData, dataSize);
            }
        }

        private void ProcessMixing(RcvData data, int dataSize)
        {
            string processingFn = string.Format("e:\\{0}_{1}_{2}.wav", data.seqnum, data.extension, data.peernumber);

            List<RecInfos> ls0 = lExtension0.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 0;
                        });

            List<RecInfos> ls1 = lExtension1.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 1;
                        });

            IsExtensionComparer isExtensionCompare = new IsExtensionComparer();
            ls0.Sort(isExtensionCompare);
            ls1.Sort(isExtensionCompare);

            int count = 0;
            int count0 = ls0.Count();
            int count1 = ls1.Count();

            if (count0 - count1 < 0)
                count = count0;
            else
                count = count1;

            for (int i = 0; i < count; i++)
            {
                if (ls0[i].seq == ls1[i].seq)
                {
                    // 믹싱

                    byte[] wavSrc0 = new byte[dataSize];
                    byte[] wavSrc1 = new byte[dataSize];

                    Array.Copy(ls0[i].voice, 12, wavSrc0, 0, wavSrc0.Length);
                    Array.Copy(ls1[i].voice, 12, wavSrc1, 0, wavSrc1.Length);

                    WaveMixerStream32 mixer = new WaveMixerStream32();
                    //mixer.AutoStop = true;

                    WaveChannel32 channelStm = null;

                    for (int j = 0; j < 2; j++)
                    {
                        MemoryStream memStm = null;
                        BufferedStream bufStm = null;
                        RawSourceWaveStream rawSrcStm = null;
                        WaveFormatConversionStream conversionStm = null;

                        if (j == 0)
                            memStm = new MemoryStream(wavSrc0);
                        else
                            memStm = new MemoryStream(wavSrc1);

                        bufStm = new BufferedStream(memStm);
                        rawSrcStm = new RawSourceWaveStream(bufStm, mulawFormat);
                        conversionStm = new WaveFormatConversionStream(pcmFormat, rawSrcStm);

                        channelStm = new WaveChannel32(conversionStm);
                        mixer.AddInputStream(channelStm);
                    }
                    mixer.Position = 0;

                    if (File.Exists(processingFn))
                    {
                        var wavefilestream = new WaveFileReader(processingFn);
                        byte[] wavefilebyte = new byte[(int)wavefilestream.Length];
                        int chk0 = wavefilestream.Read(wavefilebyte, 0, wavefilebyte.Length);

                        Wave32To16Stream to16 = new Wave32To16Stream(mixer);
                        var conversionStm = new WaveFormatConversionStream(pcmFormat, to16);
                        byte[] tobyte = new byte[(int)conversionStm.Length];
                        int chk1 = conversionStm.Read(tobyte, 0, (int)conversionStm.Length);

                        byte[] desByte = new byte[wavefilebyte.Length + tobyte.Length];

                        conversionStm.Close();
                        wavefilestream.Close();

                        Buffer.BlockCopy(wavefilebyte, 0, desByte, 0, wavefilebyte.Length);
                        Buffer.BlockCopy(tobyte, 0, desByte, wavefilebyte.Length, tobyte.Length);

                        using (MemoryStream memStm = new MemoryStream(desByte))
                        using (RawSourceWaveStream rawSrcStm = new RawSourceWaveStream(memStm, pcmFormat))
                        {
                            WaveFileWriter.CreateWaveFile(processingFn, rawSrcStm);
                        }
                    }
                    else
                    {
                        var mixedStm = new Wave32To16Stream(mixer);
                        var convStm = new WaveFormatConversionStream(pcmFormat, mixedStm);
                        WaveFileWriter.CreateWaveFile(processingFn, convStm);
                        convStm.Close();
                        mixedStm.Close();
                    }

                    mixer.Close();

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    lExtension1.Remove(ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq < 0)
                {
                    // ls0 만 믹싱
                    // ls0 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    if (File.Exists(processingFn))
                    {
                        //wavefilestream = new WaveFileReader(processingFn);
                    }
                    else
                    {

                    }

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    ls1.Insert(i + 1, ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq > 0)
                {
                    // ls1 만 믹싱
                    // ls1 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    if (File.Exists(processingFn))
                    {
                        //wavefilestream = new WaveFileReader(processingFn);
                    }
                    else
                    {

                    }

                    // 삭제
                    lExtension1.Remove(ls1[i]);
                    ls0.Insert(i + 1, ls0[i]);
                }
            }

            // isExtension 으로 소트 >> 10개의 버퍼링 리스트 seq 비교해가면서 스트림 믹스 > WaveFileWriter 로 파일 저장
        }

        private void ProcessMixing2(RcvData data, int dataSize)
        {
            string processingFn = string.Format("d:\\{0}_{1}_{2}.wav", data.seqnum, data.extension, data.peernumber);

            List<RecInfos> ls0 = lExtension0.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 0;
                        });

            List<RecInfos> ls1 = lExtension1.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 1;
                        });

            IsExtensionComparer isExtensionCompare = new IsExtensionComparer();
            ls0.Sort(isExtensionCompare);
            ls1.Sort(isExtensionCompare);

            int count = 0;
            int count0 = ls0.Count();
            int count1 = ls1.Count();

            if (count0 - count1 < 0)
                count = count0;
            else
                count = count1;

            byte[] buffWriting = new byte[320 * count];

            for (int i = 0; i < count; i++)
            {
                if (ls0[i].seq == ls1[i].seq)
                {
                    // 믹싱
                    // 코덱 종류에 따라서 바이트 길이는 달라질 수 있다. 실제로 만들 때 경우의 수 확인하고 만들어야 한다.
                    byte[] wavSrc0 = new byte[160];
                    byte[] wavSrc1 = new byte[160];

                    Array.Copy(ls0[i].voice, 12, wavSrc0, 0, wavSrc0.Length);
                    Array.Copy(ls1[i].voice, 12, wavSrc1, 0, wavSrc1.Length);

                    WaveMixerStream32 mixer = new WaveMixerStream32();
                    //mixer.AutoStop = true;

                    WaveChannel32 channelStm = null;

                    MemoryStream memStm = null;
                    BufferedStream bufStm = null;
                    RawSourceWaveStream rawSrcStm = null;
                    WaveFormatConversionStream conversionStm = null;

                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                            memStm = new MemoryStream(wavSrc0);
                        else
                            memStm = new MemoryStream(wavSrc1);

                        bufStm = new BufferedStream(memStm);
                        rawSrcStm = new RawSourceWaveStream(bufStm, mulawFormat);
                        conversionStm = new WaveFormatConversionStream(pcmFormat, rawSrcStm);

                        channelStm = new WaveChannel32(conversionStm);
                        mixer.AddInputStream(channelStm);
                    }
                    mixer.Position = 0;

                    Wave32To16Stream to16 = new Wave32To16Stream(mixer);
                    var convStm = new WaveFormatConversionStream(pcmFormat, to16);
                    byte[] tobyte = new byte[(int)convStm.Length];
                    int chk = convStm.Read(tobyte, 0, (int)convStm.Length);
                    Buffer.BlockCopy(tobyte, 0, buffWriting, i * tobyte.Length, tobyte.Length);

                    conversionStm.Close();
                    rawSrcStm.Close();
                    bufStm.Close();
                    memStm.Close();

                    convStm.Close();
                    to16.Close();
                    channelStm.Close();
                    mixer.Close();

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    lExtension1.Remove(ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq < 0)
                {
                    // ls0 만 믹싱
                    // ls0 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    // 믹싱
                    // 코덱 종류에 따라서 바이트 길이는 달라질 수 있다. 실제로 만들 때 경우의 수 확인하고 만들어야 한다.
                    byte[] wavSrc0 = new byte[160];
                    byte[] wavSrc1 = new byte[160];

                    Array.Copy(ls0[i].voice, 12, wavSrc0, 0, wavSrc0.Length);
                    Array.Copy(ls1[i].voice, 12, wavSrc1, 0, wavSrc1.Length);

                    WaveMixerStream32 mixer = new WaveMixerStream32();
                    //mixer.AutoStop = true;

                    WaveChannel32 channelStm = null;

                    MemoryStream memStm = null;
                    BufferedStream bufStm = null;
                    RawSourceWaveStream rawSrcStm = null;
                    WaveFormatConversionStream conversionStm = null;

                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                            memStm = new MemoryStream(wavSrc0);
                        else
                            memStm = new MemoryStream(wavSrc1);

                        bufStm = new BufferedStream(memStm);
                        rawSrcStm = new RawSourceWaveStream(bufStm, mulawFormat);
                        conversionStm = new WaveFormatConversionStream(pcmFormat, rawSrcStm);

                        channelStm = new WaveChannel32(conversionStm);
                        mixer.AddInputStream(channelStm);
                    }
                    mixer.Position = 0;

                    Wave32To16Stream to16 = new Wave32To16Stream(mixer);
                    var convStm = new WaveFormatConversionStream(pcmFormat, to16);
                    byte[] tobyte = new byte[(int)convStm.Length];
                    int chk = convStm.Read(tobyte, 0, (int)convStm.Length);
                    Buffer.BlockCopy(tobyte, 0, buffWriting, i * tobyte.Length, tobyte.Length);

                    conversionStm.Close();
                    rawSrcStm.Close();
                    bufStm.Close();
                    memStm.Close();

                    convStm.Close();
                    to16.Close();
                    channelStm.Close();
                    mixer.Close();

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    ls1.Insert(i + 1, ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq > 0)
                {
                    // ls1 만 믹싱
                    // ls1 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    // 믹싱
                    // 코덱 종류에 따라서 바이트 길이는 달라질 수 있다. 실제로 만들 때 경우의 수 확인하고 만들어야 한다.
                    byte[] wavSrc0 = new byte[160];
                    byte[] wavSrc1 = new byte[160];

                    Array.Copy(ls0[i].voice, 12, wavSrc0, 0, wavSrc0.Length);
                    Array.Copy(ls1[i].voice, 12, wavSrc1, 0, wavSrc1.Length);

                    WaveMixerStream32 mixer = new WaveMixerStream32();
                    //mixer.AutoStop = true;

                    WaveChannel32 channelStm = null;

                    MemoryStream memStm = null;
                    BufferedStream bufStm = null;
                    RawSourceWaveStream rawSrcStm = null;
                    WaveFormatConversionStream conversionStm = null;

                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                            memStm = new MemoryStream(wavSrc0);
                        else
                            memStm = new MemoryStream(wavSrc1);

                        bufStm = new BufferedStream(memStm);
                        rawSrcStm = new RawSourceWaveStream(bufStm, mulawFormat);
                        conversionStm = new WaveFormatConversionStream(pcmFormat, rawSrcStm);

                        channelStm = new WaveChannel32(conversionStm);
                        mixer.AddInputStream(channelStm);
                    }
                    mixer.Position = 0;

                    Wave32To16Stream to16 = new Wave32To16Stream(mixer);
                    var convStm = new WaveFormatConversionStream(pcmFormat, to16);
                    byte[] tobyte = new byte[(int)convStm.Length];
                    int chk = convStm.Read(tobyte, 0, (int)convStm.Length);
                    Buffer.BlockCopy(tobyte, 0, buffWriting, i * tobyte.Length, tobyte.Length);

                    conversionStm.Close();
                    rawSrcStm.Close();
                    bufStm.Close();
                    memStm.Close();

                    convStm.Close();
                    to16.Close();
                    channelStm.Close();
                    mixer.Close();

                    // 삭제
                    lExtension1.Remove(ls1[i]);
                    ls0.Insert(i + 1, ls0[i]);
                }
            }

            // 10개의 버프를 바이트로 만들어 WaveFileWrite
            WaveFileWriting(buffWriting, processingFn);
        }

        private void WaveFileWriting(byte[] buff, string fn)
        {
            if (buff.Length < 1) return;

            if (File.Exists(fn))
            {
                var wavefilestream = new WaveFileReader(fn);
                byte[] wavefilebyte = new byte[(int)wavefilestream.Length];
                int chk = wavefilestream.Read(wavefilebyte, 0, wavefilebyte.Length);
                wavefilestream.Close();

                byte[] desByte = new byte[wavefilebyte.Length + buff.Length];

                Buffer.BlockCopy(wavefilebyte, 0, desByte, 0, wavefilebyte.Length);
                Buffer.BlockCopy(buff, 0, desByte, wavefilebyte.Length, buff.Length);

                using (MemoryStream memStm = new MemoryStream(desByte))
                using (BufferedStream bufStm = new BufferedStream(memStm, 2048))
                using (RawSourceWaveStream rawSrcStm = new RawSourceWaveStream(bufStm, pcmFormat))
                {
                    WaveFileWriter.CreateWaveFile(fn, rawSrcStm);
                }
            }
            else
            {
                using (MemoryStream memStm = new MemoryStream(buff))
                using (BufferedStream bufStm = new BufferedStream(memStm, 2048))
                using (RawSourceWaveStream rawSrcStm = new RawSourceWaveStream(bufStm, pcmFormat))
                {
                    WaveFileWriter.CreateWaveFile(fn, rawSrcStm);
                }
            }
        }

        private void ProcessMixingFinal(RcvData data, int dataSize)
        {
            string processingFn = string.Format("e:\\{0}_{1}_{2}.wav", data.seqnum, data.extension, data.peernumber);

            List<RecInfos> ls0 = lExtension0.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 0;
                        });

            List<RecInfos> ls1 = lExtension1.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 1;
                        });

            IsExtensionComparer isExtensionCompare = new IsExtensionComparer();
            ls0.Sort(isExtensionCompare);
            ls1.Sort(isExtensionCompare);

            int count = 0;
            int count0 = ls0.Count();
            int count1 = ls1.Count();

            if (count0 - count1 < 0)
                count = count0;
            else
                count = count1;

            for (int i = 0; i < count; i++)
            {
                if (ls0[i].seq == ls1[i].seq)
                {
                    // 믹싱

                    byte[] wavSrc0 = new byte[160];
                    byte[] wavSrc1 = new byte[160];

                    Array.Copy(ls0[i].voice, 12, wavSrc0, 0, wavSrc0.Length);
                    Array.Copy(ls1[i].voice, 12, wavSrc1, 0, wavSrc1.Length);

                    WaveMixerStream32 mixer = new WaveMixerStream32();
                    //mixer.AutoStop = true;

                    WaveChannel32 channelStm = null;

                    for (int j = 0; j < 2; j++)
                    {
                        MemoryStream memStm = null;
                        BufferedStream bufStm = null;
                        RawSourceWaveStream rawSrcStm = null;
                        WaveFormatConversionStream conversionStm = null;

                        if (j == 0)
                            memStm = new MemoryStream(wavSrc0);
                        else
                            memStm = new MemoryStream(wavSrc1);

                        bufStm = new BufferedStream(memStm);
                        rawSrcStm = new RawSourceWaveStream(bufStm, mulawFormat);
                        conversionStm = new WaveFormatConversionStream(pcmFormat, rawSrcStm);

                        channelStm = new WaveChannel32(conversionStm);
                        mixer.AddInputStream(channelStm);
                    }
                    mixer.Position = 0;

                    if (File.Exists(processingFn))
                    {
                        var wavefilestream = new WaveFileReader(processingFn);
                        byte[] wavefilebyte = new byte[(int)wavefilestream.Length];
                        int chk0 = wavefilestream.Read(wavefilebyte, 0, wavefilebyte.Length);

                        Wave32To16Stream to16 = new Wave32To16Stream(mixer);
                        var conversionStm = new WaveFormatConversionStream(pcmFormat, to16);
                        byte[] tobyte = new byte[(int)conversionStm.Length];
                        int chk1 = conversionStm.Read(tobyte, 0, (int)conversionStm.Length);

                        byte[] desByte = new byte[wavefilebyte.Length + tobyte.Length];

                        conversionStm.Close();
                        wavefilestream.Close();

                        Buffer.BlockCopy(wavefilebyte, 0, desByte, 0, wavefilebyte.Length);
                        Buffer.BlockCopy(tobyte, 0, desByte, wavefilebyte.Length, tobyte.Length);

                        using (MemoryStream memStm = new MemoryStream(desByte))
                        using (BufferedStream buffStm = new BufferedStream(memStm))
                        using (RawSourceWaveStream rawSrcStm = new RawSourceWaveStream(buffStm, pcmFormat))
                        {
                            WaveFileWriter.CreateWaveFile(processingFn, rawSrcStm);
                        }
                    }
                    else
                    {
                        var mixedStm = new Wave32To16Stream(mixer);
                        var convStm = new WaveFormatConversionStream(pcmFormat, mixedStm);
                        WaveFileWriter.CreateWaveFile(processingFn, convStm);
                        convStm.Close();
                        mixedStm.Close();
                    }

                    mixer.Close();

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    lExtension1.Remove(ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq < 0)
                {
                    // ls0 만 믹싱
                    // ls0 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    if (File.Exists(processingFn))
                    {
                        //wavefilestream = new WaveFileReader(processingFn);
                    }
                    else
                    {

                    }

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    ls1.Insert(i + 1, ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq > 0)
                {
                    // ls1 만 믹싱
                    // ls1 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    if (File.Exists(processingFn))
                    {
                        //wavefilestream = new WaveFileReader(processingFn);
                    }
                    else
                    {

                    }

                    // 삭제
                    lExtension1.Remove(ls1[i]);
                    ls0.Insert(i + 1, ls0[i]);
                }
            }
        }
    }
}
