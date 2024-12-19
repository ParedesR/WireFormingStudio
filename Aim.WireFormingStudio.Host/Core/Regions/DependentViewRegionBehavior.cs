namespace Aim.WireFormingStudio.Host.Core.Regions
{
    #region Using Directives -------------------------------------------------------------------------------------------------------------------------

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;

    using Prism.Ioc;
    using Prism.Regions;

    using Aim.WireFormingStudio.Core.Attributes;
    using Aim.WireFormingStudio.Core.UserInterface;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    public class DependentViewRegionBehavior : RegionBehavior
    {
        #region Constants ----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public const string BehaviorKey = "DependentViewRegionBehavior";

        #endregion Constants -------------------------------------------------------------------------------------------------------------------------

        #region Member Variables ---------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Hook to the container
        /// </summary>
        private readonly IContainerExtension _container;

        /// <summary>
        /// Dictionary to cache dependencies
        /// </summary>
        private readonly Dictionary<object, List<DependentViewInfo>> _dependentViewCache;

        #endregion Member Variables -------------------------------------------------------------------------------------------------

        #region Constructors --------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="container"></param>
        public DependentViewRegionBehavior(IContainerExtension container)
        {
            _container = container;

            _dependentViewCache = new Dictionary<object, List<DependentViewInfo>>();
        }

        #endregion Constructors -----------------------------------------------------------------------------------------------------

        #region Overridden Functions ------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        #endregion Overridden Functions ----------------------------------------------------------------------------------------------

        #region Helper Functions ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newView in e.NewItems)
                {
                    var dependentViews = new List<DependentViewInfo>();

                    if (_dependentViewCache.ContainsKey(newView))
                    {
                        dependentViews = _dependentViewCache[newView];
                    }
                    else
                    {
                        var attributes = GetCustomAttributes<DependentViewAttribute>(newView.GetType());
                        foreach (var attribute in attributes)
                        {
                            var info = CreateDependentViewInfo(attribute);
                            if (info.View is ISupportDataContext infoDataContext &&
                                newView is ISupportDataContext viewDataContext)
                            {
                                infoDataContext.DataContext = viewDataContext.DataContext;
                            }

                            dependentViews.Add(info);
                        }

                        _dependentViewCache.Add(newView, dependentViews);
                    }

                    dependentViews.ForEach(item => Region.RegionManager.Regions[item.Region].Add(item.View));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldView in e.OldItems)
                {
                    if (!_dependentViewCache.ContainsKey(oldView))
                    {
                        continue;
                    }

                    var dependentViews = _dependentViewCache[oldView];
                    dependentViews.ForEach(item => Region.RegionManager.Regions[item.Region].Remove(item.View));

                    if (!ShouldKeepAlive(oldView))
                    {
                        _dependentViewCache.Remove(oldView);

                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldView"></param>
        /// <returns></returns>
        private bool ShouldKeepAlive(object oldView)
        {
            var regionLifeTime = GetViewOrDataContextLifeTime(oldView);
            return regionLifeTime == null || regionLifeTime.KeepAlive;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private IRegionMemberLifetime GetViewOrDataContextLifeTime(object view)
        {
            if (view is IRegionMemberLifetime regionMemberLifetime)
            {
                return regionMemberLifetime;
            }

            if (view is FrameworkElement fe)
            {
                return fe.DataContext as IRegionMemberLifetime;
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private DependentViewInfo CreateDependentViewInfo(DependentViewAttribute attribute)
        {
            var info = new DependentViewInfo
            {
                Region = attribute.Region,
                View = _container.Resolve(attribute.DependentViewType)
            };

            // Create the view instance 

            return info;
        }

        #endregion Helper Functions -------------------------------------------------------------------------------------------------
    }
}
