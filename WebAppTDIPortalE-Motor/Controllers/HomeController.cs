using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppTDIPortalE_Motor.Models;

namespace WebAppTDIPortalE_Motor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = " rtrim(ltrim(alloc.cust_num))=rtrim(ltrim('" + result.CustomerCode + "'))";
            }

            string strsql = "select det.model, det.spec, det.qty, alloc.cust_num, alloc.start_date, alloc.end_date, " +
                " isnull(" +
                " (" +
                " select sum(coitem_mst.qty_ordered) " +
                " from coitem_mst " +
                " left outer join co_mst on co_mst.co_num = coitem_mst.co_num" +
                " left outer join item_mst on item_mst.item = coitem_mst.item" +
                " where coitem_mst.stat in ('o', 'p', 'f') " +
                " and co_mst.order_date between alloc.start_date and alloc.end_date" +
                " and item_mst.Uf_model_dealer = det.model " +                //" and item_mst.Uf_spec = det.spec " +
                " and co_mst.cust_num = alloc.cust_num" +
                " ),0) qty_ordered" +
                " from tdi_emo_alloc_detail_mst det " +
                " inner join tdi_emo_alloc_mst alloc on alloc.alloc_num = det.alloc_num" +
                " where (1=1) ";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<CustAllocModel> custAllocData = new DAO<CustAllocModel>().RetrieveDataBySQL(strsql);

            ViewBag.custAllocData = custAllocData;

            strsql = "select distinct convert(varchar(6), alloc.RecordDate, 112) ID," +
                " FORMAT(alloc.RecordDate, 'MMMM yyyy') name " +
                " from tdi_identitas_motor_mst alloc " +
                " left outer join item_mst on item_mst.item = alloc.item " +
                " WHERE (1=1) ";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<period> periodList= new DAO<period>().RetrieveDataBySQL(strsql);
            //period nperiod = new period() { ID = "", name = "" };
            //periodList.Insert(0, nperiod);

            ViewBag.periodList = periodList;

            strsql = " select distinct convert(varchar(6), alloc.end_date, 112) ID," +
                " FORMAT(alloc.end_date, 'MMMM yyyy') name " + 
                " from tdi_emo_alloc_mst alloc " +
                " WHERE (1=1) ";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<period> periodAllocList = new DAO<period>().RetrieveDataBySQL(strsql);
            //periodAllocList.Insert(0, nperiod);

            ViewBag.periodAllocList = periodAllocList;

            strsql = "Select alloc.notification, alloc.cust_num from tdi_emo_dealer_notice alloc " +
                " where (1=1) ";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<TDI_Notice> noticeList= new DAO<TDI_Notice>().RetrieveDataBySQL(strsql);

            ViewBag.notification = "";
            if (noticeList.Count() > 0)
            {
                ViewBag.notification = noticeList[0].notification;
            }

            strsql = "select distinct convert(varchar(6), co.order_date, 112) ID," +
                " FORMAT(co.order_date, 'MMMM yyyy') name " +
                " from coitem_mst coi " +
                " left join co_mst co on coi.co_num = co.co_num " +
                " left join rsvd_inv_mst rsvd on coi.co_num=rsvd.ref_num and coi.co_line=rsvd.ref_line " +
                " where coi.stat in ('P', 'O') and DATEDIFF(DAY, coi.due_date, GETDATE()) > 0" +
                " order by convert(varchar(6), co.order_date, 112)";

            List<period> periodWarnList = new DAO<period>().RetrieveDataBySQL(strsql);
            ViewBag.periodWarnList = periodWarnList;

            return View();
        }

        public ActionResult GetCreditLimit()
        {

            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "cust_num='" + result.CustomerCode + "'";
            }

            string strsql = "select distinct " +
                " customer_mst.cust_num, " +
                " custaddr_mst.name, " +
                " customer_mst.posted_bal as ARBalance, " +
                " customer_mst.order_bal as OnOrderBalance, " +
                " custaddr_mst.credit_limit as CreditLimit " +
                " from customer_mst " +
                " left outer join custaddr_mst on custaddr_mst.cust_num = customer_mst.cust_num " +
                " where custaddr_mst.cust_seq = '0' ";
            if (filterCust != "")
                strsql += " and customer_mst." + filterCust;

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            return Json(custData);

        }

        public ActionResult GetAllocEMO(JqueryDatatableParam param, string period)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "select det.model, det.spec, det.qty, alloc.cust_num, alloc.start_date, alloc.end_date, " +
                " isnull(" +
                " (" +
                " select sum(coitem_mst.qty_ordered) " +
                " from coitem_mst " +
                " left outer join co_mst on co_mst.co_num = coitem_mst.co_num" +
                " left outer join item_mst on item_mst.item = coitem_mst.item" +
                " where coitem_mst.stat in ('o', 'p', 'f') and item_mst.product_code = 'FG-EMOT' " +
                " and co_mst.order_date between alloc.start_date and alloc.end_date" +
                " and ltrim(rtrim(item_mst.Uf_model_dealer)) = ltrim(rtrim(det.model))  and ltrim(rtrim(item_mst.item)) = ltrim(rtrim(det.item)) and ltrim(rtrim(co_mst.cust_num)) = ltrim(rtrim(alloc.cust_num ))" +
                " ),0) qty_ordered, " +
                " det.qty - isnull(" +
                " (" +
                " select sum(coitem_mst.qty_ordered) " +
                " from coitem_mst " +
                " left outer join co_mst on co_mst.co_num = coitem_mst.co_num" +
                " left outer join item_mst on item_mst.item = coitem_mst.item" +
                " where coitem_mst.stat in ('o', 'p', 'f') and item_mst.product_code = 'FG-EMOT' " +
                " and co_mst.order_date between alloc.start_date and alloc.end_date" +
                " and ltrim(rtrim(item_mst.Uf_model_dealer)) = ltrim(rtrim(det.model))  and ltrim(rtrim(item_mst.item)) = ltrim(rtrim(det.item)) and ltrim(rtrim(co_mst.cust_num)) = ltrim(rtrim(alloc.cust_num ))" +
                " ),0) qty_outs" +
                " from tdi_emo_alloc_detail_mst det " +
                " inner join tdi_emo_alloc_mst alloc on alloc.alloc_num = det.alloc_num " +
                " WHERE convert(varchar(6), alloc.end_date, 112) = '" + period + "'";
