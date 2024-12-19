namespace Aim.WireFormingStudio.Modules.DocumentManager
{
	#region Using Directives -------------------------------------------------------------------------------------------------------
		
	using Prism.Ioc;
	using Prism.Modularity;
	using Prism.Regions;

	using AosLibraries.SharedInterfaces.CaseDocuments.Aim;

	using Models;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Document manager module
	/// </summary>
	public class DocumentManagerModule : IModule
	{
		#region Member Variables ----------------------------------------------------------------------------------------------------

		/// <summary>
		/// Hook to the region manager
		/// </summary>
		private readonly IRegionManager _regionManager;

		#endregion Member Variables -------------------------------------------------------------------------------------------------

		#region Constructors --------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="regionManager"></param>
		public DocumentManagerModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		#endregion Constructors -----------------------------------------------------------------------------------------------------

		#region IModule Interface Implementation ------------------------------------------------------------------------------------

		/// <summary>
		/// Module has been initialized
		/// </summary>
		/// <param name="containerProvider"></param>
		public void OnInitialized(IContainerProvider containerProvider)
		{            
		}


		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register services            
			// containerRegistry.RegisterSingleton<IWfsDocumentManager, WfsDocumentManager>();
		}

		#endregion IModule Interface Implementation ---------------------------------------------------------------------------------

	}
}
