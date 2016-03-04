using System;

using System.Data;
using System.Data.SqlClient;

namespace Com.Huen.Sql
{
    public class MSDBHelper : IDisposable
    {
        private SqlConnection conn = null;
        private SqlCommand cmd = null;
        private SqlDataAdapter adapter = null;

        private DataSet ds = null;
        private DataTable dt = null;

        private object objResult = null;
        private int iResult = 0;

        #region 속성
        private string strSql = string.Empty;
        private string strConn = string.Empty;
        private CommandType commandType = CommandType.Text;

        public string Sql
        {
            get
            {
                return strSql;
            }
            set
            {
                strSql = value;
            }
        }

        
        public string Conn
        {
            get
            {
                return strConn;
            }
            set
            {
                strConn = value;
            }
        }

        public CommandType CommandType
        {
            get
            {
                return commandType;
            }
            set
            {
                commandType = value;
            }
        }
        #endregion 속성

        public MSDBHelper() { }

        public MSDBHelper(string conn) : this(string.Empty, conn, CommandType.Text) { }

        public MSDBHelper(string sql, string conn) : this(sql, conn, CommandType.Text) { }

        public MSDBHelper(string sql, string conn, CommandType cmdtype)
        {
            Sql = sql;
            Conn = conn;
            CommandType = cmdtype;

            // this.Initiate();
        }

        //private void Initiate()
        //{
        //    conn = new SqlConnection(Conn);
        //    conn.Open();

        //    cmd = new SqlCommand();
        //    cmd.CommandType = CommandType;
        //    cmd.CommandText = Sql;
        //    cmd.Connection = conn;
        //}

        public void Open()
        {
            conn = new SqlConnection(Conn);
            conn.Open();

            cmd = new SqlCommand();
            cmd.CommandType = CommandType;
            cmd.CommandText = Sql;
            cmd.Connection = conn;
        }

        public DataSet GetDataSet()
        {
            ds = new DataSet();
            try
            {
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public DataTable GetDataTable()
        {
            dt = new DataTable();
            try
            {
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public object GetData()
        {
            try
            {
                objResult = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objResult;
        }

        public int GetEffectedCount()
        {
            try
            {
                iResult = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return iResult;
        }

        public void BeginTran()
        {
            cmd.Transaction = conn.BeginTransaction();
        }

        public void Commit()
        {
            cmd.Transaction.Commit();
        }

        public void Rollback()
        {
            cmd.Transaction.Rollback();
        }

        public void Dispose()
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            conn = null;
            cmd = null;
            ds = null;
            adapter = null;
            objResult = null;
            iResult = 0;
        }

        public void ExcuteSP(string procname, DataTable variables)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;

            foreach (DataRow dr in variables.Rows)
            {
                SqlParameter sqlParam = new SqlParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            cmd.ExecuteNonQuery();
        }

        // 프로시저 실행 결과 select 리턴
        public DataTable GetDataTableSP(string procname, DataTable variables)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;

            foreach (DataRow dr in variables.Rows)
            {
                SqlParameter sqlParam = new SqlParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            dt = this.GetDataTable();

            return dt;
        }

        // 프로시저 실행 결과 select 리턴
        public DataTable GetDataTableSP(string procname)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;
            dt = this.GetDataTable();
            return dt;
        }

        // 프로시저 실행 결과 select 리턴
        public Object GetDataSP(string procname)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;
            return this.GetData();
        }

        // 프로시저 실행 결과 select 리턴
        public Object GetDataSP(string procname, DataTable variables)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;

            foreach (DataRow dr in variables.Rows)
            {
                SqlParameter sqlParam = new SqlParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            return this.GetData();
        }
    }
}
