namespace Aim.WireFormingStudio.Core.Views
{
    #region Using Directives --------------------------------------------------------------------------------------------------------------------------

    using ViewModels;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Contract that defines a View
    /// </summary>
    public interface IAimView
    {
        /// <summary>
        /// View's view model
        /// </summary>
        IViewModel ViewModel { get; set; }
    }
}
