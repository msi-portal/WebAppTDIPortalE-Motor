using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAppTDIPortalE_Motor.Models;

namespace WebAppTDIPortalE_Motor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                if (userViewModel.CustomerCode.IndexOf(" - ") < 0)
                {
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    ModelState.AddModelError("", "Supplier not found");
                    return View();
                }
                string vend = userViewModel.CustomerCode.Substring(0, userViewModel.CustomerCode.IndexOf(" - "));
                string whse = userViewModel.Warehouse.Substring(0, userViewModel.Warehouse.IndexOf(" - "));
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, CustomerCode = vend, Warehouse = whse };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                        else
                        {
                            bool isSendEmail = await SendEmail(user.Id);
                            if (isSendEmail)
                            {
                                ModelState.AddModelError("", "Email sent.");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Send email failed.");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                CustomerCode = user.CustomerCode,
                Warehouse = user.Warehouse,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        public async Task<ActionResult> SendEmailConfimation(string id)
        {

            bool isSendEmail = await SendEmail(id);
            if (isSendEmail)
            {
                ModelState.AddModelError("", "Email sent.");
            }
            else
            {
                ModelState.AddModelError("", "Send email failed.");
            }
            return RedirectToAction("Index");
        }

        public async Task<bool> SendEmail(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{ConfirmationLink}", callbackUrl);
            body = body.Replace("{UserName}", user.Email);
            bool IsSendEmail = EmailService.SendEmail.EmailSend(user.Email, "Confirm your account", body, true);
            return IsSendEmail;
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,CustomerCode,Warehouse,RolesList")] EditUserViewModel editUser, params string[] selectedRole)
        {
            var user = await UserManager.FindByIdAsync(editUser.Id);
            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    return HttpNotFound();
                }
                string vend = "";
                string whse = "";
                if (editUser.CustomerCode.IndexOf(" - ") <= 0)
                {
                    vend = editUser.CustomerCode;
                }
                else
                {
                    vend = editUser.CustomerCode.Substring(0, editUser.CustomerCode.IndexOf(" - "));
                }
                if (editUser.Warehouse.IndexOf(" - ") <= 0)
                {
                    whse = editUser.Warehouse;
                }
                else
                {
                    whse = editUser.Warehouse.Substring(0, editUser.Warehouse.IndexOf(" - "));
                }
                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.CustomerCode = vend;
                user.Warehouse = whse;
                var result = await UserManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var resultRole = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!resultRole.Succeeded)
                {
                    ModelState.AddModelError("", resultRole.Errors.First());
                    return View();
                }

                resultRole = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!resultRole.Succeeded)
                {
                    ModelState.AddModelError("", resultRole.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email = user.UserName,
                CustomerCode = user.CustomerCode,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = selectedRole.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            }
            );
        }

        //
        // GET: /Users/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ImportUserAsync(HttpPostedFileBase postedFile)
        {
            try
            {
                //using (var reader = new StreamReader("path\\to\\file.csv"))
                //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                //{
                //    var records = csv.GetRecords<RegisterViewModel>();
                //}
                List<RegisterViewModel> userModel = new List<RegisterViewModel>();
                string filePath = string.Empty;
                if (postedFile != null)
                {
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    //Read the contents of CSV file.
                    string csvData = System.IO.File.ReadAllText(filePath);
                    ApplicationDbContext context = new ApplicationDbContext();

                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                    //Execute a loop over the rows.
                    foreach (string row in csvData.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            string[] rowVar = row.Split(',');
                            if (rowVar[0] != "customercode")
                            {
                                var user = new ApplicationUser();
                                user.UserName = rowVar[2];
                                user.CustomerCode = rowVar[0];
                                user.Email = rowVar[2];
                                user.EmailConfirmed = true;
                                user.Warehouse = rowVar[4];
                                string userPWD = rowVar[3];

                                var users = await UserManager.FindByEmailAsync(user.UserName);
                                if (users == null)
                                {

                                    //var chkUser = UserManager.Create(user, userPWD); ;
                                    var chkUser = await UserManager.CreateAsync(user, userPWD);

                                    if (chkUser.Succeeded)
                                    {
                                        var result1 = UserManager.AddToRole(user.Id, "Dealer");
                                    }
                                }
                            }
                            //userModel.Add(new RegisterViewModel
                            //{
                            //    Email = rowVar[1],
                            //    SupplierCode = rowVar[0],
                            //    Password = rowVar[3],

                            //    //Email = Convert.ToInt32(row.Split(',')[0]),
                            //    //Name = row.Split(',')[1],
                            //    //Country = row.Split(',')[2]
                            //});

                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }


            return RedirectToAction("Index");
        }

    }

}