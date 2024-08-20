using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppTDIPortalE_Motor.Models
{
    public class PKPModel
    {
        public PKPModel()
        {
        }
        
        public string stat { get; set; }
        public string cust_seq { get; set; }
        public string pkp { get; set; }
        public DateTime pkp_date { get; set; }
        public string cust_num { get; set; }
        public string cust_name { get; set; }
        public string ship_to { get; set; }
        public string pic { get; set; }
        public string jabatan { get; set; }
        public string model { get; set; }
        public string item { get; set; }
        public string description { get; set; }
        public int qty { get; set; }
        public string um { get; set; }
        public string jol { get; set; }
        public string no_rangka { get; set; }
        public int tdi { get; set; }
        public int nontdi { get; set; }
        public int kep_perbaikan { get; set; }
        public byte[] attach { get; set; }
        public string strattach { get; set; }
        public string analisa_penyebab { get; set; }
        public string tindakan_perbaikan { get; set; }
        public string catatan { get; set; }
        public HttpPostedFileBase pictureFile { get; set; }
    }
}