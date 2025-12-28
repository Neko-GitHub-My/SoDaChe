using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpUtils
{

    public delegate void OnHttpCompleted(string err, object body);

    // http://bycwedu.com:6080/test?uname=blake&upwd=123456
    public static void Get(string url, string param, OnHttpCompleted OnCompleted)
    {
        string urlPath = url;
        if (param != null) {
            urlPath = url + "?" + param;
        }

        UnityWebRequest wq = UnityWebRequest.Get(urlPath);
        wq.SendWebRequest().completed += (AsyncOperation opt) => {
            if (wq.error != null) {
                if (OnCompleted != null) {
                    OnCompleted(wq.error, null);
                }
            }
            else {
                if (OnCompleted != null) {
                    if (wq.downloadHandler.text != null) {
                        OnCompleted(null, wq.downloadHandler.text);
                    }
                    else {
                        OnCompleted(null, wq.downloadHandler.data);
                    }
                }
            }
            wq.Dispose();
        };
    }

    public static void Post(string url, string param, byte[] body, OnHttpCompleted OnCompleted)
    {
        string urlPath = url;
        if (param != null) {
            urlPath = url + "?" + param;
        }

        /*
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        // 设置请求体为二进制数据
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/octet-stream");
        */

        // 为了Cocos在浏览器状态下发送二进制数据(模拟器状态下是OK)，使用文本base64传递
        // 如果后续找到了方法可以直接传二进制，就后期传;
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        // 设置请求体为二进制数据, 将二进制数据等价转换成可打印字符编码的BASE64;
        string base64String = System.Convert.ToBase64String(body);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(base64String));
        request.downloadHandler = new DownloadHandlerBuffer();
        // 设置请求头
        request.SetRequestHeader("Content-Type", "text/plain");


        request.SendWebRequest().completed += (AsyncOperation opt) => {
            if (request.error != null)
            {
                if (OnCompleted != null)
                {
                    OnCompleted(request.error, null);
                }
            }
            else
            {
                if (OnCompleted != null)
                {
                    OnCompleted(null, request.downloadHandler.data);
                }
            }
            request.Dispose();
        };
    }

    public static void Post(string url, string param, string jsonBody, OnHttpCompleted OnCompleted)
    {
        string urlPath = url;
        if (param != null)
        {
            urlPath = url + "?" + param;
        }

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        // 设置请求体为二进制数据
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");


        request.SendWebRequest().completed += (AsyncOperation opt) => {
            if (request.error != null)
            {
                if (OnCompleted != null)
                {
                    OnCompleted(request.error, null);
                }
            }
            else
            {
                if (OnCompleted != null)
                {
                    OnCompleted(null, request.downloadHandler.data);
                }
            }
            request.Dispose();
        };
    }
    /*
    public static void Post(string url, string param, string jsonBody, OnHttpCompleted OnCompleted) {
        string urlPath = url;
        if (param != null) {
            urlPath = url + "?" + param;
        }

        UnityWebRequest wq = UnityWebRequest.Post(urlPath, jsonBody, "application/json");
        wq.SendWebRequest().completed += (AsyncOperation opt) => {
            if (wq.error != null) {
                if (OnCompleted != null) {
                    OnCompleted(wq.error, null);
                }
            }
            else {
                if (OnCompleted != null) {
                    if (wq.downloadHandler.text != null) {
                        OnCompleted(null, wq.downloadHandler.text);
                    }
                    else
                    {
                        OnCompleted(null, wq.downloadHandler.data);
                    }
                }
            }
            wq.Dispose();
        };
    }
    */
}
