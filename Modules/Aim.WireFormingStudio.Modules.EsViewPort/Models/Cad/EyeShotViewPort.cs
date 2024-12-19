namespace Aim.WireFormingStudio.Modules.EsViewPort.Models.Cad
{
	#region Using Directives ------------------------------------------------------------------------------------------------------

	using System;
	using System.IO;
	using System.Linq;
	using System.Drawing;
	using System.Windows;
	using System.Windows.Input;
	using System.Collections.Generic;

	using Prism.Events;

	using devDept.Eyeshot;
	using devDept.Graphics;
	using devDept.Geometry;
	using devDept.Eyeshot.Entities;

	using AosLibraries.SharedInterfaces.Cad;
	using AosLibraries.SharedInterfaces.Cad.Mesh;
	using AosLibraries.SharedInterfaces.EsCad.Services;
	using AosLibraries.SharedInterfaces.Cad.Services;
	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;
	using AosLibraries.SharedInterfaces.Math.Geometry;
	using AosLibraries.SharedInterfaces.DomainData.Services;

	using AosLibraries.Graphics.Cad.Mesh;

	using AosLibraries.FileConversion.StlConverter;

	using AosLibraries.Kernel.DomainData.Math.Geometry;
	using AosLibraries.Kernel.DomainData.EsMath.Geometry;
	using AosLibraries.Kernel.DomainData.Core.OrthodonticTemplates.FreeForm;
	using AosLibraries.Kernel.DomainData.EsMath.Geometry.Extensions;

	using Core.Events;

	using Aim.WireFormingStudio.Modules.DocumentManager;
	
	#endregion Using Directives ---------------------------------------------------------------------------------------------------

	/// <summary>
	/// 
	/// </summary>
	public class EyeShotViewPort : EyeShotViewPortControlService
	{		
		#region Member Variables --------------------------------------------------------------------------------------------------

		/// <summary>
		/// Hook to the region manager
		/// </summary>
		private readonly IEventAggregator _eventAggregator;

		/// <summary>
		/// Hook to the document manager
		/// </summary>
		private readonly IWfsDocumentManager _wfsDocumentManager;
		
		/// <summary>
		/// Hook to the domain types naming factory
		/// </summary>
		private readonly DomainTypesNamingFactory _domainNamesFactory;
		
		#endregion Member Variables -----------------------------------------------------------------------------------------------

		#region Constructors ------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default Constructor
		/// </summary>        /// 
		/// <param name="eventAggregator"></param>
		/// <param name="wfsDocumentManager"></param>		
		public EyeShotViewPort(IEventAggregator eventAggregator, IWfsDocumentManager wfsDocumentManager) : base()
		{
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
			_wfsDocumentManager = wfsDocumentManager ?? throw new ArgumentNullException(nameof(wfsDocumentManager));

			CompileUserInterfaceElements();

			_domainNamesFactory = DomainTypesNamingFactory.Instance;
		}

		#endregion Constructors ---------------------------------------------------------------------------------------------------

		#region Public Properties -------------------------------------------------------------------------------------------------

		#endregion Public Properties ----------------------------------------------------------------------------------------------
			
		#region Public Functions --------------------------------------------------------------------------------------------------
				
		#endregion Public Functions ----------------------------------------------------------------------------------------------2
			
		#region Helper Functions --------------------------------------------------------------------------------------------------
			
		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
