using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using WebAppTDIPortalE_Motor.Models;

[assembly: OwinStartupAttribute(typeof(WebAppTDIPortalE_Motor.Startup))]
namespace WebAppTDIPortalE_Motor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //IDOWebServiceReference.DOWebServiceSoapClient soapClient = new IDOWebServiceReference.DOWebServiceSoapClient();
            //string _token = "";
            //try
            //{
            //    string userId = ConfigurationManager.AppSettings["UserId"].ToString();
            //    string pswd = ConfigurationManager.AppSettings["Pswd"].ToString();
            //    string config = ConfigurationManager.AppSettings["Config"].ToString();
            //    _token = soapClient?.CreateSessionToken(userId, pswd, config);

            //}
            //catch (System.Exception)
            //{
            //    _token = "";
            //}

            //Global.Token = _token;
            //string filter = "";
            //string ido = "SLParms";
            //string properties = "Site";
            //string orderBy = "Site";
            //string postQueryMethod = string.Empty;
            //int recordCap = -1;

            //DataSet sites = soapClient.LoadDataSet(Global.Token, ido, properties, filter, orderBy, postQueryMethod, recordCap);
            //DataTable idoTable = sites.Tables[0];

            //for (int i = 0; i < idoTable.Rows.Count; i++)
            //{
            //    DataRow row = idoTable.Rows[i];
            //    Global.Site = row["Site"].ToString();
            //}

            //string _token = "";
            
            //using (HttpClient client = new HttpClient())
            //{
            //    // provide your Mongoose credentails in the request url
            //    string userId = ConfigurationManager.AppSettings["UserId"].ToString();
            //    string pswd = ConfigurationManager.AppSettings["Pswd"].ToString();
            //    string config = ConfigurationManager.AppSettings["Config"].ToString();
            //    string requestUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString() + $"/js/token/{config}";
            //    // provide your Mongoose credentials as headers
            //    client.DefaultRequestHeaders.Add("userid", userId);
            //    client.DefaultRequestHeaders.Add("password", pswd);
            //    // send the get request
            //    HttpResponseMessage response = client.GetAsync(requestUrl).Result;
            //    using (HttpContent content = response.Content)
            //    {
            //        Task<string> result = content.ReadAsStringAsync();
            //        // this contains your token
            //        _token = result.Result;
            //    }
            //}

            Global.Site = ConfigurationManager.AppSettings["Site"].ToString();

            ConfigureAuth(app);
            CreateRolesandUsers();
        }

        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var roles = new[] { "Admin", "Dealer" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }
            }

            string adminUser = "msiportal.adm@gmail.com";
            string adminEmail = "msiportal.adm@gmail.com";
            string userPWD = "P4s5Adm1n2023!!";

            if (userManager.FindByName(adminUser) == null)
            {
                var user = new ApplicationUser();
                user.UserName = adminUser;
                user.Email = adminEmail;
                user.EmailConfirmed = true;
                user.UserDescription = "Admin Portal MSI";
                user.CustomerCode = "9999999";
                user.Warehouse = "9999";

                userManager.Create(user, userPWD);

                userManager.AddToRole(user.Id, "Admin");

                if (userManager.FindByName(adminUser) != null)
                {
                    user = new ApplicationUser();
                    user.UserName = "msiportal.usr@gmail.com";
                    user.Email = "msiportal.usr@gmail.com";
                    user.EmailConfirmed = true;
                    user.UserDescription = "User Portal MSI";
                    user.CustomerCode = "9999999";
                    user.Warehouse = "9999";
                    userPWD = "PortalP4s52023!!";

                    userManager.Create(user, userPWD);

                    userManager.AddToRole(user.Id, "Dealer");
                }

            }

        }
    }
}
