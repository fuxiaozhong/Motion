using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Motion.Config;
using Motion.Core;
using System.IO;

namespace Motion.ViewModel
{
    public partial class IOViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _dialogCoordinator;


        public IOViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    LoginDialogSettings settings = new LoginDialogSettings()
                    {
                        AffirmativeButtonText = "Yes",
                        FirstAuxiliaryButtonText = "Cancel",
                        DialogButtonFontSize = 20D,
                        MaximumBodyHeight = 200
                    };


                    if (!Directory.Exists("Config"))
                    {
                        Directory.CreateDirectory("Config");
                    }

                    if (SerializerJson.SaveToJson(GlobalParameter.InputSignals, "Config//InputSignals.json") && SerializerJson.SaveToJson(GlobalParameter.OutputSignals, "Config//OutputSignals.json"))
                    {
                        this._dialogCoordinator.ShowModalMessageExternal(this, "Tips!", $"保存成功", MessageDialogStyle.Affirmative, settings);
                    }

                });
            }
        }

    }
}