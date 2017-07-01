using UnityEngine;
using System.Collections;
using System.Text;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class BackendService
{
    private string GetDefaultUrl(string dataModel, bool isGetRequest)
    {
        var parameters = isGetRequest ? "?hrefs=false&withClassnameFilter=true&withReferencedHrefs=true" : string.Empty;
        return string.Format("{0}/{1}/{2}/models/{3}/{4}{5}", BackendConstants.BaseUrl, BackendConstants.EndpointApps, BackendConstants.AppName, BackendConstants.ModuleName, dataModel, parameters);
    }

    private void AddDefaultHeaders(UnityWebRequest request, bool isGetRequest)
    {
        var basicAuthorization = string.Format("{0}:{1}", BackendConstants.AuthorizationUsername, BackendConstants.AuthorizationPassword);
        var basicAuthorizationBytes = Encoding.UTF8.GetBytes(basicAuthorization);
        var basicAuthorizationEncoded = Convert.ToBase64String(basicAuthorizationBytes);

        request.SetRequestHeader("Accept", BackendConstants.ApplicationJson);
        request.SetRequestHeader("Authorization", string.Format("Basic {0}", basicAuthorizationEncoded));

        if (!isGetRequest)
        {
            request.SetRequestHeader("Content-Type", BackendConstants.ApplicationJson);
        }

        request.SetRequestHeader("X-apiomat-system", BackendConstants.Environment);
        request.SetRequestHeader("X-apiomat-sdkVersion", BackendConstants.SdkVersion);
        request.SetRequestHeader("X-apiomat-apikey", BackendConstants.AuthorizationApiKey);
    }

    private IEnumerator FetchData<T>(string dataModel, Action<List<T>> action)
    {
        var url = GetDefaultUrl(dataModel, true);
        var request = UnityWebRequest.Get(url);
        AddDefaultHeaders(request, true);

        yield return request.Send();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError(string.Format("Error while fetching data: {0}", request.error));
            action(null);
        }
        else
        {
            var text = request.downloadHandler.text;
            var list = JsonConvert.DeserializeObject<List<T>>(text);
            action(list);
        }
    }

    private IEnumerator PostCameraOrder(CameraOrderModel cameraOrder, Action<bool> action)
    {
        var url = GetDefaultUrl(BackendConstants.DataModelCameraOrder, false);
        var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        var postData = cameraOrder.GetPostDictionary(BackendConstants.AppName);
        var body = JsonConvert.SerializeObject(postData);
        var bytes = Encoding.UTF8.GetBytes(body);
        var handler = new UploadHandlerRaw(bytes);
        handler.contentType = BackendConstants.ApplicationJson;
        request.uploadHandler = handler;

        Debug.LogError(string.Format("Body: {0}", body));

        AddDefaultHeaders(request, false);

        yield return request.Send();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.LogError(string.Format("Error while posting data: {0}", request.error));

            foreach (var header in request.GetResponseHeaders())
            {
                Debug.LogError(string.Format("Header: {0}, Value: {1}", header.Key, header.Value));
            }

            action(false);
        }
        else
        {
            action(true);
        }
    }

    public IEnumerator FetchClients(Action<List<ClientModel>> action)
    {
        yield return FetchData(BackendConstants.DataModelClient, action);
    }

    public IEnumerator FetchCameras(Action<List<CameraModel>> action)
    {
        yield return FetchData(BackendConstants.DataModelCamera, action);
    }

    public IEnumerator FetchOrders(Action<List<CameraOrderModel>> action)
    {
        yield return FetchData(BackendConstants.DataModelCameraOrder, action);
    }

    public IEnumerator PostOrder(CameraOrderModel order, Action<bool> action)
    {
        yield return PostCameraOrder(order, action);
    }
}
