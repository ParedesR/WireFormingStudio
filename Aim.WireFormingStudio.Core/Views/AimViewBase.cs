namespace Aim.WireFormingStudio.Core.Views
{
    #region Using Directives --------------------------------------------------------------------------------------------------------------------------

    using System.Windows.Controls;

    using Prism.Events;
    using Prism.Regions;

    using ViewModels;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Base class for all user control based views in the application
    /// </summary>
    public abstract class AimViewBase : UserControl, IAimView, INavigationAware
    {
        #region Constructors -------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        protected AimViewBase()
        {
        }


        /// <summary>
        /// Overridden constructor -- view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected AimViewBase(IViewModel viewModel) : this()
        {
            ViewModel = viewModel;
        }


        /// <summary>
        /// Overridden constructor -- region manager
        /// </summary>
        /// <param name="regionManager"></param>
        protected AimViewBase(IRegionManager regionManager) : this()
        {
            RegionManager = regionManager;
        }


        /// <summary>
        /// Overridden constructor -- view model, region manager
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="regionManager"></param>
        protected AimViewBase(IViewModel viewModel, IRegionManager regionManager) : this(viewModel)
        {
            RegionManager = regionManager;
        }


        /// <summary>
        /// Overridden constructor
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="eventAggregator"></param>
        protected AimViewBase(IViewModel viewModel, IEventAggregator eventAggregator) : this(viewModel)
        {
            EventAggregator = eventAggregator;
        }


        /// <summary>
        /// Overridden constructor
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="regionManager"></param>
        /// <param name="eventAggregator"></param>
        protected AimViewBase(IViewModel viewModel, IRegionManager regionManager, IEventAggregator eventAggregator) : this(viewModel, regionManager)
        {
            EventAggregator = eventAggregator;
        }

        #endregion Constructors ----------------------------------------------------------------------------------------------------------------------

        #region Public Properties --------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Hook to Region Manager
        /// </summary>
        protected IRegionManager RegionManager { get; }


        /// <summary>
        /// Hook to event aggregator
        /// </summary>
        protected IEventAggregator EventAggregator { get; }

        #endregion Public Properties -----------------------------------------------------------------------------------------------------------------

        #region IViewBase Interface Implementation ---------------------------------------------------------------------------------------------------

        /// <summary>
        /// Hook to view model
        /// </summary>
        public IViewModel ViewModel
        {
            get => (IViewModel)DataContext;
            set => DataContext = value;
        }

        #endregion IViewBase Interface Implementation ------------------------------------------------------------------------------------------------

        #region INavigationAware Interface Implementation --------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns></returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }


        /// <summary>
        /// Moving away form view
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }


        /// <summary>
        /// Moving into view
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        #endregion INavigationAware Interface Implementation -----------------------------------------------------------------------------------------
    }
}
