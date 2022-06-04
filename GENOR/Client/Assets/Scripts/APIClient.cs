using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

public class APIClient
{
    private const string BASE_URL = "http://51.222.155.135:8000/";

    public static IEnumerator Get<TResponseType>(string parameter, Action<TResponseType> onSuccess)
    {
       UnityWebRequest www = UnityWebRequest.Get(BASE_URL + parameter);

        yield return www.Send();

        while (!www.isDone) {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.Success) {
            string rawResponse = www.downloadHandler.text;
            rawResponse = rawResponse.Substring(1);
            rawResponse = rawResponse.Remove(rawResponse.Length - 1, 1);
            rawResponse = rawResponse.Replace(@"\", "");

            onSuccess(JsonConvert.DeserializeObject<TResponseType>(rawResponse,
                new JsonSerializerSettings { 
                    NullValueHandling = NullValueHandling.Ignore
                }));

        } else {
            Debug.LogError(www.error);
            onSuccess(default);
        }
    }

    public static IEnumerator Post<TRequestType, TResponseType>(string parameter, TRequestType body, Action<TResponseType> onSuccess)
    {
        UnityWebRequest www = UnityWebRequest.Post(BASE_URL + parameter, JsonConvert.SerializeObject(body));
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.Send();

        while (!www.isDone) {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.Success) {
            string rawResponse = www.downloadHandler.text;
            rawResponse = rawResponse.Substring(1);
            rawResponse = rawResponse.Remove(rawResponse.Length - 1, 1);
            rawResponse = rawResponse.Replace(@"\", "");

            onSuccess(JsonConvert.DeserializeObject<TResponseType>(rawResponse));

        } else {
            Debug.LogError(www.error);
            onSuccess(default);
        }
    }

    public static IEnumerator Post<TResponseType>(string parameter, string fileName, Action<TResponseType> onSuccess)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("file", File.ReadAllBytes(fileName), "uploadedFile", ""));

        UnityWebRequest www = UnityWebRequest.Post(BASE_URL + parameter, formData);

        yield return www.Send();

        while (!www.isDone) {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.Success) {
            string rawResponse = www.downloadHandler.text;
            rawResponse = rawResponse.Substring(1);
            rawResponse = rawResponse.Remove(rawResponse.Length - 1, 1);
            rawResponse = rawResponse.Replace(@"\", "");

            onSuccess(JsonConvert.DeserializeObject<TResponseType>(rawResponse));

        } else {
            Debug.LogError(www.error);
            onSuccess(default);
        }
    }

    public static IEnumerator Delete<TResponseType>(string parameter, Action<TResponseType> onSuccess)
    {
        UnityWebRequest www = UnityWebRequest.Delete(BASE_URL + parameter);

        yield return www.Send();

        while (!www.isDone) {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.Success) {
            string rawResponse = www.downloadHandler.text;
            rawResponse = rawResponse.Substring(1);
            rawResponse = rawResponse.Remove(rawResponse.Length - 1, 1);
            rawResponse = rawResponse.Replace(@"\", "");

            onSuccess(JsonConvert.DeserializeObject<TResponseType>(rawResponse,
                new JsonSerializerSettings { 
                    NullValueHandling = NullValueHandling.Ignore
                }));

        } else {
            Debug.LogError(www.error);
            onSuccess(default);
        }
    }
}
