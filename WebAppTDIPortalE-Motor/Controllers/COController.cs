using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppTDIPortalE_Motor.Models;

namespace WebAppTDIPortalE_Motor.Controllers
{
    [Authorize]
    public class COController : Controller
    {
        private List<COItemModel> listOfCOItemModels;
        public COController()
        {
            listOfCOItemModels = new List<COItemModel>();
        }
        // GET: CO
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

            strsql = "select ci.co_num, ci.co_line, ci.item, it.description, ci.qty_ordered," +
                " it.u_m, co.order_date, ci.due_date, co.cust_po, ci.stat, co.cust_num, co.credit_hold ";
            strsql += " from coitem_mst ci";
            strsql += " inner join co_mst co on co.co_num = ci.co_num and co.site_ref = ci.site_ref ";
            strsql += " inner join item_mst it on it.item = ci.item and it.site_ref = ci.site_ref ";
            strsql += " where ci.site_ref = '" + Global.Site + "' and ltrim(rtrim(co.cust_num))=ltrim(rtrim('" + custNum +"'))" +
                " order by co.CreateDate desc";

            List<COLinesModel> coData = new DAO<COLinesModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                coData = coData.Where(x => x.co_num.ToLower().Contains(param.sSearch.ToLower())
                                              || x.co_line.ToString().Contains(param.sSearch.ToLower())
                                              || x.item.ToLower().Contains(param.sSearch.ToLower())
                                              || (x.description ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.u_m ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || x.order_date.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.sSearch.ToLower())
                                              || x.due_date.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.cust_po ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || x.stat.ToLower().Contains(param.sSearch.ToLower())
                                              || (x.credit_hold == 1 ? "Yes" : "No").ToLower().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var displayResult = coData.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = coData.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CreateCO(COAddViewModel model)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(ca.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "";
            //strsql = "select isnull(max(co.co_num), 'CD00000000') as co_num from co_mst co ";
            //strsql += " where co.co_num like 'CD%'  and co.site_ref = '" + Global.Site + "'";

            //List<COModel> coData = new DAO<COModel>().RetrieveDataBySQL(strsql);

            //string sCoNum = "0000000" + Convert.ToString(Convert.ToInt32(coData[0].co_num.Substring(2, 8)) + 1);
            //model.co_num = "CD" + sCoNum.Substring(sCoNum.Length-8);

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

            string filterWhse = "";

            if (result.Warehouse != "9999" && !(result.Warehouse is null))
            {
                filterWhse = "whse='" + result.Warehouse + "'";
            }

            strsql = "select whse, name from whse_mst wh ";
            strsql += " where wh.site_ref = '" + Global.Site + "'";
            if (filterWhse != "")
                strsql += " and wh." + filterWhse;

            List<WhseModel> whseData = new DAO<WhseModel>().RetrieveDataBySQL(strsql);

            model.whse = whseData[0].whse;
            model.order_date = DateTime.Today.Date;

            
            strsql = "select cust_num, cust_seq, name from custaddr_mst ca ";
            strsql += " where ca.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustAddrModel> custAddrData = new DAO<CustAddrModel>().RetrieveDataBySQL(strsql);

            model.cust_seq = custAddrData[0].cust_seq;
            model.ship_name = custAddrData[0].name;

            Session["CartCounter"] = null;
            Session["COItem"] = listOfCOItemModels = new List<COItemModel>();

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveCO(COAddViewModel model, FormCollection collection)
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

            model.cust_num = custData[0].cust_num;
            model.cust_name = custData[0].name;

            if (result.CustomerCode == "9999999")
            {
                filterCust = " ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + custData[0].cust_num + "'))";
            }

            strsql = "select cust_num, curr_code, fixed_rate, exch_rate, is_default ";
            strsql += " from customer_currency_mst cus where is_default=1 ";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustomerCurrencyModel> currData = new DAO<CustomerCurrencyModel>().RetrieveDataBySQL(strsql);

            try
            {
                int retVal = 0;
                //int retVal = new DAO<COModel>().UpdateDataBySP("TDIPortal_InsertCO"
                //    , new string[] { "@co_num", "@cust_num", "@cust_po", "@cust_seq", "@whse", "@order_date", "@curr_code", "@site_ref" }
                //    , new object[] { model.co_num, model.cust_num, model.cust_po, model.cust_seq, model.whse, model.order_date, currData[0].curr_code, Global.Site });
                DateTime oDate = DateTime.ParseExact(collection["order_date_val"], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.order_date = oDate;
                decimal price = 0;
                decimal cost = 0;
                decimal matl_cost_t = 0;
                listOfCOItemModels = Session["CoItem"] as List<COItemModel>;
                foreach (COItemModel coitem in listOfCOItemModels)
                {
                    matl_cost_t += (coitem.matl_cost * coitem.qty_ordered);
                    cost += (coitem.unit_cost * coitem.qty_ordered);
                    price += (coitem.price * coitem.qty_ordered);
                }
                string co_num = "";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "TDI_PortalEMotor_InsertCO";
                    cmd.Parameters.Add("@co_num", SqlDbType.VarChar, 10);
                    cmd.Parameters["@co_num"].Direction = ParameterDirection.InputOutput;
                    cmd.Parameters["@co_num"].Value = "00";
                    cmd.Parameters.AddWithValue("@cust_num", model.cust_num);
                    cmd.Parameters.AddWithValue("@cust_po", model.cust_po);
                    cmd.Parameters.AddWithValue("@cust_seq", model.cust_seq);
                    cmd.Parameters.AddWithValue("@whse", model.whse);
                    cmd.Parameters.AddWithValue("@order_date", oDate);
                    cmd.Parameters.AddWithValue("@curr_code", currData[0].curr_code);
                    cmd.Parameters.AddWithValue("@site_ref", Global.Site);
                    cmd.Parameters.AddWithValue("@matl_cost_t", matl_cost_t);
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@price", price);
                    Boolean tempValue = collection["sparepart_val"] == "1" ? true : false;
                    cmd.Parameters.AddWithValue("@sparepart", (tempValue ? 1 : 0));
                    int i = cmd.ExecuteNonQuery();
                    co_num = Convert.ToString(cmd.Parameters["@co_num"].Value);
                }

                foreach (COItemModel coitem in listOfCOItemModels)
                {
                    icost = coitem.unit_cost;
                    strsql = "insert into coitem_mst (" +
                    " co_num," +
                    " co_line," +
                    " item," +
                    " qty_ordered," +
                    " u_m," +
                    " due_date," +
                    " price," +
                    " site_ref, " +
                    " whse, " +
                    " qty_ordered_conv, " +
                    " cust_seq, " +
                    " co_cust_num, " +
                    " ship_site, " +
                    " co_orig_site, " +
                    " description," +
                    " [cost]      " +
                    ",[cgs_total]      " +
                    ",[price_conv]      " +
                    ",[matl_cost]      " +
                    ",[cost_conv]      " +
                    ",[matl_cost_conv]      " +
                    ",[Uf_Item_price]      " +
                    ",[Uf_net_price]      " +
                    ",[Uf_price_aft_disc1]      " +
                    ",[Uf_price_aft_disc2]      " +
                    ",[Uf_price_aft_disc3]" +
                    ") " +
                    " values (" +
                    " '" + co_num + "', " +
                    " '" + coitem.co_line + "', " +
                    " '" + coitem.item + "', " +
                    " '" + coitem.qty_ordered + "', " +
                    " '" + coitem.u_m + "', " +
                    " '" + coitem.due_date.ToString("yyyy-MM-dd") + "', " +
                    " " + coitem.price + "," +
                    " '" + Global.Site + "', " +
                    " '" + model.whse + "', " +
                    " " + coitem.qty_ordered + ", " +
                    " '" + model.cust_seq + "', " +
                    " '" + model.cust_num + "', " +
                    " '" + Global.Site + "', " +
                    " '" + Global.Site + "', " +
                    " '" + coitem.description + "'," +
                    " " + coitem.unit_cost + "    " +
                    "," + coitem.unit_cost + "     " +
                    "," + coitem.price + "      " +
                    "," + coitem.matl_cost + "     " +
                    "," + coitem.unit_cost + "     " +
                    "," + coitem.matl_cost + "      " +
                    "," + coitem.price + "      " +
                    "," + coitem.price + "      " +
                    "," + coitem.price + "      " +
                    "," + coitem.price + "      " +
                    "," + coitem.price + "" + 
                    ")";

                    retVal = new DAO<COItemModel>().UpdateDataBySP(strsql);
                    ModelState.AddModelError("", co_num + " - " + coitem.co_line + " created.");

                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", icost + "|" + strsql + "-" + e.Message);
                return View("CreateCO", model);
            }

            return RedirectToAction("Index", "CO");
        }

        public ActionResult GetDataCoItem(JqueryDatatableParam param)
        {
            listOfCOItemModels = Session["CoItem"] as List<COItemModel>;

            var displayResult = listOfCOItemModels.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = listOfCOItemModels.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = listOfCOItemModels
            }, JsonRequestBehavior.AllowGet);
        }

