namespace Aim.WireFormingStudio.Host.ViewModels
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using System;
	using System.IO;
	using System.Linq;
	using System.Windows;
	using System.Collections.ObjectModel;

	using Microsoft.Win32;

	using Prism.Commands;
	using Prism.Regions;
	using Prism.Mvvm;
	using Prism.Events;
	using Prism.Services.Dialogs;
		
	using AosLibraries.SharedInterfaces.Machines;
	using AosLibraries.SharedInterfaces.DomainData.Services;
	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;

	using AosLibraries.Kernel.DataAccess.Repositories;

	using Aim.WireFormingStudio.Core;
	using Aim.WireFormingStudio.Core.Events;
	
	using Languages;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Main window view model
	/// </summary>
	public class MainViewModel : BindableBase
	{
		#region Member Variables ---------------------------------------------------------------------------------------------------
		
		/// <summary>
		/// Hook to the region manager
		/// </summary>
		private readonly IRegionManager _regionManager;

		/// <summary>
		/// Hook to the region manager
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
		/// Hook to the wire bending service
		/// </summary>
		private readonly IWfsDocumentManager _wfsDocumentManager;

		/// <summary>
		/// Main window title
		/// </summary>
		private string _title;

		/// <summary>
		/// Navigation command
		/// </summary>
		private DelegateCommand<string> _navigateCommand;

		/// <summary>
		/// Micro bending machine to be added
		/// </summary>
		private ObservableCollection<IWireBendingMachine> _configuredMicroBenderMachines;

		/// <summary>
		/// Micro bending machine to be added
		/// </summary>
		private IWireBendingMachine _microBenderMachine;

		/// <summary>
		/// List of parameters for setting up a bending machine
		/// </summary>
		private string _benderMachineSetUpOptions;

		#endregion Member Variables ------------------------------------------------------------------------------------------------

		#region Constructors --------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="regionManager"></param>
		/// <param name="eventAggregator"></param>
		/// <param name="dialogService"></param>
		/// <param name="wireBendingService"></param>
		/// <param name="wfsDocumentManager"></param>
		public MainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService,
								IWireBendingService wireBendingService, IWfsDocumentManager wfsDocumentManager)
		{
			_regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
			_dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
			_wireBendingService = wireBendingService ?? throw new ArgumentNullException(nameof(wireBendingService));
			_wfsDocumentManager = wfsDocumentManager ?? throw new ArgumentNullException(nameof(wfsDocumentManager));

			//////////////////////////////////////////////////////////////////////////////////////
			// Events
			//
			_eventAggregator.GetEvent<JdrlFileLoadedEvent>().Subscribe(OnJdrlFileLoadedEvent, ThreadOption.UIThread);
			_eventAggregator.GetEvent<JdrlFileSentEvent>().Subscribe(OnJdrlFileSentEvent, ThreadOption.UIThread);

			////////////////////////////////////////////////////////////////////////////////////
			// Commands
			//
			// File Tab
			ShowImportFileViewCommand = new DelegateCommand(ExecuteShowImportFileViewCommand, CanExecuteShowImportFileViewCommand);
			ShowBenderMachinesViewCommand = new DelegateCommand(ExecuteShowBenderMachinesViewCommand, CanExecuteShowBenderMachinesViewCommand);
			ShowBenderMachineSetUpViewCommand = new DelegateCommand(ExecuteShowBenderMachineSetUpViewCommand, CanExecuteShowBenderMachineSetUpViewCommand);

			ExitApplicationCommand = new DelegateCommand(ExecuteExitApplicationCommand, CanExecuteExitApplicationCommand);

			Title = WireFormingStudioStrings.MainView_Title;

			ConfiguredMicroBenderMachines = new ObservableCollection<IWireBendingMachine>();			
		}

		#endregion Constructors -----------------------------------------------------------------------------------------------------

		#region Public Properties ---------------------------------------------------------------------------------------------------

		/// <summary>
		/// Opens up the import file form
		/// </summary>
		public DelegateCommand ShowImportFileViewCommand { get; set; }

		/// <summary>
		/// Opens up the bender machines view
		/// </summary>
		public DelegateCommand ShowBenderMachinesViewCommand { get; set; }

		/// <summary>
		/// Opens up the bender machine setup form
		/// </summary>
		public DelegateCommand ShowBenderMachineSetUpViewCommand { get; set; }

		/// <summary>
		/// Closes down the application
		/// </summary>
		public DelegateCommand ExitApplicationCommand { get; set; }

		/// <summary>
		/// Gets / Sets the main window title property
		/// </summary>
		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		public DelegateCommand<string> NavigateCommand => _navigateCommand ??= new DelegateCommand<string>(ExecuteNavigateCommand);

		/// <summary>
		/// Micro bending machine to be added
		/// </summary>
		public ObservableCollection<IWireBendingMachine> ConfiguredMicroBenderMachines
		{
			get => _configuredMicroBenderMachines;
			set => SetProperty(ref _configuredMicroBenderMachines, value);
		}

		/// <summary>
		/// Micro bending machine to be added
		/// </summary>
		public IWireBendingMachine MicroBenderMachine
		{
			get => _microBenderMachine;
			set
			{
				if (value == _microBenderMachine)
				{
					return;
				}

				SetProperty(ref _microBenderMachine, value);

				ShowBenderMachineSetUpViewCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// List of parameters for setting up bending machines
		/// </summary>
		public string BenderMachineSetUpOptions
		{
			get => _benderMachineSetUpOptions;
			set
			{
				if (value == _benderMachineSetUpOptions)
				{
					return;
				}

				SetProperty(ref _benderMachineSetUpOptions, value);
			}
		}

		#endregion Public Properties ------------------------------------------------------------------------------------------------

		#region Helper Functions --------------------------------------------------------------------------------------------------

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="navigationPath"></param>
		private void ExecuteNavigateCommand(string navigationPath)
		{
			if (string.IsNullOrEmpty(navigationPath))
			{
				throw new ArgumentNullException(nameof(navigationPath));
			}

			_regionManager.RequestNavigate(HostRegionNames.ContentRegion, navigationPath);
		}

		/// <summary>
		/// Checks if the import file command can be executed
		/// </summary>
		private bool CanExecuteShowImportFileViewCommand()
		{
			return true;
		}

		/// <summary>
		/// Opens the file import form
		/// </summary>
		private void ExecuteShowImportFileViewCommand()
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
		/// 
		/// </summary>
		private bool CanExecuteShowBenderMachinesViewCommand()
		{
			return true;
		}

		/// <summary>
		/// Opens the add bender machine view
		/// </summary>
		private void ExecuteShowBenderMachinesViewCommand()
		{			
			var parameters = new DialogParameters { { "ConfiguredMicroBenderMachines", ConfiguredMicroBenderMachines } };
			_dialogService.ShowDialog("BenderMachinesView", parameters, OnBenderMachinesViewClosed);
		}

		/// <summary>
		/// 
		/// </summary>
		private bool CanExecuteShowBenderMachineSetUpViewCommand()
		{
			return (MicroBenderMachine != null);
		}

		/// <summary>
		/// Opens the bender machine set up view
		/// </summary>
		private void ExecuteShowBenderMachineSetUpViewCommand()
		{
			var parameters = new DialogParameters { { "UpdatedMicroBenderMachine", MicroBenderMachine } };
			_dialogService.ShowDialog("BenderMachineSetUpView", parameters, OnBenderMachineSetUpViewClosed);
		}

		/// <summary>
		/// 
		/// </summary>
		private bool CanExecuteExitApplicationCommand()
		{
			return true;
		}

		/// <summary>
		/// Closes the application
		/// </summary>
		private void ExecuteExitApplicationCommand()
		{
			Application.Current.Shutdown();
		}

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#region Events ------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isLoaded"></param>
		private void OnJdrlFileLoadedEvent(bool isLoaded)
		{
			if(!isLoaded)
			{
				return;
			}

			// Update view title
			if(_wfsDocumentManager.ActiveWfsDocument == null)
			{
				return;
			}

			// Update view title			
			var fileName = Path.GetFileNameWithoutExtension(_wfsDocumentManager.ActiveWfsDocument.DocumentLocation);
			Title = string.IsNullOrEmpty(fileName) ? WireFormingStudioStrings.MainView_Title :
													$"{WireFormingStudioStrings.MainView_Title} - {fileName}";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isSent"></param>
		private void OnJdrlFileSentEvent(bool isSent)
		{

		}

		/// <summary>
		/// On bender machines view closed
		/// </summary>
		private void OnBenderMachinesViewClosed(IDialogResult result)
		{
			if (result.Result != ButtonResult.OK)
			{
				return;
			}

			if ((result.Parameters == null) || !result.Parameters.ContainsKey("ConfiguredMicroBenderMachines"))
			{
				return;
			}

			ConfiguredMicroBenderMachines = result.Parameters.GetValue<ObservableCollection<IWireBendingMachine>>("ConfiguredMicroBenderMachines");			
			if((ConfiguredMicroBenderMachines == null) || !ConfiguredMicroBenderMachines.Any())
			{
				return;
			}			
			foreach(var benderMachine in ConfiguredMicroBenderMachines)
			{
				var benderMachineKey = new Tuple<long, string>(benderMachine.MachineId, benderMachine.IpAddress);
				if(_wireBendingService.AvailableBenderMachines.ContainsKey(benderMachineKey))
				{
					continue;
				}
				_wireBendingService.AvailableBenderMachines.Add(benderMachineKey, benderMachine);
			}
			MicroBenderMachine = ConfiguredMicroBenderMachines.FirstOrDefault(microBender => microBender.MachineStatus != WireBendingMachinesGlobals.WireBendingMachineStatus.NotConnected);
			// Set the active bender in the service
			_wireBendingService.SelectedBenderMachine = MicroBenderMachine;
		}

		/// <summary>
		/// On bender machine set up view closed
		/// </summary>
		private void OnBenderMachineSetUpViewClosed(IDialogResult result)
		{
			if (result.Result != ButtonResult.OK)
			{
				return;
			}

			if ((result.Parameters == null) || !result.Parameters.ContainsKey("UpdatedMicroBenderMachine"))
			{
				return;
			}

			MicroBenderMachine = result.Parameters.GetValue<IWireBendingMachine>("UpdatedMicroBenderMachine");			
		}

		#endregion Events ------------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
