using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppTDIPortalE_Motor.Models
{
    public class SiteModels
    {
        public SiteModels()
        {

        }

        public class Items
        {
            public string Site { get; set; }
            public string _ItemId { get; set; }
        }

        public class Root
        {
            public string Message { get; set; }
            public int MessageCode { get; set; }
            public List<Items> Items { get; set; }
            public string Bookmark { get; set; }
        }
    }
}