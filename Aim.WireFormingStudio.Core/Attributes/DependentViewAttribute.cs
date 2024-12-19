namespace Aim.WireFormingStudio.Core.Attributes
{
    #region Using Directives -------------------------------------------------------------------------------------------------------

    using System;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Attributes for Ribbon Tabs
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependentViewAttribute : Attribute
    {
        public DependentViewAttribute(string region, Type dependentViewType)
        {
            if (string.IsNullOrEmpty(region))
            {
                throw new ArgumentNullException(nameof(region));
            }

            Region = region;
            DependentViewType = dependentViewType ?? throw new ArgumentNullException(nameof(dependentViewType));
        }

        public Type DependentViewType { get; set; }

        public string Region { get; set; }
    }
}
