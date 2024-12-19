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
	/// Bender machines view model 
	/// </summary>
	public class BenderMachinesViewModel : ViewModelBase, IDialogAware
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
		/// List of available wire bending machines
		/// </summary>
		private ObservableCollection<IWireBendingMachine> _availableMicroBenderMachines;
		
		/// <summary>
		/// Selected micro bender machine
		/// </summary>
		private IWireBendingMachine _selectedMicroBenderMachine;

		/// <summary>
		/// List of added (configured) wire bending machines
		/// </summary>
		private IList<IWireBendingMachine> _configuredMicroBenderMachines;

		#endregion Member Variables -----------------------------------------------------------------------------------------------

		#region Constructors ------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		public BenderMachinesViewModel()
		{
			// Commands           
			DoAddBenderMachinesCommand = new DelegateCommand(ExecuteDoAddBenderMachinesCommand, CanExecuteDoAddBenderMachinesCommand);
			CancelAddBenderMachinesCommand = new DelegateCommand(ExecuteCancelAddBenderMachinesCommand);
			SearchBenderByNameCommand = new DelegateCommand(ExecuteSearchBenderByNameCommand, CanExecuteSearchBenderByNameCommand);
			SearchBenderByIpCommand = new DelegateCommand(ExecuteSearchBenderByIpCommand, CanExecuteSearchBenderByIpCommand);

			InitializeProperties();
		}

		#endregion Constructors -----------------------------------------------------------------------------------------------------

		#region Public Properties -------------------------------------------------------------------------------------------------

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Adds bending machines 
		/// </summary>
		public DelegateCommand DoAddBenderMachinesCommand { get; set; }

		/// <summary>
		/// Cancels the add bender machines function and closes the view
		/// </summary>
		public DelegateCommand CancelAddBenderMachinesCommand { get; set; }

		/// <summary>
		/// Searches a bender machine by name
		/// </summary>
		public DelegateCommand SearchBenderByNameCommand { get; set; }

		/// <summary>
		/// Searches a machine by IP Address
		/// </summary>
		public DelegateCommand SearchBenderByIpCommand { get; set; }

		#endregion Commands -------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets/Sets the micro bender name
		/// </summary>
		public string MicroBenderName
		{
			get => _microBenderName;
			set
			{
				if(value == _microBenderName)
				{
					return;
				}

				_microBenderName = value;
				RaisePropertyChanged();

				if (SelectedMicroBenderMachine == null)
				{
					return;
				}

				SelectedMicroBenderMachine.MachineName = _microBenderName;
			}
		}

		/// <summary>
		/// IP Address of the micro bender
		/// </summary>
		public string MicroBenderIpAddress
		{
			get => _microBenderIpAddress;
			set
			{
				if (value == _microBenderIpAddress)
				{
					return;
				}

				_microBenderIpAddress = value;
				RaisePropertyChanged();

				if(SelectedMicroBenderMachine == null)
				{
					return;
				}

				SelectedMicroBenderMachine.IpAddress = _microBenderIpAddress;
			}
		}

		/// <summary>
		/// List of available (configured) wire bending machines
		/// </summary>
		public ObservableCollection<IWireBendingMachine> AvailableMicroBenderMachines		
		{
			get => _availableMicroBenderMachines;
			set
			{
				_availableMicroBenderMachines = value;
				RaisePropertyChanged();
			}
		}

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

				DoAddBenderMachinesCommand.RaiseCanExecuteChanged();

				if (_selectedMicroBenderMachine == null)
				{
					return;
				}

				MicroBenderName = _selectedMicroBenderMachine.MachineName;
				MicroBenderIpAddress = _selectedMicroBenderMachine.IpAddress;				
			}
		}

		#endregion Public Properties ----------------------------------------------------------------------------------------------

		#region IDialogAware Interface Implementation -----------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		public string Title => "Add Bender Machines";

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
			if (!parameters.ContainsKey("ConfiguredMicroBenderMachines"))
			{
				return;
			}

			_configuredMicroBenderMachines?.Clear();

			_configuredMicroBenderMachines = parameters.GetValue<ObservableCollection<IWireBendingMachine>>("ConfiguredMicroBenderMachines");
			if( (_configuredMicroBenderMachines == null) || !_configuredMicroBenderMachines.Any())
			{
				return;
			}

			foreach(var microBender in _configuredMicroBenderMachines)
			{
				AvailableMicroBenderMachines.Add(microBender);
			}						
			SelectedMicroBenderMachine = AvailableMicroBenderMachines.FirstOrDefault();			
		}

		#endregion IDialogAware Interface Implementation --------------------------------------------------------------------------

		#region Helper Functions --------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		private void InitializeProperties()
		{
			MicroBenderName = "Unknown";
			MicroBenderIpAddress = "10.0.2.156";

			AvailableMicroBenderMachines = new ObservableCollection<IWireBendingMachine>();
			_configuredMicroBenderMachines = new List<IWireBendingMachine>();
		}

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks if the add bending machines command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteDoAddBenderMachinesCommand()
		{
			return (SelectedMicroBenderMachine != null);
		}

		/// <summary>
		/// Adds a bending machine 
		/// </summary>
		private void ExecuteDoAddBenderMachinesCommand()
		{
			if (SelectedMicroBenderMachine == null)
			{
				return;
			}

			if(!_configuredMicroBenderMachines.Contains(SelectedMicroBenderMachine))
			{
				_configuredMicroBenderMachines.Add(SelectedMicroBenderMachine);							
			}			

			MicroBenderName = "Unknown";
			MicroBenderIpAddress = "10.0.2.156";
			SelectedMicroBenderMachine = null;

			/*
			var resultParameters = new DialogParameters { { "MicroBenderMachine", SelectedMicroBenderMachine } };
			RequestClose?.Invoke(new DialogResult(ButtonResult.OK, resultParameters)); 
			*/
		}

		/// <summary>
		/// Cancels the add bending machine command function
		/// </summary>
		private void ExecuteCancelAddBenderMachinesCommand()
		{
			if((_configuredMicroBenderMachines != null) && _configuredMicroBenderMachines.Any())
			{
				if(SelectedMicroBenderMachine == null)
				{
					SelectedMicroBenderMachine = _configuredMicroBenderMachines.FirstOrDefault();
				}
				var resultParameters = new DialogParameters { { "ConfiguredMicroBenderMachines", _configuredMicroBenderMachines } };
				RequestClose?.Invoke(new DialogResult(ButtonResult.OK, resultParameters));

				return;
			}

			RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
		}

		/// <summary>
		/// Checks if the search bending machines by name command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteSearchBenderByNameCommand()
		{
			return (!string.IsNullOrEmpty(MicroBenderName) && (MicroBenderName != "Unknown"));
		}

		/// <summary>
		/// Searches a bending machine by name 
		/// </summary>
		private void ExecuteSearchBenderByNameCommand()
		{			
		}

		/// <summary>
		/// Checks if the search bending machines by IP Address command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteSearchBenderByIpCommand()
		{
			return !string.IsNullOrEmpty(MicroBenderIpAddress);
		}

		/// <summary>
		/// Searches a bending machine by IP Address 
		/// </summary>
		private void ExecuteSearchBenderByIpCommand()
		{
			/*
			var ipAddressTokens = MicroBenderIpAddress.Split(".");
			
			var cleanIpAddress = string.Empty; 
			for(var index = 0; index < ipAddressTokens.Length; index++)
			{
				var numberToken = short.Parse(ipAddressTokens[index]);
				cleanIpAddress += (index < ipAddressTokens.Length - 1)? $"{numberToken}." : $"{numberToken}";
			}
			*/

			var microBender = MicroBender.FindMachineAtIpAddress(MicroBenderIpAddress);
			if(microBender == null)
			{
				return;
			}
			if(!AvailableMicroBenderMachines.Contains(microBender))
			{
				AvailableMicroBenderMachines.Add(microBender);
			}

			SelectedMicroBenderMachine = microBender; 		
		}

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
