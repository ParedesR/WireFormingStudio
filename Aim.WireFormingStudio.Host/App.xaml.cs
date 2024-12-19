namespace Aim.WireFormingStudio.Host
{
    #region Using Directives ------------------------------------------------------------------------------------------------------

    using System.Windows;

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    
    using Infragistics.Themes;   
    using Infragistics.Windows.Ribbon;
    using Infragistics.Windows.DockManager;
       
    using AosLibraries.SharedInterfaces.DomainData.Services;
    using AosLibraries.SharedInterfaces.CaseDocuments.Aim;
   
    using AosLibraries.Kernel.DomainData.Core.Services;

    using Aim.WireFormingStudio.Core.Commands;

    using Modules.SmartEditor;
    using Modules.EsViewPort;
    using Modules.BenderController;
    using Modules.DocumentManager;
    using Modules.DocumentManager.Models;

    using Core.Regions;
    using Views;
    using Views.Machines;

    #endregion Using Directives ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Overridden Functions ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Create shell
        /// </summary>
        /// <returns></returns>
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shell"></param>
        protected override void InitializeShell(Window shell)
        {
            ThemeManager.ApplicationTheme = new Office2013Theme();
            base.InitializeShell(shell);
        }


        /// <summary>
        /// Register types
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {           
            containerRegistry.RegisterSingleton<IApplicationCommand, ApplicationCommand>();
            // Register services            
            containerRegistry.RegisterSingleton<IWireBendingService, WireBendingService>();
            containerRegistry.RegisterSingleton<IWfsDocumentManager, WfsDocumentManager>();          

            // Register dialog service
            containerRegistry.RegisterDialog<BenderMachinesView>();
            containerRegistry.RegisterDialog<BenderMachineSetUpView>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<SmartEditorModule>();            
            moduleCatalog.AddModule<BenderControllerModule>();
            // - moduleCatalog.AddModule<EsViewPortModule>();
            moduleCatalog.AddModule<DocumentManagerModule>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="regionAdapterMappings"></param>
        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);

            regionAdapterMappings.RegisterMapping(typeof(TabGroupPane), Container.Resolve<TabGroupPaneRegionAdapter>());           
            regionAdapterMappings.RegisterMapping(typeof(XamRibbon), Container.Resolve<XamRibbonRegionAdapter>());
        }


        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);

            regionBehaviors.AddIfMissing(TabGroupPaneRegionBehavior.BehaviorKey, typeof(TabGroupPaneRegionBehavior));
            regionBehaviors.AddIfMissing(DependentViewRegionBehavior.BehaviorKey, typeof(DependentViewRegionBehavior));
        }

        #endregion Overridden Functions ----------------------------------------------------------------------------------------------
    }
}
