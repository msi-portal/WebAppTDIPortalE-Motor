using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppTDIPortalE_Motor.Models;

namespace WebAppTDIPortalE_Motor.Controllers
{
    [Authorize]
    public class PKPController : Controller
    {
        // GET: PKP
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCustomer()
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

            List<CustModel> listCust = new List<CustModel>();
            CustModel newItem = new CustModel();
            newItem.cust_num = custData[0].cust_num;
            newItem.name = custData[0].name;
            listCust.Add(newItem);

            return Json(new { Success = true, Counter = custData.Count, data = listCust }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetData(DataTableAjaxPostModel param)
        {

            //string dateFrom = Request.Form["dateFrom"].ToString();
            //string dateTo = Request.Form["dateTo"].ToString();
            //string grnFrom = Request.Form["grnFrom"].ToString();
            //string grnTo = Request.Form["grnTo"].ToString();
            //string vend = Request.Form["vend"].ToString();

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

            strsql = "select pkp.*, ca.name cust_name ";
            strsql += " from tdi_pkp_mst pkp " +
                " inner join  customer_mst cus on cus.cust_num=pkp.cust_num ";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where pkp.pkp like 'P%' and ltrim(rtrim(pkp.cust_num))=ltrim(rtrim('" + custNum + "'))";
            strsql += " order by pkp.CreateDate desc";

            List<PKPModel> listPKP = new DAO<PKPModel>().RetrieveDataBySQL(strsql);

            ViewBag.grnData = listPKP;
            string sortBy = "";
            bool sortDir = true;

            if (param.order != null)
            {
                // in this example we just default sort on the 1st column
                sortBy = param.columns[param.order[0].column].data;
                sortDir = param.order[0].dir.ToLower() == "asc";

            }

            if (param.order[0].dir == "asc")
                listPKP = listPKP.OrderBy(x => x.pkp_date).ToList();
            else
                listPKP = listPKP.OrderByDescending(x => x.pkp_date).ToList();


            if (!string.IsNullOrEmpty(param.search.value))
            {
                listPKP = listPKP.Where(x => (x.pkp ?? "").ToLower().Contains(param.search.value.ToLower())
                                              || (x.cust_num ?? "").ToLower().Contains(param.search.value.ToLower())
                                              || (x.cust_name ?? "").ToLower().Contains(param.search.value.ToLower())
                                              || x.pkp_date.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.search.value.ToLower())
                                              || (x.ship_to ?? "").ToString().Contains(param.search.value.ToLower())
                                              || (x.pic ?? "").ToString().Contains(param.search.value.ToLower())
                                              || (x.jabatan ?? "").ToString().Contains(param.search.value.ToLower())
                                              || (x.model ?? "").ToString().Contains(param.search.value.ToLower())
                                              || (x.item ?? "").ToString().Contains(param.search.value.ToLower())
                                              || (x.description ?? "").ToString().Contains(param.search.value.ToLower())
                                              ).ToList();
            }

            //string sortBy = "";
            //bool sortDir = true;

            //if (param.order != null)
            //{
            //    // in this example we just default sort on the 1st column
            //    sortBy = model.columns[model.order[0].column].data;
            //    sortDir = model.order[0].dir.ToLower() == "asc";
            //}

            var displayResult = listPKP.Skip(param.start)
            .Take(param.length).ToList();
            var totalRecords = listPKP.Count();

            return Json(new
            {
                param.draw,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ModelAutoCompleteAll(string prefix)
        {

            string strsql = "";

            strsql = "SELECT distinct it.Uf_model_dealer Uf_Model, typ.description Uf_spec " +
            " FROM [item_mst] it " +
            " inner join [tdi_type] typ on it.Uf_model_dealer=typ.code" +
            " where it.Uf_model_dealer like '" + prefix + "%'";
            strsql += " and it.product_code in('FG-EMOT','PARTS-EMOT')";

            List<ItemModel> itemData = new DAO<ItemModel>().RetrieveDataBySQL(strsql);

            List<SelectListItem> resData = new List<SelectListItem>();
            if (itemData != null)
            {
                itemData.ForEach(row =>
                {

                    resData.Add(new SelectListItem
                    {
                        Value = row.Uf_Model,
                        Text = row.Uf_spec// + (sparepart == "false" ? " - " + row.Uf_spec : "")
                    });

                });
            }

            return Json(resData);
        }

        [HttpPost]
        public JsonResult ItemAutoCompleteAll(string prefix, string model)
        {

            string strsql = "";

            strsql = "SELECT distinct it.item Uf_Model, it.description Uf_spec " +
                " FROM [item_mst] it " +
                " where it.Uf_model_dealer ='" + model + "'";
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

        public ActionResult InputPKP(PKPModel model)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(ca.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "";

            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            model.cust_num = custData[0].cust_num;
            model.cust_name = custData[0].name;

            if (result.CustomerCode == "9999999")
            {
                filterCust = "ltrim(rtrim(ca.cust_num))=ltrim(rtrim('" + custData[0].cust_num + "'))";
            }

            model.pkp_date = DateTime.Today.Date;

            strsql = "select cust_num, cust_seq, name from custaddr_mst ca ";
            strsql += " where ca.cust_seq = 0 and ca.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustAddrModel> custAddrData = new DAO<CustAddrModel>().RetrieveDataBySQL(strsql);

            model.ship_to = custAddrData[0].name;
            model.tdi = 0;
            model.nontdi = 0;

            return View(model);
        }


        [HttpPost]
        public JsonResult Save(PKPModel model, FormCollection formCollection, HttpPostedFileBase pictureFile)
        {

            
            string strsql = "";
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            string custNum = custData[0].cust_num;

            try
            {
                string path = Server.MapPath("~/Uploads/");
                string fileName = "";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                byte[] bytes = null;
                if (pictureFile != null)
                {
                    fileName = DateTime.Now.ToString("yyyyMMddTHHmmss");
                    fileName += Path.GetFileName(pictureFile.FileName);
                    pictureFile.SaveAs(path + fileName);
                    CompressImage.Compressimage(pictureFile.InputStream, path + fileName);
                    bytes = ReadFile(path + fileName);
                }

                strsql = "select top 1 pkp from tdi_pkp_mst ";
                strsql += " where left(pkp,5)='P" + DateTime.Today.ToString("yyMM") + "' order by CreateDate desc";

                List<PKPModel> pkpData = new DAO<PKPModel>().RetrieveDataBySQL(strsql);

                //Prefix(1digit)->P
                //Bulan(2digit)-> 07
                //Tahun(2digit)-> 24
                //Running Number(5digit)
                string pkp = "P" + DateTime.Today.ToString("yyMM") + "00001";
                if (pkpData.Count > 0)
                {
                    string no_pkp = "0000" + (Convert.ToInt32(pkpData[0].pkp.Substring(6)) + 1);
                    pkp = "P" + DateTime.Today.ToString("yyMM") + no_pkp.Substring(no_pkp.Length - 5);
                }

                model = new PKPModel();
                model.pkp_date = DateTime.ParseExact(formCollection["pkp_date"] ?? DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture); //formCollection["pkp_date"];
                model.cust_num = custNum;
                model.ship_to = formCollection["ship_to"];
                model.pic = formCollection["pic"];
                model.jabatan = formCollection["jabatan"];
                model.model = formCollection["model"];
                model.item = formCollection["item"];
                model.description = formCollection["description"];
                model.qty = Convert.ToInt32(formCollection["qty"]);
                model.um = formCollection["um"];
                model.jol = formCollection["jol"];
                model.no_rangka = formCollection["no_rangka"];
                model.tdi = Convert.ToInt32(formCollection["tdi"]);
                model.nontdi = Convert.ToInt32(formCollection["nontdi"]);
                model.kep_perbaikan = Convert.ToInt32(formCollection["kep_perbaikan"]);
                model.analisa_penyebab = formCollection["analisa_penyebab"];
                model.tindakan_perbaikan = formCollection["tindakan_perbaikan"];
                model.catatan = formCollection["catatan"];

                if (fileName != "")
                {
                    strsql = "INSERT INTO [tdi_pkp_mst]" +
                    " ([pkp]" +
                    " ,[pkp_date]" +
                    " ,[cust_num]" +
                    " ,[ship_to]" +
                    " ,[pic]" +
                    " ,[jabatan]" +
                    " ,[model]" +
                    " ,[item]" +
                    " ,[description]" +
                    " ,[qty]" +
                    " ,[um]" +
                    " ,[jol]" +
                    " ,[no_rangka]" +
                    " ,[tdi]" +
                    " ,[nontdi]" +
                    " ,[kep_perbaikan]" +
                    " ,[attach]" +
                    " ,[analisa_penyebab]" +
                    " ,[tindakan_perbaikan]" +
                    " ,[catatan])" +
                    "     VALUES" +
                    " ('" + pkp + "'" +
                    " ,'" + model.pkp_date.ToString("yyyy-MM-dd") + "'" +
                    " ,'" + model.cust_num + "'" +
                    " ,'" + model.ship_to + "'" +
                    " ,'" + model.pic + "'" +
                    " ,'" + model.jabatan + "'" +
                    " ,'" + model.model + "'" +
                    " ,'" + model.item + "'" +
                    " ,'" + model.description + "'" +
                    " ,'" + model.qty + "'" +
                    " ,'" + model.um + "'" +
                    " ,'" + model.jol + "'" +
                    " ,'" + model.no_rangka + "'" +
                    " ,'" + model.tdi + "'" +
                    " ,'" + model.nontdi + "'" +
                    " ,'" + model.kep_perbaikan + "'" +
                    " ,@bytes" +
                    " ,'" + model.analisa_penyebab + "'" +
                    " ,'" + model.tindakan_perbaikan + "'" +
                    " ,'" + model.catatan + "'" +
                    ")";
                }
                else
                {
                    strsql = "INSERT INTO [tdi_pkp_mst]" +
                    " ([pkp]" +
                    " ,[pkp_date]" +
                    " ,[cust_num]" +
                    " ,[ship_to]" +
                    " ,[pic]" +
                    " ,[jabatan]" +
                    " ,[model]" +
                    " ,[item]" +
                    " ,[description]" +
                    " ,[qty]" +
                    " ,[um]" +
                    " ,[jol]" +
                    " ,[no_rangka]" +
                    " ,[tdi]" +
                    " ,[nontdi]" +
                    " ,[kep_perbaikan]" +
                    " ,[analisa_penyebab]" +
                    " ,[tindakan_perbaikan]" +
                    " ,[catatan])" +
                    "     VALUES" +
                    " ('" + pkp + "'" +
                    " ,'" + model.pkp_date.ToString("yyyy-MM-dd") + "'" +
                    " ,'" + model.cust_num + "'" +
                    " ,'" + model.ship_to + "'" +
                    " ,'" + model.pic + "'" +
                    " ,'" + model.jabatan + "'" +
                    " ,'" + model.model + "'" +
                    " ,'" + model.item + "'" +
                    " ,'" + model.description + "'" +
                    " ,'" + model.qty + "'" +
                    " ,'" + model.um + "'" +
                    " ,'" + model.jol + "'" +
                    " ,'" + model.no_rangka + "'" +
                    " ,'" + model.tdi + "'" +
                    " ,'" + model.nontdi + "'" +
                    " ,'" + model.kep_perbaikan + "'" +
                    " ,'" + model.analisa_penyebab + "'" +
                    " ,'" + model.tindakan_perbaikan + "'" +
                    " ,'" + model.catatan + "'" +
                    ")";
                }
                

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
                return Json(new { Success = true, pkp = pkp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", strsql + "-" + e.Message);
                //return View("CreateCO", model);
                return Json(new { Success = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }

            //return RedirectToAction("Index", "CustomerFeedback");
        }

        [HttpPost]
        public JsonResult FindItem(string item)
        {

            string strsql = "";

            strsql = "SELECT distinct it.item, it.description, it.u_m" +
                " FROM [item_mst] it " +
                " where (1=1) ";
            strsql += " and it.item='" + item + "'";

            List<ItemModel> itemData = new DAO<ItemModel>().RetrieveDataBySQL(strsql);

            return Json(itemData);
        }
        
        public ActionResult ViewPKP(string pkp)
        {
            if (string.IsNullOrEmpty(pkp))
            {
                return RedirectToAction("Index");
            }
            else
            {

                PKPModel model = new PKPModel();

                string strsql = "";

                strsql = "select * ";
                strsql += " from tdi_pkp_mst tdi_pkp ";
                strsql += " where tdi_pkp.pkp='" + pkp + "'";

                List<PKPModel> pkpData = new DAO<PKPModel>().RetrieveDataBySQL(strsql);

                ViewBag.pkpData = pkpData;

                strsql = "select cus.cust_num, ca.name from customer_mst cus";
                strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
                strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'  and ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + pkpData[0].cust_num + "'))";

                List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

                model.pkp = pkpData[0].pkp;
                model.cust_num = pkpData[0].cust_num;
                model.cust_name = custData[0].name;
                model.pkp_date = pkpData[0].pkp_date;
                model.ship_to = pkpData[0].ship_to;
                model.pic = pkpData[0].pic;
                model.jabatan = pkpData[0].jabatan;
                model.model = pkpData[0].model;
                model.item = pkpData[0].item;
                model.description = pkpData[0].description;
                model.qty = Convert.ToInt32(pkpData[0].qty);
                model.um = pkpData[0].um;
                model.jol = pkpData[0].jol;
                model.no_rangka = pkpData[0].no_rangka;
                model.analisa_penyebab = pkpData[0].analisa_penyebab;
                model.stat = pkpData[0].stat;
                if (pkpData[0].attach != null) {
                    string base64String = Convert.ToBase64String(pkpData[0].attach);
                    model.strattach = $"data:image/jpeg;base64,{base64String}";
                }

                return View(model);
            }
        }

        public ActionResult EditPKP(string pkp)
        {
            if (string.IsNullOrEmpty(pkp))
            {
                return RedirectToAction("Index");
            }
            else
            {

                PKPModel model = new PKPModel();

                string strsql = "";

                strsql = "select * ";
                strsql += " from tdi_pkp_mst tdi_pkp ";
                strsql += " where tdi_pkp.pkp='" + pkp + "'";

                List<PKPModel> pkpData = new DAO<PKPModel>().RetrieveDataBySQL(strsql);

                ViewBag.pkpData = pkpData;

                strsql = "select cus.cust_num, ca.name from customer_mst cus";
                strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
                strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'  and ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + pkpData[0].cust_num + "'))";

                List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

                model.pkp = pkpData[0].pkp;
                model.cust_num = pkpData[0].cust_num;
                model.cust_name = custData[0].name;
                model.pkp_date = pkpData[0].pkp_date;
                model.ship_to = pkpData[0].ship_to;
                model.pic = pkpData[0].pic;
                model.jabatan = pkpData[0].jabatan;
                model.model = pkpData[0].model;
                model.item = pkpData[0].item;
                model.description = pkpData[0].description;
                model.qty = Convert.ToInt32(pkpData[0].qty);
                model.um = pkpData[0].um;
                model.jol = pkpData[0].jol;
                model.no_rangka = pkpData[0].no_rangka;
                model.analisa_penyebab = pkpData[0].analisa_penyebab;
                model.stat = pkpData[0].stat;
                string base64String = Convert.ToBase64String(pkpData[0].attach);
                model.strattach = $"data:image/jpeg;base64,{base64String}";

                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Update(PKPModel model, FormCollection formCollection, HttpPostedFileBase pictureFile)
        {

            string strsql = "";
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            string custNum = custData[0].cust_num;

            try
            {
                string path = Server.MapPath("~/Uploads/");
                string fileName = "";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                byte[] bytes = null;
                if (pictureFile != null)
                {
                    fileName = DateTime.Now.ToString("yyyyMMddTHHmmss");
                    fileName += Path.GetFileName(pictureFile.FileName);
                    pictureFile.SaveAs(path + fileName);
                    CompressImage.Compressimage(pictureFile.InputStream, path + fileName);
                    bytes = ReadFile(path + fileName);
                }

                model = new PKPModel();
                model.pkp = formCollection["pkp"];
                model.pkp_date = DateTime.ParseExact(formCollection["pkp_date"] ?? DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture); //formCollection["pkp_date"];
                model.cust_num = custNum;
                model.ship_to = formCollection["ship_to"];
                model.pic = formCollection["pic"];
                model.jabatan = formCollection["jabatan"];
                model.model = formCollection["model"];
                model.item = formCollection["item"];
                model.description = formCollection["description"];
                model.qty = Convert.ToInt32(formCollection["qty"]);
                model.um = formCollection["um"];
                model.jol = formCollection["jol"];
                model.no_rangka = formCollection["no_rangka"];
                model.analisa_penyebab = formCollection["analisa_penyebab"];

                if (fileName != "")
                {
                    strsql = "Update [tdi_pkp_mst]" +
                    " set " +
                    " [pkp_date]='" + model.pkp_date.ToString("yyyy-MM-dd") + "'" +
                    " ,[cust_num]='" + model.cust_num + "'" +
                    " ,[ship_to]='" + model.ship_to + "'" +
                    " ,[pic]='" + model.pic + "'" +
                    " ,[jabatan]='" + model.jabatan + "'" +
                    " ,[model]='" + model.model + "'" +
                    " ,[item]='" + model.item + "'" +
                    " ,[description]='" + model.description + "'" +
                    " ,[qty]='" + model.qty + "'" +
                    " ,[um]='" + model.um + "'" +
                    " ,[jol]='" + model.jol + "'" +
                    " ,[no_rangka]='" + model.no_rangka + "'" +
                    " ,[analisa_penyebab]='" + model.analisa_penyebab + "'" +
                    " ,[attach]=@bytes" +
                    " where pkp='" + model.pkp + "'";
                }
                else
                {
                    strsql = "Update [tdi_pkp_mst]" +
                    " set " +
                    " [pkp_date]='" + model.pkp_date.ToString("yyyy-MM-dd") + "'" +
                    " ,[cust_num]='" + model.cust_num + "'" +
                    " ,[ship_to]='" + model.ship_to + "'" +
                    " ,[pic]='" + model.pic + "'" +
                    " ,[jabatan]='" + model.jabatan + "'" +
                    " ,[model]='" + model.model + "'" +
                    " ,[item]='" + model.item + "'" +
                    " ,[description]='" + model.description + "'" +
                    " ,[qty]='" + model.qty + "'" +
                    " ,[um]='" + model.um + "'" +
                    " ,[jol]='" + model.jol + "'" +
                    " ,[no_rangka]='" + model.no_rangka + "'" +
                    " ,[analisa_penyebab]='" + model.analisa_penyebab + "'" +
                    " where pkp='" + model.pkp + "'";
                }

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
                return Json(new { Success = true, pkp = model.pkp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", strsql + "-" + e.Message);
                //return View("CreateCO", model);
                return Json(new { Success = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }

            //return RedirectToAction("Index", "CustomerFeedback");
        }

        [HttpPost]
        public JsonResult GetCustAddr(string prefix)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "cust_num='" + result.CustomerCode + "'";
            }

            string strsql = "";
            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and cus." + filterCust;

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            string cust_num = custData[0].cust_num;

            strsql = "select cust_num, cust_seq, name, city " +
                " from custaddr_mst " +
                " where cust_num = '" + cust_num + "' " +
                " and (name like '%" + prefix + "%' " +
                " or city like '%" + prefix + "%'" +
                " or cust_seq like '%" + prefix + "%')";

            List<CustAddrModel> custAddrData = new DAO<CustAddrModel>().RetrieveDataBySQL(strsql);

            List<SelectListItem> resData = new List<SelectListItem>();
            if (custAddrData != null)
            {
                custAddrData.ForEach(row =>
                {

                    resData.Add(new SelectListItem
                    {
                        Value = row.cust_seq.ToString(),
                        Text = row.name
                    });

                });
            }

            return Json(resData);
        }

    }
}