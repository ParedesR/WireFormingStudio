namespace Aim.WireFormingStudio.Host.Core.Regions
{
    #region Using Directives -------------------------------------------------------------------------------------------------------------------------

    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;

    using Infragistics.Windows.DockManager;

    using Prism.Regions;
    using Prism.Regions.Behaviors;

    using Aim.WireFormingStudio.Core.PrismEx;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    public class TabGroupPaneRegionBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        #region Constants ----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public const string BehaviorKey = "TabGroupPaneRegionBehavior";

        #endregion Constants -------------------------------------------------------------------------------------------------------------------------

        #region Member Variables ---------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        private XamDockManager _parentDockManager;

        /// <summary>
        /// 
        /// </summary>
        private TabGroupPane _hostControl;

        #endregion Member Variables ------------------------------------------------------------------------------------------------------------------

        #region Public Properties --------------------------------------------------------------------------------------------------------------------       

        /// <summary>
        /// 
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public Lazy<object, IViewRegionRegistration>[] RegisteredViews { get; set; }

        #endregion Public Properties -----------------------------------------------------------------------------------------------------------------

        #region Overridden Functions -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttach()
        {            
            if (HostControl == null)
            {
                return;
            }

            _parentDockManager = XamDockManager.GetDockManager(HostControl);
            if (_parentDockManager != null)
            {
                _parentDockManager.ActivePaneChanged += DockManager_ActivePaneChanged;
            }

            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        #endregion Overridden Functions --------------------------------------------------------------------------------------------------------------

        #region IHostAwareRegionBehavior Interface Implementation ------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public DependencyObject HostControl
        {
            get => _hostControl;
            set => _hostControl = value as TabGroupPane;
        }

        #endregion IHostAwareRegionBehavior Interface Implementation ---------------------------------------------------------------------------------

        #region Helper Functions ---------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DockManager_ActivePaneChanged(object sender, RoutedPropertyChangedEventArgs<ContentPane> e)
        {
            if (e.OldValue != null)
            {
                var item = e.OldValue;

                // Are we dealing with a ContentPane directly
                if (Region.Views.Contains(item) && Region.ActiveViews.Contains(item))
                {
                    Region.Deactivate(item);
                }
                else
                {
                    // Now check to see if we have any views that were injected
                    if (item is ContentControl contentControl && contentControl.Content != null)
                    {
                        var injectedView = contentControl.Content;
                        var injectedViewType = injectedView.GetType().ToString();
                        if ((injectedViewType.IndexOf("CadEditorView", StringComparison.Ordinal) < 0) &&
                           Region.Views.Contains(injectedView) && Region.ActiveViews.Contains(injectedView))
                        {
                            Region.Deactivate(injectedView);
                        }
                    }
                }
            }

            if (e.NewValue == null)
            {
                return;
            }

            var newItem = e.NewValue;

            //are we dealing with a ContentPane directly
            if (Region.Views.Contains(newItem) && !Region.ActiveViews.Contains(newItem))
            {
                Region.Activate(newItem);
            }
            else
            {
                // Now check to see if we have any views that were injected
                if (newItem is ContentControl contentControl)
                {
                    var injectedView = contentControl.Content;
                    if (Region.Views.Contains(injectedView) && !Region.ActiveViews.Contains(injectedView))
                    {
                        Region.Activate(injectedView);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            if (e.NewItems[0] is FrameworkElement frameworkElement)
            {
                var contentPane = frameworkElement as ContentPane ?? frameworkElement.Parent as ContentPane;

                if (contentPane != null && !contentPane.IsActivePane)
                {
                    contentPane.Activate();
                }
            }
            else
            {
                // Must be a view model
                var viewModel = e.NewItems[0];
                var contentPane = GetContentPaneFromViewModel(viewModel);
                contentPane?.Activate();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private ContentPane GetContentPaneFromViewModel(object viewModel)
        {
            var panes = XamDockManager.GetDockManager(_hostControl).GetPanes(PaneNavigationOrder.VisibleOrder);
            return panes.FirstOrDefault(contentPane => contentPane.DataContext == viewModel);
        }

        #endregion Helper Functions ------------------------------------------------------------------------------------------------------------------
    }
}
