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

        private static KeyValuePair<int, string> CURRENT_FOLDER;
        private static Stack<KeyValuePair<int,string>> _folderStack;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _folderStack = new Stack<KeyValuePair<int, string>>();
            CURRENT_FOLDER = new KeyValuePair<int, string>(1, "root");
            _folderStack.Push(CURRENT_FOLDER);

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

        public static void MoveNextFolder(int dst,string folderName)
        {
            CURRENT_FOLDER = new KeyValuePair<int, string>(dst, folderName);
            _folderStack.Push(CURRENT_FOLDER);
        }

        public static KeyValuePair<int,string> FindPreFolder()
        {
            _folderStack.Pop();
            return CURRENT_FOLDER = _folderStack.Peek();
        }

        public static KeyValuePair<int,string> GetCurrentFolder()
        {
            return CURRENT_FOLDER;
        }
    }
}
