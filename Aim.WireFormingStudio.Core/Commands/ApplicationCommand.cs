namespace Aim.WireFormingStudio.Core.Commands
{
    #region Using Directives -------------------------------------------------------------------------------------------------------

    using Prism.Commands;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Implements an application command
    /// </summary>
    public class ApplicationCommand : IApplicationCommand
    {
        public CompositeCommand NavigateCommand { get; }
    }
}