//            " where (1=1) and alloc.end_date>GETDATE() ";
            if (filterCust != "") 
                strsql += " and " + filterCust;
                strsql += " order by det.model";

            List<CustAllocModel> custAllocData = new DAO<CustAllocModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                custAllocData = custAllocData.Where(x => x.model.ToLower().Contains(param.sSearch.ToLower())
                                                || (x.spec ?? "").ToLower().Contains(param.sSearch.ToLower())
                                                || (x.qty).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.qty_ordered).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.qty_outs).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.start_date.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
                                                || x.end_date.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var displayResult = custAllocData.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = custAllocData.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCOWarn(JqueryDatatableParam param, string period)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(co.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "select distinct coi.co_num, co.order_date, coi.item, coi.due_date, coi.qty_ordered " +
                " , DATEDIFF(DAY, coi.due_date, GETDATE()) outs_day " +
                " from coitem_mst coi " +
                " left join co_mst co on coi.co_num = co.co_num " +
                " left join rsvd_inv_mst rsvd on coi.co_num=rsvd.ref_num and coi.co_line=rsvd.ref_line " +
                " where coi.stat in ('P', 'O') and DATEDIFF(DAY, coi.due_date, GETDATE()) > 0" +
                " and convert(varchar(6), co.order_date, 112) = '" + period + "' ";
            if (filterCust != "")
                strsql += " and " + filterCust;
            strsql += " and rsvd.ref_num in (" +
                " select coi.co_num " +
                "  from coitem_mst coi " +
                " left join co_mst co on coi.co_num = co.co_num " +
                " where (1=1) ";
            if (filterCust != "")
                strsql += " and " + filterCust;
            strsql += ")";


            List<COWarnModel> custAllocData = new DAO<COWarnModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                custAllocData = custAllocData.Where(x => x.co_num.ToLower().Contains(param.sSearch.ToLower())
                                                || (x.item ?? "").ToLower().Contains(param.sSearch.ToLower())
                                                || (x.qty_ordered).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.outs_day).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.order_date.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
                                                || x.due_date.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var displayResult = custAllocData.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = custAllocData.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStockInfo(JqueryDatatableParam param, string period)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(tdi_identitas_motor_mst.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "select distinct " +
                " item_mst.description as Spec, " +
                " item_mst.Uf_Model as Model, " +
                " count(tdi_identitas_motor_mst.atas_nama) as SoldQty, " +
                " count(tdi_identitas_motor_mst.item) as BeginningStock, " +
                " count(tdi_identitas_motor_mst.item) - count(tdi_identitas_motor_mst.atas_nama) as AvailableQty " +
                " from tdi_identitas_motor_mst " +
                " left outer join item_mst on item_mst.item = tdi_identitas_motor_mst.item " +
                " WHERE convert(varchar(6), tdi_identitas_motor_mst.RecordDate, 112) = '" + period + "'";
                if (filterCust != "")
                    strsql += " and " + filterCust;
            strsql += " group by item_mst.Uf_Model, item_mst.description ";

            List<StockInfoModel> stockInfoData = new DAO<StockInfoModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                stockInfoData = stockInfoData.Where(x => x.Spec.ToLower().Contains(param.sSearch.ToLower())
                                                || x.Model.ToLower().Contains(param.sSearch.ToLower())
                                                || (x.AvailableQty).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.SoldQty).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.BeginningStock).ToString().ToLower().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var displayResult = stockInfoData.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = stockInfoData.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllocGraph(string period)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "ltrim(rtrim(alloc.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            //string strsql = "select " +
            //    " coitem_mst.item, " +
            //    " item_mst.Uf_Model Model, " +
            //    " item_mst.Uf_spec Spec, " +
            //    " sum(coitem_mst.qty_ordered) as Qty " +
            //    " from coitem_mst " +
            //    " left outer join co_mst on co_mst.co_num = coitem_mst.co_num " +
            //    " left outer join item_mst on item_mst.item = coitem_mst.item " +
            //    " where coitem_mst.stat in('o', 'p', 'f') " +
            //    " and convert(varchar(6), co_mst.order_date, 112) = '" + period + "'";
            //if (filterCust != "")
            //    strsql += " and " + filterCust;
            //strsql += " group by coitem_mst.item,item_mst.Uf_Model,item_mst.Uf_spec ";

            string strsql = "select det.model Model, det.spec Spec, det.qty Qty_Alloc, alloc.cust_num, alloc.start_date, alloc.end_date, " +
                " isnull(" +
                " (" +
                " select sum(coitem_mst.qty_ordered) " +
                " from coitem_mst " +
                " left outer join co_mst on co_mst.co_num = coitem_mst.co_num" +
                " left outer join item_mst on item_mst.item = coitem_mst.item" +
                " where coitem_mst.stat in ('o', 'p', 'f') and item_mst.product_code = 'FG-EMOT' " +
                " and co_mst.order_date between alloc.start_date and alloc.end_date" +
                " and ltrim(rtrim(item_mst.Uf_model)) = ltrim(rtrim(det.model))  and ltrim(rtrim(item_mst.Uf_spec)) = ltrim(rtrim(det.spec)) and ltrim(rtrim(co_mst.cust_num)) = ltrim(rtrim(alloc.cust_num ))" +
                " ),0) Qty, " +
                " det.qty - isnull(" +
                " (" +
                " select sum(coitem_mst.qty_ordered) " +
                " from coitem_mst " +
                " left outer join co_mst on co_mst.co_num = coitem_mst.co_num" +
                " left outer join item_mst on item_mst.item = coitem_mst.item" +
                " where coitem_mst.stat in ('o', 'p', 'f')  and item_mst.product_code = 'FG-EMOT' " +
                " and co_mst.order_date between alloc.start_date and alloc.end_date" +
                " and ltrim(rtrim(item_mst.Uf_model)) = ltrim(rtrim(det.model))  and ltrim(rtrim(item_mst.Uf_spec)) = ltrim(rtrim(det.spec)) and ltrim(rtrim(co_mst.cust_num)) = ltrim(rtrim(alloc.cust_num ))" +
                " ),0) qty_outs" +
                " from tdi_emo_alloc_detail_mst det " +
                " inner join tdi_emo_alloc_mst alloc on alloc.alloc_num = det.alloc_num " +
                " WHERE convert(varchar(6), alloc.end_date, 112) = '" + period + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<AllocGraphModel> AllocGraphData = new DAO<AllocGraphModel>().RetrieveDataBySQL(strsql);

            return Json(AllocGraphData);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}