using NetDisk.Win.Desktop.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetDisk.Win.Desktop.ViewModel
{
    public class MainViewModel :ViewModelBase
    {
        public ObservableCollection<UserFileModel> Data { get; set; }
        public ObservableCollection<UserFileModel> FolderData   { get; set; }
        public ObservableCollection<UserFileModel> FileData { get; set; }
        public ObservableCollection<KeyValuePair<int,string>> NavFolder { get; set; }

        private RelayCommand _changeFolder;
        private RelayCommand _changeToPreFolder;
        private RelayCommand _changeToNextFolder;
        private RelayCommand _back;

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

        private async void initData()
        {
            Data = await Utils.NetUtils.GetRoot();
            NavFolder.Add(App.GetCurrentFolder());
            spliteData();
            OnPropertyChanged("Data");
            OnPropertyChanged("FolderData");
            OnPropertyChanged("FileData");
            OnPropertyChanged("NavFolder");
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
    }
}
