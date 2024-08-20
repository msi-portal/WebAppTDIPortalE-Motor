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
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult COReservation()
        {
            return View();
        }

        public ActionResult COR_GetData(JqueryDatatableParam param)
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

            string custNum = custData[0].cust_num;

            strsql = "Select " +
                " co.order_date, " +
                " co.cust_num, " +
                " ca.name, " +
                " rsvd.ref_num, " +
                " rsvd.ref_line, " +
                " rsvd.item, " +
                " rsvd.u_m, " +
                " rsvd.qty_rsvd_conv, " +
                " rsvd.qty_rsvd, " +
                " rsvd.whse, " +
                " rsvd.RecordDate," +
                " SUBSTRING(ser.ser_num, 1, CHARINDEX('/', ser.ser_num) - 1) as No_Rangka," +
                " SUBSTRING(ser.ser_num, CHARINDEX('/', ser.ser_num) + 1, LEN(ser.ser_num)) as No_Mesin " +
                " from rsvd_inv_mst rsvd " +
                " LEFT OUTER JOIN co_mst co on co.co_num = rsvd.ref_num " +
                " left outer join custaddr_mst ca on ca.cust_num = co.cust_num and ca.cust_seq = co.cust_seq " +
                " left join serial_mst ser on rsvd.rsvd_num=ser.rsvd_num" +
                " WHERE qty_rsvd > 0 and " +
                " co.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and co." + filterCust;
            strsql += " order by co.CreateDate desc";

            List<COReservationModel> coData = new DAO<COReservationModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                coData = coData.Where(x => x.cust_num.ToLower().Contains(param.sSearch.ToLower())
                                              || x.ref_line.ToString().Contains(param.sSearch.ToLower())
                                              || x.qty_rsvd.ToString().Contains(param.sSearch.ToLower())
                                              || x.qty_rsvd_conv.ToString().Contains(param.sSearch.ToLower())
                                              || x.item.ToLower().Contains(param.sSearch.ToLower())
                                              || x.name.ToLower().Contains(param.sSearch.ToLower())
                                              || x.u_m.ToLower().Contains(param.sSearch.ToLower())
                                              || x.order_date.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.sSearch.ToLower())
                                              || x.RecordDate.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.whse ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || x.ref_num.ToLower().Contains(param.sSearch.ToLower())
                                              || x.No_Mesin.ToLower().Contains(param.sSearch.ToLower())
                                              || x.No_Rangka.ToLower().Contains(param.sSearch.ToLower())
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

        [HttpGet]
        public ActionResult IKBReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult IKB_GetData(JqueryDatatableParam param)
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

            string custNum = custData[0].cust_num;

            strsql = "select * "+
                ", isnull((select count(*) from serial_mst" +
                " where serial_mst.ser_num = concat(tdi_id.no_rangka, '/', tdi_id.no_mesin) " +
                " and serial_mst.ref_type = 'o' and serial_mst.stat = 'i'),0) sreturn ";
            strsql += " from tdi_identitas_motor_mst tdi_id ";
            if (filterCust != "")
                strsql += " where tdi_id." + filterCust;
            strsql += " order by tdi_id.CreateDate desc";

            List<IKBModel> coData = new DAO<IKBModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                coData = coData.Where(x => x.cust_num.ToLower().Contains(param.sSearch.ToLower())
                                                || (x.alamat ?? "").ToLower().Contains(param.sSearch.ToLower())
                                                || (x.atas_nama ?? "").ToLower().Contains(param.sSearch.ToLower())
                                                || (x.bahan_bakar ?? "").ToLower().Contains(param.sSearch.ToLower())
                                                || x.co_line.ToString().Contains(param.sSearch.ToLower())
                                                || x.co_line_qty.ToString().Contains(param.sSearch.ToLower())
                                                || x.co_num.ToLower().Contains(param.sSearch.ToLower())
                                                || x.description.ToLower().Contains(param.sSearch.ToLower())
                                                || (x.formulir_AB ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.identity_line).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.item ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.Item_price).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.jenis ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.merk ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.model ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.name ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.no_faktur ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.no_ktp ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.no_mesin ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.no_rangka ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.pib ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.seq).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.silinder ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.site_ref ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.srut ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.sut ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.tahun).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.tgl_faktur.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
                                                || (x.tpt ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.type ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.uf_harga_revisi).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.uf_revisi).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.warna ?? "").ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || (x.sreturn == 1 ? "Return" : "").ToLower().Contains(param.sSearch.ToLower())
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

        [HttpGet]
        public ActionResult OutstandingARReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OutstandingAR_GetData(JqueryDatatableParam param, string view)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = "cust_num='" + result.CustomerCode + "'";
            }

            string strsql = "select art.cust_num as Customer," +
                " art.apply_to_inv_num as Faktur, art.co_num, (select top 1 cust_po from co_mst where co_mst.co_num = art.co_num) cust_po, " +
                " sum( case when type = 'P' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end) as Kredit, " +
                " sum( case when type = 'I' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end) + " +
                " sum( case when type = 'D' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end) - " +
                " sum( case when type = 'C' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end) as Debit," +
                " ((" +
                " sum( case when type = 'I' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end) +" +
                " sum( case when type = 'D' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end) - " +
                " sum( case when type = 'C' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end)" +
                " ) - " +
                " sum( case when type = 'P' then isnull(amount, 0) + isnull(freight, 0) + isnull(misc_charges, 0) + isnull(sales_tax, 0) " +
                " else 0 end) " +
                " ) Sisa," +
                " ( " +
                " select top 1 artran_mst.inv_seq " +
                " from artran_mst with(nolock) " +
                " where artran_mst.apply_to_inv_num = art.apply_to_inv_num " +
                " and artran_mst.inv_seq > 0 " +
                " ) NoBukti, " +
                " (select top 1 artran_mst.inv_date " +
                " from artran_mst with(nolock) " +
                " where artran_mst.apply_to_inv_num = art.apply_to_inv_num " +
                " and artran_mst.type = 'I' " +
                " order by artran_mst.inv_date desc " +
                " ) Tanggal, " +
                " ( " +
                " select top 1 artran_mst.due_date " +
                " from artran_mst with(nolock) " +
                " where artran_mst.apply_to_inv_num = art.apply_to_inv_num " +
                " and artran_mst.type = 'I' " +
                " order by artran_mst.inv_date desc " +
                " ) TanggalDueDate," +
                " ( " +
                " select top 1 artran_mst.inv_date" +
                " from artran_mst with(nolock) " +
                " where artran_mst.apply_to_inv_num = art.apply_to_inv_num " +
                " and artran_mst.type = 'P' " +
                " order by artran_mst.inv_date desc" +
                " ) TanggalBayar " +
                " from artran_mst art with(nolock) " +
                " where ltrim(rtrim(art.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))" +
                " AND convert(varchar(6), art.inv_date, 112) <= '" + DateTime.Today.ToString("yyyyMM") +"'" +
                " and art.apply_to_inv_num <> '0' " +
                " group by art.cust_num, art.apply_to_inv_num, art.co_num";

            List<OutstandingARModel> coData = new DAO<OutstandingARModel>().RetrieveDataBySQL(strsql);

            if (view == "1")
            {
                coData = coData.Where(x => x.Sisa == 0).ToList();
            } else if (view == "2")
            {
                coData = coData.Where(x => x.Sisa > 0).ToList();
            }

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                coData = coData.Where(x => x.Faktur.ToLower().Contains(param.sSearch.ToLower())
                                                || (x.co_num ?? "").ToLower().Contains(param.sSearch.ToLower())
                                                || (x.cust_po ?? "").ToLower().Contains(param.sSearch.ToLower())
                                                || (x.Sisa).ToString().ToLower().Contains(param.sSearch.ToLower())
                                                || x.Tanggal.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
                                                || x.TanggalDueDate.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
                                                || x.TanggalBayar.ToString("dd'/'MM'/'yyyy").Contains(param.sSearch.ToLower())
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
    }
}