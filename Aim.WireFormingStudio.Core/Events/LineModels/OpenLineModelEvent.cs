namespace Aim.WireFormingStudio.Core.Events.LineModels
{
	#region Using Directives -------------------------------------------------------------------------------------------------------------------------

	using Prism.Events;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Event triggered when the user wants to open a line model file
	/// </summary>
	public class OpenLineModelEvent : PubSubEvent<string>
	{
	}
}
