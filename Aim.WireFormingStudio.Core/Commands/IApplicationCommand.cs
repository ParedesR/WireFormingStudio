namespace Aim.WireFormingStudio.Core.Commands
{
    #region Using Directives -------------------------------------------------------------------------------------------------------

    using Prism.Commands;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Contract that defines an application command
    /// </summary>
    public interface IApplicationCommand
    {
        CompositeCommand NavigateCommand { get; }
    }
}
