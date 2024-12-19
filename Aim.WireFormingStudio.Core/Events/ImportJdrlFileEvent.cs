namespace Aim.WireFormingStudio.Core.Events
{
    #region Using Directives -------------------------------------------------------------------------------------------------------------------------

    using Prism.Events;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Event triggered when the user wants to import a JDRL file
    /// </summary>
    public class ImportJdrlFileEvent : PubSubEvent<string>
    {
    }
}
