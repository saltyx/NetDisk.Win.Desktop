using NetDisk.Win.Desktop.Model;
using NetDisk.Win.Desktop.ViewModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NetDisk.Win.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string TOKEN;
        public static string URL;
        public static readonly string DB_NAME = "disk.db";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var db = new SQLiteConnection(DB_NAME);
            db.CreateTable<SettingModel>();
            db.CreateTable<UserFileModel>();
            if (0 == db.Table<SettingModel>().Count())
            {
                WelcomeWindow view = new WelcomeWindow();
                WelcomeViewModel vm = new WelcomeViewModel();
                view.DataContext = vm;
                view.Show();
            } else
            {
                var setting = db.Table<SettingModel>().First();
                TOKEN = setting.Token;
                URL = setting.URL;
                Debug.WriteLine("TOKEN = {0}", TOKEN+URL);
                MainWindow view = new MainWindow();
                MainViewModel vm = new MainViewModel();
                view.DataContext = vm;
                view.Show();
                
            }
            
        }
    }
}
