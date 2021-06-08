using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace yias.service
{
    public class ssqservice
    {

        public void GetAllSsqData()
        {
            
        }


        class httpUtilCalss
        {
            HttpClient _client;
            HttpResponseMessage _response;

            public bool actionRequest(string url, out string respResult)
            {
                respResult = null;
                HttpContent httpContent = new StringContent("");
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                try
                {
                    string sUrl = url;
                    _response = _client.PostAsync(sUrl, httpContent).Result;
                    if (!_response.IsSuccessStatusCode)
                    {
                        respResult = "接口请求失败！" + _response.RequestMessage;
                        return false;
                    }
                    respResult = _response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    respResult = "接口请求失败！" + ex.Message;
                    return false;
                }
                return true;
            }
        }
    }
}
