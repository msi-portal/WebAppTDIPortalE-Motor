using CsvHelper;
using ExcelDataReader;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
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
    public class FakturKendaraanController : Controller
    {
        // GET: FakturKendaraan
        private List<IKBModel> listOfFakturKendaraan;

        public ActionResult Index()
        {
            return RedirectToAction("DownloadFaktur");
        }

        public ActionResult DownloadFaktur()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetDataFaktur(DataTableAjaxPostModel param, string start_date, string end_date)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            start_date = Request.Form["start_date"].ToString();
            if (start_date == "" || (start_date is null))
            {
                start_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
            }
            DateTime oDate = DateTime.ParseExact(start_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            end_date = Request.Form["end_date"].ToString();
            if (end_date == "" || (end_date is null))
            {
                end_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
            }
            DateTime iDate = DateTime.ParseExact(end_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

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

            strsql = "select tdi_id.*";
            strsql += " from tdi_identitas_motor_mst tdi_id ";
            strsql += " where ltrim(rtrim(tdi_id.cust_num))=ltrim(rtrim('" + custNum + "'))";
            strsql += " and FORMAT(tgl_faktur,'yyyy-MM-dd') between '" + oDate.ToString("yyyy-MM-dd") + "' and '" + iDate.ToString("yyyy-MM-dd") + "'";
            strsql += " order by tdi_id.CreateDate desc";

            List<IKBModel> ikbData = new DAO<IKBModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.search.value))
            {
                ikbData = ikbData.Where(x => (x.alamat ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.atas_nama ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.bahan_bakar ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.co_line.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.co_line_qty.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.co_num ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.cust_num ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.description ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.formulir_AB ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.identity_line.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.item ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.Item_price.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.jenis ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.merk ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.model ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.warna ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.no_faktur ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.no_ktp ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.no_mesin ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.no_rangka ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.pib ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.seq.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.silinder ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.site_ref ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.srut ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.sut ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.tahun.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.tgl_faktur.ToString("'MM'/'dd'/yyyy") ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.tpt ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.type ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.uf_harga_revisi.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.uf_revisi.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.uf_claim_cashback.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.uf_claim_sepeda.ToString() ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.SubsidiClaimStatus ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.provinsi ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.kabupaten ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.kecamatan ?? "").ToLower().Contains(param.search.value.ToLower())
                                                || (x.desa ?? "").ToLower().Contains(param.search.value.ToLower())
                                            
                                            ).ToList();
            }

            var displayResult = ikbData.ToList();
            var totalRecords = ikbData.Count();

            return Json(new
            {
                param.draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult
            }, JsonRequestBehavior.AllowGet);
            
        }

        //[HttpPost]
        //public ActionResult DownloadDataFaktur(string start_date, string end_date)
        //{
        //    var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
        //    start_date = Request.Form["start_date"].ToString();
        //    if (start_date == "" || (start_date is null))
        //    {
        //        start_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
        //    }
        //    DateTime oDate = DateTime.ParseExact(start_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        //    end_date = Request.Form["end_date"].ToString();
        //    if (end_date == "" || (end_date is null))
        //    {
        //        end_date = DateTime.Today.Date.ToString("MM/dd/yyyy");
        //    }
        //    DateTime iDate = DateTime.ParseExact(end_date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        //    string filterCust = "";
        //    if (result.CustomerCode != "9999999")
        //    {
        //        filterCust = "ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
        //    }

        //    string strsql = "";
        //    strsql = "select cus.cust_num, ca.name from customer_mst cus";
        //    strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
        //    strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
        //    if (filterCust != "")
        //        strsql += " and " + filterCust;

        //    List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

        //    string custNum = custData[0].cust_num;

        //    strsql = "select tdi_id.*";
        //    strsql += " from tdi_identitas_motor_mst tdi_id ";
        //    strsql += " where ltrim(rtrim(tdi_id.cust_num))=ltrim(rtrim('" + custNum + "'))";
        //    strsql += " and FORMAT(tgl_faktur,'yyyy-MM-dd') between '" + oDate.ToString("yyyy-MM-dd") + "' and '" + iDate.ToString("yyyy-MM-dd") + "'";
        //    strsql += " order by tdi_id.CreateDate desc";

        //    DataSet ikbData = new DAO<IKBModel>().RetrieveDataTablebySP(strsql);

        //    CreateXLSFile("FakturKendaraan.xlsx", ikbData);

        //    return Json(new { Success = true, Counter = ikbData.Tables[0].Rows.Count}, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult UploadFaktur()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(DataTableAjaxPostModel param, HttpPostedFileBase postedFile)
        {
            Session["FKDataCounter"] = null;
            Session["FKData"] = listOfFakturKendaraan = new List<IKBModel>();
            List<IKBModel> FKData = new List<IKBModel>();
            string path = Server.MapPath("~/Uploads/");
            string fileNamex = "";
            string fileName = "";
            string extension = "";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //byte[] bytes = null;
            if (postedFile != null)
            {
                fileNamex = Path.GetFileNameWithoutExtension(postedFile.FileName);
                fileName = Path.GetFileName(postedFile.FileName);
                extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(path + fileName);
            }

            string conString = "";
            if (extension == ".xls" || extension == ".xlsx")
            {
                if (extension == ".xls")
                {
                    conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                }
                else if (extension == ".xlsx")
                {
                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                }

                conString = string.Format(conString, path + fileName);
                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    excel_con.Open();
                    string sheet1 = excel_con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null).Rows[0]["TABLE_NAME"].ToString();
                    DataTable dtExcelData = new DataTable();

                    dtExcelData.Columns.AddRange(new DataColumn[39] {
                        new DataColumn("Alamat", typeof(string)),
                        new DataColumn("Atas Nama", typeof(string)),
                        new DataColumn("Bahan Bakar", typeof(string)),
                        new DataColumn("Co Line", typeof(int)),
                        new DataColumn("Co Line Qty", typeof(int)),
                        new DataColumn("Co Num", typeof(string)),
                        new DataColumn("Cust Num", typeof(string)),
                        new DataColumn("Description", typeof(string)),
                        new DataColumn("Formulir Ab", typeof(string)),
                        new DataColumn("Identity Line", typeof(int)),
                        new DataColumn("Item", typeof(string)),
                        new DataColumn("Item Price", typeof(decimal)),
                        new DataColumn("Jenis", typeof(string)),
                        new DataColumn("Merk", typeof(string)),
                        new DataColumn("Model", typeof(string)),
                        new DataColumn("Warna", typeof(string)),
                        new DataColumn("No Faktur", typeof(string)),
                        new DataColumn("No KTP", typeof(string)),
                        new DataColumn("No Mesin", typeof(string)),
                        new DataColumn("No Rangka", typeof(string)),
                        new DataColumn("PIB", typeof(string)),
                        new DataColumn("Seq", typeof(int)),
                        new DataColumn("Silinder", typeof(string)),
                        new DataColumn("Site Ref", typeof(string)),
                        new DataColumn("SRUT", typeof(string)),
                        new DataColumn("SUT", typeof(string)),
                        new DataColumn("Tahun", typeof(int)),
                        new DataColumn("Tgl Faktur", typeof(DateTime)),
                        new DataColumn("TPT", typeof(string)),
                        new DataColumn("Type", typeof(string)),
                        new DataColumn("Harga Revisi", typeof(decimal)),
                        new DataColumn("Revisi", typeof(int)),
                        new DataColumn("Claim Cashback", typeof(int)),
                        new DataColumn("Claim Sepeda", typeof(int)),
                        new DataColumn("SubsidiClaimStatus", typeof(string)),
                        new DataColumn("Provinsi", typeof(string)),
                        new DataColumn("Kabupaten", typeof(string)),
                        new DataColumn("Kecamatan", typeof(string)),
                        new DataColumn("Desa", typeof(string))
                    })
                    ;

                    using (OleDbDataAdapter oda = new OleDbDataAdapter("SELECT * FROM [" + sheet1 + "]", excel_con))
                    {
                        oda.Fill(dtExcelData);
                    }
                    excel_con.Close();

                    foreach (DataRow item in dtExcelData.Rows.OfType<DataRow>())
                    {
                        IKBModel ikb = new IKBModel();
                        ikb.alamat = item.Field<string>("Alamat");
                        ikb.atas_nama = item.Field<string>("Atas Nama");
                        ikb.bahan_bakar = item.Field<string>("Bahan Bakar");
                        ikb.co_line = item.Field<int>("Co Line");// coline;
                        ikb.co_line_qty = item.Field<int>("Co Line Qty");
                        ikb.co_num = item.Field<string>("Co Num");
                        ikb.cust_num = item.Field<string>("Cust Num");
                        ikb.description = item.Field<string>("Description");
                        ikb.formulir_AB = item.Field<string>("Formulir Ab");
                        ikb.identity_line = item.Field<int>("Identity Line");
                        ikb.item = item.Field<string>("Item");
                        ikb.Item_price = item.Field<decimal>("Item Price");
                        ikb.jenis = item.Field<string>("Jenis");
                        ikb.merk = item.Field<string>("Merk");
                        ikb.model = item.Field<string>("Model");
                        ikb.warna = item.Field<string>("Warna");
                        ikb.no_faktur = item.Field<string>("No Faktur");
                        ikb.no_ktp = item.Field<string>("No KTP");
                        ikb.no_mesin = item.Field<string>("No Mesin");
                        ikb.no_rangka = item.Field<string>("No Rangka");
                        ikb.pib = item.Field<string>("PIB");
                        ikb.seq = item.Field<int>("Seq");
                        ikb.silinder = item.Field<string>("Silinder");
                        ikb.site_ref = item.Field<string>("Site Ref");
                        ikb.srut = item.Field<string>("SRUT");
                        ikb.sut = item.Field<string>("SUT");
                        ikb.tahun = item.Field<int>("Tahun");
                        ikb.tgl_faktur = item.Field<DateTime>("Tgl Faktur");
                        ikb.tpt = item.Field<string>("TPT");
                        ikb.type = item.Field<string>("Type");
                        ikb.uf_harga_revisi = item.Field<decimal>("Harga Revisi");
                        ikb.uf_revisi = item.Field<int>("Revisi");
                        ikb.uf_claim_cashback = item.Field<int>("Claim Cashback");
                        ikb.uf_claim_sepeda = item.Field<int>("Claim Sepeda");
                        ikb.SubsidiClaimStatus = item.Field<string>("SubsidiClaimStatus");
                        ikb.provinsi = item.Field<string>("Provinsi");
                        ikb.kabupaten = item.Field<string>("Kabupaten");
                        ikb.kecamatan = item.Field<string>("Kecamatan");
                        ikb.desa = item.Field<string>("Desa");
                        FKData.Add(ikb);
                    }

                    //FKData = dtExcelData.Rows.OfType<DataRow>()
                    //.Select(dr => new IKBModel
                    //{
                    //    alamat = dr.Field<string>("Alamat"),
                    //    atas_nama = dr.Field<string>("Atas Nama"),
                    //    bahan_bakar = dr.Field<string>("Bahan Bakar"),
                    //    co_line = dr.Field<int>("Co Line"),
                    //    co_line_qty = dr.Field<int>("Co Line Qty"),
                    //    co_num = dr.Field<string>("Co Num"),
                    //    cust_num = dr.Field<string>("Cust Num"),
                    //    description = dr.Field<string>("Description"),
                    //    formulir_AB = dr.Field<string>("Formulir Ab"),
                    //    identity_line = dr.Field<int>("Identity Line"),
                    //    item = dr.Field<string>("Item"),
                    //    Item_price = dr.Field<decimal>("Item Price"),
                    //    jenis = dr.Field<string>("Jenis"),
                    //    merk = dr.Field<string>("Merk"),
                    //    model = dr.Field<string>("Model"),
                    //    warna = dr.Field<string>("Warna"),
                    //    no_faktur = dr.Field<string>("No Faktur"),
                    //    no_ktp = dr.Field<string>("No KTP"),
                    //    no_mesin = dr.Field<string>("No Mesin"),
                    //    no_rangka = dr.Field<string>("No Rangka"),
                    //    pib = dr.Field<string>("PIB"),
                    //    seq = dr.Field<int>("Seq"),
                    //    silinder = dr.Field<string>("Silinder"),
                    //    site_ref = dr.Field<string>("Site Ref"),
                    //    srut = dr.Field<string>("SRUT"),
                    //    sut = dr.Field<string>("SUT"),
                    //    tahun = dr.Field<int>("Tahun"),
                    //    tgl_faktur = dr.Field<DateTime>("Tgl Faktur"),
                    //    tpt = dr.Field<string>("TPT"),
                    //    type = dr.Field<string>("Type"),
                    //    uf_harga_revisi = dr.Field<decimal>("Harga Revisi"),
                    //    uf_revisi = dr.Field<int>("Revisi"),
                    //    uf_claim_cashback = dr.Field<int>("Claim Cashback"),
                    //    uf_claim_sepeda = dr.Field<int>("Claim Sepeda"),
                    //    SubsidiClaimStatus = dr.Field<string>("SubsidiClaimStatus"),
                    //    provinsi = dr.Field<string>("Provinsi"),
                    //    kabupaten = dr.Field<string>("Kabupaten"),
                    //    kecamatan = dr.Field<string>("Kecamatan"),
                    //    desa = dr.Field<string>("Desa")
                    //}).ToList();

                }

            }
            else if (extension == ".csv")
            {
                using (var reader = new StreamReader(path + fileName))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    using (var dr = new CsvDataReader(csv))
                    {
                        var dt = new DataTable();
                        dt.Columns.Add("Alamat", typeof(string));
                        dt.Columns.Add("Atas Nama", typeof(string));
                        dt.Columns.Add("Bahan Bakar", typeof(string));
                        dt.Columns.Add("Co Line", typeof(int));
                        dt.Columns.Add("Co Line Qty", typeof(int));
                        dt.Columns.Add("Co Num", typeof(string));
                        dt.Columns.Add("Cust Num", typeof(string));
                        dt.Columns.Add("Description", typeof(string));
                        dt.Columns.Add("Formulir Ab", typeof(string));
                        dt.Columns.Add("Identity Line", typeof(int));
                        dt.Columns.Add("Item", typeof(string));
                        dt.Columns.Add("Item Price", typeof(decimal));
                        dt.Columns.Add("Jenis", typeof(string));
                        dt.Columns.Add("Merk", typeof(string));
                        dt.Columns.Add("Model", typeof(string));
                        dt.Columns.Add("Warna", typeof(string));
                        dt.Columns.Add("No Faktur", typeof(string));
                        dt.Columns.Add("No KTP", typeof(string));
                        dt.Columns.Add("No Mesin", typeof(string));
                        dt.Columns.Add("No Rangka", typeof(string));
                        dt.Columns.Add("PIB", typeof(string));
                        dt.Columns.Add("Seq", typeof(int));
                        dt.Columns.Add("Silinder", typeof(string));
                        dt.Columns.Add("Site Ref", typeof(string));
                        dt.Columns.Add("SRUT", typeof(string));
                        dt.Columns.Add("SUT", typeof(string));
                        dt.Columns.Add("Tahun", typeof(int));
                        dt.Columns.Add("Tgl Faktur", typeof(DateTime));
                        dt.Columns.Add("TPT", typeof(string));
                        dt.Columns.Add("Type", typeof(string));
                        dt.Columns.Add("Harga Revisi", typeof(decimal));
                        dt.Columns.Add("Revisi", typeof(int));
                        dt.Columns.Add("Claim Cashback", typeof(int));
                        dt.Columns.Add("Claim Sepeda", typeof(int));
                        dt.Columns.Add("SubsidiClaimStatus", typeof(string));
                        dt.Columns.Add("Provinsi", typeof(string));
                        dt.Columns.Add("Kabupaten", typeof(string));
                        dt.Columns.Add("Kecamatan", typeof(string));
                        dt.Columns.Add("Desa", typeof(string));

                        dt.Load(dr);

                        foreach (DataRow item in dt.Rows.OfType<DataRow>())
                        {
                            IKBModel ikb = new IKBModel();
                            ikb.alamat = item.Field<string>("Alamat");
                            ikb.atas_nama = item.Field<string>("Atas Nama");
                            ikb.bahan_bakar = item.Field<string>("Bahan Bakar");
                            ikb.co_line = item.Field<int>("Co Line");// coline;
                            ikb.co_line_qty = item.Field<int>("Co Line Qty");
                            ikb.co_num = item.Field<string>("Co Num");
                            ikb.cust_num = item.Field<string>("Cust Num");
                            ikb.description = item.Field<string>("Description");
                            ikb.formulir_AB = item.Field<string>("Formulir Ab");
                            ikb.identity_line = item.Field<int>("Identity Line");
                            ikb.item = item.Field<string>("Item");
                            ikb.Item_price = item.Field<decimal>("Item Price");
                            ikb.jenis = item.Field<string>("Jenis");
                            ikb.merk = item.Field<string>("Merk");
                            ikb.model = item.Field<string>("Model");
                            ikb.warna = item.Field<string>("Warna");
                            ikb.no_faktur = item.Field<string>("No Faktur");
                            ikb.no_ktp = item.Field<string>("No KTP");
                            ikb.no_mesin = item.Field<string>("No Mesin");
                            ikb.no_rangka = item.Field<string>("No Rangka");
                            ikb.pib = item.Field<string>("PIB");
                            ikb.seq = item.Field<int>("Seq");
                            ikb.silinder = item.Field<string>("Silinder");
                            ikb.site_ref = item.Field<string>("Site Ref");
                            ikb.srut = item.Field<string>("SRUT");
                            ikb.sut = item.Field<string>("SUT");
                            ikb.tahun = item.Field<int>("Tahun");
                            ikb.tgl_faktur = item.Field<DateTime>("Tgl Faktur");
                            ikb.tpt = item.Field<string>("TPT");
                            ikb.type = item.Field<string>("Type");
                            ikb.uf_harga_revisi = item.Field<decimal>("Harga Revisi");
                            ikb.uf_revisi = item.Field<int>("Revisi");
                            ikb.uf_claim_cashback = item.Field<int>("Claim Cashback");
                            ikb.uf_claim_sepeda = item.Field<int>("Claim Sepeda");
                            ikb.SubsidiClaimStatus = item.Field<string>("SubsidiClaimStatus");
                            ikb.provinsi = item.Field<string>("Provinsi");
                            ikb.kabupaten = item.Field<string>("Kabupaten");
                            ikb.kecamatan = item.Field<string>("Kecamatan");
                            ikb.desa = item.Field<string>("Desa");
                            FKData.Add(ikb);
                        }
                    }
                }
            }

            var displayResult = FKData;
            var totalRecords = FKData.Count();

            listOfFakturKendaraan = FKData;

            Session["FKData"] = listOfFakturKendaraan;
            Session["FKDataCounter"] = listOfFakturKendaraan.Count();

            return Json(new { Success = true, Counter = listOfFakturKendaraan.Count() }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public ActionResult GetFKData(DataTableAjaxPostModel param)
        {
            listOfFakturKendaraan = Session["FKData"] as List<IKBModel>;

            string strsql = "";

            bool fkValid = true;
            foreach (IKBModel model in listOfFakturKendaraan)
            {
                string idPr = null;
                string idKab = null;
                string idKec = null;
                string idDes = null;
                fkValid = true;


                List<provinsi> listProp = new WilayahServices<provinsi>().GetWilayah("provinsi");

                listProp = listProp.Where(x => (x.description ?? "").ToLower().Contains(model.provinsi.ToLower())).ToList();
                if (listProp.Count() == 0)
                {
                    fkValid = false;
                    model.notes += "Provinsi tidak terdaftar,";
                }
                else
                {
                    idPr = listProp[0].id;
                    List<kabupaten> listKab = new WilayahServices<kabupaten>().GetWilayah("kabupaten/getByProvinsi", idPr);

                    listKab = listKab.Where(x => (x.description ?? "").ToLower().Contains(model.kabupaten.ToLower())).ToList();
                    if (listKab.Count() == 0)
                    {
                        fkValid = false;
                        model.notes += "Kabupaten/Kota tidak terdaftar, ";
                    }
                    else
                    {
                        idKab = listKab[0].id;
                        List<kecamatan> listKec = new WilayahServices<kecamatan>().GetWilayah("kecamatan/getByKabupaten", idKab);

                        listKec = listKec.Where(x => (x.description ?? "").ToLower().Contains(model.kecamatan.ToLower())).ToList();
                        if (listKec.Count() == 0)
                        {
                            fkValid = false;
                            model.notes += "Kecamatan tidak terdaftar, ";
                        }
                        else
                        {
                            idKec = listKec[0].id;
                            List<desa> listDes = new WilayahServices<desa>().GetWilayah("desa/getByKecamatan", idKec);

                            listDes = listDes.Where(x => (x.description ?? "").ToLower().Contains(model.desa.ToLower())).ToList();
                            if (listDes.Count() == 0)
                            {
                                fkValid = false;
                                model.notes += "Desa tidak terdaftar, ";
                            }
                            else
                            {
                                idDes = listDes[0].id;
                            }
                        }
                    }
                }

                if (fkValid)
                {

                    strsql = "Update tdi_identitas_motor_mst set " +
                    " merk='" + model.merk + "'" +
                    " ,jenis='" + model.jenis + "'" +
                    " ,model='" + model.model + "'" +
                    " ,tahun='" + model.tahun + "'" +
                    " ,silinder='" + model.silinder + "'" +
                    " ,warna='" + model.warna + "'" +
                    " ,bahan_bakar='" + model.bahan_bakar + "'" +
                    " ,atas_nama='" + model.atas_nama + "'" +
                    " ,alamat='" + model.alamat + "'" +
                    " ,no_ktp='" + model.no_ktp + "'" +
                    " ,SubsidiClaimStatus='" + model.SubsidiClaimStatus + "'" +
                    " ,uf_claim_cashback='" + model.uf_claim_cashback + "'" +
                    " ,uf_claim_sepeda='" + model.uf_claim_sepeda + "'" +
                    " ,provinsi='" + model.provinsi + "'" +
                    " ,kabupaten='" + model.kabupaten + "'" +
                    " ,kecamatan='" + model.kecamatan + "'" +
                    " ,desa='" + model.desa + "'" +
                    " ,provinsiId='" + idPr + "'" +
                    " ,kabupatenId='" + idKab + "'" +
                    " ,kecamatanId='" + idKec + "'" +
                    " ,desaId='" + idDes + "'";
                    strsql += " where no_rangka='" + model.no_rangka + "' and no_mesin='" + model.no_mesin + "' " +
                        "  and co_line='" + model.co_line + "' and co_num='" + model.co_num + "' and identity_line='" + model.identity_line + "'";

                    try
                    {
                        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString()))
                        {
                            con.Open();
                            SqlCommand cmd = new SqlCommand(strsql, con);
                            cmd.ExecuteNonQuery();
                            con.Close();
                            model.notes += "Done";
                        }
                    }
                    catch (Exception e)
                    {
                        model.notes += e.Message.ToString();
                    }
                }

            }

            var displayResult = listOfFakturKendaraan.Skip(param.start)
            .Take(param.length).ToList();
            var totalRecords = listOfFakturKendaraan.Count();

            return Json(new
            {
                param.draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = listOfFakturKendaraan
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadData()
        {

            Session["FKDataCounter"] = null;
            Session["FKData"] = listOfFakturKendaraan = new List<IKBModel>();

            return Json(new { Success = true, Counter = listOfFakturKendaraan.Count() }, JsonRequestBehavior.AllowGet);

        }
        
    }

}