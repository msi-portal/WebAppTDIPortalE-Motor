using System.Web;
using System.Web.Mvc;

namespace WebAppTDIPortalE_Motor
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
