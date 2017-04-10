using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetDisk.Win.Desktop.Model
{
    public class SettingModel
    {
        [PrimaryKey]
        public string Token { get; set; }
        public string URL { get; set; }
    }
}
