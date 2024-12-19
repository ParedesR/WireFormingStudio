namespace Aim.WireFormingStudio.Host.Views
{
    #region Using Directives ------------------------------------------------------------------------------------------------------

    using Aim.WireFormingStudio.Core.Commands;

    #endregion Using Directives ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        #region Member Variables ---------------------------------------------------------------------------------------------------

        /// <summary>
        /// Hook to application commands interface
        /// </summary>
        private readonly IApplicationCommand _applicationCommand;

        #endregion Member Variables ------------------------------------------------------------------------------------------------

        #region Constructors --------------------------------------------------------------------------------------------------------

        public MainView(IApplicationCommand applicationCommand)
        {
            InitializeComponent();

            _applicationCommand = applicationCommand;

            // Set the active tab
        }

        #endregion Constructors -----------------------------------------------------------------------------------------------------

        #region Helper Functions ----------------------------------------------------------------------------------------------------
               
        #endregion Helper Functions -------------------------------------------------------------------------------------------------
    }
}
