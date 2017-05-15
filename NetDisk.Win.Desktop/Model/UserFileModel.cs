using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDisk.Win.Desktop.Model
{
    public class UserFileModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int user_id { get; set; }
        public string file_name { get; set; }
        public string file_size
        {
            get
            {
                if (_fileSize < 1024) return string.Format("{0} b", _fileSize.ToString("0.00"));
                if (_fileSize < 1024 * 1024) return string.Format("{0} kb", (_fileSize / 1024).ToString("0.00"));
                return string.Format("{0} mb", (_fileSize / (1024 * 1024)).ToString("0.00"));
            }
            set
            {
                double temp = double.Parse(value);
                if (temp != _fileSize)
                {
                    _fileSize = temp;
                }
            }
        }
        public string file_path     {get;set;}
        public bool is_folder       {get;set;}
        public int from_folder      {get;set;}
        public bool is_shared       {get;set;}
        public bool is_encrypted    {get;set;}
        public string download_link {get;set;}
        public int download_times   {get;set;}
        public string created_at    {get;set;}
        public string updated_at    {get;set;}
        public string iv            {get;set;}
        public string sha256 { get; set; }

        private double _fileSize;
    }
}
