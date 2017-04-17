using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NetDisk.Win.Desktop.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
        private RelayCommand _uploadFile;
        private RelayCommand _downloadFile;

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

        public ICommand Upload
        {
            get
            {
                if (_uploadFile == null)
                {
                    _uploadFile = new RelayCommand(x => this.upload());
                }
                return _uploadFile;
            }
        }

        public ICommand Download
        {
            get
            {
                if (null == _downloadFile)
                {
                    _downloadFile = new RelayCommand(x => this.download((UserFileModel)x));
                }
                return _downloadFile;
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
            set
            {
                _choosenFile.file_name = value;
                OnPropertyChanged("ChoosenFolderName");
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
            var result = await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowInputAsync("Input the folder's name", "");
            if (result == null || result.Length == 0)
                return;
            int currentFolder = NavFolder.ElementAt(NavFolder.Count - 1).Key;
            int resultCode = await Utils.NetUtils.createNewFolder(result, currentFolder);
            if (resultCode == -1)
            {
                await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowMessageAsync("", "Error");
            } else
            {
                await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowMessageAsync("","Success!");
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
            var controller = await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowProgressAsync("Please wait...", "Connecting to the server...", settings: mSetting);
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
  
            controller = await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowProgressAsync("Please wait...", "Refreshing", settings: mSetting);
            controller.SetIndeterminate();
            controller.SetCancelable(false);
            initData(); 
            await controller.CloseAsync();
        }

        private void showDetailedFolder(UserFileModel file)
        {
            ChoosenFile = file;
            log(file.is_encrypted);
            IsDetailedInfoOpen = true;
            _canceled = true;
            ChoosenFileEncryptionState = file.is_encrypted;
            OnPropertyChanged("ChoosenFolderName");
            OnPropertyChanged("ChoosenFile");
        }

        private async void encrypt()
        {
            if (_canceled)
            {
                _canceled = false;
                return;
            }
            var result = await ((MetroWindow)System.Windows.Application.Current.MainWindow)
                .ShowInputAsync("Please input the passphrase", "DO NOT FORGET YOUR PASSPHRASE!");
            if (result == null || result.Length == 0)
            {
                _canceled = true;
                ChoosenFileEncryptionState = false;
                return;
            }
            bool feedback = await Utils.NetUtils.encryptFolder(_choosenFile.id, result);
            if (feedback)
            {
                _choosenFile.is_encrypted = true;
                IsDetailedInfoOpen = false;
            } else
            {
                await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowMessageAsync("ERROR", "An error occurred");
            }
        }

        private async void decrypt()
        {
            if (_canceled)
            {
                _canceled = false; return;
            }
            var result = await ((MetroWindow)System.Windows.Application.Current.MainWindow)
                .ShowInputAsync("Please input the passphrase", "");
            if (result == null || result.Length == 0)
            {
                _canceled = true;
                ChoosenFileEncryptionState = true;
                return;
            }
            bool feedback = await Utils.NetUtils.decryptFolder(_choosenFile.id, result);
            if (feedback)
            {
                _choosenFile.is_encrypted = false;
                IsDetailedInfoOpen = false;
            } else
            {
                await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowMessageAsync("ERROR", "An error occurred");
            }
        }

        private async void rename()
        {
            var result = await ((MetroWindow)System.Windows.Application.Current.MainWindow)
                .ShowInputAsync("Please input the new name", "");
            if (result == null || result.Length == 0)
                return;
            var feedback = await Utils.NetUtils.renameFolder(_choosenFile.id, result);
            if (feedback)
            {
                ChoosenFolderName = result;
                refresh();
            }
        }

        private async void delete()
        {
            MessageDialogResult msg = await ((MetroWindow)System.Windows.Application.Current.MainWindow)
                .ShowMessageAsync("Delete the folder?", "You are trying to delete "+ChoosenFile.file_name, MessageDialogStyle.AffirmativeAndNegative);
            
            if ("Affirmative".Equals(msg.ToString())) {
                bool result = await Utils.NetUtils.deleteFolder(_choosenFile.id);
                if (result)
                {
                    IsDetailedInfoOpen = false;
                    refresh();
                } else
                {
                    await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowMessageAsync("ERROR","An error occurred");
                }

            }
        }

        private async void upload()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = dialog.OpenFile();
                if (stream == null) return;
                using (stream)
                {
                    FileInfo fileInfo = new FileInfo(dialog.FileName);
                    
                    var length = string.Format("{0}",fileInfo.Length);
                    var streamContent = new StreamContent(stream);
                    var lengthContent = new StringContent(length, Encoding.UTF8);
                    int result = await Utils.NetUtils.uplaodFile(streamContent, lengthContent,NavFolder.Last().Key,
                        fileInfo.Name);
                    if (result == -1)
                    {
                        showAlertDialog("UPLAOD FAILED");
                    } else
                    {
                        showSuccessDialog("");
                    }
                }
            }
        }
        
        private async void download(UserFileModel file)
        {
            Stream stream = await Utils.NetUtils.downloadFile(file.id);
            if (null == stream) return;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (DialogResult.OK == result)
            {
                string folderName = dialog.SelectedPath;
                string fullFilePath = string.Format("{0}{1}", folderName, file.file_name);
                Stream temp = File.Create(fullFilePath);
                stream.CopyTo(temp);
                temp.Dispose();
                showSuccessDialog("Downloaded!");
            }
        }

        private async void refresh()
        {
            int id = NavFolder.Last().Key;
            Data = await Utils.NetUtils.GetFilesAndFoldersById(id);
            spliteData();
        }

        private void log(object obj)
        {
            Debug.WriteLine(obj);
        }
        
        private async void showAlertDialog(string content)
        {
            await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowMessageAsync("An error occured", content);
        }

        private async void showSuccessDialog(string content)
        {
            await ((MetroWindow)System.Windows.Application.Current.MainWindow).ShowMessageAsync("SUCCESS!", content);
        }
    }
}
