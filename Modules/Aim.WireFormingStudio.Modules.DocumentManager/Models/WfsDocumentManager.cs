namespace Aim.WireFormingStudio.Modules.DocumentManager.Models
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;	
	using System.Collections.Concurrent;

	using Prism.Events;

	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;
	using AosLibraries.SharedInterfaces.DomainData.Services;

	using AosLibraries.Kernel.DomainData.Core.OrthodonticWires;
	
	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Implements a manager for wire forming studio documents
	/// </summary>
	public class WfsDocumentManager : IWfsDocumentManager
	{
		#region Member Variables ---------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Hook to the event manager 
		/// </summary>

		private readonly IEventAggregator _eventAggregator;

		/// <summary>
		/// Hook to the Wire Bending Service
		/// </summary>        
		private readonly IWireBendingService _bendingService;
		
		#endregion Member Variables ------------------------------------------------------------------------------------------------------------------

		#region Constructors ------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor, initializes the document factory
		/// </summary>
		/// <param name="eventAggregator"></param>
		/// <param name="bendingService"></param>   
		/// <exception cref="ArgumentNullException"></exception>        
		public WfsDocumentManager(IEventAggregator eventAggregator, IWireBendingService bendingService)
		{
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
			_bendingService = bendingService ?? throw new ArgumentNullException(nameof(bendingService));

			WfsDocumentStore = new ConcurrentDictionary<string, IWfsDocument>();
		}

		#endregion Constructors ---------------------------------------------------------------------------------------------------

		#region Public Properties -------------------------------------------------------------------------------------------------

		#endregion Public Properties ----------------------------------------------------------------------------------------------

		#region Public Functions --------------------------------------------------------------------------------------------------

		#endregion Public Functions -----------------------------------------------------------------------------------------------   

		#region IWfsDocumnetManager Interface Implementation ----------------------------------------------------------------------

		#region Properties --------------------------------------------------------------------------------------------------------

		/// <summary>
		/// In memory keyed repository
		/// </summary>
		public IDictionary<string, IWfsDocument> WfsDocumentStore { get; private set; }

		/// <summary>
		/// The currently active document
		/// </summary>
		public IWfsDocument ActiveWfsDocument { get; private set; }

		/// <summary>
		/// Flag to indicate if there is an open document
		/// </summary>
		public bool IsDocumentOpened
		{
			get
			{
				return (ActiveWfsDocument == null) ? false : ActiveWfsDocument.IsDocumentOpened;
			}
		}

		/// <summary>
		/// Flag to indicate if the document has been modified
		/// </summary>
		public bool IsDocumentDirty
		{
			get
			{
				return (ActiveWfsDocument == null) ? false : ActiveWfsDocument.IsDocumentDirty;
			}
		}

		/// <summary>
		/// Flag to indicate that the document has just been created
		/// </summary>
		public bool IsNewDocument
		{
			get
			{
				return (ActiveWfsDocument == null) ? false : ActiveWfsDocument.IsNewDocument;
			}
		}

		#endregion Properties -----------------------------------------------------------------------------------------------------

		#region Functions --------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a wire forming studio document using passed parameters
		/// </summary>
		/// <param name="documentData"></param>
		public IWfsDocument CreateDocument(IWfsDocument documentData)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a wire forming studio document from another wire forming studio document
		/// </summary>
		/// <param name="documentData"></param>
		/// <param name="saveOpenedDocument"></param>
		/// <returns></returns>        
		public IWfsDocument CreateDocument(IWfsDocument documentData, bool saveOpenedDocument)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Starts bending the active document
		/// </summary>
		public void BendActiveDocument()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stops lasing the Active Document
		/// </summary>
		public void StopBendingActiveDocument()
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// Pauses bending the active case
		/// </summary>
		public void PauseBendingActiveDocument()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Resets the flag that indicates that the active document is being bend 
		/// </summary>
		/// <param name="isFinished"></param>
		public void BendingActiveDocumentFinished(bool isFinished)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="documentName"></param>
		/// <returns></returns>
		public IWfsDocument GetDocument(string documentName)
		{
			if(string.IsNullOrEmpty(documentName))
			{
				throw new ArgumentNullException(nameof(documentName));
			}

			if(!WfsDocumentStore.Keys.Contains(documentName))
			{
				return null;
			}

			OpenDocument(documentName, true);

			return WfsDocumentStore[documentName];
		}

		/// <summary>
		/// Returns a list of all of the documents in the repository
		/// </summary>
		/// <param name="refreshed"></param>        
		/// <returns></returns>
		public IReadOnlyDictionary<string, IWfsDocument> GetAllDocuments(bool refreshed = false)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Loads the document passed as parameter from the repository if it is not in the store.  
		/// </summary>
		/// <param name="documentName"></param>        
		/// <returns></returns>
		public void LoadDocument(string documentName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Loads a wire forming studio document from the pass location
		/// </summary>
		/// <param name="fullPathFileName">Full path file name location of the document to be loaded.</param>
		/// <param name="wfsDocument">Reference to loaded document.</param>
		/// <returns></returns>
		public void LoadDocument(string fullPathFileName, out IWfsDocument wfsDocument)
		{
			if (string.IsNullOrEmpty(fullPathFileName))
			{
				throw new ArgumentNullException(nameof(fullPathFileName));
			}

			wfsDocument = null;
			if (!File.Exists(fullPathFileName))
			{				
				return;
			}

			wfsDocument = new WfsDocument(_eventAggregator, fullPathFileName);
			using var reader = new BinaryReader(File.Open(fullPathFileName, FileMode.Open));
			var fileNameBytes = reader.ReadBytes(100);
			var documentName = Encoding.UTF8.GetString(fileNameBytes);
			var endIndex = documentName.IndexOf('\0');
			documentName = documentName.Substring(0, endIndex);
			if (!string.IsNullOrEmpty(documentName))
			{
				wfsDocument.DocumentName = documentName;
			}			
			wfsDocument.DocumentVersion = reader.ReadDouble().ToString();
			wfsDocument.NumberOfVariables = (int)reader.ReadInt64();
			wfsDocument.NumberOfPiecesToMake = (int)reader.ReadDouble();
			while (reader.BaseStream.Position != reader.BaseStream.Length)
			{
				var length = reader.ReadDouble();
				var rotation = reader.ReadDouble();
				var angle = reader.ReadDouble();
				var radius = reader.ReadDouble();

				var segmentAsLrar = new WireSegmentAsLrar()
				{
					Length = length,
					Rotation = rotation,
					Angle = angle,
					Radius = radius
				};
				wfsDocument.LrarSegmentsList.Add(segmentAsLrar);
			}

			if(!WfsDocumentStore.Keys.Contains(wfsDocument.DocumentName))
			{
				WfsDocumentStore.Add(wfsDocument.DocumentName, wfsDocument);
			}

			ActiveWfsDocument = wfsDocument;			
		}

		/// <summary>
		/// Opens an existing document
		/// </summary>
		/// <param name="documentName">Name of the document to be opened</param>
		/// <param name="saveOpenedDocument">If there is a document previously opened, this flag indicates if it is to be saved before closing it.</param>        
		public IWfsDocument OpenDocument(string documentName, bool saveOpenedDocument)
		{
			return null;
			// throw new NotImplementedException();
		}

		/// <summary>
		/// Persists the active document 
		/// <param name="filename">If a no null file name is passed, it will be used to create the file.  Otherwise, the 
		/// document name will be used</param>
		/// </summary>
		public void SaveActiveDocument(string filename = null)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Close the currently opened document
		/// </summary>
		/// <param name="saveDocument">Saves document before changes</param>
		public void CloseDocument(bool saveDocument)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Clears all of the Active Document Data
		/// </summary>
		/// <param name="clearAll"></param>
		public void ClearActiveDocumentData(bool clearAll = true)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if document exists in the store
		/// </summary>
		/// <param name="documentName"></param>
		/// <returns></returns>
		public bool ContainsDocument(string documentName)
		{
			throw new NotImplementedException();
		}

		#endregion Functions ------------------------------------------------------------------------------------------------------

		#endregion IWfsDocumnetManager Interface Implementation -------------------------------------------------------------------
	}
}
