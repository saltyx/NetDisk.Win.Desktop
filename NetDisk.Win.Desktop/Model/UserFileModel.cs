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
        public long file_size { get; set; }
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
    }
}
