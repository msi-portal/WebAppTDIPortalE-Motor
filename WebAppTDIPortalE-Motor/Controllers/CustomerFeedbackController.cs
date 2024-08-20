using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppTDIPortalE_Motor.Models;

namespace WebAppTDIPortalE_Motor.Controllers
{
    public class CustomerFeedbackController : Controller
    {
        // GET: CustomerFeedback
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(JqueryDatatableParam param)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "";
            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;


            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            string custNum = custData[0].cust_num;

            strsql = "select cf.cf_num, cf.item, cf.picture, cf.description ";
            strsql += " from tdi_customer_feedback_mst cf";
            strsql += " inner join item_mst it on it.item = cf.item and it.site_ref = cf.site_ref ";
            strsql += " where cf.site_ref = '" + Global.Site + "' and ltrim(rtrim(cf.CustomerCode))=ltrim(rtrim('" + custNum + "'))" +
                " order by cf.CreateDate desc";

            List<CustomerFeedbackModel> coData = new DAO<CustomerFeedbackModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                coData = coData.Where(x => x.cf_num.ToString().ToLower().Contains(param.sSearch.ToLower())
                                              || x.item.ToLower().Contains(param.sSearch.ToLower())
                                              || (x.description ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var displayResult = coData.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = coData.Count();

            foreach (var item in displayResult)
            {
                if (item.picture != null)
                {
                    string base64String = Convert.ToBase64String(item.picture);
                    item.strpicture = $"data:image/jpeg;base64,{base64String}";
                }
            }

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }

        byte[] ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes 
            //to read from file.
            //In this case we want to read entire file. 
            //So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);

            return data;
        }

        [HttpPost]
        public JsonResult SaveFeedback(CustomerFeedbackModel model, FormCollection collection, HttpPostedFileBase pictureFile)
        {

            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "";
            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);
            decimal icost = 0;

            model.CustomerCode = custData[0].cust_num;
 
            if (result.CustomerCode == "9999999")
            {
                filterCust = " ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + custData[0].cust_num + "'))";
            }

            try
            {
                string path = Server.MapPath("~/Uploads/");
                string fileName = DateTime.Now.ToString("yyyyMMddTHHmmss");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                byte[] bytes = null;
                if (pictureFile != null)
                {

                    fileName += Path.GetFileName(pictureFile.FileName);
                    pictureFile.SaveAs(path + fileName);
                    CompressImage.Compressimage(pictureFile.InputStream, path + fileName);
                    bytes = ReadFile(path + fileName);
                }

                strsql = "INSERT INTO [tdi_customer_feedback_mst]" +
                    "           (" +
                    "           [item]" +
                    "           ,[CustomerCode]" +
                    "           ,[description]" +
                    "           ,[picture]" +
                    "           ,[site_ref])" +
                    "     VALUES" +
                    "           ('" + model.item + "'" +
                    "           ,'" + model.CustomerCode + "'" +
                    "           ,'" + model.description + "'" +
                    "           , @bytes" +
                    "           ,'" + Global.Site + "')";

                //int retVal = new DAO<IKBInputViewModel>().UpdateDataBySP(strsql);
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(strsql, con);
                    if (fileName != "")
                    {
                        IDataParameter par = cmd.CreateParameter();
                        par.ParameterName = "@bytes";
                        par.DbType = DbType.Binary;
                        par.Value = bytes;
                        cmd.Parameters.Add(par);
                    }
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", icost + "|" + strsql + "-" + e.Message);
                //return View("CreateCO", model);
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            //return RedirectToAction("Index", "CustomerFeedback");
        }

        [HttpPost]
        public JsonResult ItemAutoCompleteAll(string prefix, string cust_num)
        {
            string strsql = "";

            strsql = "SELECT distinct it.item Uf_Model, spec Uf_spec " +
                " FROM [tdi_emo_alloc_detail_mst] det_alloc" +
                " inner join [tdi_emo_alloc_mst] alloc on alloc.alloc_num=det_alloc.alloc_num" +
                " inner join [item_mst] it on it.Uf_model_dealer=det_alloc.model and it.item=det_alloc.item" +
                " and ltrim(rtrim(alloc.cust_num))=ltrim(rtrim('" + cust_num + "'))";
            strsql += " and it.product_code = 'FG-EMOT'";
            if (prefix != "")
            {
                strsql += " and it.item like '" + prefix + "%' or it.description like  '" + prefix + "%'";
            }

            List<ItemModel> itemData = new DAO<ItemModel>().RetrieveDataBySQL(strsql);

            List<SelectListItem> resData = new List<SelectListItem>();
            if (itemData != null)
            {
                itemData.ForEach(row =>
                {

                    resData.Add(new SelectListItem
                    {
                        Value = (string)row.Uf_Model,
                        Text = row.Uf_spec
                    });

                });
            }

            return Json(resData);
        }
    }
}