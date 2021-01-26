using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSY.Interface.Aft
{
    /// <summary>
    /// Backend for the primary interface for the AI System
    /// </summary>
    public class AIInterface : INotifyPropertyChanged
    {
        /// <summary>
        /// Event on Property Changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        private bool _isInitializing;
        public bool IsInitializing
        {
            get
            {
                return _isInitializing;
            }
            set
            {
                if (_isInitializing == value)
                    return;
                _isInitializing = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsInitializing)));
            }
        }

        private bool _isInitialized;
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            private set
            {
                if (_isInitialized == value)
                    return;
                _isInitialized = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsInitialized)));
            }
        }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AIInterface()
        {
            _isInitializing = false;
            _isInitialized = false;
        }
    }
}