        //SaveCOItem
        [HttpPost]
        public JsonResult SaveCOItem(FormCollection collection)
        {
            //formCollection["GRNDate"]
            COItemModel objCOItemModel = new COItemModel();

            if (Session["CartCounter"] != null)
            {
                listOfCOItemModels = Session["COItem"] as List<COItemModel>;
            }
            int listCount = listOfCOItemModels.Where(x => x.item.ToLower().Equals(collection["item"].ToLower())).ToList().Count;

            if (listCount > 0)
            {
                return Json(new { Success = false, Counter = 9999 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                objCOItemModel.co_line = listOfCOItemModels.Count + 1;
                objCOItemModel.item = collection["item"];
                objCOItemModel.description = collection["description"];
                objCOItemModel.u_m = collection["um"];
                objCOItemModel.qty_ordered = Convert.ToDecimal(collection["qty"]);
                objCOItemModel.price = Convert.ToDecimal(collection["price"]);
                objCOItemModel.unit_cost = Convert.ToDecimal(collection["unit_cost"]);
                objCOItemModel.matl_cost = Convert.ToDecimal(collection["matl_cost"]);
                objCOItemModel.net_price = Convert.ToDecimal(collection["qty"]) * Convert.ToDecimal(collection["price"]);
                objCOItemModel.due_date = DateTime.ParseExact(collection["due_date"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                listOfCOItemModels.Add(objCOItemModel);

                Session["CartCounter"] = listOfCOItemModels.Count;
                Session["COItem"] = listOfCOItemModels;

                return Json(new { Success = true, Counter = listOfCOItemModels.Count }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult DeleteCOItem(int id)
        {
            //formCollection["GRNDate"]
            COItemModel objCOItemModel = new COItemModel();

            if (Session["CartCounter"] != null)
            {
                listOfCOItemModels = Session["COItem"] as List<COItemModel>;
            }

            listOfCOItemModels.RemoveAt(id - 1);

            Session["CartCounter"] = listOfCOItemModels.Count;
            Session["COItem"] = listOfCOItemModels;

            return Json(new { Success = true, Counter = listOfCOItemModels.Count }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ItemAutoCompleteAll(string prefix, string model, string sparepart, string cust_num, string order_date)
        {
            if (order_date == "" || (order_date is null))
            {
                order_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
            }
            DateTime oDate = DateTime.ParseExact(order_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            string strsql = "";
            //strsql = "select distinct top 200 it.Uf_spec ";
            //strsql += " from item_mst it";
            ////strsql += " where it.description like '%e-motor%' and it.family_code = 'EMO' " +
            //strsql += " where it.Uf_spec like '" + prefix + "%' and Uf_Model='" + model + "'";
            //if (sparepart == "true")
            //{
            //    //strsql += " and (Uf_model like '%scooter%' or Uf_model like '%cruiser%' or Uf_model like  '%trail%') and  isnull(it.family_code, '') <> 'EMO'";
            //    strsql += " and it.product_code = 'PARTS-EMOT'";
            //}
            //else if (sparepart == "false")
            //{
            //    //strsql += " and it.description like '%e-motor%' and isnull(it.family_code, '') = 'EMO' ";
            //    strsql += " and it.product_code = 'FG-EMOT'";
            //}

            if (sparepart == "false")
            {
                strsql = "SELECT distinct it.item Uf_Model, spec Uf_spec " +
                " FROM [tdi_emo_alloc_detail_mst] det_alloc" +
                " inner join [tdi_emo_alloc_mst] alloc on alloc.alloc_num=det_alloc.alloc_num" +
                " inner join [item_mst] it on it.Uf_model_dealer=det_alloc.model and it.item=det_alloc.item" +
                " and ltrim(rtrim(alloc.cust_num))=ltrim(rtrim('" + cust_num + "'))" +
                " and det_alloc.model ='" + model + "'" +
                " and '" + oDate.ToString("yyyy-MM-dd") + "' between alloc.start_date and alloc.end_date ";
                strsql += " and it.product_code = 'FG-EMOT'";
                if (prefix != "")
                {
                    strsql += " and it.item like '" + prefix + "%' or it.description like  '" + prefix + "%'";
                }
            }
            //string strsql = "";
            //strsql = "select distinct top 200 it.item, it.description, it.u_m ";
            //strsql += " from item_mst it";
            //strsql += " where it.item in (select item from item_mst where description like '%e-motor%' and family_code = 'EMO')" +
            //    " and item like '" + prefix + "%' or description like '%" + prefix + "%'"; 

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

        //[HttpPost]
        //public JsonResult ModelAutoCompleteAll(string prefix, string sparepart)
        //{

        //    string strsql = "";
        //    strsql = "select distinct top 200 it.Uf_Model ";
        //    strsql += " from item_mst it";
        //    //strsql += " where it.description like '%e-motor%' and it.family_code = 'EMO' " +
        //    //    " and it.Uf_Model like '" + prefix + "%'";
        //    strsql += " where it.Uf_Model like '%" + prefix + "%'";
        //    if (sparepart == "true")
        //    {
        //        //strsql += " and (Uf_model like '%scooter%' or Uf_model like '%cruiser%' or Uf_model like  '%trail%') and  isnull(it.family_code, '') <> 'EMO'";
        //        strsql += " and it.product_code = 'PARTS-EMOT'"; // atas permintaan Wiwid - update kriteria sparepart yg bisa dibuat CO
        //    }
        //    else if (sparepart == "false")
        //    {
        //        //strsql += " and it.description like '%e-motor%' and isnull(it.family_code, '') = 'EMO' ";
        //        strsql += " and it.product_code = 'FG-EMOT'"; // atas permintaan Wiwid - update kriteria e-motor yg bisa dibuat CO
        //    }

        //    List<ItemModel> itemData = new DAO<ItemModel>().RetrieveDataBySQL(strsql);

        //    List<SelectListItem> resData = new List<SelectListItem>();
        //    if (itemData != null)
        //    {
        //        itemData.ForEach(row =>
        //        {

        //            resData.Add(new SelectListItem
        //            {
        //                Value = (string)row.Uf_Model,
        //                Text = row.Uf_Model
        //            });

        //        });
        //    }

        //    return Json(resData);
        //}

        [HttpPost]
        public JsonResult ModelAutoCompleteAll(string prefix, string sparepart, string cust_num, string order_date)
        {

            // Permintaan Update Wiwid :
            // Perubahan pengambilan data barang yg bisa dibuat CO
            // hanya diambil dari data tdi_emo_alloc_detail_mst berdasarkan cust_num 
            if (order_date == "" || (order_date is null))
            {
                order_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
            }
            DateTime oDate = DateTime.ParseExact(order_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            string strsql = "";
            
            if (sparepart == "true")
            {
                strsql = "select distinct top 200 it.item Uf_Model";
                strsql += " from item_mst it";
                //strsql += " inner join [lot_loc_mst] loc on it.item=loc.item and lower(loc.loc)='aftspt'";
                strsql += " inner join [itemwhse_mst] itw on it.item=itw.item and lower(itw.whse)='aft'";
                //strsql += " where it.description like '%e-motor%' and it.family_code = 'EMO' " +
                //    " and it.Uf_Model like '" + prefix + "%'";
                strsql += " where it.item like '%" + prefix + "%'";
                strsql += " and it.product_code = 'PARTS-EMOT' and it.item like 'V%' ";
            }
            else if (sparepart == "false")
            {
                strsql = "SELECT distinct [model] Uf_Model, typ.description Uf_spec " +
                " FROM [tdi_emo_alloc_detail_mst] det_alloc" +
                " inner join [tdi_emo_alloc_mst] alloc on alloc.alloc_num=det_alloc.alloc_num" +
                " inner join [item_mst] it on it.Uf_model_dealer=det_alloc.model" +
                " inner join [tdi_type] typ on it.Uf_model_dealer=typ.code" +
                " where ltrim(rtrim(alloc.cust_num))=ltrim(rtrim('" + cust_num + "'))" +
                " and det_alloc.model like '" + prefix + "%'" +
                " and '" + oDate.ToString("yyyy-MM-dd") + "' between alloc.start_date and alloc.end_date ";
                strsql += " and it.product_code = 'FG-EMOT'";
            }


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
        public JsonResult SpecAutoCompleteAll(string prefix, string model, string sparepart, string cust_num, string order_date)
        {
            if (order_date == "" || (order_date is null))
            {
                order_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
            }
            DateTime oDate = DateTime.ParseExact(order_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            string strsql = "";
            //strsql = "select distinct top 200 it.Uf_spec ";
            //strsql += " from item_mst it";
            ////strsql += " where it.description like '%e-motor%' and it.family_code = 'EMO' " +
            //strsql += " where it.Uf_spec like '" + prefix + "%' and Uf_Model='" + model + "'";
            //if (sparepart == "true")
            //{
            //    //strsql += " and (Uf_model like '%scooter%' or Uf_model like '%cruiser%' or Uf_model like  '%trail%') and  isnull(it.family_code, '') <> 'EMO'";
            //    strsql += " and it.product_code = 'PARTS-EMOT'";
            //}
            //else if (sparepart == "false")
            //{
            //    //strsql += " and it.description like '%e-motor%' and isnull(it.family_code, '') = 'EMO' ";
            //    strsql += " and it.product_code = 'FG-EMOT'";
            //}

            if (sparepart == "false")
            {
                strsql = "SELECT distinct [model] Uf_Model, spec Uf_spec " +
                " FROM [tdi_emo_alloc_detail_mst] det_alloc" +
                " inner join [tdi_emo_alloc_mst] alloc on alloc.alloc_num=det_alloc.alloc_num" +
                " inner join [item_mst] it on it.Uf_model_dealer=det_alloc.model and it.item=det_alloc.item" +
                " and ltrim(rtrim(alloc.cust_num))=ltrim(rtrim('" + cust_num + "'))" +
                " and det_alloc.model ='" + model + "'" +
                " and it.item like '" + prefix + "%' or it.description like  '" + prefix + "%'" +
                " and '" + oDate.ToString("yyyy-MM-dd") + "' between alloc.start_date and alloc.end_date ";
                strsql += " and it.product_code = 'FG-EMOT'";
            }

            List<ItemModel> itemData = new DAO<ItemModel>().RetrieveDataBySQL(strsql);

            List<SelectListItem> resData = new List<SelectListItem>();
            if (itemData != null)
            {
                itemData.ForEach(row =>
                {

                    resData.Add(new SelectListItem
                    {
                        Value = (string)row.Uf_spec,
                        Text = row.Uf_Model +  " - " + row.Uf_spec
                    });

                });
            }

            return Json(resData);
        }

        [HttpPost]
        public JsonResult COCheckWH(string sparepart)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string strsql = "";
            string filterWhse = "";

            if (result.Warehouse != "9999" && !(result.Warehouse is null))
            {
                if (sparepart == "true")
                {
                    filterWhse = "upper(wh.whse)='AFT'";
                }
                else
                {
                    filterWhse = "upper(wh.whse)='" + result.Warehouse.ToUpper() + "'";
                }
            }

            strsql = "select whse, name from whse_mst wh ";
            strsql += " where wh.site_ref = '" + Global.Site + "'";
            if (filterWhse != "")
                strsql += " and " + filterWhse;
            
            List<COAddViewModel> whseDate = new DAO<COAddViewModel>().RetrieveDataBySQL(strsql);

            return Json(whseDate);
        }

        [HttpPost]
        public JsonResult COFindItem(string model, string sparepart, string order_date, string cust_num, string model_vendor = "")
        {

            // Permintaan Update Wiwid :
            // Perubahan pengambilan data barang yg bisa dibuat CO
            // hanya diambil dari data tdi_emo_alloc_detail_mst berdasarkan cust_num 
            if (order_date == "" || (order_date is null))
            {
                order_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
            }
            DateTime oDate = DateTime.ParseExact(order_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string strsql = "";
            
            if (sparepart == "true")
            {
                strsql = "select distinct top 200 it.Uf_Model, it.Uf_spec, it.item, it.description, it.u_m, (isnull(itw.qty_on_hand,0) - isnull(itw.qty_alloc_co,0)) qty, " +
                        //" CAST(99999 AS DECIMAL(19,8)) qty , " +
                        //" isnull(" +
                        //" (" +
                        //" select sum(coi.qty_ordered) " +
                        //" from coitem_mst coi " +
                        //" inner join co_mst co on co.co_num = coi.co_num" +
                        //" where coi.stat in ('o', 'p', 'f') and coi.item=it.item " +
                        //" and convert(varchar(6), co.order_date, 112)='" + oDate.ToString("yyyyMM") +  "'" +
                        ////" and ltrim(rtrim(co.cust_num)) = '" + cust_num + "'" +
                        //" ),0) qty, " +
                        " isnull((select top 1 unit_price1 from itemprice_mst ip where ip.item = it.item order by effect_date desc),0) as unit_price " +
                        " ,it.unit_cost, it.matl_cost ";
                strsql += " from item_mst it";
                //strsql += " inner join [lot_loc_mst] loc on it.item=loc.item and lower(loc.loc)='aftspt'";
                strsql += " inner join [itemwhse_mst] itw on it.item=itw.item and lower(itw.whse)='aft'";
                strsql += " where it.item='" + model + "' ";
                strsql += " and it.product_code = 'PARTS-EMOT'";

                // qty_on_hand - qty_alloc_co as qtyAvailable
                // from itemwhse_mst where whse = 'AFT'

            }
            else if (sparepart == "false")
            {
                strsql = "select distinct it.Uf_model_dealer, it.Uf_spec, it.item, it.description, it.u_m, det_alloc.qty- " +
                " isnull(" +
                " (" +
                " select sum(coi.qty_ordered) " +
                " from coitem_mst coi " +
                " inner join co_mst co on co.co_num = coi.co_num" +
                " where coi.stat in ('o', 'p', 'f') and coi.item=it.item " +
                " and co.order_date between alloc.start_date and alloc.end_date" +
                " and ltrim(rtrim(co.cust_num)) = ltrim(rtrim(alloc.cust_num))" +
                " ),0) qty, " +
                " isnull((select top 1 unit_price1 from itemprice_mst ip where ip.item = it.item order by effect_date desc),0) as unit_price" +
                " ,it.unit_cost, it.matl_cost " +
                " FROM [tdi_emo_alloc_detail_mst] det_alloc" +
                " inner join [tdi_emo_alloc_mst] alloc on alloc.alloc_num=det_alloc.alloc_num" +
                " inner join [item_mst] it on it.Uf_model_dealer=det_alloc.model and it.item=det_alloc.item" +
                " where det_alloc.item='" + model + "' and det_alloc.model='" + model_vendor  + "' " +
                " and ltrim(rtrim(alloc.cust_num))=ltrim(rtrim('" + cust_num + "'))" +
                " and '" + oDate.ToString("yyyy-MM-dd") + "' between alloc.start_date and alloc.end_date ";
                strsql += " and it.product_code = 'FG-EMOT'";
            }

            List<ItemModel> itemData = new DAO<ItemModel>().RetrieveDataBySQL(strsql);

            return Json(itemData);
        }

        //[HttpPost]
        //public JsonResult COFindItem(string spec, string model, string sparepart)
        //{

        //    string strsql = "";
        //    strsql = "select distinct top 200 it.Uf_Model, it.Uf_spec, it.item, it.description, it.u_m, " +
        //        " isnull((select top 1 unit_price1 from itemprice_mst ip where ip.item = it.item order by effect_date desc),0) as unit_price ";
        //    strsql += " from item_mst it";
        //    strsql += " where it.Uf_spec='" + spec + "' and Uf_Model='" + model + "' ";
        //    if (sparepart == "true")
        //    {
        //        //strsql += " and (Uf_model like '%scooter%' or Uf_model like '%cruiser%' or Uf_model like  '%trail%') and  isnull(it.family_code, '') <> 'EMO'";
        //        strsql += " and it.product_code = 'PARTS-EMOT'";
        //    }
        //    else if (sparepart == "false")
        //    {
        //        //strsql += " and it.description like '%e-motor%' and isnull(it.family_code, '') = 'EMO' ";
        //        strsql += " and it.product_code = 'FG-EMOT'";
        //    }

        //    List<ItemModel> itemData = new DAO<ItemModel>().RetrieveDataBySQL(strsql);

        //    return Json(itemData);
        //}

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
                        Text = row.cust_seq + " | " + row.name + " | " + row.city
                    });

                });
            }

            return Json(resData);
        }

    }
}