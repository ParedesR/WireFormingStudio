namespace Aim.WireFormingStudio.Host.Core.Regions
{
    #region Using Directives ------------------------------------------------------------------------------------------------------

    using System;
    using System.Collections.Specialized;

    using Prism.Regions;

    using Infragistics.Windows.Ribbon;

    #endregion Using Directives ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Adapter for the XamRibbon Control
    /// </summary>
    public class XamRibbonRegionAdapter : RegionAdapterBase<XamRibbon>
    {
        #region Constructors -------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="behaviorFactory"></param>
        public XamRibbonRegionAdapter(IRegionBehaviorFactory behaviorFactory) : base(behaviorFactory)
        {
        }

        #endregion Constructors ----------------------------------------------------------------------------------------------------------------------

        #region Overridden Functions -----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="regionTarget"></param>
        protected override void Adapt(IRegion region, XamRibbon regionTarget)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (regionTarget == null)
            {
                throw new ArgumentNullException(nameof(regionTarget));
            }

            region.Views.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            foreach (var view in e.NewItems)
                            {
                                AddViewToRegion(view, regionTarget);
                            }

                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            foreach (var view in e.OldItems)
                            {
                                RemoveViewFromRegion(view, regionTarget);
                            }

                            break;
                        }
                }
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="regionTarget"></param>
        protected override void AttachBehaviors(IRegion region, XamRibbon regionTarget)
        {
            base.AttachBehaviors(region, regionTarget);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="regionTarget"></param>
        protected override void AttachDefaultBehaviors(IRegion region, XamRibbon regionTarget)
        {
            base.AttachDefaultBehaviors(region, regionTarget);
        }

        #endregion Overridden Functions -------------------------------------------------------------------------------------------

        #region Helper Functions --------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a tab item to the ribbon tab
        /// </summary>
        /// <param name="view"></param>
        /// <param name="xamRibbon"></param>
        private static void AddViewToRegion(object view, XamRibbon xamRibbon)
        {
            if (!(view is RibbonTabItem ribbonTabItem))
            {
                return;
            }

            if (xamRibbon.Tabs.Contains((ribbonTabItem)))
            {
                return;
            }

            xamRibbon.Tabs.Add(ribbonTabItem);
        }


        /// <summary>
        /// Remove a tab item from tab
        /// </summary>
        /// <param name="view"></param>
        /// <param name="xamRibbon"></param>
        private static void RemoveViewFromRegion(object view, XamRibbon xamRibbon)
        {
            if (!(view is RibbonTabItem ribbonTabItem))
            {
                return;
            }

            if (!xamRibbon.Tabs.Contains(ribbonTabItem))
            {
                return;
            }

            xamRibbon.Tabs.Remove(ribbonTabItem);
        }

        #endregion Helper Functions -----------------------------------------------------------------------------------------------
    }
}
