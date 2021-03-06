﻿using System;
using System.Linq;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Com.Huen.DataModel
{
    public class CustGroupViewModel : INotifyPropertyChanged
    {
        #region Data

        readonly ObservableCollection<CustGroupViewModel> _children;
        readonly CustGroupViewModel _parent;
        readonly CustGroup _custgroup;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public CustGroupViewModel(CustGroup custgroup)
            : this(custgroup, null)
        {
        }

        private CustGroupViewModel(CustGroup custgroup, CustGroupViewModel parent)
        {
            _custgroup = custgroup;
            _parent = parent;

            _children = new ObservableCollection<CustGroupViewModel>(
                    (from child in _custgroup.Children
                     select new CustGroupViewModel(child, this))
                     .ToList<CustGroupViewModel>());
        }

        #endregion // Constructors

        #region Person Properties

        public ObservableCollection<CustGroupViewModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _custgroup.Name; }
        }

        public string Idx
        {
            get { return _custgroup.Idx; }
        }

        #endregion // Person Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public CustGroupViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
