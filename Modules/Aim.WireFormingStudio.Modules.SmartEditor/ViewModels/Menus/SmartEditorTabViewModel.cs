namespace Aim.WireFormingStudio.Modules.SmartEditor.ViewModels.Menus
{
	#region Using Directives ------------------------------------------------------------------------------------------------------

	using System;	
	using System.Linq;	
	using System.Collections.ObjectModel;
	using System.Windows.Media.Imaging;
	
	using Microsoft.Win32;

	using Prism;
	using Prism.Events;
	using Prism.Commands;
	using Prism.Regions;
	using Prism.Services.Dialogs;

	using AosLibraries.SharedInterfaces.Machines;
	using AosLibraries.SharedInterfaces.DomainData.Services;
	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;

	using AosLibraries.Kernel.DataAccess.Repositories;
			
	using Core.PrismEx;
	using Core.ViewModels;
	using Core.Events;
	using Core.Events.Views;

	using Languages;

	#endregion Using Directives ---------------------------------------------------------------------------------------------------

	public class SmartEditorTabViewModel : ViewModelBase, IActiveAware, IDockAware, IDisposable
	{
		#region Member Variables --------------------------------------------------------------------------------------------------     

		/// <summary>
		/// Hook to the Region Manager
		/// </summary>
		private readonly IRegionManager _regionManager;

		/// <summary>
		/// Hook to the Event Aggregator
		/// </summary>
		private readonly IEventAggregator _eventAggregator;

		/// <summary>
		/// Dialog service
		/// </summary>
		private readonly IDialogService _dialogService;

		/// <summary>
		/// Hook to the wire bending service
		/// </summary>
		private readonly IWireBendingService _wireBendingService;

		/// <summary>
		/// Hook to the wire forming studion document manager
		/// </summary>
		private readonly IWfsDocumentManager _wfsDocumentManager;

		/// <summary>
		/// List of available designer selection modes
		/// </summary>
		private ObservableCollection<IWireBendingMachine> _benderMachinesList;

		/// <summary>
		/// Selected selection mode
		/// </summary>
		private IWireBendingMachine _selectedBenderMachine;

		/// <summary>
		/// Content pane title
		/// </summary>
		private string _header;

		/// <summary>
		/// Content pane image
		/// </summary>
		private BitmapImage _image;

		/// <summary>
		/// 
		/// </summary>
		private bool _isActive;

		/// <summary>
		/// Is this object disposed
		/// </summary>
		private bool _disposed;

		#endregion Member Variables -----------------------------------------------------------------------------------------------

		#region Constructors ------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="regionManager"></param>
		/// <param name="eventAggregator"></param>
		/// <param name="dialogService"></param>       
		/// <param name="wireBendingService"></param>
		/// <param name="wfsDocumentManager"></param>
		public SmartEditorTabViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService,
										IWireBendingService wireBendingService, IWfsDocumentManager wfsDocumentManager)
		{
			_regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
			_dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
			_wireBendingService = wireBendingService ?? throw new ArgumentNullException(nameof(wireBendingService));
			_wfsDocumentManager = wfsDocumentManager ?? throw new ArgumentNullException(nameof(wfsDocumentManager));

			////////////////////////////////////////////////////////////////////////////////////
			// Commands
			//
			ConnectBenderMachineCommand = new DelegateCommand(ExecuteConnectBenderMachineCommand, CanExecuteConnectBenderMachineCommand);
			PowerOnBenderMachineCommand = new DelegateCommand(ExecutePowerOnBenderMachineCommand, CanExecutePowerOnBenderMachineCommand);
			PowerOffBenderMachineCommand = new DelegateCommand(ExecutePowerOffBenderMachineCommand, CanExecutePowerOffBenderMachineCommand);
			HomeBenderMachineCommand = new DelegateCommand(ExecuteHomeBenderMachineCommand, CanExecuteHomeBenderMachineCommand);
			StartBendingCommand = new DelegateCommand(ExecuteStartBendingCommand, CanExecuteStartBendingCommand);
			StopBendingCommand = new DelegateCommand(ExecuteStopBendingCommand, CanExecuteStopBendingCommand);
			PauseBendingCommand = new DelegateCommand(ExecutePauseBendingCommand, CanExecutePauseBendingCommand);
			LoadJdrlFileCommand = new DelegateCommand(ExecuteLoadJdrlFileCommand, CanExecuteLoadJdrlFileCommand);
			SendJdrlFileCommand = new DelegateCommand(ExecuteSendJdrlFileCommand, CanExecuteSendJdrlFileCommand);
			SaveJdrlFileCommand = new DelegateCommand(ExecuteSaveJdrlFileCommand, CanExecuteSaveJdrlFileCommand);

			//////////////////////////////////////////////////////////////////////////////////////
			// Events

			InitializeProperties();
		}


		/// <summary>
		/// Destructor
		/// </summary>
		~SmartEditorTabViewModel()
		{
			Dispose(false);
		}

		#endregion Constructors ---------------------------------------------------------------------------------------------------

		#region Public Properties -------------------------------------------------------------------------------------------------

		/// <summary>
		/// Loads a model to the view port 
		/// </summary>
		public DelegateCommand ConnectBenderMachineCommand { get; set; }

		/// <summary>
		/// Loads a model to the view port 
		/// </summary>
		public DelegateCommand PowerOnBenderMachineCommand { get; set; }

		/// <summary>
		/// Sets the model occlusal plane
		/// </summary>
		public DelegateCommand PowerOffBenderMachineCommand { get; set; }

		/// <summary>
		/// Sets the model curvature
		/// </summary>
		public DelegateCommand HomeBenderMachineCommand { get; set; }

		/// <summary>
		/// Starts the wire bending process
		/// </summary>
		public DelegateCommand StartBendingCommand { get; set; }

		/// <summary>
		/// Stops the wire bending process
		/// </summary>
		public DelegateCommand StopBendingCommand { get; set; }

		/// <summary>
		/// Pauses the wire bending process
		/// </summary>
		public DelegateCommand PauseBendingCommand { get; set; }

		/// <summary>
		/// Imports a JDRL file 
		/// </summary>
		public DelegateCommand LoadJdrlFileCommand { get; set; }

		/// <summary>
		/// Sends a JDRL file for bending 
		/// </summary>
		public DelegateCommand SendJdrlFileCommand { get; set; }

		/// <summary>
		/// Sends a JDRL file for bending 
		/// </summary>
		public DelegateCommand SaveJdrlFileCommand { get; set; }

		/// <summary>
		/// List of available bender machine
		/// </summary>
		public ObservableCollection<IWireBendingMachine> BenderMachinesList
		{
			get => _benderMachinesList;
			set
			{
				_benderMachinesList = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Selected bender machine
		/// </summary>
		public IWireBendingMachine SelectedBenderMachine
		{
			get => _selectedBenderMachine;
			set
			{
				if (value == _selectedBenderMachine)
				{
					return;
				}

				_selectedBenderMachine = value;
				RaisePropertyChanged();
			}
		}

		#endregion Public Properties ----------------------------------------------------------------------------------------------

		#region Public Functions --------------------------------------------------------------------------------------------------

		#endregion Public Functions -----------------------------------------------------------------------------------------------

		#region ViewModel Override Functions --------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected override string OnValidate(string propertyName)
		{
			return base.OnValidate(propertyName);
		}

		#endregion ViewModel Override Functions ------------------------------------------------------------------------------------------------------

		#region IActiveAware Interface Implementation ------------------------------------------------------------------------------------------------

		public event EventHandler IsActiveChanged;

		/// <summary>
		/// 
		/// </summary>
		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (value == _isActive)
				{
					return;
				}

				_isActive = value;
				RaisePropertyChanged();

				if (_isActive)
				{
					_eventAggregator.GetEvent<ActiveViewChangedEvent>().Publish(@"SmartEditoTab");
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		protected void OnIsActiveChanged(object sender, EventArgs args)
		{
			IsActiveChanged?.Invoke(sender, args);
		}

		#endregion IActiveAware Interface Implementation ---------------------------------------------------------------------------------------------

		#region IDockAware Interface Implementation --------------------------------------------------------------------------------------------------

		/// <summary>
		/// Content pane title
		/// </summary>
		public string Header
		{
			get => _header;
			set
			{
				if (value == _header)
				{
					return;
				}

				_header = value;
				RaisePropertyChanged();
			}
		}


		/// <summary>
		/// Content pane image 
		/// </summary>
		public BitmapImage Image
		{
			get => _image;
			set
			{
				if (Equals(value, _image))
				{
					return;
				}

				_image = value;
				RaisePropertyChanged();
			}
		}

		#endregion IDockAware Interface Implementation -----------------------------------------------------------------------------------------------

		#region IDisposable Interface Implementation -------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		public virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
			}

			_disposed = true;
		}

		#endregion IDisposable Interface Implementation ---------------------------------------------------------------------------

		#region Helper Functions --------------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes view model properties
		/// </summary>
		private void InitializeProperties()
		{
			ViewTitle = WireFormingStudioStrings.SmartEditor_TabView_Title;
			Header = WireFormingStudioStrings.SmartEditor_TabView_Header;
			var imageUri = new Uri("pack://application:,,,/Aim.WireFormingStudio.Modules.SmartEditor;component/Resources/Images/16x16/Printer.png");
			Image = new BitmapImage(imageUri);

			/*
			_wireBendingService.AddAddressToConnectionsAddresses("192.168.254.42");
			_wireBendingService.AddAddressToConnectionsAddresses("192.168.254.45");
			_wireBendingService.RefreshBenderMachineConnections();
			*/

			// Bender machines
			var connectedBenderMachines = _wireBendingService.AvailableBenderMachines.Values.ToList();
			if (connectedBenderMachines == null)
			{
				BenderMachinesList = new ObservableCollection<IWireBendingMachine>();
			}			
			else
			{
				BenderMachinesList = new ObservableCollection<IWireBendingMachine>(connectedBenderMachines);
				if (BenderMachinesList.Any())
				{
					SelectedBenderMachine = BenderMachinesList.First();
				}
			}

			_disposed = false;
		}
		
		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Verifies if the Connect command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteConnectBenderMachineCommand()
		{
			return (SelectedBenderMachine != null);
		}


		/// <summary>
		/// PowersOn the selected bender machine
		/// </summary>
		private void ExecuteConnectBenderMachineCommand()
		{

		}


		/// <summary>
		/// Verifies if the PowerOn command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecutePowerOnBenderMachineCommand()
		{
			return (SelectedBenderMachine != null);
		}


		/// <summary>
		/// PowersOn the selected bender machine
		/// </summary>
		private void ExecutePowerOnBenderMachineCommand()
		{

		}


		/// <summary>
		/// Verifies if the PowerOff command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecutePowerOffBenderMachineCommand()
		{
			return (SelectedBenderMachine != null);
		}


		/// <summary>
		/// Powers off the selected bender machine
		/// </summary>
		private void ExecutePowerOffBenderMachineCommand()
		{
		}


		/// <summary>
		/// Verifies if the Home command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteHomeBenderMachineCommand()
		{
			return (SelectedBenderMachine != null);
		}


		/// <summary>
		/// Sends the selected bender machine home 
		/// </summary>
		private void ExecuteHomeBenderMachineCommand()
		{
		}

		/// <summary>
		/// Verifies if the start bending command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteStartBendingCommand()
		{
			return (SelectedBenderMachine != null);
		}

		/// <summary>
		/// Starts the wire bending process of the selected wires
		/// </summary>
		private void ExecuteStartBendingCommand()
		{
			//_eventAggregator.GetEvent<BendingProcessCommandChangedEvent>().Publish("Start");
		}

		/// <summary>
		/// Verifies if the stop bending command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteStopBendingCommand()
		{
			return (SelectedBenderMachine != null);
		}

		/// <summary>
		/// Stops the wire bending process of the selected wires 
		/// </summary>
		private void ExecuteStopBendingCommand()
		{
			// _eventAggregator.GetEvent<BendingProcessCommandChangedEvent>().Publish("Stop");
		}

		/// <summary>
		/// Verifies if the pause bending command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecutePauseBendingCommand()
		{
			return (SelectedBenderMachine != null);
		}

		/// <summary>
		/// Pauses the wire bending process of the selected wires 
		/// </summary>
		private void ExecutePauseBendingCommand()
		{
			// _eventAggregator.GetEvent<BendingProcessCommandChangedEvent>().Publish("Pause");
		}

		/// <summary>
		/// Verifies if the load JDRL file command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteLoadJdrlFileCommand()
		{
			return true;
		}


		/// <summary>
		/// Loads a JDRL file
		/// </summary>
		private void ExecuteLoadJdrlFileCommand()
		{
			// Open the file browser dialog
			var fileOpenDlg = new OpenFileDialog
			{
				InitialDirectory = FolderNames.AosExportedWiresFolder,
				FileName = FolderNames.DefaultJdrlFilename,
				DefaultExt = FolderNames.JawDrawWireAsLraFileExtension,
				Filter = FolderNames.JawDrawWireAsLraFilenameFilter
			};

			var result = fileOpenDlg.ShowDialog();
			if (result != true)
			{
				return;
			}

			var fullPathFileName = fileOpenDlg.FileName;
			if (string.IsNullOrEmpty(fullPathFileName))
			{
				return;
			}

			_eventAggregator.GetEvent<ImportJdrlFileEvent>().Publish(fullPathFileName);			
		}

		/// <summary>
		/// Verifies if the send JDRL file command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteSendJdrlFileCommand()
		{
			return true;
		}


		/// <summary>
		/// Sends a JDRL file to a machine
		/// </summary>
		private void ExecuteSendJdrlFileCommand()
		{
			if (_wfsDocumentManager.ActiveWfsDocument == null)
			{
				return;
			}

			var wfsDocumentLocation = _wfsDocumentManager.ActiveWfsDocument.DocumentLocation;
			if(string.IsNullOrEmpty(wfsDocumentLocation))
			{
				return;
			}

			_eventAggregator.GetEvent<SendJdrlFileEvent>().Publish(wfsDocumentLocation);		
		}

		/// <summary>
		/// Verifies if the save JDRL file command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteSaveJdrlFileCommand()
		{
			return true;
		}


		/// <summary>
		/// Saves a JDRL file to a machine
		/// </summary>
		private void ExecuteSaveJdrlFileCommand()
		{
			if (_wfsDocumentManager.ActiveWfsDocument == null)
			{
				return;
			}

			_wfsDocumentManager.ActiveWfsDocument.SaveWfsDocument();

			_eventAggregator.GetEvent<SavedJdrlFileEvent>().Publish();
		}

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#region Events ------------------------------------------------------------------------------------------------------------

		#endregion Events ---------------------------------------------------------------------------------------------------------

		#endregion Helper Functions ----------------------------------------------------------------------------------------------- 
	}
}
