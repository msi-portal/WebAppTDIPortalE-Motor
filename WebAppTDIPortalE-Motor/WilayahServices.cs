using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using WebAppTDIPortalE_Motor.Models;

namespace WebAppTDIPortalE_Motor
{
    public class WilayahServices<T>
    {
        public WilayahServices()
        {

        }

        string Baseurl = ConfigurationManager.AppSettings["API_URL"].ToString();
        string ApiKey = ConfigurationManager.AppSettings["API_KEY"].ToString();

        public List<T> GetWilayah(string wilayah, string id = null)
        {
            List<T> sInfo = new List<T>();
            using (HttpClient client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ApiKey);
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = client.GetAsync("/" + wilayah + "/" + id).Result;
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    // Parsing success response
                    var sResponse = Res.Content.ReadAsStringAsync().Result;
                    sInfo = JsonConvert.DeserializeObject<List<T>>(sResponse);
                }
                else
                {
                    // Handling error response and returning ApiErrorResponse in List<T>
                    ApiErrorResponse errorResponse = new ApiErrorResponse
                    {
                        StatusCode = (int)Res.StatusCode,
                        ReasonPhrase = Res.ReasonPhrase,
                        Content = Res.Content.ReadAsStringAsync().Result,
                        Headers = Res.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                        id = ((int)Res.StatusCode).ToString(),
                        description = Res.ReasonPhrase
                    };

                    // To return the error response as a part of the List<T>, we need to convert it to T type
                    // This can be done by serializing the errorResponse and deserializing it to T type
                    sInfo.Add(JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(errorResponse)));
                }
                //returning the employee list to view
                return sInfo;
            }
        }

        public List<T> GetWilayahId(string wilayah)
        {
            List<T> sInfo = new List<T>();
            using (HttpClient client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ApiKey);
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = client.GetAsync("/" + wilayah).Result;
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    // Parsing success response
                    var sResponse = Res.Content.ReadAsStringAsync().Result;
                    sInfo = JsonConvert.DeserializeObject<List<T>>(sResponse);
                }
                else
                {
                    // Handling error response and returning ApiErrorResponse in List<T>
                    ApiErrorResponse errorResponse = new ApiErrorResponse
                    {
                        StatusCode = (int)Res.StatusCode,
                        ReasonPhrase = Res.ReasonPhrase,
                        Content = Res.Content.ReadAsStringAsync().Result,
                        Headers = Res.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                        id = ((int)Res.StatusCode).ToString(),
                        description = Res.ReasonPhrase
                    };

                    // To return the error response as a part of the List<T>, we need to convert it to T type
                    // This can be done by serializing the errorResponse and deserializing it to T type
                    sInfo.Add(JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(errorResponse)));
                }
                //returning the employee list to view
                return sInfo;
            }
        }
    }
}