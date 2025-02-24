using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using Motion.Config;
using Motion.Core;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Motion.ViewModel
{
    public partial class SystemSettingViewModel : ObservableObject
    {
        private readonly IDialogCoordinator _dialogCoordinator;


        [ObservableProperty]
        public AppThemeMenuData selectAppThemeMenuData;

        [ObservableProperty]
        public AccentColorMenuData selectAccentColorMenuData;

        public SystemSettingViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;


            this.AccentColors = ThemeManager.Current.Themes
                                            .GroupBy(x => x.ColorScheme)
                                            .OrderBy(a => a.Key)
                                            .Select(a => new AccentColorMenuData { Name = a.Key, ColorBrush = a.First().ShowcaseBrush })
                                            .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.Current.Themes
                                         .GroupBy(x => x.BaseColorScheme)
                                         .Select(x => x.First())
                                         .Select(a => new AppThemeMenuData { Name = a.BaseColorScheme, BorderColorBrush = a.Resources["MahApps.Brushes.ThemeForeground"] as Brush, ColorBrush = a.Resources["MahApps.Brushes.ThemeBackground"] as Brush })
                                         .ToList();




            SelectAccentColorMenuData = this.AccentColors.Find(o => o.Name == GlobalParameter.SystemConfig.AccentColor);
            SelectAppThemeMenuData = this.AppThemes.Find(o => o.Name == GlobalParameter.SystemConfig.AppTheme);
        }

        public List<AppThemeMenuData> AppThemes { get; set; }
        public List<AccentColorMenuData> AccentColors { get; set; }



        public RelayCommand AppThemesSaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, SelectAppThemeMenuData.Name);
                    ThemeManager.Current.ChangeThemeColorScheme(Application.Current, SelectAccentColorMenuData.Name);
                    GlobalParameter.SystemConfig.AppTheme = SelectAppThemeMenuData.Name;
                    GlobalParameter.SystemConfig.AccentColor = SelectAccentColorMenuData.Name;

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

                    if (SerializerJson.SaveToJson(GlobalParameter.SystemConfig, "Config//SystemConfig.json"))
                    {
                        this._dialogCoordinator.ShowModalMessageExternal(this, "Tips!", $"保存成功", MessageDialogStyle.Affirmative, settings);
                    }

                });
            }
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(string? name)
        {
            if (name is not null)
            {
                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, name);
            }
        }
    }

    public class AccentColorMenuData
    {
        public string? Name { get; set; }

        public Brush? BorderColorBrush { get; set; }

        public Brush? ColorBrush { get; set; }

        public AccentColorMenuData()
        {
            this.ChangeAccentCommand = new SimpleCommand<string?>(o => true, this.DoChangeTheme);
        }

        public ICommand ChangeAccentCommand { get; }

        protected virtual void DoChangeTheme(string? name)
        {
            if (name is not null)
            {
                ThemeManager.Current.ChangeThemeColorScheme(Application.Current, name);
            }
        }
    }
}