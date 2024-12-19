namespace Aim.WireFormingStudio.Host.ViewModels.Machines
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	using Prism.Commands;
	using Prism.Services.Dialogs;

	using AosLibraries.SharedInterfaces.Machines;

	using Aim.WireFormingStudio.Core.ViewModels;

	using AosLibraries.HardwareDevices.Benders.Aos;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Bender machines setup view model
	/// </summary>
	public class BenderMachineSetUpViewModel : ViewModelBase, IDialogAware
	{
		#region Member Variables --------------------------------------------------------------------------------------------------

		/// <summary>
		/// Name of the micro bender 
		/// </summary>
		private string _microBenderName;

		/// <summary>
		/// IP Address of the micro bender
		/// </summary>
		private string _microBenderIpAddress;

		/// <summary>
		/// List of available configured wire bending machines
		/// </summary>
		private ObservableCollection<IWireBendingMachine> _configuredMicroBenderMachines;

		/// <summary>
		/// Selected micro bender machine
		/// </summary>
		private IWireBendingMachine _selectedMicroBenderMachine;
		
		#endregion Member Variables -----------------------------------------------------------------------------------------------

		#region Constructors ------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		public BenderMachineSetUpViewModel()
		{
			// Commands           
			DoSetUpBenderMachinesCommand = new DelegateCommand(ExecuteDoSetUpBenderMachinesCommand, CanExecuteDoSetUpBenderMachinesCommand);
			CancelSetUpBenderMachinesCommand = new DelegateCommand(ExecuteCancelSetUpBenderMachinesCommand);

			InitializeProperties();
		}

		#endregion Constructors -----------------------------------------------------------------------------------------------------

		#region Public Properties -------------------------------------------------------------------------------------------------

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Sets up bending machines 
		/// </summary>
		public DelegateCommand DoSetUpBenderMachinesCommand { get; set; }

		/// <summary>
		/// Cancels the set up bender machines function and closes the view
		/// </summary>
		public DelegateCommand CancelSetUpBenderMachinesCommand { get; set; }

		#endregion Commands -------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Selected micro bender machine
		/// </summary>
		public IWireBendingMachine SelectedMicroBenderMachine
		{
			get => _selectedMicroBenderMachine;
			set
			{
				if (value == _selectedMicroBenderMachine)
				{
					return;
				}

				_selectedMicroBenderMachine = value;
				RaisePropertyChanged();

				DoSetUpBenderMachinesCommand.RaiseCanExecuteChanged();				
			}
		}

		#endregion Public Properties ----------------------------------------------------------------------------------------------

		#region IDialogAware Interface Implementation -----------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		public string Title => "Set Up Bender Machines";

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
			if (!parameters.ContainsKey("UpdatedMicroBenderMachine"))
			{
				return;
			}

			SelectedMicroBenderMachine = parameters.GetValue<IWireBendingMachine>("UpdatedMicroBenderMachine");
			if (SelectedMicroBenderMachine == null)
			{
				return;
			}
		}

		#endregion IDialogAware Interface Implementation --------------------------------------------------------------------------

		#region Helper Functions --------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		private void InitializeProperties()
		{
			_configuredMicroBenderMachines = new ObservableCollection<IWireBendingMachine>();
		}

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks if the add bending machines command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteDoSetUpBenderMachinesCommand()
		{
			return true;
		}

		/// <summary>
		/// Sets up a bending machine 
		/// </summary>
		private void ExecuteDoSetUpBenderMachinesCommand()
		{
			if(SelectedMicroBenderMachine == null)
			{
				return;
			}
						
			var resultParameters = new DialogParameters { { "UpdatedMicroBenderMachine", SelectedMicroBenderMachine } };
			RequestClose?.Invoke(new DialogResult(ButtonResult.OK, resultParameters));   			
		}

		/// <summary>
		/// Cancels the set up bending machine command function
		/// </summary>
		private void ExecuteCancelSetUpBenderMachinesCommand()
		{
			RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
		}

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
