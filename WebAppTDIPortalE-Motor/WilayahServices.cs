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

        public List<T> GetWilayah(string wilayah, string id = null)
        {
            List<T> sInfo = new List<T>();
            using (HttpClient client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = client.GetAsync("/" + wilayah + "/" + id).Result;
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var sResponse = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    sInfo = JsonConvert.DeserializeObject<List<T>>(sResponse);
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
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = client.GetAsync("/" + wilayah).Result;
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var sResponse = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    sInfo = JsonConvert.DeserializeObject<List<T>>(sResponse);
                }
                //returning the employee list to view
                return sInfo;
            }
        }
    }
}