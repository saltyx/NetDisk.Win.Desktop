

using NetDisk.Win.Desktop.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetDisk.Win.Desktop.Utils
{
    public class NetUtils
    {
        private static readonly HttpClient client;
        public static readonly string BASE_FOLDER_URL = App.URL + "/v1/folder";
        static NetUtils()
        {
            client = new HttpClient();
        }

        public static async Task<ServerBack> LoginAsync(string url,string username, string password)
        {
            ServerBack back ;

            var data = new LoginModel
            {
                user = new UserModel
                {
                    name = username,
                    password = password
                }
            };
            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync(url, content);

            return back = new ServerBack
            {
                Code = (int)result.StatusCode,
                Message = await result.Content.ReadAsStringAsync()
            };
        }

        public static async Task<ObservableCollection<UserFileModel>> GetRoot()
        {
            return await GetFilesAndFoldersById(1);
        }

        public static async Task<ObservableCollection<UserFileModel>> GetFilesAndFoldersById(int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format(BASE_FOLDER_URL + "/{0}", id));
            request.Method = HttpMethod.Get;
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);
            HttpResponseMessage result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var feedback = JsonConvert.DeserializeObject<SuccessFeedBack>(content);
                return JsonConvert.DeserializeObject<ObservableCollection<UserFileModel>>(feedback.info);
            } else
            {

                return null;
            }
        }

        private class SuccessFeedBack
        {
            public string success { get; set; }
            public string info { get; set; }
        }

    }
}
