using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NetDisk.Win.Desktop.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NetDisk.Win.Desktop.ViewModel
{
    public class WelcomeViewModel : ViewModelBase
    {
        private RelayCommand showDialogCommand;

        public WelcomeViewModel()
        {

        }

        public ICommand ShowSettingDialog
        {
            get
            {
                if (showDialogCommand == null)
                {
                    showDialogCommand = new RelayCommand(x => this.ShowDialog());
                }
                return showDialogCommand;
            }
        }

        private async void ShowDialog()
        {
            var result = await ((MetroWindow)Application.Current.MainWindow).ShowInputAsync("Input your IP and port", "Example : 1.1.1.1:3000");          
            if (result == null || result.Length == 0)
                return;
            LoginDialogData data = await ((MetroWindow)Application.Current.MainWindow).ShowLoginAsync("Login", "Your IP is " + (result));
            if (data == null || data.Username == null || data.Password == null)
                return;

            var mSetting = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel",
                AnimateShow = false,
                AnimateHide = false,
                DialogTitleFontSize = 20,
                DialogMessageFontSize = 16
            };

            var controller = await ((MetroWindow)Application.Current.MainWindow).ShowProgressAsync("Please wait...", "Connecting to the server...",settings:mSetting);
            controller.SetIndeterminate();
            controller.SetCancelable(true);
            ServerBack back = await Utils.NetUtils.LoginAsync(result + "/v1/login", data.Username, data.Password);
            await controller.CloseAsync();
            var conn = new SQLiteAsyncConnection(App.DB_NAME);
            await conn.InsertAsync(new SettingModel
            {
                Token = back.Message,
                URL = result
            });
            MessageDialogResult msg = await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("SERVER BACK",back.Message);
        }
    }
}
