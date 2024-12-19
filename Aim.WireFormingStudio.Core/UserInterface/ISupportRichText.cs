namespace Aim.WireFormingStudio.Core.UserInterface
{
    #region Using Directives ------------------------------------------------------------------------------------------------------

    using Infragistics.Controls.Editors;

    #endregion Using Directives ---------------------------------------------------------------------------------------------------

    public interface ISupportRichText
    {
        XamRichTextEditor RichTextEditor { get; set; }
    }
}
