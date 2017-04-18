

using NetDisk.Win.Desktop.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetDisk.Win.Desktop.Utils
{
    public class NetUtils
    {
        private static readonly HttpClient client;
        public static readonly string BASE_FOLDER_URL = App.URL + "/api/v1/folder";
        public static readonly string BASE_UPLAOD_URL = App.URL + "/api/v1/upload";
        public static readonly string BASE_FILE_URL = App.URL + "/api/v1/file";

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

        public static async Task<int> createNewFolder(string name, int fromId)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, String.Format(BASE_FOLDER_URL + "/create"));
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);
            var data = new FolderParam
            {
                folder = new NewFolderParam
                {
                    folder_name = name,
                    from_folder = fromId
                }
            };
            string json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var feedback = JsonConvert.DeserializeObject<SuccessFeedBack>(content);
                if (feedback.success == 200)
                    return int.Parse(feedback.info);
                return -1;
            } else
            {
                return -1;
            }
        }

        public static async Task<bool> renameFolder(int id, string newName)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put,
                String.Format(BASE_FOLDER_URL + "/update"));
            request.Method = HttpMethod.Put;
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);

            var data = new FolderParam
            {
                folder = new UpdateFolderParam
                {
                    new_name = newName,
                    folder_id = id
                }
            };
            string json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var feedback = JsonConvert.DeserializeObject<SuccessFeedBack>(content);
                if (feedback.success == 200)
                    return true;
                return false;
            }
            return false;
        }

        public static async Task<bool> deleteFolder(int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete,
                String.Format(BASE_FOLDER_URL + "/delete"));
            request.Method = HttpMethod.Delete;
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);

            var data = new FolderParam
            {
                folder = new DeleteFolderParam
                {
                    folder_id = id
                }
            };
            string json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var feedback = JsonConvert.DeserializeObject<SuccessFeedBack>(content);
                if (feedback.success == 200)
                    return true;
                return false;
            }
            return false;
        }

        public static async Task<bool> encryptFolder(int id, string passPhrase)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                String.Format(BASE_FOLDER_URL + "/encrypt"));
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);

            var data = new FolderParam
            {
                folder = new EncryptFolderParam
                {
                    folder_id = id,
                    pass_phrase = passPhrase
                }
            };
            string json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var feedback = JsonConvert.DeserializeObject<SuccessFeedBack>(content);
                if (feedback.success == 200)
                    return true;
                return false;
            }
            return false;
        }

        public static async Task<bool> decryptFolder(int id, string passPhrase)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                String.Format(BASE_FOLDER_URL + "/decrypt"));
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);

            var data = new FolderParam
            {
                folder = new EncryptFolderParam
                {
                    folder_id = id,
                    pass_phrase = passPhrase
                }
            };
            string json = JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                var feedback = JsonConvert.DeserializeObject<SuccessFeedBack>(content);
                if (feedback.success == 200)
                    return true;
                return false;
            }
            return false;
        }

        public static async Task<int> uplaodFile(StreamContent fileStreamContent, StringContent fileSizeContent, int fromFolderId, string fileName)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format("{0}/{1}", BASE_UPLAOD_URL, fromFolderId));
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);
            using (var content = new MultipartFormDataContent())
            {
                content.Add(fileStreamContent, "file", fileName);
                content.Add(fileSizeContent,"filesize");
                request.Content = content;
                using (var message = await client.SendAsync(request))
                {
                    if (message.IsSuccessStatusCode)
                    {
                        var result = await message.Content.ReadAsStringAsync();
                        var feedback = JsonConvert.DeserializeObject<SuccessFeedBack>(result);
                        if (feedback.success == 200)
                        {
                            return int.Parse(feedback.info);
                        }
                    }
                    return -1;
                }
            }
        }

        public static async Task<Stream> downloadFile(int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, 
                string.Format("{0}/{1}",BASE_FILE_URL, id));
            request.Headers.Add("Authorization", "Token token=" + App.TOKEN);
            HttpResponseMessage result = await client.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStreamAsync();
            }
            return null;
        }

        private class SuccessFeedBack
        {
            public int success { get; set; }
            public string info { get; set; }
        }

        private class FolderParam
        {
            public BaseFolderParam folder { get; set; }
        }

        private class NewFolderParam : BaseFolderParam
        {
            public string folder_name { get; set; }
            public int from_folder { get; set; }
        }

        private class UpdateFolderParam : BaseFolderParam
        {
            public string new_name { get; set; }
            public int folder_id { get; set; }
        }

        private class DeleteFolderParam : BaseFolderParam
        {
            public int folder_id { get; set; }
        }

        private class EncryptFolderParam : BaseFolderParam
        {
            public int folder_id { get; set; }
            public string pass_phrase { get; set; } 
        }

        private abstract class BaseFolderParam { }
    }
}
