namespace Aim.WireFormingStudio.Modules.SmartEditor.ViewModels
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	using Prism.Commands;
	using Prism.Services.Dialogs;

	using AosLibraries.SharedInterfaces.Machines;
	using AosLibraries.SharedInterfaces.DomainData.WireEntities;

	using Aim.WireFormingStudio.Core.ViewModels;

	using AosLibraries.HardwareDevices.Benders.Aos;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Edit LRAr segment view model
	/// </summary>
	public class EditLrarSegmentViewModel : ViewModelBase, IDialogAware
	{
		#region Member Variables --------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		private double _lrarLength;

		/// <summary>
		/// 
		/// </summary>
		private double _lrarRotation;

		/// <summary>
		/// 
		/// </summary>
		private double _lrarAngle;

		/// <summary>
		/// 
		/// </summary>
		private double _lrarRadius;

		#endregion Member Variables -----------------------------------------------------------------------------------------------

		#region Constructors ------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		public EditLrarSegmentViewModel()
		{
			// Commands           
			DoEditLrarSegmentCommand = new DelegateCommand(ExecuteDoEditLrarSegmentCommand, CanExecuteDoEditLrarSegmentCommand);
			CancelEditLrarSegmentCommand = new DelegateCommand(ExecuteCancelEditLrarSegmentCommand);

			InitializeProperties();
		}

		#endregion Constructors -----------------------------------------------------------------------------------------------------

		#region Public Properties -------------------------------------------------------------------------------------------------

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Accepts the edits of the LRAr segment 
		/// </summary>
		public DelegateCommand DoEditLrarSegmentCommand { get; set; }

		/// <summary>
		/// Cancels the edits of the LRAr segment
		/// </summary>
		public DelegateCommand CancelEditLrarSegmentCommand { get; set; }

		#endregion Commands -------------------------------------------------------------------------------------------------------

		/// <summary>
		/// LRAr segment being edit
		/// </summary>
		public IWireSegmentAsLrar LrarSegment { get; private set; }

		/// <summary>
		/// Gets/Sets the length property of the LRAr segment
		/// </summary>
		public double LrarLength
		{
			get => _lrarLength;
			set
			{
				if (value == _lrarLength)
				{
					return;
				}

				_lrarLength = value;
				RaisePropertyChanged();				
			}
		}

		/// <summary>
		/// Gets/Sets the rotation property of the LRAr segment
		/// </summary>
		public double LrarRotation
		{
			get => _lrarRotation;
			set
			{
				if (value == _lrarRotation)
				{
					return;
				}

				_lrarRotation = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Gets/Sets the angle property of the LRAr segment
		/// </summary>
		public double LrarAngle
		{
			get => _lrarAngle;
			set
			{
				if (value == _lrarAngle)
				{
					return;
				}

				_lrarAngle = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Gets/Sets the radius property of the LRAr segment
		/// </summary>
		public double LrarRadius
		{
			get => _lrarRadius;
			set
			{
				if (value == _lrarRadius)
				{
					return;
				}

				_lrarRadius = value;
				RaisePropertyChanged();
			}
		}
		#endregion Public Properties ----------------------------------------------------------------------------------------------

		#region IDialogAware Interface Implementation -----------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		public string Title => "Edit LRAr Segment";

		/// <summary>
		/// 
		/// </summary>
		public event Action<IDialogResult> RequestClose;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool CanCloseDialog() => true;

		/// <summary>
		/// 
		/// </summary>
		public void OnDialogClosed()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameters"></param>
		public void OnDialogOpened(IDialogParameters parameters)
		{
			if (!parameters.ContainsKey("AddLrarSegment"))
			{
				return;
			}

			LrarSegment = parameters.GetValue<IWireSegmentAsLrar>("AddLrarSegment");
			if(LrarSegment == null)
			{
				return;
			}

			LrarLength = LrarSegment.Length;
			LrarRotation = LrarSegment.Rotation;
			LrarAngle = LrarSegment.Angle;
			LrarRadius = LrarSegment.Radius;
		}

		#endregion IDialogAware Interface Implementation --------------------------------------------------------------------------

		#region Helper Functions --------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		private void InitializeProperties()
		{
			LrarLength = 1.0;			
			LrarRotation = 0.0;
			LrarAngle = 0.0;
			LrarRadius = 1.0;
		}

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks if the edit LRAr segment command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteDoEditLrarSegmentCommand()
		{
			return true;
		}

		/// <summary>
		/// Adds a LRAr segment  
		/// </summary>
		private void ExecuteDoEditLrarSegmentCommand()
		{
			if(LrarSegment == null)
			{
				return;
			}

			LrarSegment.Length = LrarLength;
			LrarSegment.Rotation = LrarRotation;
			LrarSegment.Angle = LrarAngle;
			LrarSegment.Radius = LrarRadius;

			var resultParameters = new DialogParameters { { "AddLrarSegment", LrarSegment } };
			RequestClose?.Invoke(new DialogResult(ButtonResult.OK, resultParameters));
		}

		/// <summary>
		/// Cancels the edit LRAr command function
		/// </summary>
		private void ExecuteCancelEditLrarSegmentCommand()
		{
			RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
		}

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
