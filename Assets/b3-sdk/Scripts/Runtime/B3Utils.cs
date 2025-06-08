using System;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
namespace BasementSDK.Utils {
    public static class B3Utils
    {
        public class JSONAble
        {
            public string ToJSON() => JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static Task<string> GetJSON(string url, string serviceMethodHeader, Action<string> callback)
        {
            return GetJSONAsyncHandler(url, serviceMethodHeader, callback);
        }
        public static Task<string> PostJSON(string url, string serviceMethodHeader, string body, Action<string> callback)
        {
            return PostJSONAsyncHandler(url, serviceMethodHeader, body, callback);
        }
        #if UNITY_WEBGL 
        public static async Task<string> PostJSONAsyncHandler(string url, string serviceMethodHeader, string body, Action<string> callback)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                try {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(serviceMethodHeader))
                    request.SetRequestHeader("X-Service-Method", serviceMethodHeader);
                
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(null);
                    return null;
                }
                
                string res = request.downloadHandler.text;
                callback?.Invoke(res);
                return res;
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw e;
                }
            }
        }

        public static async Task<string> GetJSONAsyncHandler(string url, string serviceMethodHeader, Action<string> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                try {
                    if (!string.IsNullOrEmpty(serviceMethodHeader))
                    request.SetRequestHeader("X-Service-Method", serviceMethodHeader);
                
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }
                    
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        callback?.Invoke(null);
                        return null;
                    }
                    
                    string res = request.downloadHandler.text;
                    callback?.Invoke(res);
                    return res;
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw e;
                }
                
            }
        }
        #else
        public static async Task<string> PostJSONAsyncHandler(string url, string serviceMethodHeader, string body, Action<string> callback)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(3);
            if (serviceMethodHeader != null)
                client.DefaultRequestHeaders.Add("X-Service-Method", serviceMethodHeader);
            var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            try
            {
                var httpResponse = await client.PostAsync(url, httpContent);
                if (!httpResponse.IsSuccessStatusCode){
                    callback?.Invoke(null);
                    return null;
                }
                string res = await httpResponse.Content.ReadAsStringAsync();
                callback?.Invoke(res);
                return res;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                // Handle timeout.
                callback?.Invoke(null);
                return null;
            }
        }

        public static async Task<string> GetJSONAsyncHandler(string url, string serviceMethodHeader, Action<string> callback)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(3);
            if (serviceMethodHeader != null)
                client.DefaultRequestHeaders.Add("X-Service-Method", serviceMethodHeader);
            var httpResponse = await client.GetAsync(url);
            if (!httpResponse.IsSuccessStatusCode) {
                callback?.Invoke(null);
                return null;
            }
            string res = await httpResponse.Content.ReadAsStringAsync();
            callback?.Invoke(res);
            return res;
        }
        #endif

        public static async Task<byte[]> RequestGetRaw(string url, Action<byte[]> callback)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(3);
            var httpResponse = await client.GetAsync(url);
            if (!httpResponse.IsSuccessStatusCode) {
                callback?.Invoke(null);
                return null;
            }
            byte[] res = await httpResponse.Content.ReadAsByteArrayAsync();
            callback?.Invoke(res);
            return res;
        }

        public static bool IsConnected => !(Application.internetReachability == NetworkReachability.NotReachable);

        public static IEnumerator WaitForConnectionCoroutine(Action onConnect)
        {
            while (!IsConnected) yield return null;
            onConnect?.Invoke();
        }
    }
}