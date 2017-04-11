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

        public MainViewModel()
        {
            Data = new ObservableCollection<UserFileModel>();
            FolderData = new ObservableCollection<UserFileModel>();
            FileData = new ObservableCollection<UserFileModel>();
            initData();
        }

        private async void initData()
        {
            Data = await Utils.NetUtils.GetRoot();
            foreach(UserFileModel file in Data)
            {
                if (file.is_folder)
                {
                    FolderData.Add(file);
                } else
                {
                    FileData.Add(file);
                    Debug.WriteLine("file", file.file_name);
                }
            }
            OnPropertyChanged("Data");
            OnPropertyChanged("FolderData");
            OnPropertyChanged("FileData");
        }
        
        

    }
}
