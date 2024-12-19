namespace Aim.WireFormingStudio.Modules.EsViewPort.ViewModels
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using System;
	using System.Windows.Media.Imaging;
	
	using Prism;
	using Prism.Regions;
	using Prism.Events;
	using Prism.Services.Dialogs;

	using devDept.Eyeshot.Entities;

	using AosLibraries.SharedInterfaces.Cad.Services;
	using AosLibraries.SharedInterfaces.Cad.Mesh;
	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;
	using AosLibraries.SharedInterfaces.DomainData.Services;
	
	using Core.PrismEx;
	using Core.ViewModels;
		
	using Languages;
	using Models.Cad;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// 
	/// </summary>	
	public class EsViewPortViewModel : ViewModelBase, IActiveAware, IDockAware, IDisposable
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
				
		#endregion Member Variables ------------------------------------------------------------------------------------------------------------------

		#region Constructors -------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="regionManager"></param>
		/// <param name="eventAggregator"></param>
		/// <param name="dialogService"></param>
		/// <param name="viewPortControlService"></param>
		/// <param name="wireBendingService"></param>
		/// <param name="wfsDocumentManager"></param>
		public EsViewPortViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService,
									ICadViewPortControlService<IEyeShotDentalMesh> viewPortControlService, IWireBendingService wireBendingService,
									IWfsDocumentManager wfsDocumentManager)
		{
			_regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
			_dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
			ViewPortControlService = viewPortControlService ?? throw new ArgumentNullException(nameof(viewPortControlService));
			_wireBendingService = wireBendingService ?? throw new ArgumentNullException(nameof(wireBendingService));
			_wfsDocumentManager = wfsDocumentManager ?? throw new ArgumentNullException(nameof(wfsDocumentManager));
			
			InitializeProperties();
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~EsViewPortViewModel()
		{
			Dispose(false);
		}

		#endregion Constructors ----------------------------------------------------------------------------------------------------------------------

		#region Public Properties --------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Hook to the CAD Editor control service
		/// </summary>
		public ICadViewPortControlService<IEyeShotDentalMesh> ViewPortControlService { get; }

		/// <summary>
		/// Hook to the CAD View Port.  We need it for data binding
		/// </summary>
		public EyeShotViewPort CadViewPort => (EyeShotViewPort)ViewPortControlService?.CadViewPort;

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
			ViewTitle = WireFormingStudioStrings.EsViewPort_View_Title;
			Header = WireFormingStudioStrings.EsViewPort_TabView_Header;
			var imageUri = new Uri("pack://application:,,,/Aim.WireFormingStudio.Modules.EsViewPort;component/Resources/Images/16x16/Eye.png");
			Image = new BitmapImage(imageUri);

			CreateCadViewPort();

			_disposed = false;
		}

		/// <summary>
		/// Creates the eye shot view port
		/// </summary>
		private void CreateCadViewPort()
		{
			ViewPortControlService.CreateCadViewPort();
		}

		#region Commands ----------------------------------------------------------------------------------------------------------

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#region Events ------------------------------------------------------------------------------------------------------------

		#endregion Events ------------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
