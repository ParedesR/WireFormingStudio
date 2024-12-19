namespace Aim.WireFormingStudio.Modules.EsViewPort
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using Prism.Ioc;
	using Prism.Modularity;
	using Prism.Regions;
	using Prism.Mvvm;
		
	using AosLibraries.SharedInterfaces.Cad.Services;
	using AosLibraries.SharedInterfaces.Cad.Mesh;
	
	using Core;

	using Models.Cad;
	using ViewModels;
	using ViewModels.Menus;
	using Views;
	using Menus;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Eye Shot View Port Module
	/// </summary>
	public class EsViewPortModule : IModule
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
		public EsViewPortModule(IRegionManager regionManager)
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
			/*
			// Register the view port tab
			_regionManager.RegisterViewWithRegion(HostRegionNames.MainRibbonNavigationRegion, typeof(EsViewPortTab));
			// Register the view port view
			_regionManager.RegisterViewWithRegion(HostRegionNames.MainTabWorkPaneRegion, typeof(EsViewPortView));
			*/
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			/*
			ViewModelLocationProvider.Register<EsViewPortTab, EsViewPortTabViewModel>();
			ViewModelLocationProvider.Register<EsViewPortView, EsViewPortViewModel>();			

			// Register services
			containerRegistry.RegisterSingleton<ICadViewPortControlService<IEyeShotDentalMesh>, EyeShotViewPort>();		
			*/
		}

		#endregion IModule Interface Implementation ---------------------------------------------------------------------------------
	}
}
