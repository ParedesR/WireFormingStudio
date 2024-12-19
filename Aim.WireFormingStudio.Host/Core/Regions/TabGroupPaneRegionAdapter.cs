namespace Aim.WireFormingStudio.Host.Core.Regions
{
    #region Using Directives -------------------------------------------------------------------------------------------------------------------------

    using System;
    using System.Windows;
    using System.ComponentModel.Composition;
    using System.Windows.Data;
    using System.Collections.Specialized;
    using System.Windows.Controls;

    using Infragistics.Windows.DockManager;

    using Prism.Regions;

    using Aim.WireFormingStudio.Core.PrismEx;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Adapter for the XamTab control
    /// </summary>
    [Export(typeof(TabGroupPaneRegionAdapter))]
    public class TabGroupPaneRegionAdapter : RegionAdapterBase<TabGroupPane>
    {
        /// <summary>
        /// Used to determine what views were injected and ContentPanes were generated for
        /// </summary>
        private static readonly DependencyProperty IsGeneratedProperty =
                                            DependencyProperty.RegisterAttached("IsGenerated", typeof(bool), typeof(TabGroupPaneRegionAdapter), null);

        /// <summary>
        /// Used to track the region that a ContentPane belongs to so that we can access the region from within the ContentPane.Closed event handler
        /// </summary>
        private static readonly DependencyProperty RegionProperty =
                                            DependencyProperty.RegisterAttached("Region", typeof(IRegion), typeof(TabGroupPaneRegionAdapter), null);

        #region Constructors -------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="behaviorFactory"></param>
        public TabGroupPaneRegionAdapter(IRegionBehaviorFactory behaviorFactory) : base(behaviorFactory)
        {
        }

        #endregion Constructors ----------------------------------------------------------------------------------------------------------------------

        #region Overridden Functions -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="regionTarget"></param>
        protected override void Adapt(IRegion region, TabGroupPane regionTarget)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (regionTarget == null)
            {
                throw new ArgumentNullException(nameof(regionTarget));
            }

            if (regionTarget.ItemsSource != null)
            {
                throw new InvalidOperationException("ItemsSource property is not empty. This control is being associated with a region, but the control is already bound to something else. " +
                        "If you did not explicitly set the control's ItemSource property, this exception may be caused by a change in the value of the inherited RegionManager attached property.");
            }

            SynchronizeItems(region, regionTarget);

            region.Views.CollectionChanged += (s, e) =>
            {
                OnViewsCollectionChanged(e, region, regionTarget);
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="regionTarget"></param>
        protected override void AttachBehaviors(IRegion region, TabGroupPane regionTarget)
        {
            base.AttachBehaviors(region, regionTarget);

            if (!region.Behaviors.ContainsKey(TabGroupPaneRegionBehavior.BehaviorKey))
            {
                region.Behaviors.Add(TabGroupPaneRegionBehavior.BehaviorKey,
                   new TabGroupPaneRegionBehavior { HostControl = regionTarget });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }

        #endregion Overridden Functions --------------------------------------------------------------------------------------------------------------

        #region Protected Functions ------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Prepares a view being injected as a ContentPane
        /// </summary>
        /// <param name="item">the view</param>
        /// <param name="region"></param>
        /// <returns>The injected view as a ContentPane</returns>
        protected ContentPane PrepareContainerForItem(object item, IRegion region)
        {
            if (!(item is ContentPane container))
            {
                container = new ContentPane { Content = item, DataContext = ResolveDataContext(item) };
                // The content is the view being injected make sure the dataContext is the same as the view.
                // Most likely a ViewModel
                container.SetValue(IsGeneratedProperty, true); // We generated this one
                CreateDockAwareBindings(container);
            }

            // Let's keep track of which region the container belongs to
            container.SetValue(RegionProperty, region);
            // Make it easy on ourselves and have the pane manage removing itself from the XamDockManager
            container.CloseAction = PaneCloseAction.RemovePane;
            container.Closed += Container_Closed;

            return container;
        }


        /// <summary>
        /// Sets the Content property of a generated ContentPane to null.
        /// </summary>
        /// <param name="contentPane">The ContentPane</param>
        protected virtual void ClearContainerForItem(ContentPane contentPane)
        {
            if (!(bool)contentPane.GetValue(IsGeneratedProperty))
            {
                return;
            }

            // Remove any bindings
            contentPane.ClearValue(HeaderedContentControl.HeaderProperty);
            contentPane.Content = null;
        }

        #endregion Protected Functions ---------------------------------------------------------------------------------------------------------------

        #region Helper Functions ---------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="region"></param>
        /// <param name="regionTarget"></param>
        private void OnViewsCollectionChanged(NotifyCollectionChangedEventArgs e, IRegion region, ItemsControl regionTarget)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //we want to add them behind any previous views that may have been manually declare in XAML or injected
                var startIndex = e.NewStartingIndex;
                foreach (var newItem in e.NewItems)
                {
                    var contentPane = PrepareContainerForItem(newItem, region);

                    if (regionTarget.Items.Count != startIndex)
                    {
                        startIndex = 0;
                    }

                    // We must make sure we bring the TabGroupPane into view.  If we don't a System.StackOverflowException will occur
                    // in UIAutomationProvider.dll if trying to add a ContentPane to a TabGroupPane that is not in view.  This is
                    // most common when using nested TabGroupPane regions. If you don't this, you can comment it out.
                    regionTarget.BringIntoView();

                    regionTarget.Items.Insert(startIndex, contentPane);

                    // Not very clean but we need to activate the CAD Editor here.  Since it is a windows forms control does not 
                    // obey the rendering of wpf.
                    var dataContextType = contentPane.DataContext.GetType().ToString();
                    if (!contentPane.IsActivePane && (dataContextType.IndexOf("CadEditorViewModel", StringComparison.Ordinal) > 0))
                    {
                        contentPane.Activate();
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (regionTarget.Items.Count == 0)
                {
                    return;
                }

                var contentPanes = XamDockManager.GetDockManager(regionTarget).GetPanes(PaneNavigationOrder.VisibleOrder);
                foreach (var contentPane in contentPanes)
                {
                    if (e.OldItems.Contains(contentPane) || e.OldItems.Contains(contentPane.Content))
                    {
                        contentPane.ExecuteCommand(ContentPaneCommands.Close);
                    }
                }
            }
        }


        /// <summary>
        /// Takes all the views that were declared in XAML manually and merges them with the region.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="regionTarget"></param>
        private void SynchronizeItems(IRegion region, ItemsControl regionTarget)
        {
            if ((regionTarget.Items.Count <= 0))
            {
                return;
            }

            foreach (var item in regionTarget.Items)
            {
                PrepareContainerForItem(item, region);
                region.Add(item);
            }
        }


        /// <summary>
        /// Executes when a ContentPane is closed.
        /// </summary>
        /// <remarks>Responsible for removing the ContentPane from the region, any event handlers, and clears the content as well as any
        /// bindings from the ContentPane to prevent memory leaks.</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Container_Closed(object sender, Infragistics.Windows.DockManager.Events.PaneClosedEventArgs e)
        {
            if (!(sender is ContentPane contentPane))
            {
                return;
            }

            contentPane.Closed -= Container_Closed;

            // Get the region associated with the ContentPane so that we can remove it.
            if (contentPane.GetValue(RegionProperty) is IRegion region)
            {
                // We are dealing with a ContentPane directly
                if (region.Views.Contains(contentPane))
                {
                    region.Remove(contentPane);
                }

                // This view was injected and set as the content of our ContentPane
                var item = contentPane.Content;
                if (item != null && region.Views.Contains(item))
                {
                    region.Remove(item);
                }
            }

            // Reduce memory leaks
            ClearContainerForItem(contentPane);
        }


        /// <summary>
        /// Checks to see if the View or the View's DataContext (Most likely a ViewModel) implements the IDockAware interface and creates
        /// the necessary data bindings.
        /// </summary>
        /// <param name="contentPane"></param>
        private static void CreateDockAwareBindings(ContentControl contentPane)
        {
            var headerBinding = new Binding("Header");
            var imageBinding = new Binding("Image");

            // Let's first check the view that was injected for IDockAware
            if (contentPane.Content is IDockAware dockAwareContent)
            {
                headerBinding.Source = dockAwareContent;
            }

            if ((headerBinding.Source == null) || (imageBinding.Source == null))
            {
                // Fall back to data context of the content pane.
                if (contentPane.DataContext is IDockAware dockAwareDataContext)
                {
                    headerBinding.Source = dockAwareDataContext;
                    imageBinding.Source = dockAwareDataContext;
                }
            }

            contentPane.SetBinding(HeaderedContentControl.HeaderProperty, headerBinding);
            contentPane.SetBinding(ContentPane.ImageProperty, imageBinding);
        }


        /// <summary>
        /// Finds the DataContext of the view.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static object ResolveDataContext(object item)
        {
            return !(item is FrameworkElement frameworkElement) ? item : frameworkElement.DataContext;
        }

        #endregion Helper Functions ------------------------------------------------------------------------------------------------------------------
    }
}
