namespace Aim.WireFormingStudio.Core.ViewModels
{
    #region Using Directives -------------------------------------------------------------------------------------------------------

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.ObjectModel;
    
    using Prism.Mvvm;
    using Prism.Regions;

    #endregion Using Directives ----------------------------------------------------------------------------------------------------

    /// <summary>
    /// Base class for all view models
    /// </summary>    
    [Export(typeof(ViewModelBase))]
    public abstract class ViewModelBase : BindableBase, IViewModel, IDataErrorInfo, IConfirmNavigationRequest
    {
        #region Member Variables ---------------------------------------------------------------------------------------------------------------------

        #endregion Member Variables ------------------------------------------------------------------------------------------------------------------

        #region Constructors ------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Default constructor
        /// </summary>        
        protected ViewModelBase()
        {
        }

        #endregion Constructors ---------------------------------------------------------------------------------------------------------------------

        #region Properties ---------------------------------------------------------------------------------------------------------------------------        

        /// <summary>
        /// Number of decimal digits to display for doubles
        /// </summary>
        public virtual int NumberOfDecimalDigits { get; set; }

        #endregion Properties ------------------------------------------------------------------------------------------------------------------------

        #region IViewModel Interface Implementation --------------------------------------------------------------------------------------------------

        /// <summary>
        /// Title of the view
        /// </summary>
        public string ViewTitle { get; set; }

        #endregion IViewModel Interface Implementation -----------------------------------------------------------------------------------------------

        #region IDataErrorInfo Interface Implementation ----------------------------------------------------------------------------------------------

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName] => OnValidate(columnName);


        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public string Error => throw new NotImplementedException();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected virtual string OnValidate(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };

            var results = new Collection<ValidationResult>();
            var isValid = Validator.TryValidateObject(this, context, results, true);

            return !isValid ? results[0].ErrorMessage : null;
        }

        #endregion IDataErrorInfo Interface Implementation -------------------------------------------------------------------------------------------

        #region IConfirmNavigationRequest Interface Implementation ------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <param name="continuationCallback"></param>
        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns></returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        #endregion IConfirmNavigationRequest Interface Implementation ---------------------------------------------------------------
    }
}
