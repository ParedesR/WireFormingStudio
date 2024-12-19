namespace Aim.WireFormingStudio.Core.Events.Views
{
	#region Using Directives -------------------------------------------------------------------------------------------------------------------------

	using Prism.Events;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Event to be raised when a view from the main region is activated
	/// </summary>
	public class ActiveViewChangedEvent : PubSubEvent<string>
	{
	}
}
