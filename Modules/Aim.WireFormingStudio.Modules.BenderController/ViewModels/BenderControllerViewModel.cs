namespace Aim.WireFormingStudio.Modules.BenderController.ViewModels
{
    #region Using Directives -------------------------------------------------------------------------------------------------------

    using System;
    using System.Windows.Media.Imaging;
    using System.Collections.ObjectModel;

    using Prism;
    using Prism.Events;
    using Prism.Commands;
       
    using Languages;
    using Core.PrismEx;
    using Core.ViewModels;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// 
    /// </summary>
    public class BenderControllerViewModel : ViewModelBase, IActiveAware, IDockAware, IDisposable
    {
        #region Member Variables ---------------------------------------------------------------------------------------------------------------------      

        /// <summary>
        /// 
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Content pane title
        /// </summary>
        private string _header;

        /// <summary>
        /// Content pane image
        /// </summary>
        private BitmapImage _image;

        /// <summary>
        /// 
        /// </summary>
        private bool _isActive;

        /// <summary>
        /// Is this object disposed
        /// </summary>
        private bool _disposed;
        
        #endregion Member Variables ------------------------------------------------------------------------------------------------------------------

        #region Constructors -------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventAggregator"></param>
        public BenderControllerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            ViewTitle = WireFormingStudioStrings.BenderController_View_Title;
            Header = WireFormingStudioStrings.BenderController_TabView_Header;
            var imageUri = new Uri("pack://application:,,,/Aim.WireFormingStudio.Modules.BenderController;component/Resources/Images/16x16/BenderController.png");
            Image = new BitmapImage(imageUri);            
        }


        /// <summary>
        /// Destructor
        /// </summary>
        ~BenderControllerViewModel()
        {
            Dispose(false);
        }

        #endregion Constructors ----------------------------------------------------------------------------------------------------------------------

        #region Public Properties --------------------------------------------------------------------------------------------------------------------
        
        #endregion Public Properties -----------------------------------------------------------------------------------------------------------------

        #region ViewModel Override Functions ---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string OnValidate(string propertyName)
        {
            return base.OnValidate(propertyName);
        }

        #endregion ViewModel Override Functions ------------------------------------------------------------------------------------------------------

        #region IActiveAware Interface Implementation ------------------------------------------------------------------------------------------------

        public event EventHandler IsActiveChanged;

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value == _isActive)
                {
                    return;
                }

                _isActive = value;
                RaisePropertyChanged();

                // Enabling this event messes up the docking functionality     
                /*
                if (_isActive)
                {
                    _eventAggregator.GetEvent<ActiveViewChangedEvent>().Publish(@"DesignView");
                }
                */
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected void OnIsActiveChanged(object sender, EventArgs args)
        {
            IsActiveChanged?.Invoke(sender, args);
        }

        #endregion IActiveAware Interface Implementation ---------------------------------------------------------------------------------------------

        #region IDockAware Interface Implementation --------------------------------------------------------------------------------------------------

        /// <summary>
        /// Content pane title
        /// </summary>
        public string Header
        {
            get => _header;
            set
            {
                if (value == _header)
                {
                    return;
                }

                _header = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// Content pane image 
        /// </summary>
        public BitmapImage Image
        {
            get => _image;
            set
            {
                if (Equals(value, _image))
                {
                    return;
                }

                _image = value;
                RaisePropertyChanged();
            }
        }

        #endregion IDockAware Interface Implementation -----------------------------------------------------------------------------------------------

        #region IDisposable Interface Implementation -------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            _disposed = true;
        }

        #endregion IDisposable Interface Implementation ---------------------------------------------------------------------------

        #region Helper Functions --------------------------------------------------------------------------------------------------
        
        #endregion Helper Functions -----------------------------------------------------------------------------------------------
    }
}
