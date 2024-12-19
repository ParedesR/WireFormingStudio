namespace Aim.WireFormingStudio.Core.UserInterface
{
    #region Using Directives ------------------------------------------------------------------------------------------------------

    using System;

    using Prism.Services.Dialogs;
    
    #endregion Using Directives ---------------------------------------------------------------------------------------------------
    
    public interface IRegionDialogService
    {
        void Show(string name, IDialogParameters dialogParameters, Action<IDialogResult> callback);
    }
}
