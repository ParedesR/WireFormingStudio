namespace Aim.WireFormingStudio.Modules.SmartEditor
{
    #region Using Directives -------------------------------------------------------------------------------------------------------

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using Prism.Mvvm;

    using Core;
    using Menus;
    using ViewModels;
    using ViewModels.Menus;
    using Views;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Smart editor module
    /// </summary>
    public class SmartEditorModule : IModule
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
        public SmartEditorModule(IRegionManager regionManager)
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
            _regionManager.RegisterViewWithRegion(HostRegionNames.MainRibbonNavigationRegion, typeof(SmartEditorTab));
            // Register the smart editor view to the main work region
            _regionManager.RegisterViewWithRegion(HostRegionNames.MainTabWorkPaneRegion, typeof(SmartEditorView));
        }


        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ViewModelLocationProvider.Register<SmartEditorView, SmartEditorViewModel>();
            ViewModelLocationProvider.Register<SmartEditorTab, SmartEditorTabViewModel>();

            // Register services            
            // containerRegistry.RegisterSingleton<IWireBendingService, WireBendingService>();

            // Register dialog service
            containerRegistry.RegisterDialog<EditLrarSegmentView>();            
        }

        #endregion IModule Interface Implementation ---------------------------------------------------------------------------------
    }
}
