using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;


namespace eMapy.BusinessLogic
{
    class JSONMethods
    {
        private void GetPOSTResponse(Uri uri, string data, Action<JSONDataContract.Response> callback)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.Method = "POST";
            request.ContentType = "text/plain;charset=utf-8";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] bytes = encoding.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Send the data.
                requestStream.Write(bytes, 0, bytes.Length);
            }

            request.BeginGetResponse((x) =>
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                {
                    if (callback != null)
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JSONDataContract.Response));
                        callback(ser.ReadObject(response.GetResponseStream()) as JSONDataContract.Response);
                    }
                }
            }, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="callback"></param>
        public void GetResponse(Uri uri, Action<JSONDataContract.Response> callback)
        {
            WebClient wc = new WebClient();

            wc.OpenReadCompleted += (o, a) =>
            {
                if (callback != null)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JSONDataContract.Response));
                    callback(ser.ReadObject(a.Result) as JSONDataContract.Response);
                }
            };
            wc.OpenReadAsync(uri);
        }
    }
}
