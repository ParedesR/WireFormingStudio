namespace Aim.WireFormingStudio.Modules.EsViewPort.ViewModels.Menus
{
	#region Using Directives ------------------------------------------------------------------------------------------------------

	using System;
	using System.Windows.Media.Imaging;

	using Microsoft.Win32;

	using Prism;
	using Prism.Regions;
	using Prism.Events;
	using Prism.Commands;
	using Prism.Services.Dialogs;

	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;
	using AosLibraries.SharedInterfaces.DomainData.Services;

	using AosLibraries.Kernel.DataAccess.Repositories;

	using Core.Events.LineModels;
	using Core.PrismEx;
	using Core.ViewModels;

	using Languages;

	#endregion Using Directives ---------------------------------------------------------------------------------------------------

	/// <summary>
	/// Eye Shot view port tab view model
	/// </summary>
	public class EsViewPortTabViewModel : ViewModelBase, IActiveAware, IDockAware, IDisposable
	{
		#region Member Variables ---------------------------------------------------------------------------------------------------------------------      

		/// <summary>
		/// Hook to the Region Manager
		/// </summary>
		private readonly IRegionManager _regionManager;

		/// <summary>
		/// 
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
		/// Hook to the wire forming studio document manager
		/// </summary>
		private readonly IWfsDocumentManager _wfsDocumentManager;

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

		/// <summary>
		/// Flag to indicate if at least one line model has been loaded
		/// </summary>
		private bool _isLineModelLoaded;

		#endregion Member Variables ------------------------------------------------------------------------------------------------------------------

		#region Constructors -------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="regionManager"></param>
		/// <param name="eventAggregator"></param>
		/// <param name="dialogService"></param>
		/// <param name="wireBendingService"></param>
		/// <param name="wfsDocumentManager"></param>
		public EsViewPortTabViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService,
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
			OpenLineModelCommand = new DelegateCommand(ExecuteOpenLineModelCommand, CanExecuteOpenLineModelCommand);

			ViewTitle = WireFormingStudioStrings.SmartEditor_View_Title;
			Header = WireFormingStudioStrings.SmartEditor_TabView_Header;
			var imageUri = new Uri("pack://application:,,,/Aim.WireFormingStudio.Modules.SmartEditor;component/Resources/Images/16x16/Printer.png");
			Image = new BitmapImage(imageUri);

			InitializeProperties();
		}


		/// <summary>
		/// Destructor
		/// </summary>
		~EsViewPortTabViewModel()
		{
			Dispose(false);
		}

		#endregion Constructors ----------------------------------------------------------------------------------------------------------------------

		#region Public Properties --------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Loads a model to the view port 
		/// </summary>
		public DelegateCommand OpenLineModelCommand { get; set; }

		/// <summary>
		/// Flag to indicate if at least one line model has been loaded
		/// </summary>
		public bool IsLineModelLoaded
		{
			get => _isLineModelLoaded;
			set
			{
				if (value == _isLineModelLoaded)
				{
					return;
				}

				_isLineModelLoaded = value;
				RaisePropertyChanged();
			}
		}

		#endregion Public Properties -----------------------------------------------------------------------------------------------------------------

		#region ViewModel Override Functions ---------------------------------------------------------------------------------------------------------

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
		/// Initializes the class properties
		/// </summary>
		private void InitializeProperties()
		{
			_disposed = false;

		}

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Verifies if the Open Line Model command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteOpenLineModelCommand()
		{
			return true;
		}

		/// <summary>
		/// Loads a line model into the View Port
		/// </summary>
		private void ExecuteOpenLineModelCommand()
		{
			// Open the file browser dialog
			var fileOpenDlg = new OpenFileDialog
			{
				InitialDirectory = FolderNames.ApplicationLineModelFilesDataFolder,
				FileName = FolderNames.DefaultLineModelFilename,
				DefaultExt = FolderNames.DefaultLineModelFilenameExtension,
				Filter = FolderNames.DefaultLineModelFilenameFilter
			};

			var result = fileOpenDlg.ShowDialog();
			if (result != true)
			{
				return;
			}

			var fileName = fileOpenDlg.FileName;
			if (string.IsNullOrEmpty(fileName))
			{
				return;
			}

			_eventAggregator.GetEvent<OpenLineModelEvent>().Publish(fileName);

			IsLineModelLoaded = true;
		}

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#region Events ------------------------------------------------------------------------------------------------------------

		#endregion Events ------------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
