namespace Aim.WireFormingStudio.Modules.DocumentManager.Models
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Runtime.Serialization;

	using Prism.Events;

	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;
	using AosLibraries.SharedInterfaces.DomainData.WireEntities;
	using AosLibraries.Kernel.DataAccess.Repositories;
	using static AosLibraries.Kernel.DataAccess.Repositories.DataSerialization.WfsDocumentHelper;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Implements a wire forming studio document
	/// </summary>
	public class WfsDocument : IWfsDocument
	{
		#region Constants ---------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Latest version of the documents
		/// </summary>
		public const string CurrentDocumentVersion = @"1.0.0";

		#endregion Constants ------------------------------------------------------------------------------------------------------

		#region Member Variables --------------------------------------------------------------------------------------------------

		/// <summary>
		/// Hook to the region manager
		/// </summary>
		private readonly IEventAggregator _eventAggregator;

		// - Temp ROLA
		/// <summary>
		/// Hook to the naming factory
		/// </summary>
		// private readonly DomainTypesNamingFactory _namingFactory;

		/// <summary>
		/// Is this object disposed
		/// </summary>
		private bool _disposed;

		/// <summary>
		/// Name of the user currently logged in this computer.  Used for default document ownership
		/// </summary>
		private string _userName;

		#endregion Member Variables -----------------------------------------------------------------------------------------------

		#region Constructors ------------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventAggregator"></param>
		/// <param name="location">Full path file name of the document.</param>
		public WfsDocument(IEventAggregator eventAggregator, string documnetLocation)
		{
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
			DocumentLocation = string.IsNullOrEmpty(documnetLocation) ? throw new ArgumentNullException(nameof(documnetLocation)) :
															documnetLocation;
			DocumentName = Path.GetFileName(DocumentLocation);

			// - Temp ROLA
			// - _namingFactory = DomainTypesNamingFactory.Instance;
			_disposed = false;

			InitializeToDefaultValues();
		}
		
		#endregion Constructors ---------------------------------------------------------------------------------------------------

		#region IWfsDocument Interface Implementation -----------------------------------------------------------------------------

		#region Properties --------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Unique Id of this wire forming studio document
		/// </summary> 
		public long DocumentId { get; }

		/// <summary>
		/// Full path file name of the document
		/// </summary>
		public string DocumentName { get; set; }

		/// <summary>
		/// Description of the document to be created
		/// </summary>
		public string DocumentDescription { get; set; }

		/// <summary>
		/// Full path file name of the document
		/// </summary>
		public string DocumentLocation { get; set; }

		/// <summary>
		/// Version of the document
		/// </summary>
		public string DocumentVersion { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int NumberOfVariables { get; set; }

		/// <summary>
		/// Number of repeated wire pieces to bend 
		/// </summary>
		public int NumberOfPiecesToMake { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IList<IWireSegmentAsLrar> LrarSegmentsList { get; set; }

		/// <summary>
		/// Flag that indicates if the document was read from file or just created
		/// </summary>
		public bool IsNewDocument { get; set; }

		/// <summary>
		/// Flag that indicates if the document is currently opened
		/// </summary>
		public bool IsDocumentOpened { get; private set; }

		/// <summary>
		/// Flag that indicates if the document has been changed since the last saved
		/// </summary>
		public bool IsDocumentDirty { get; private set; }

		/// <summary>
		/// Time stamp when the document was created 
		/// </summary>
		public DateTime CreationDate { get; set; }

		/// <summary>
		/// Time stamp when the document was modified
		/// </summary>
		public DateTime LastModified { get; set; }

		/// <summary>
		/// Name of the person that created this document
		/// </summary>
		public string CreatedBy { get; set; }

		/// <summary>
		/// Name of the person that last modified this document
		/// </summary>
		public string LastModifiedBy { get; set; }

		/// <summary>
		/// File size of this document
		/// </summary>
		public long FileSize { get; set; }

		#endregion Properties -----------------------------------------------------------------------------------------------------

		#region Functions ---------------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="folderLocation"></param>
		public void SaveWfsDocument(string folderLocation = null)
		{
			string saveToLocation = string.IsNullOrEmpty(folderLocation) ?
							string.IsNullOrEmpty(DocumentLocation) ?
									$"{FolderNames.OrthoDentalDocumentsDataFolder}\\{DocumentName}{FolderNames.JawDrawWireAsLraFileExtension}" :
									DocumentLocation :
									folderLocation;
			
			int datasize = 8 * 4 * LrarSegmentsList.Count;
			byte[] wireSegmentsData = new byte[datasize];			
			using (var writer = new BinaryWriter(new MemoryStream(wireSegmentsData)))
			{
				foreach (var lraSegment in LrarSegmentsList)
				{
					writer.Write((double)lraSegment.Length);
					writer.Write((double)lraSegment.Rotation);
					writer.Write((double)lraSegment.Angle);
					writer.Write((double)lraSegment.Radius);
				}
			}
			byte[] finalBytes = CreateWfsFilebytes(DocumentName, wireSegmentsData);						
			if(File.Exists(saveToLocation))
			{
				File.Delete(saveToLocation);
			}

			File.WriteAllBytes(saveToLocation, finalBytes);

			IsNewDocument = false;
			IsDocumentDirty = false;			
		}

		/// <summary>
		/// Updates the three properties that change with the document updates
		/// </summary>
		public void UpdateDocumentInfo()
		{
		}

		/// <summary>
		/// Updates the properties data of this document
		/// </summary>
		public void UpdateDocumentData(IList<IWireSegmentAsLrar> segmentsAsLrars)
		{
			if(segmentsAsLrars == null)
			{
				throw new ArgumentNullException(nameof(segmentsAsLrars));
			}

			LrarSegmentsList.Clear();

			foreach(var segmentAsLrar in segmentsAsLrars)
			{
				LrarSegmentsList.Add(segmentAsLrar);
			}

			IsDocumentDirty = true;
			IsNewDocument = false;
			IsDocumentOpened = true;
		}

		#endregion Functions ------------------------------------------------------------------------------------------------------

		#endregion IWfsDocument Interface Implementation --------------------------------------------------------------------------
		#region ICloneable Interface Implementation -------------------------------------------------------------------------------

		/// <summary>
		/// Returns an AOS Case Document clone
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return MemberwiseClone();
		}

		#endregion ICloneable Interface Implementation ----------------------------------------------------------------------------

		#region IComparable Interface Implementation ------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(IWfsDocument other)
		{
			var retVal = 0;

			if (DocumentId > other.DocumentId)
			{
				retVal = 1;
			}
			else if (DocumentId < other.DocumentId)
			{
				retVal = -1;
			}

			return retVal;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			if (obj is WfsDocument wfsDocument)
			{
				return DocumentId.CompareTo(wfsDocument.DocumentId);
			}
			else
			{
				throw new ArgumentException("WfsDocument::CompareTo() Error : Object is not a WfsDocument");
			}
		}

		#endregion IComparable Interface Implementation ---------------------------------------------------------------------------

		#region IEquatable Interface Implementation -------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(IWfsDocument other)
		{
			return other != null && (DocumentId == other.DocumentId);
		}

		#endregion IEquatable Interface Implementation ----------------------------------------------------------------------------

		#region ISerializable Interface Implementation ----------------------------------------------------------------------------

		/// <summary>
		/// Constructor for serialization
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public WfsDocument(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			DocumentId = info.GetInt64("DocumentId");
			DocumentName = info.GetString("DocumentName");
			DocumentDescription = info.GetString("DocumentDescription");
			DocumentLocation = info.GetString("DocumentLocation");
			DocumentVersion = info.GetString("DocumentVersion");
			CreationDate = (DateTime)info.GetValue("CreationDate", typeof(DateTime));
			LastModified = (DateTime)info.GetValue("LastModified", typeof(DateTime));
			LastModified = (DateTime)info.GetValue("LastModified", typeof(DateTime));
			CreatedBy = info.GetString("CreatedBy");
			LastModifiedBy = info.GetString("LastModifiedBy");
			FileSize = info.GetInt64("FileSize");
		}


		/// <summary>
		/// Serializes the class
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			info.AddValue("DocumentId", DocumentId);
			info.AddValue("DocumentName", DocumentName);
			info.AddValue("DocumentDescription", DocumentDescription);
			info.AddValue("DocumentLocation", DocumentLocation);
			info.AddValue("DocumentVersion", DocumentVersion);
			info.AddValue("CreationDate", CreationDate);
			info.AddValue("LastModified", LastModified);
			info.AddValue("CreatedBy", CreatedBy);
			info.AddValue("LastModifiedBy", LastModifiedBy);
			info.AddValue("FileSize", FileSize);
		}


		/// <summary>
		/// Adds values into the data file during serialization.
		/// </summary>
		/// <param name="context"></param>       
		[OnSerializing()]
		internal void OnSerializingMethod(StreamingContext context)
		{
		}


		/// <summary>
		/// Resets values after data serialization.
		/// </summary>
		/// <param name="context"></param>
		[OnSerialized()]
		internal void OnSerializedMethod(StreamingContext context)
		{
		}


		/// <summary>
		/// Sets values during data serialization
		/// </summary>
		/// <param name="context"></param>  
		[OnDeserializing()]
		public void OnDeserializingMethod(StreamingContext context)
		{
		}


		/// <summary>
		/// Sets values after data serialization
		/// </summary>
		/// <param name="context"></param>       
		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
		}

		#endregion ISerializable Interface Implementation -------------------------------------------------------------------------

		#region IXmlSerializable Interface Implementation -------------------------------------------------------------------------

		/// <summary>
		/// XML De-Serialization
		/// </summary>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			reader.Read();
		}


		/// <summary>
		/// XML Serialization
		/// </summary>
		/// <param name="writer"></param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
		}


		/// <summary>
		/// Returns schema
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion IXmlSerializable Interface Implementation ----------------------------------------------------------------------

		#region IDisposable Interface Implementation ------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Dispose of the lists
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
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
		/// Gets default values
		/// </summary>
		private void InitializeToDefaultValues()
		{
			// - Temp ROLA
			/*
			DocumentId = _namingFactory.CreateUniqueId();
			DocumentName = _namingFactory.CreateDocumentName();
			DocumentDescription = _namingFactory.CreateDomainTypeDefaultDescription(DocumentName);
			*/            
			DocumentVersion = CurrentDocumentVersion;
			IsNewDocument = true;
			IsDocumentOpened = true;
			IsDocumentDirty = true;
			CreationDate = DateTime.Today;
			_userName = Environment.UserName;
			CreatedBy = _userName;

			LrarSegmentsList = new List<IWireSegmentAsLrar>();

			UpdateDocumentInfo();
		}


		/// <summary>
		/// Parses the document regenerating the entities flat map
		/// </summary>
		private void RefreshDocumentMaps()
		{
		}

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
