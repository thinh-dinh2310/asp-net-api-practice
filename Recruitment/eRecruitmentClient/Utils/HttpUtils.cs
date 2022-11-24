using BusinessObject.DTO;
using eRecruitmentClient.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utils.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Utils
{
    public static class HttpUtils
    {
        static async Task<string> SendRequestGetNewAccessToken(string refreshToken)
        {
            string getAccessTokenUrl = CommonEnums.API_PATH + "auth/accesstoken";
            string res = await SendPostRequestAsync<string>(getAccessTokenUrl, refreshToken);
            string token = DeserializeResponse<string>(res);

            return token;
        }
        static async Task<string> CheckResponseIsSuccess (HttpResponseMessage response, string strData)
        {
            
            if (!response.IsSuccessStatusCode)
            {
                
                JObject responseJson = (strData == "") ? JObject.Parse("{}") : JObject.Parse(strData);
                string msg = responseJson.Value<string>("message") ?? responseJson.Value<string>("detail") ?? "Something went wrong!";
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    msg = "Forbidden";
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (msg == "JWT Expired!")
                    {
                        string token = await SendRequestGetNewAccessToken(AuthUtils.loginUser.RefreshToken);
                        if (token != null && token != "")
                        {
                            LoginUser login = AuthUtils.loginUser;
                            login.AccessToken = token;
                            AuthUtils.Login(login);
                            return "Call request again!";
                        }
                    } else
                    {
                        msg = "Unauthorized!";
                    }
                }
                throw new Exception(msg);
            }
            return "OK";
        }
        public static async Task<string> SendGetRequestAsync(string url)
        {
            HttpClient client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            string accessToken = (AuthUtils.loginUser == null) ? "" : AuthUtils.loginUser.AccessToken;
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            HttpResponseMessage response = await client.GetAsync(url);
            string strData = await response.Content.ReadAsStringAsync();
            string check = await CheckResponseIsSuccess(response, strData);
            if (check == "Call request again!")
            {
                response = await client.GetAsync(url);
                strData = await response.Content.ReadAsStringAsync();
                check = await CheckResponseIsSuccess(response, strData);
            }
            return strData;
        }

        public static async Task<string> SendDeleteRequestAsync(string url)
        {
            HttpClient client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            string accessToken = (AuthUtils.loginUser == null) ? "" : AuthUtils.loginUser.AccessToken;
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            HttpResponseMessage response = await client.DeleteAsync(url);
            string strData = await response.Content.ReadAsStringAsync();
            string check = await CheckResponseIsSuccess(response, strData);
            if (check == "Call request again!")
            {
                response = await client.DeleteAsync(url);
                strData = await response.Content.ReadAsStringAsync();
                check = await CheckResponseIsSuccess(response, strData);
            }
            return strData;
        }

        public static async Task<string> SendPostRequestAsync<T>(string url, T payload) where T : class
        {
            HttpClient client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            string accessToken = (AuthUtils.loginUser == null) ? "" : AuthUtils.loginUser.AccessToken;
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            
            HttpResponseMessage response = await client.PostAsJsonAsync(url, payload);
            string strData = await response.Content.ReadAsStringAsync();
            string check = await CheckResponseIsSuccess(response, strData);
            if (check == "Call request again!")
            {
                response = await client.PostAsJsonAsync(url, payload);
                strData = await response.Content.ReadAsStringAsync();
                check = await CheckResponseIsSuccess(response, strData);
            }
            return strData;
        }

        public static async Task<string> SendPostRequestApplicationFormAsync(string url, ApplicationPostForCreationDto applicationPost)
        {
            if (applicationPost.Resume.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    //Get the file steam from the multiform data uploaded from the browser
                    await applicationPost.Resume.CopyToAsync(memoryStream);

                    //Build an multipart/form-data request to upload the file to Web API
                    using var multipartContent = new MultipartFormDataContent();
                    using var fileContent = new ByteArrayContent(memoryStream.ToArray());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                    multipartContent.Add(new StringContent(applicationPost.Message), "Message");
                    multipartContent.Add(new StringContent(applicationPost.PostId.ToString()), "PostId");

                    multipartContent.Add(fileContent, "Resume", applicationPost.Resume.FileName);

                    HttpClient client = new HttpClient();
                    //var contentType = new MediaTypeWithQualityHeaderValue("multipart/form-data");
                    //client.DefaultRequestHeaders.Accept.Add(contentType);
                    string accessToken = (AuthUtils.loginUser == null) ? "" : AuthUtils.loginUser.AccessToken;
                    if (accessToken != "")
                    {
                        client.DefaultRequestHeaders.Authorization
                                 = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Content = multipartContent;
                    HttpResponseMessage response = await client.SendAsync(request);
                    string strData = await response.Content.ReadAsStringAsync();
                    string check = await CheckResponseIsSuccess(response, strData);
                    if (check == "Call request again!")
                    {
                        response = await client.SendAsync(request);

                        strData = await response.Content.ReadAsStringAsync();
                        check = await CheckResponseIsSuccess(response, strData);
                    }
                    return strData;
                }
            }
            else
            {
                throw new Exception("Invalid file!");
            }

        }


        public static async Task<string> SendPostRequestFormFileAsync(string url, IFormFile file)
        {
            if (file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    //Get the file steam from the multiform data uploaded from the browser
                    await file.CopyToAsync(memoryStream);

                    //Build an multipart/form-data request to upload the file to Web API
                    using var form = new MultipartFormDataContent();
                    using var fileContent = new ByteArrayContent(memoryStream.ToArray());
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                    form.Add(fileContent, "file", file.FileName);

                    HttpClient client = new HttpClient();
                    //var contentType = new MediaTypeWithQualityHeaderValue("multipart/form-data");
                    //client.DefaultRequestHeaders.Accept.Add(contentType);
                    string accessToken = (AuthUtils.loginUser == null) ? "" : AuthUtils.loginUser.AccessToken;
                    if (accessToken != "")
                    {
                        client.DefaultRequestHeaders.Authorization
                                 = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Content = form;
                    HttpResponseMessage response = await client.SendAsync(request);
                    string strData = await response.Content.ReadAsStringAsync();
                    string check = await CheckResponseIsSuccess(response, strData);
                    if (check == "Call request again!")
                    {
                        response = await client.SendAsync(request);

                        strData = await response.Content.ReadAsStringAsync();
                        check = await CheckResponseIsSuccess(response, strData);
                    }
                    return strData;
                }
            } else
            {
                throw new Exception("Invalid file!");
            }
            
        }

        public static async Task<string> SendPutRequestAsync<T>(string url, T payload) where T : class
        {
            HttpClient client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            string accessToken = (AuthUtils.loginUser == null) ? "" : AuthUtils.loginUser.AccessToken;
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            HttpResponseMessage response = await client.PutAsJsonAsync(url, payload);
            string strData = await response.Content.ReadAsStringAsync();
            string check = await CheckResponseIsSuccess(response, strData);
            if (check == "Call request again!")
            {
                response = await client.PutAsJsonAsync(url, payload);
                strData = await response.Content.ReadAsStringAsync();
                check = await CheckResponseIsSuccess(response, strData);
            }
            return strData;
        }

        public static async Task<string> SendPatchRequestAsync<T>(string url, T payload) where T : class
        {
            HttpClient client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            string accessToken = (AuthUtils.loginUser == null) ? "" : AuthUtils.loginUser.AccessToken;
            if (accessToken != "")
            {
                client.DefaultRequestHeaders.Authorization
                         = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            var requestContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PatchAsync(url, requestContent);
            string strData = await response.Content.ReadAsStringAsync();
            string check = await CheckResponseIsSuccess(response, strData);
            if (check == "Call request again!")
            {
                response = await client.PatchAsync(url, requestContent);
                strData = await response.Content.ReadAsStringAsync();
                check = await CheckResponseIsSuccess(response, strData);
            }
            return strData;
        }

        public static T DeserializeResponse<T>(string responseStringData) where T : class
        {
            T objectData = JsonSerializer.Deserialize<T>(responseStringData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            return objectData;
        }

        public static List<T> DeserializeODataJArrayControllerResponse<T>(string responseStringData) where T : class
        {
            List<T> res = new List<T>();
            try
            {
                JObject responseJson = (responseStringData == "") ? JObject.Parse("{ 'value': null }") : JObject.Parse(responseStringData);
                JArray jArray = (JArray)responseJson["value"];
                res = jArray.ToObject<List<T>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "Error at DeserializeODataJArrayControllerResponse: " + ex.Message);
            }
            return res;
        }
    }
}
