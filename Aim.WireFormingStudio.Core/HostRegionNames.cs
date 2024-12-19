namespace Aim.WireFormingStudio.Core
{
    #region Using Directives ------------------------------------------------------------------------------------------------------

    #endregion Using Directives ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    public static class HostRegionNames
    {
        public const string StatusBarRegion = @"StatusBarRegion";
        
        /// <summary>
        /// 
        /// </summary>
        public const string ContentRegion = @"ContentRegion";
                
        /// <summary>
        /// Navigation tool bar that uses the Ribbon control
        /// </summary>
        public const string MainRibbonNavigationRegion = @"MainRibbonNavigationRegion";

        /// <summary>
        /// Main tool bar Navigation Region -- toolbar, views are injected
        /// </summary>
        public const string MainToolbarNavigationRegion = @"MainToolbarNavigationRegion";

        /// <summary>
        /// Main Tab Navigation Region -- Main Navigation Tab Views are injected (The Host is tabbed, the view is simple pane)
        /// </summary>
        public const string MainTabNavigationRegion = @"MainTabNavigationRegion";

        /// <summary>
        /// Main Pane Navigation Region -- Main Navigation Pane Views are injected (The view is tabbed not the host) 
        /// </summary>
        public const string MainPaneNavigationRegion = @"MainPaneNavigationRegion";

        /// <summary>
        /// Main Tab Work Pane Region -- Cad Viewer, Operator Console, Map View, and Command Editor views are injected
        /// </summary>
        public const string MainTabWorkPaneRegion = @"MainTabWorkPaneRegion";

        /// <summary>
        /// Main Tab Info Pane Region -- 
        /// </summary>
        public const string MainTabInfoPaneRegion = @"MainTabInfoPaneRegion";

        /// <summary>
        /// Status And Help Assistant Region -- Status and help information views are injected
        /// </summary>
        public const string StatusAndHelpAssistantRegion = @"StatusAndHelpAssistantRegion";
    }
}
