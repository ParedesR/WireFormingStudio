namespace Aim.WireFormingStudio.Core.Events
{
	#region Using Directives -------------------------------------------------------------------------------------------------------------------------

	using Prism.Events;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Event triggered when the user wants to send a JDRL file to a machine
	/// </summary>
	public class SendJdrlFileEvent : PubSubEvent<string>
	{
	}
}
