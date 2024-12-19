namespace Aim.WireFormingStudio.Core.Events
{
	#region Using Directives -------------------------------------------------------------------------------------------------------------------------

	using Prism.Events;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Event triggered when a JDRL file has bee sent
	/// </summary>
	public class JdrlFileSentEvent : PubSubEvent<bool>
	{
	}
}
