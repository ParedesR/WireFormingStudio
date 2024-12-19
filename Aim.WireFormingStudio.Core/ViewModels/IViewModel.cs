namespace Aim.WireFormingStudio.Core.ViewModels
{
    /// <summary>
    /// Contract that defines a View Model
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Title of the view associated with this View Model
        /// </summary>
        string ViewTitle { get; set; }
    }
}
