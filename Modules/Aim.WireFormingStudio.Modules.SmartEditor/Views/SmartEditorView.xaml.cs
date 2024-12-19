namespace Aim.WireFormingStudio.Modules.SmartEditor.Views
{
	#region Using Directives -------------------------------------------------------------------------------------------------------

	using Infragistics.Windows.DataPresenter;
	using Infragistics.Windows.DataPresenter.Events;

	using Aim.WireFormingStudio.Core.Views;

	using ViewModels;
	using ViewModels.DataPresenters;

	#endregion Using Directives ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Interaction logic for SmartEditorView.xaml
	/// </summary>
	public partial class SmartEditorView : AimViewBase
	{
		public SmartEditorView()
		{
			InitializeComponent();
		}

		#region Helper Functions --------------------------------------------------------------------------------------------------

		#region Events ------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SmartEditorDataGrid_EditModeEnded(object sender, EditModeEndedEventArgs e)
		{
			if (DataContext is not SmartEditorViewModel viewModel)
			{
				return;
			}

			viewModel.IsGridDataDirty = true;
		}

		/// <summary>
		/// Data grid selected items changed event handler
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SmartEditorDataGrid_SelectedItemsChanged(object sender, SelectedItemsChangedEventArgs e)
		{
			if (e.Source is not XamDataGrid sourceDataGrid)
			{
				return;
			}
			
			if(DataContext is not SmartEditorViewModel viewModel)
			{
				return;
			}

			var selectedItem = sourceDataGrid.SelectedDataItem;
			if(selectedItem == null) 
			{
				return;
			}

			viewModel.SelectedSegmentAsLrar = selectedItem as SegmentAsLrarDataProvider;
						
			sourceDataGrid.SelectedDataItem = null;
		}

		#endregion Events ---------------------------------------------------------------------------------------------------------

		#endregion Helper Functions -----------------------------------------------------------------------------------------------
	}
}
