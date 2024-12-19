namespace Aim.WireFormingStudio.Modules.SmartEditor.ViewModels
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using System;
	using System.IO;
	using System.Linq;	
	using System.Windows.Media.Imaging;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;

	using Prism;
	using Prism.Regions;
	using Prism.Events;
	using Prism.Commands;
	using Prism.Services.Dialogs;

	using AosLibraries.SharedInterfaces.Machines;
	using AosLibraries.SharedInterfaces.DomainData.Services;
	using AosLibraries.SharedInterfaces.DomainData.WireEntities;
	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;
	using static AosLibraries.SharedInterfaces.Machines.WireBendingMachinesGlobals;

	using AosLibraries.Kernel.DomainData.Core.OrthodonticWires;
	using static AosLibraries.Kernel.DataAccess.Repositories.DataSerialization.WfsDocumentHelper;

	using Core.Events;
	using Core.PrismEx;
	using Core.ViewModels;

	using DataPresenters;
	using Languages;
	
	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	public class SmartEditorViewModel : ViewModelBase, IActiveAware, IDockAware, IDisposable
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
		/// 
		/// </summary>
		private ObservableCollection<SegmentAsLrarDataProvider> _segmentsAsLrarCollection;

		/// <summary>
		/// Selected LRAr record in the data grid
		/// </summary>
		private SegmentAsLrarDataProvider _selectedSegmentAsLrar;

		/// <summary>
		/// Selected LRAr record in the data grid
		/// </summary>
		private int _selectedSegmentAsLrarIndex;

		/// <summary>
		/// Flag that indicates if the grid data was modified
		/// </summary>
		private bool _isGridDataDirty;

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
		public SmartEditorViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService,
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
			AddRecordCommand = new DelegateCommand(ExecuteAddRecordCommand, CanExecuteAddRecordCommand);
			InsertRecordCommand = new DelegateCommand(ExecuteInsertRecordCommand, CanExecuteInsertRecordCommand);
			DeleteRecordCommand = new DelegateCommand(ExecuteDeleteRecordCommand, CanExecuteDeleteRecordCommand);
			MoveUpRecordCommand = new DelegateCommand(ExecuteMoveUpRecordCommand, CanExecuteMoveUpRecordCommand);
			MoveDownRecordCommand = new DelegateCommand(ExecuteMoveDownRecordCommand, CanExecuteMoveDownRecordCommand);

			//////////////////////////////////////////////////////////////////////////////////////
			// Events
			//
			_eventAggregator.GetEvent<ImportJdrlFileEvent>().Subscribe(OnImportJdrlFileEvent, ThreadOption.UIThread);
			_eventAggregator.GetEvent<SendJdrlFileEvent>().Subscribe(OnSendJdrlFileEvent, ThreadOption.UIThread);
			_eventAggregator.GetEvent<SavedJdrlFileEvent>().Subscribe(OnSavedJdrlFileEvent, ThreadOption.UIThread);


			ViewTitle = WireFormingStudioStrings.SmartEditor_View_Title;
			Header = WireFormingStudioStrings.SmartEditor_TabView_Header;
			var imageUri = new Uri("pack://application:,,,/Aim.WireFormingStudio.Modules.SmartEditor;component/Resources/Images/16x16/Printer.png");
			Image = new BitmapImage(imageUri);

			InitializeProperties();
		}


		/// <summary>
		/// Destructor
		/// </summary>
		~SmartEditorViewModel()
		{
			Dispose(false);
		}

		#endregion Constructors ----------------------------------------------------------------------------------------------------------------------

		#region Public Properties --------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Adds a LRAr record to the data grid. 
		/// </summary>
		public DelegateCommand AddRecordCommand { get; set; }

		/// <summary>
		/// Inserts a LRAr record to the data grid. 
		/// </summary>
		public DelegateCommand InsertRecordCommand { get; set; }

		/// <summary>
		/// Deletes a LRAr record to the data grid. 
		/// </summary>
		public DelegateCommand DeleteRecordCommand { get; set; }

		/// <summary>
		/// Moves up the selected LRAr record in the data grid. 
		/// </summary>
		public DelegateCommand MoveUpRecordCommand { get; set; }

		/// <summary>
		/// Moves up the selected LRAr record in the data grid. 
		/// </summary>
		public DelegateCommand MoveDownRecordCommand { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<SegmentAsLrarDataProvider> SegmentsAsLrarCollection
		{
			get => _segmentsAsLrarCollection;
			private set => _segmentsAsLrarCollection = value;
		}

		/// <summary>
		/// Selected LRAr record in the data grid
		/// </summary>
		public SegmentAsLrarDataProvider SelectedSegmentAsLrar
		{
			get => _selectedSegmentAsLrar;
			set
			{
				_selectedSegmentAsLrar = value;
				if(_selectedSegmentAsLrar == null)
				{
					return;
				}
				SelectedSegmentAsLrarIndex = SegmentsAsLrarCollection.IndexOf(_selectedSegmentAsLrar);
			}
		}

		/// <summary>
		/// Selected LRAr record in the data grid
		/// </summary>
		public int SelectedSegmentAsLrarIndex
		{
			get => _selectedSegmentAsLrarIndex;
			set => _selectedSegmentAsLrarIndex = value;
		}

		/// <summary>
		/// Flag that indicates if the grid data was modified
		/// </summary>
		public bool IsGridDataDirty
		{
			get => _isGridDataDirty;
			set
			{
				_isGridDataDirty = value;

				if(!_isGridDataDirty)
				{
					return;
				}

				UpdateActiveDocument();
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

			if(SegmentsAsLrarCollection != null)
			{
				SegmentsAsLrarCollection.CollectionChanged -= OnSegmentAsLrarCollectionChanged;
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

			SegmentsAsLrarCollection = new ObservableCollection<SegmentAsLrarDataProvider>();
			SegmentsAsLrarCollection.CollectionChanged += OnSegmentAsLrarCollectionChanged;
			SelectedSegmentAsLrarIndex = -1;
		}
		
		/// <summary>
		/// After moving items the step sequence is re-numbered
		/// </summary>
		private void ResequenceSteps()
		{
			if(!SegmentsAsLrarCollection.Any())
			{
				return;
			}

			for (var index = 0; index < SegmentsAsLrarCollection.Count; index++)
			{
				SegmentsAsLrarCollection[index].SequenceStep = index;
			}
		}

		/// <summary>
		/// Updates the active document
		/// </summary>
		private void UpdateActiveDocument()
		{
			if(_wfsDocumentManager.ActiveWfsDocument == null)
			{
				return;
			}

			var segmentAsLrarsList = new List<IWireSegmentAsLrar>();
			foreach (var segmentAsLrar in SegmentsAsLrarCollection)
			{
				segmentAsLrarsList.Add(new WireSegmentAsLrar(segmentAsLrar.SegmentAsLrar));
			}

			_wfsDocumentManager.ActiveWfsDocument.UpdateDocumentData(segmentAsLrarsList);
		}

		#region Commands ----------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Verifies if the add record command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteAddRecordCommand()
		{
			return (SegmentsAsLrarCollection != null);
		}

		/// <summary>
		/// Adds an LRAr record to the data grid
		/// </summary>
		private void ExecuteAddRecordCommand()
		{
			IWireSegmentAsLrar segmentAsLrar;
			// If not selected segment, add at the bottom
			if (SelectedSegmentAsLrar == null)
			{
				SelectedSegmentAsLrarIndex = SegmentsAsLrarCollection.Count;
				segmentAsLrar = new WireSegmentAsLrar();
			}
			else
			{
				SelectedSegmentAsLrarIndex += 1;
				segmentAsLrar = new WireSegmentAsLrar(SelectedSegmentAsLrar.SegmentAsLrar);
			}
			
			var parameters = new DialogParameters { { "AddLrarSegment", segmentAsLrar } };
			_dialogService.ShowDialog("EditLrarSegmentView", parameters, OnEditLrarSegmentViewClosed);
		}

		/// <summary>
		/// Verifies if the insert record command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteInsertRecordCommand()
		{
			return (SegmentsAsLrarCollection != null);
		}

		/// <summary>
		/// Inserts an LRAr record to the data grid
		/// </summary>
		private void ExecuteInsertRecordCommand()
		{
			IWireSegmentAsLrar segmentAsLrar;
			// If not selected segment, insert at the top
			if (SelectedSegmentAsLrar == null)
			{
				SelectedSegmentAsLrarIndex = 0;
				segmentAsLrar = new WireSegmentAsLrar();
			}
			else
			{				
				segmentAsLrar = new WireSegmentAsLrar(SelectedSegmentAsLrar.SegmentAsLrar);
			}					 
																
			var parameters = new DialogParameters { { "AddLrarSegment", segmentAsLrar } };
			_dialogService.ShowDialog("EditLrarSegmentView", parameters, OnEditLrarSegmentViewClosed);
		}

		/// <summary>
		/// Verifies if the delete record command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteDeleteRecordCommand()
		{
			return (SegmentsAsLrarCollection != null);
		}

		/// <summary>
		/// Deletes an LRAr record to the data grid
		/// </summary>
		private void ExecuteDeleteRecordCommand()
		{
			if (SelectedSegmentAsLrar == null)
			{
				return;
			}
						
			SegmentsAsLrarCollection.Remove(SelectedSegmentAsLrar);
			ResequenceSteps();
			IsGridDataDirty = true;
			UpdateActiveDocument();
		}

		/// <summary>
		/// Verifies if the move up record command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteMoveUpRecordCommand()
		{
			return ((SegmentsAsLrarCollection != null) && SegmentsAsLrarCollection.Any());
		}

		/// <summary>
		/// Moves up the selected LRAr record to the data grid
		/// </summary>
		private void ExecuteMoveUpRecordCommand()
		{
			if(SelectedSegmentAsLrar == null)
			{
				return;
			}

			SelectedSegmentAsLrarIndex = SegmentsAsLrarCollection.IndexOf(SelectedSegmentAsLrar);
			if(SelectedSegmentAsLrarIndex <= 0)
			{
				return;
			}

			SegmentsAsLrarCollection.Remove(SelectedSegmentAsLrar);
			SegmentsAsLrarCollection.Insert((SelectedSegmentAsLrarIndex - 1), SelectedSegmentAsLrar);
			IsGridDataDirty = true;
			// ResequenceSteps();
			UpdateActiveDocument();			
		}

		/// <summary>
		/// Verifies if the move down record command can be executed
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteMoveDownRecordCommand()
		{
			return ((SegmentsAsLrarCollection != null) && SegmentsAsLrarCollection.Any());
		}

		/// <summary>
		/// Moves down the selected LRAr record to the data grid
		/// </summary>
		private void ExecuteMoveDownRecordCommand()
		{
			if (SelectedSegmentAsLrar == null)
			{
				return;
			}

			SelectedSegmentAsLrarIndex = SegmentsAsLrarCollection.IndexOf(SelectedSegmentAsLrar);
			if (SelectedSegmentAsLrarIndex >= (SegmentsAsLrarCollection.Count - 1))
			{
				return;
			}

			SegmentsAsLrarCollection.Remove(SelectedSegmentAsLrar);
			SegmentsAsLrarCollection.Insert((SelectedSegmentAsLrarIndex + 1), SelectedSegmentAsLrar);
			IsGridDataDirty = true;
			// ResequenceSteps();
			UpdateActiveDocument();
		}

		#endregion Commands -------------------------------------------------------------------------------------------------------

		#region Events ------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Loads a JDRL File into grid
		/// </summary>
		/// <param name="fullPathFileName"></param>
		private void OnImportJdrlFileEvent(string fullPathFileName)
		{
			if (string.IsNullOrEmpty(fullPathFileName))
			{
				throw new ArgumentNullException(nameof(fullPathFileName));
			}

			_wfsDocumentManager.LoadDocument(fullPathFileName, out var wfsDocument);
			var wireSegmentsAsLrars = wfsDocument?.LrarSegmentsList;
			if ((wireSegmentsAsLrars == null) || !wireSegmentsAsLrars.Any())
			{
				return;
			}

			SegmentsAsLrarCollection.Clear();

			var sequenceNumber = 1;
			foreach(var wireSegment in wireSegmentsAsLrars)
			{            
				if(!wireSegment.IsSegmentValid)
				{
					continue;
				}

				var newSegmentDP = new SegmentAsLrarDataProvider(sequenceNumber, wireSegment);
				SegmentsAsLrarCollection.Add(newSegmentDP);
				sequenceNumber++;
			}

			IsGridDataDirty = false;
			_eventAggregator.GetEvent<JdrlFileLoadedEvent>().Publish(true);
		}

		/// <summary>
		/// Sends a JDRL File to a machine
		/// </summary>
		/// <param name="fullPathFileName"></param>
		private void OnSendJdrlFileEvent(string fullPathFileName)
		{
			if (string.IsNullOrEmpty(fullPathFileName))
			{
				throw new ArgumentNullException(nameof(fullPathFileName));
			}

			if(!File.Exists(fullPathFileName))
			{
				return;
			}
						
			IWireBendingMachine activeBender;
			if (_wireBendingService.SelectedBenderMachine != null)
			{
				activeBender = _wireBendingService.SelectedBenderMachine;
			}
			else
			{
				activeBender = _wireBendingService.AvailableBenderMachines.FirstOrDefault(kvp => kvp.Value.MachineStatus != WireBendingMachineStatus.NotConnected).Value;
			}

			if (activeBender == null)
			{
				return;
			}

			// Check document
			var fileName = Path.GetFileName(fullPathFileName);
			var wfsDocument = _wfsDocumentManager.GetDocument(fileName);
			if(wfsDocument == null)
			{
				return;
			}
						
			if (!IsGridDataDirty)
			{
				var fileLocation = wfsDocument.DocumentLocation;
				var wires = new List<byte[]>();
				var fileData = File.ReadAllBytes(fileLocation);
				wires.Add(fileData);

				activeBender.SendAppliances(wires);

				return;
			}
			
			var wireAsLrarsSegments = new List<IWireSegmentAsLrar>();
			foreach(var segmentAsLrarDP in SegmentsAsLrarCollection)
			{
				if(segmentAsLrarDP.SegmentAsLrar == null)
				{
					continue;
				}

				wireAsLrarsSegments.Add(segmentAsLrarDP.SegmentAsLrar);				
			}
			
			int datasize = 8 * 4 * wireAsLrarsSegments.Count;
			byte[] wireSegmentsData = new byte[datasize];
			var wireAsLrars = new List<byte[]>();
			using (var writer = new BinaryWriter(new MemoryStream(wireSegmentsData)))
			{
				foreach (var lraSegment in wireAsLrarsSegments)
				{
					writer.Write((double)lraSegment.Length);
					writer.Write((double)lraSegment.Rotation);
					writer.Write((double)lraSegment.Angle);
					writer.Write((double)lraSegment.Radius);
				}						
			}
			
			byte[] finalBytes = CreateWfsFilebytes(fileName, wireSegmentsData);
			wireAsLrars.Add(finalBytes);
			activeBender.SendAppliances(wireAsLrars);

			IsGridDataDirty = false;

			_eventAggregator.GetEvent<JdrlFileSentEvent>().Publish(true);
		}

		/// <summary>
		/// Wire forming studio document was saved event handler
		/// </summary>
		private void OnSavedJdrlFileEvent()
		{
			IsGridDataDirty = false;
		}

		/// <summary>
		/// Edit LRAr record view closed event 
		/// </summary>
		/// <param name="result"></param>
		private void OnEditLrarSegmentViewClosed(IDialogResult result)
		{
			if (result.Result != ButtonResult.OK)
			{
				return;
			}

			if ((result.Parameters == null) || !result.Parameters.ContainsKey("AddLrarSegment"))
			{
				return;
			}

			var lrarSegmnet = result.Parameters.GetValue<IWireSegmentAsLrar>("AddLrarSegment");
			if(lrarSegmnet == null)
			{
				return;
			}

			if (SelectedSegmentAsLrarIndex >= SegmentsAsLrarCollection.Count)
			{
				SegmentsAsLrarCollection.Add(new SegmentAsLrarDataProvider(SegmentsAsLrarCollection.Count, lrarSegmnet));
			}
			else if(SelectedSegmentAsLrarIndex < 0)
			{
				SegmentsAsLrarCollection.Insert(0, new SegmentAsLrarDataProvider(SegmentsAsLrarCollection.Count, lrarSegmnet));
			}
			else
			{
				SegmentsAsLrarCollection.Insert(SelectedSegmentAsLrarIndex, new SegmentAsLrarDataProvider(SegmentsAsLrarCollection.Count, lrarSegmnet));
			}
			ResequenceSteps();

			var segmentAsLrarsList = new List<IWireSegmentAsLrar>();
			foreach(var segmentAsLrar in SegmentsAsLrarCollection)
			{
				segmentAsLrarsList.Add(new WireSegmentAsLrar(segmentAsLrar.SegmentAsLrar));
			}
			_wfsDocumentManager.ActiveWfsDocument.UpdateDocumentData(segmentAsLrarsList);
		}

		/// <summary>
		/// Segments collection changed event handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSegmentAsLrarCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			DeleteRecordCommand.RaiseCanExecuteChanged();
			InsertRecordCommand.RaiseCanExecuteChanged();
			MoveUpRecordCommand.RaiseCanExecuteChanged();
			MoveDownRecordCommand.RaiseCanExecuteChanged();
		}

		#endregion Events ------------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
