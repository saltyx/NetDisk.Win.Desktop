using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NetDisk.Win.Desktop.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetDisk.Win.Desktop.ViewModel
{
    public class MainViewModel :ViewModelBase
    {
        public ObservableCollection<UserFileModel> Data { get; set; }
        public ObservableCollection<UserFileModel> FolderData   { get; set; }
        public ObservableCollection<UserFileModel> FileData { get; set; }
        public ObservableCollection<KeyValuePair<int,string>> NavFolder { get; set; }

        private bool _canceled = false;

        private bool _isOpen = false;
        private bool _isDetailedOpen = false;

        private string _url;
        private string _username;
        private string _password;

        private UserFileModel _choosenFile;

        private RelayCommand _changeFolder;
        private RelayCommand _back;
        private RelayCommand _newFolder;
        private RelayCommand _showSetting;
        private RelayCommand _reset;
        private RelayCommand _showDetailedFolder;
        private RelayCommand _deleteFolder;
        private RelayCommand _renameFolder;
        private RelayCommand _encrypt;
        private RelayCommand _decrypt;

        public MainViewModel()
        {
            
            Data = new ObservableCollection<UserFileModel>();
            FolderData = new ObservableCollection<UserFileModel>();
            FileData = new ObservableCollection<UserFileModel>();
            NavFolder = new ObservableCollection<KeyValuePair<int, string>>();
            initData();
            
        }

        public ICommand ChangeFolder
        {
            get
            {
                if (_changeFolder == null)
                {
                    _changeFolder = new RelayCommand((x) =>
                    {
                        changeFolder((UserFileModel)x);
                    });
                }
                return _changeFolder;
            }
        }

        public ICommand Back
        {
            get
            {
                if (_back == null)
                {
                    _back = new RelayCommand(x => this.back());
                }
                return _back;
            }
        }

        public ICommand NewFolder
        {
            get
            {
                if (_newFolder == null)
                {
                    _newFolder = new RelayCommand(x => this.newFolder());
                }
                return _newFolder;
            }
        }

        public ICommand ShowSetting
        {
            get
            {
                if (_showSetting == null)
                {
                    _showSetting = new RelayCommand(x => this.showSetting());
                }
                return _showSetting;
            }
        }

        public ICommand Reset
        {
            get
            {
                if (_reset == null)
                {
                    _reset = new RelayCommand(x => this.resetDefaultSetting(x));
                }
                return _reset;
            }
        }

        public ICommand ShowDetailedFolder
        {
            get
            {
                if (_showDetailedFolder == null)
                {
                    _showDetailedFolder = new RelayCommand(x => this.showDetailedFolder((UserFileModel)x));
                }
                return _showDetailedFolder;
            }
        }

        public ICommand DeleteFolder
        {
            get
            {
                if (_deleteFolder == null)
                {
                    _deleteFolder = new RelayCommand(x => this.delete());
                }
                return _deleteFolder;
            }
        }

        public ICommand RenameFolder
        {
            get
            {
                if (_renameFolder == null)
                {
                    _renameFolder = new RelayCommand(x => this.rename());
                }
                return _renameFolder;
            }
        }

        public ICommand Encrypt
        {
            get
            {
                if (_encrypt == null)
                {
                    _encrypt = new RelayCommand(x => this.encrypt());
                }
                return _encrypt;
            }
        }

        public ICommand Decrypt
        {
            get
            {
                if (_decrypt == null)
                {
                    _decrypt = new RelayCommand(x => this.decrypt());
                }
                return _decrypt;
            }
        }

        public bool IsSettingOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (value.Equals(_isOpen))
                    return;
                _isOpen = value;
                OnPropertyChanged("IsSettingOpen");
            }
        }

        public string ChoosenFolderName
        {
            get
            {
                if (_choosenFile == null)
                {
                    return "NULL";
                }
                return "Name: " + _choosenFile.file_name;
            }
        }

        public bool IsDetailedInfoOpen
        {
            get
            {
                return _isDetailedOpen;
            }
            set
            {
                if (value == _isDetailedOpen)
                    return;
                _isDetailedOpen = value;
                OnPropertyChanged("IsDetailedInfoOpen");
            }
        }

        public bool ChoosenFileEncryptionState
        {
            get
            {
                if (_choosenFile == null)
                    return false;
                return _choosenFile.is_encrypted;
            }
            set
            {
                _choosenFile.is_encrypted = value;
                OnPropertyChanged("ChoosenFileEncryptionState");
            }
        }

        public UserFileModel ChoosenFile
        {
            get
            {
                return _choosenFile;
            }
            set
            {
                _choosenFile = value;
                OnPropertyChanged("ChoosenFile");
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (value == _url)
                    return;
                _url = value;
                App.URL = _url;
                OnPropertyChanged("Url");
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (value == _username)
                    return;
                _username = value;
                OnPropertyChanged("Username");
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value == _password)
                    return;
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        private async void initData()
        {
            App.InitData();
            Data = await Utils.NetUtils.GetRoot();
            NavFolder.Clear();
            NavFolder.Add(App.GetCurrentFolder());
            spliteData();
            OnPropertyChanged("Data");
            OnPropertyChanged("FolderData");
            OnPropertyChanged("FileData");
            OnPropertyChanged("NavFolder");
            OnPropertyChanged("IsSettingOpen");
        }
        
        private async void changeFolder(UserFileModel file)
        {

            NavFolder.Add(new KeyValuePair<int, string>(file.id, file.file_name));
            Data = await Utils.NetUtils.GetFilesAndFoldersById(file.id);
            spliteData();
        }

        private async void back()
        {
            if (NavFolder.Count <= 1) return;
            NavFolder.RemoveAt(NavFolder.Count-1);
            Data = await Utils.NetUtils.GetFilesAndFoldersById(NavFolder.Last().Key);
            spliteData();

        }

        private void spliteData()
        {
            FolderData.Clear();
            FileData.Clear();
            foreach (UserFileModel file in Data)
            {
                if (file.is_folder)
                {
                    FolderData.Add(file);
                }
                else
                {
                    FileData.Add(file);
                    Debug.WriteLine("file", file.file_name);
                }
            }
        }

        private async void newFolder()
        {
            var result = await ((MetroWindow)Application.Current.MainWindow).ShowInputAsync("Input the folder's name", "");
            if (result == null || result.Length == 0)
                return;
            int currentFolder = NavFolder.ElementAt(NavFolder.Count - 1).Key;
            int resultCode = await Utils.NetUtils.createNewFolder(result, currentFolder);
            if (resultCode == -1)
            {
                await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("", "Error");
            } else
            {
                await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("","Success!");
                UserFileModel model = new UserFileModel();
                model.file_name = result;
                model.id = resultCode;
                model.created_at = DateTime.Now.ToString();
                FolderData.Add(model);
                Data.Add(model);
            }
        }

        private void showSetting()
        {
            IsSettingOpen = true;
        }

        private async void resetDefaultSetting(object x)
        {
            var passwordBox = x as PasswordBox;
            Password = passwordBox.Password;
            IsSettingOpen = false;
            var mSetting = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel",
                AnimateShow = false,
                AnimateHide = false,
                DialogTitleFontSize = 20,
                DialogMessageFontSize = 16
            };
            var controller = await ((MetroWindow)Application.Current.MainWindow).ShowProgressAsync("Please wait...", "Connecting to the server...", settings: mSetting);
            controller.SetIndeterminate();
            controller.SetCancelable(true);
            ServerBack back = await Utils.NetUtils.LoginAsync(Url + "/v1/login", Username, Password);
            await controller.CloseAsync();
            var conn = new SQLiteAsyncConnection(App.DB_NAME);
            await conn.ExecuteAsync("Delete from SettingModel");
            await conn.InsertAsync(new SettingModel
            {
                Token = back.Message,
                URL = Url
            });
            App.TOKEN = back.Message;
  
            controller = await ((MetroWindow)Application.Current.MainWindow).ShowProgressAsync("Please wait...", "Refreshing", settings: mSetting);
            controller.SetIndeterminate();
            controller.SetCancelable(false);
            initData(); 
            await controller.CloseAsync();
        }

        private void showDetailedFolder(UserFileModel file)
        {
            _choosenFile = file;
            IsDetailedInfoOpen = true;
            OnPropertyChanged("ChoosenFolderName");
           
        }

        private async void encrypt()
        {
            Debug.WriteLine(_canceled);
            if (_canceled)
            {
                _canceled = false;
                return;
            }
            var result = await ((MetroWindow)Application.Current.MainWindow)
                .ShowInputAsync("Please input the passphrase", "DO NOT FORGET YOUR PASSPHRASE!");
            if (result == null || result.Length == 0)
            {
                _canceled = true;
                ChoosenFileEncryptionState = false;
                return;
            }
        }

        private async void decrypt()
        {
            Debug.WriteLine(_canceled);

            if (_canceled)
            {
                _canceled = false; return;
            }
            var result = await ((MetroWindow)Application.Current.MainWindow)
                .ShowInputAsync("Please input the passphrase", "");
            if (result == null || result.Length == 0)
            {
                _canceled = true;
                ChoosenFileEncryptionState = true;
                return;
            }
        }

        private async void rename()
        {
            var result = await ((MetroWindow)Application.Current.MainWindow)
                .ShowInputAsync("Please input the new name", "");
            if (result == null || result.Length == 0)
                return;
        }

        private async void delete()
        {
            MessageDialogResult msg = await ((MetroWindow)Application.Current.MainWindow)
                .ShowMessageAsync("Delete the folder?", "You are trying to delete "+ChoosenFile.file_name, MessageDialogStyle.AffirmativeAndNegative);
            
            if ("Affirmative".Equals(msg.ToString())) {
                IsDetailedInfoOpen = false;
            }
        }
    }
}
