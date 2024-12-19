namespace Aim.WireFormingStudio.Modules.BenderController
{
    #region Using Directives -------------------------------------------------------------------------------------------------------

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using Prism.Mvvm;

    using Core;
    using Menus;
    using ViewModels;
    using Views;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Bender controller module
    /// </summary>
    public class BenderControllerModule : IModule
    {
        #region Member Variables ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Hook to the region manager
        /// </summary>
        private readonly IRegionManager _regionManager;

        #endregion Member Variables -------------------------------------------------------------------------------------------------

        #region Constructors --------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="regionManager"></param>
        public BenderControllerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #endregion Constructors -----------------------------------------------------------------------------------------------------

        #region IModule Interface Implementation ------------------------------------------------------------------------------------

        /// <summary>
        /// Module has been initialized
        /// </summary>
        /// <param name="containerProvider"></param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Register the smart editor menu 
            _regionManager.RegisterViewWithRegion(HostRegionNames.MainRibbonNavigationRegion, typeof(BenderControllerTab));
            // Register the smart editor view to the main work region
            // _regionManager.RegisterViewWithRegion(HostRegionNames.MainTabWorkPaneRegion, typeof(BenderControllerView));
        }


        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // ViewModelLocationProvider.Register<BenderControllerView, BenderControllerViewModel>();
        }

        #endregion IModule Interface Implementation ---------------------------------------------------------------------------------
    }
}
