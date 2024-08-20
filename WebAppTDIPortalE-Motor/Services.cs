using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace WebAppTDIPortalE_Motor
{
    public class Services<T>
    {
        public Services()
        {

        }

        public DataSet GetDataSet(string ido, string properties, string filter = null, string orderBy = "")
        {
            DataSet ds = new DataSet();

            //;
            //string postQueryMethod = string.Empty;
            //int recordCap = 0;

            //IDOWebServiceReference.DOWebServiceSoapClient soapClient = new IDOWebServiceReference.DOWebServiceSoapClient();

            //ds = soapClient.LoadDataSet(Global.Token, ido, properties, filter, orderBy, postQueryMethod, recordCap);

            return ds;
        }

        public string GetJSON(string ido, string properties, string filter = null, string orderBy = "")
        {
            string ds = "";

            //;
            //string postQueryMethod = string.Empty;
            //int recordCap = 0;

            //IDOWebServiceReference.DOWebServiceSoapClient soapClient = new IDOWebServiceReference.DOWebServiceSoapClient();

            //try
            //{
            //    ds = soapClient.LoadJson(Global.Token, ido, properties, filter, orderBy, postQueryMethod, recordCap);

            //}
            //catch (Exception er)
            //{
            //    ds = er.Message.ToString();
            //}

            return ds;
        }

        public T JsonToList(string ido, string properties, string filter = null, string orderBy = "")
        {
            //string ds = "";
            T resultList = JsonConvert.DeserializeObject<T>("");

            //;
            //string postQueryMethod = string.Empty;
            //int recordCap = 0;

            //IDOWebServiceReference.DOWebServiceSoapClient soapClient = new IDOWebServiceReference.DOWebServiceSoapClient();

            //try
            //{
            //    ds = soapClient.LoadJson(Global.Token, ido, properties, filter, orderBy, postQueryMethod, recordCap);

            //}
            //catch (Exception er)
            //{
            //    ds = er.Message.ToString();
            //}

            //var settings = new JsonSerializerSettings
            //{
            //    NullValueHandling = NullValueHandling.Ignore,
            //    MissingMemberHandling = MissingMemberHandling.Ignore
            //};
            //resultList = JsonConvert.DeserializeObject<T>(ds, settings);

            return resultList;
        }

        public T GetList(string ido, string properties, string filter)
        {
            using (HttpClient client = new HttpClient())
            {
                T resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<T>("");
                string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/js/{ido}/adv?props={properties}&filter={filter}&rowcap=0";
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Global.Token);
                HttpResponseMessage response = client.GetAsync(requestUrl).Result;

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                using (HttpContent content = response.Content)
                {
                    string jsonData = content.ReadAsStringAsync().Result;
                    resultList = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData, settings);
                }

                return resultList;
            }
        }

    }
}