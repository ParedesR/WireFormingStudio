namespace Aim.WireFormingStudio.Core.PrismEx
{
    #region Using Directives -------------------------------------------------------------------------------------------------------------------------

    using System.Windows.Media.Imaging;

    #endregion Using Directives ---------------------------------------------------------------------------------------------------------------------- 

    /// <summary>
    /// Contract that defines a view that is aware of docking
    /// </summary>
    public interface IDockAware
    {
        /// <summary>
        /// Header of the dock-able view
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Content panel image
        /// </summary>
        BitmapImage Image { get; set; }
    }
}
