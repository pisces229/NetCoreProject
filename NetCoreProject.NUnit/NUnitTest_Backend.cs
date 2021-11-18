using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Logic;
using NetCoreProject.BusinessLayer.Model.Test;
using NetCoreProject.DataLayer.Manager;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreProject.NUnit
{
    public class NUnitTest_Backend
    {
        public NUnitTest_Backend()
        {

        }
        private string GetHttpClientUrl(string method) 
            => $"https://localhost:44392/api/{ method }";
        private HttpClientHandler DefaultHttpMessageHandler()
            => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        #region Model
        private class CommonApiResultModel<T> where T : class
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
        #endregion
        [SetUp]
        public async Task Setup()
        {
            await Task.FromResult("do something");
        }
        [Test]
        public async Task Test_Token()
        {
            var jwtToken = string.Empty;
            {
                Console.WriteLine("---------- Login/SignIn ----------");
                var postData = new Dictionary<string, string>
                {
                    { "Username", "Username" },
                    { "Password", "Password" }
                };
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                using var stringContent = new StringContent(JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Login/SignIn"), stringContent);
                Console.WriteLine($"> StatusCode:{ httpResponseMessage.StatusCode }");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
                    Console.WriteLine($"> { contentString }");
                    jwtToken = JsonSerializer.Deserialize<CommonApiResultModel<string>>(contentString).Data;
                }
            }
            {
                Console.WriteLine("---------- Test/PostValueByValue ----------");
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                using var stringContent = new StringContent(JsonSerializer.Serialize(jwtToken), Encoding.UTF8, "application/json");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Test/PostValueByValue"), stringContent);
                Console.WriteLine($"> StatusCode:{ httpResponseMessage.StatusCode }");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"> { await httpResponseMessage.Content.ReadAsStringAsync() }");
                }
            }
            Thread.Sleep(5000);
            {
                Console.WriteLine("---------- Test/PostValueByValue ----------");
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                using var stringContent = new StringContent(JsonSerializer.Serialize(jwtToken), Encoding.UTF8, "application/json");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Test/PostValueByValue"), stringContent);
                Console.WriteLine($"> StatusCode:{ httpResponseMessage.StatusCode }");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"> { await httpResponseMessage.Content.ReadAsStringAsync() }");
                }
            }
            {
                Console.WriteLine("---------- Login/Refresh ----------");
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                using var stringContent = new StringContent(JsonSerializer.Serialize(jwtToken), Encoding.UTF8, "application/json");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Login/Refresh"), stringContent);
                Console.WriteLine($"> StatusCode:{ httpResponseMessage.StatusCode }");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
                    Console.WriteLine($"> { contentString }");
                    jwtToken = contentString;
                }
            }
            {
                Console.WriteLine("---------- Test/PostValueByValue ----------");
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                using var stringContent = new StringContent(JsonSerializer.Serialize(jwtToken), Encoding.UTF8, "application/json");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Test/PostValueByValue"), stringContent);
                Console.WriteLine($"> StatusCode:{ httpResponseMessage.StatusCode }");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"> { await httpResponseMessage.Content.ReadAsStringAsync() }");
                }
            }
        }
        //[Test]
        public async Task Test_Download()
        {
            var jwtToken = string.Empty;
            {
                var postData = new Dictionary<string, string>
                {
                    { "Username", "Username" },
                    { "Password", "Password" }
                };
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                using var stringContent = new StringContent(JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Login/SignIn"), stringContent);
                Console.WriteLine($">>>>>SignIn:{ httpResponseMessage.StatusCode }");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    jwtToken = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            {
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                var httpResponseMessage = await httpClient.GetAsync(GetHttpClientUrl("Test/GetDownload"));
                Console.WriteLine($">>>>>Download:{ httpResponseMessage.StatusCode }");
                //httpResponseMessage.Headers.AsEnumerable().ToList().ForEach(header =>
                //{
                //    Console.WriteLine($"Key:{header.Key}");
                //    header.Value.AsEnumerable().ToList().ForEach(value =>
                //    {
                //        Console.WriteLine($"Value:{value}");
                //    });
                //});
                Console.WriteLine($"ContentDisposition:{ httpResponseMessage.Content.Headers.ContentDisposition }");
                Console.WriteLine($"ContentType:{ httpResponseMessage.Content.Headers.ContentType }");
                Console.WriteLine($"ContentLength:{ httpResponseMessage.Content.Headers.ContentLength }");
                Console.WriteLine($"Content:{ await httpResponseMessage.Content.ReadAsStringAsync() }");
            }
        }
        //[Test]
        public async Task Test_Uploads()
        {
            var jwtToken = string.Empty;
            {
                var postData = new Dictionary<string, string>
                {
                    { "Username", "Username" },
                    { "Password", "Password" }
                };
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                using var stringContent = new StringContent(JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Login/SignIn"), stringContent);
                Console.WriteLine($">>>>>SignIn:{ httpResponseMessage.StatusCode }");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    jwtToken = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
            {
                using var httpClient = new HttpClient(DefaultHttpMessageHandler());
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                using var multipartFormDataContent = new MultipartFormDataContent();
                var fileStream = File.OpenRead("D:/WorkSpace/SampleProject/Temp/test.txt");
                using var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                multipartFormDataContent.Add(streamContent, "[0].UPLOAD_FILE", "test.txt");
                multipartFormDataContent.Add(new StringContent("UPLOAD_NAME0", Encoding.UTF8), "[0].UPLOAD_NAME");
                multipartFormDataContent.Add(new StringContent("UPLOAD_TYPE0", Encoding.UTF8), "[0].UPLOAD_TYPE");
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                multipartFormDataContent.Add(streamContent, "[1].UPLOAD_FILE", "test.txt");
                multipartFormDataContent.Add(new StringContent("UPLOAD_NAME1", Encoding.UTF8), "[1].UPLOAD_NAME");
                multipartFormDataContent.Add(new StringContent("UPLOAD_TYPE1", Encoding.UTF8), "[1].UPLOAD_TYPE");
                var httpResponseMessage = await httpClient.PostAsync(GetHttpClientUrl("Test/Uploads"), multipartFormDataContent);
                Console.WriteLine($">>>>>Uploads:{ httpResponseMessage.StatusCode }");
            }
        }
    }
}
