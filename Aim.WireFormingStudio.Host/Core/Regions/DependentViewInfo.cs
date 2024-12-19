namespace Aim.WireFormingStudio.Host.Core.Regions
{
    /// <summary>
    /// Implements a class that encapsulates the info for the dependent view attribute
    /// </summary>
    public class DependentViewInfo
    {
        /// <summary>
        /// View to be injected
        /// </summary>
        public object View { get; set; }

        /// <summary>
        /// Region to inject view
        /// </summary>
        public string Region { get; set; }
    }
}
