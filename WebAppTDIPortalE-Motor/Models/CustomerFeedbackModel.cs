using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppTDIPortalE_Motor.Models
{
    public class CustomerFeedbackModel
    {
        public CustomerFeedbackModel()
        {

        }

        public Int64 cf_num { get; set; }
        public string CustomerCode { get; set; }
        public string item { get; set; }
        public byte[] picture { get; set; }
        public string strpicture { get; set; }
        public string description { get; set; }
        public HttpPostedFileBase pictureFile { get; set; }

    }
}