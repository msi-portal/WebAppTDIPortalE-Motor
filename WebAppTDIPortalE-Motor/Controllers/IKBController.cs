using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAppTDIPortalE_Motor.Models;

namespace WebAppTDIPortalE_Motor.Controllers
{
    [Authorize]
    public class IKBController : Controller
    {
        // GET: IKB
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GetListData(JqueryDatatableParam param)
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

            strsql = "select tdi_id.*" +
                ", cast(co_line_qty as decimal(19,8)) qty_shipped, isnull((select count(*) from serial_mst" +
                " where serial_mst.ser_num = concat(tdi_id.no_rangka, '/', tdi_id.no_mesin) " +
                " and serial_mst.ref_type = 'o' and serial_mst.stat = 'i'),0) sreturn ";
            strsql += " from tdi_identitas_motor_mst tdi_id ";
            strsql += " where ltrim(rtrim(tdi_id.cust_num))=ltrim(rtrim('" + custNum + "'))" ;
            strsql += " order by tdi_id.CreateDate desc";

            List<IKBHeaderModel> ikbData = new DAO<IKBHeaderModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                ikbData = ikbData.Where(x => (x.co_num ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || x.co_line.ToString().Contains(param.sSearch.ToLower())
                                              || (x.item ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.description ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.sreturn == 1 ? "Return" : "").ToLower().Contains(param.sSearch.ToLower())
                                              || x.qty_shipped.ToString().Contains(param.sSearch.ToLower())
                                              || (x.no_rangka ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.no_mesin ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.SubsidiClaimStatus ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              || (x.merk ?? "").ToLower().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var displayResult = ikbData.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = ikbData.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetDataIKB(string co_num, string co_line, string identity_line)
        {
            IKBInputViewModel model = new IKBInputViewModel();
            model.co_num = co_num;
            model.co_line = Convert.ToInt32(co_line);
            model.identity_line = Convert.ToInt32(identity_line);
            string strsql = "";

            strsql = "select * ";
            strsql += " from tdi_identitas_motor_mst tdi_id ";
            strsql += " where tdi_id.co_num='" + co_num + "' and tdi_id.co_line='" + co_line + "' and tdi_id.identity_line='" + identity_line +"'";

            List<IKBHeaderModel> ikbData = new DAO<IKBHeaderModel>().RetrieveDataBySQL(strsql);

            ViewBag.ikbData = ikbData;

            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'  and ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + ikbData[0].cust_num + "'))";

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            model.cust_num = ikbData[0].cust_num;
            model.name = custData[0].name;
            model.co_num = ikbData[0].co_num;
            model.co_line = ikbData[0].co_line;
            model.co_line_qty = ikbData[0].qty_shipped;
            model.no_rangka = ikbData[0].no_rangka;
            model.no_mesin = ikbData[0].no_mesin;

            strsql = "select code, description from tdi_type";
            model.TypeList = new DAO<type>().RetrieveDataBySQL(strsql);

            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Merk Motor'";
            model.MerkList = new DAO<merk>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'";
            model.JenisList = new DAO<jenis>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'";
            model.ModelList = new DAO<model>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'";
            model.TahunList = new DAO<tahun>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'";
            model.SilinderList = new DAO<silinder>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'";
            model.WarnaList = new DAO<warna>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'";
            model.BahanBakarList = new DAO<bahan_bakar>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'";
            model.TptList = new DAO<tpt>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'";
            model.SutList = new DAO<sut>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'";
            model.SrutList = new DAO<srut>().RetrieveDataBySQL(strsql);

            return PartialView("DetailIKB", model);
        }

        public ActionResult EditIKB(string no_rangka, string no_mesin, string identity_line)
        {
            if (string.IsNullOrEmpty(no_rangka) && string.IsNullOrEmpty(no_mesin) && string.IsNullOrEmpty(identity_line))
            {
                return RedirectToAction("Index");
            }
            else
            {

                IKBInputViewModel model = new IKBInputViewModel();

                string strsql = "";

                strsql = "select * ";
                strsql += " from tdi_identitas_motor_mst tdi_id ";
                strsql += " where tdi_id.no_rangka='" + no_rangka + "' and tdi_id.no_mesin='" + no_mesin + "'" +
                    " and tdi_id.identity_line='" + identity_line + "'";

                List<IKBModel> ikbData = new DAO<IKBModel>().RetrieveDataBySQL(strsql);

                ViewBag.ikbData = ikbData;

                strsql = "select cus.cust_num, ca.name from customer_mst cus";
                strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
                strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'  and ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + ikbData[0].cust_num + "'))";

                List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

                model.cust_num = ikbData[0].cust_num;
                model.name = custData[0].name;
                model.co_num = ikbData[0].co_num;
                model.co_line = ikbData[0].co_line;
                model.co_line_qty = ikbData[0].co_line_qty;
                model.no_rangka = ikbData[0].no_rangka;
                model.no_mesin = ikbData[0].no_mesin;
                model.identity_line = ikbData[0].identity_line;
                model.alamat = ikbData[0].alamat;
                model.atas_nama = ikbData[0].atas_nama;
                model.bahan_bakar = ikbData[0].bahan_bakar;
                model.description = ikbData[0].description;
                model.formulir_AB = ikbData[0].formulir_AB;
                model.item = ikbData[0].item;
                model.Item_price = ikbData[0].Item_price;
                model.jenis = ikbData[0].jenis;
                model.merk = ikbData[0].merk;
                model.model = ikbData[0].model;
                model.no_faktur = ikbData[0].no_faktur;
                model.no_ktp = ikbData[0].no_ktp;
                model.pib = ikbData[0].pib;
                model.seq = ikbData[0].seq;
                model.silinder = ikbData[0].silinder;
                model.srut = ikbData[0].srut;
                model.sut = ikbData[0].sut;
                model.tahun = ikbData[0].tahun;
                model.tgl_faktur = ikbData[0].tgl_faktur;
                model.tpt = ikbData[0].tpt;
                model.type = ikbData[0].type;
                model.warna = ikbData[0].warna;
                model.uf_harga_revisi = ikbData[0].uf_harga_revisi;
                model.uf_revisi = ikbData[0].uf_revisi;
                model.uf_claim_cashback = ikbData[0].uf_claim_cashback;
                model.uf_claim_cashbacks = ikbData[0].uf_claim_cashback == 1 ? true : false;
                model.uf_claim_sepeda = ikbData[0].uf_claim_sepeda;
                model.uf_claim_sepedas = ikbData[0].uf_claim_sepeda == 1 ? true : false;
                model.no_rangka = ikbData[0].no_rangka;
                model.no_mesin = ikbData[0].no_mesin;
                model.provinsi = ikbData[0].provinsi;
                model.kabupaten = ikbData[0].kabupaten;
                model.kecamatan = ikbData[0].kecamatan;
                model.desa = ikbData[0].desa;
                model.provinsiId = ikbData[0].provinsiId;
                model.kabupatenId = ikbData[0].kabupatenId;
                model.kecamatanId = ikbData[0].kecamatanId;
                model.desaId = ikbData[0].desaId;
                model.ImageIdentityPath = ikbData[0].ImageIdentityPath;
                model.ImageIdentity = ikbData[0].ImageIdentity;
                model.SubsidiClaimStatus = ikbData[0].SubsidiClaimStatus;
                model.ClaimStatus = ikbData[0].SubsidiClaimStatus == "Requested" || ikbData[0].SubsidiClaimStatus == "Approved" ? true : false;


                strsql = "select code, concat(description, ' | ',price) as description  from tdi_type";
                model.TypeList = new DAO<type>().RetrieveDataBySQL(strsql);
                type ntype = new type() { code = "", description = "" };
                model.TypeList.Insert(0, ntype);

                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Merk Motor'";
                model.MerkList = new DAO<merk>().RetrieveDataBySQL(strsql);
                merk nmerk = new merk() { ID = "", name = "" };
                model.MerkList.Insert(0, nmerk);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'";
                model.JenisList = new DAO<jenis>().RetrieveDataBySQL(strsql);
                jenis njenis = new jenis() { ID = "", name = "" };
                model.JenisList.Insert(0, njenis);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'";
                model.ModelList = new DAO<model>().RetrieveDataBySQL(strsql);
                model nmodel = new model() { ID = "", name = "" };
                model.ModelList.Insert(0, nmodel);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'";
                model.TahunList = new DAO<tahun>().RetrieveDataBySQL(strsql);
                tahun ntahun = new tahun() { ID = "", name = "" };
                model.TahunList.Insert(0, ntahun);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'";
                model.SilinderList = new DAO<silinder>().RetrieveDataBySQL(strsql);
                silinder nsilinder = new silinder() { ID = "", name = "" };
                model.SilinderList.Insert(0, nsilinder);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'";
                model.WarnaList = new DAO<warna>().RetrieveDataBySQL(strsql);
                warna nwarna = new warna() { ID = "", name = "" };
                model.WarnaList.Insert(0, nwarna);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'";
                model.BahanBakarList = new DAO<bahan_bakar>().RetrieveDataBySQL(strsql);
                bahan_bakar nbahan_bakar = new bahan_bakar() { ID = "", name = "" };
                model.BahanBakarList.Insert(0, nbahan_bakar);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'";
                model.TptList = new DAO<tpt>().RetrieveDataBySQL(strsql);
                tpt ntpt = new tpt() { ID = "", name = "" };
                model.TptList.Insert(0, ntpt);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'";
                model.SutList = new DAO<sut>().RetrieveDataBySQL(strsql);
                sut nsut = new sut() { ID = "", name = "" };
                model.SutList.Insert(0, nsut);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'";
                model.SrutList = new DAO<srut>().RetrieveDataBySQL(strsql);
                srut nsrut = new srut() { ID = "", name = "" };
                model.SrutList.Insert(0, nsrut);

                model.ProvinsiList = new WilayahServices<provinsi>().GetWilayah("provinsi");
                provinsi nprovinsi = new provinsi() { id = "", description = "" };
                model.ProvinsiList.Insert(0, nprovinsi);
                model.KabupatenList = new WilayahServices<kabupaten>().GetWilayah("kabupaten/getByProvinsi", Convert.ToString(ikbData[0].provinsiId));
                kabupaten nkabupaten = new kabupaten() { id = "", description = "" };
                model.KabupatenList.Insert(0, nkabupaten);
                model.KecamatanList = new WilayahServices<kecamatan>().GetWilayah("kecamatan/getByKabupaten", Convert.ToString(ikbData[0].kabupatenId));
                kecamatan nkecamatan = new kecamatan() { id = "", description = "" };
                model.KecamatanList.Insert(0, nkecamatan);
                model.DesaList = new WilayahServices<desa>().GetWilayah("desa/getByKecamatan", Convert.ToString(ikbData[0].kecamatanId));
                desa ndesa = new desa() { id = "", description = "" };
                model.DesaList.Insert(0, ndesa);

                return View(model);
            }
        }

        public ActionResult GetData(JqueryDatatableParam param)
        {
            var result = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());

            string filterCust = "";
            if (result.CustomerCode != "9999999")
            {
                filterCust = " ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + result.CustomerCode + "'))";
            }

            string strsql = "";
            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;


            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            string custNum = custData[0].cust_num;

            if (filterCust == "")
                filterCust = " ltrim(rtrim(co.cust_num))= ltrim(rtrim('" + custNum + "'))";

            strsql = "";
            strsql = "select co.cust_num, cs.co_num, cs.co_line, ci.item, ci.description, cs.qty_shipped, ci.u_m, co.order_date, cs.ship_date";
            strsql += " from co_mst co";
            strsql += " inner join coitem_mst ci on co.co_num = ci.co_num and co.site_ref = ci.site_ref";
            strsql += " inner join co_ship_mst cs on ci.co_num = cs.co_num and ci.co_line = cs.co_line and cs.site_ref = ci.site_ref";
            strsql += " where co.site_ref = '" + Global.Site + "'";
            if (filterCust != "")
                strsql += " and " + filterCust;

            List<IKBHeaderModel> ikbData = new DAO<IKBHeaderModel>().RetrieveDataBySQL(strsql);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                ikbData = ikbData.Where(x => x.co_num.ToLower().Contains(param.sSearch.ToLower())
                                              || x.co_line.ToString().Contains(param.sSearch.ToLower())
                                              || x.item.ToLower().Contains(param.sSearch.ToLower())
                                              || x.description.ToLower().Contains(param.sSearch.ToLower())
                                              || x.u_m.ToLower().Contains(param.sSearch.ToLower())
                                              || x.order_date.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.sSearch.ToLower())
                                              || x.ship_date.ToString("dd'/'MM'/'yyyy").ToLower().Contains(param.sSearch.ToLower())
                                              ).ToList();
            }

            var displayResult = ikbData.Skip(param.iDisplayStart)
            .Take(param.iDisplayLength).ToList();
            var totalRecords = ikbData.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GenerateIKB(string co_num, string co_line)
        {

            IKBInputViewModel model = new IKBInputViewModel();

            try
            {
                //string infobar = "";
                //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection01"].ToString()))
                //{
                //    con.Open();
                //    SqlCommand cmd = new SqlCommand();
                //    cmd.Connection = con;
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.CommandText = "TDI_GenerateIdentitasSp";
                //    cmd.Parameters.AddWithValue("@CoNum", co_num);
                //    cmd.Parameters.AddWithValue("@CoLine", co_line);
                //    cmd.Parameters.Add("@infobar", SqlDbType.VarChar, 2800);
                //    cmd.Parameters["@infobar"].Direction = ParameterDirection.Output;
                //    int i = cmd.ExecuteNonQuery();
                //    infobar = Convert.ToString(cmd.Parameters["@infobar"].Value);
                //}

                //int retVal = new DAO<COModel>().UpdateDataBySP("TDI_GenerateIdentitasSp"
                //    , new string[] { "@CoNum", "@CoLine", "@infobar" }
                //    , new object[] { co_num, co_line, "" });

                string strsql = "";

                strsql = "select * ";
                strsql += " from tdi_identitas_motor_mst tdi_id ";
                strsql += " where tdi_id.co_num='" + co_num + "' and tdi_id.co_line='" + co_line + "'";

                List<IKBHeaderModel> ikbData = new DAO<IKBHeaderModel>().RetrieveDataBySQL(strsql);

                ViewBag.ikbData = ikbData;

                strsql = "select cus.cust_num, ca.name from customer_mst cus";
                strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
                strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'  and ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + ikbData[0].cust_num + "'))";

                List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

                model.cust_num = ikbData[0].cust_num;
                model.name = custData[0].name;
                model.co_num = ikbData[0].co_num;
                model.co_line = ikbData[0].co_line;
                model.co_line_qty = ikbData[0].qty_shipped;
                model.no_rangka = ikbData[0].no_rangka;
                model.no_mesin = ikbData[0].no_mesin;

                strsql = "select code, description from tdi_type";
                model.TypeList = new DAO<type>().RetrieveDataBySQL(strsql);

                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Merk Motor'";
                model.MerkList = new DAO<merk>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'";
                model.JenisList = new DAO<jenis>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'";
                model.ModelList = new DAO<model>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'";
                model.TahunList = new DAO<tahun>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'";
                model.SilinderList = new DAO<silinder>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'";
                model.WarnaList = new DAO<warna>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'";
                model.BahanBakarList = new DAO<bahan_bakar>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'";
                model.TptList = new DAO<tpt>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'";
                model.SutList = new DAO<sut>().RetrieveDataBySQL(strsql);
                //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'
                strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'";
                model.SrutList = new DAO<srut>().RetrieveDataBySQL(strsql);

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            return View("ListIKB", model);

        }

        public ActionResult CreateIKB(string co_num, string co_line, IKBInputViewModel model)
        {

            if (co_num == "" || co_line == "" || co_num is null || co_line is null) return RedirectToAction("Index");

            string strsql = "";

            strsql = "select co.cust_num, cs.co_num, cs.co_line, ci.item, ci.description, cs.qty_shipped, ci.u_m, co.order_date, cs.ship_date";
            strsql += " , SUBSTRING(sr.ser_num,1, CHARINDEX('/', sr.ser_num) - 1) as no_rangka";
            strsql += " , SUBSTRING(sr.ser_num, CHARINDEX('/', sr.ser_num) +1, LEN(sr.ser_num)) as no_mesin";
            strsql += " from co_mst co";
            strsql += " inner join coitem_mst ci on co.co_num = ci.co_num and co.site_ref = ci.site_ref";
            strsql += " inner join co_ship_mst cs on ci.co_num = cs.co_num and ci.co_line = cs.co_line and cs.site_ref = ci.site_ref";
            strsql += " INNER JOIN serial_mst sr ON ci.co_num = sr.ref_num and ci.co_line = sr.ref_line AND ci.item = sr.item ";
            strsql += " where co.site_ref = '" + Global.Site + "'";
            strsql += " and cs.co_num='" + co_num + "' and cs.co_line='" + co_line + "'";

            List<IKBHeaderModel> ikbData = new DAO<IKBHeaderModel>().RetrieveDataBySQL(strsql);

            strsql = "select cus.cust_num, ca.name from customer_mst cus";
            strsql += " inner join custaddr_mst ca on ca.cust_num = cus.cust_num and cus.site_ref = ca.site_ref ";
            strsql += " where ca.cust_seq = 0 and cus.site_ref = '" + Global.Site + "'  and ltrim(rtrim(cus.cust_num))=ltrim(rtrim('" + ikbData[0].cust_num + "'))";

            List<CustModel> custData = new DAO<CustModel>().RetrieveDataBySQL(strsql);

            model.cust_num = ikbData[0].cust_num;
            model.name = custData[0].name;
            model.co_num = ikbData[0].co_num;
            model.co_line = ikbData[0].co_line;
            model.co_line_qty = ikbData[0].qty_shipped;
            model.no_rangka = ikbData[0].no_rangka;
            model.no_mesin = ikbData[0].no_mesin;

            strsql = "select code, description, pib, formulir_AB, srut, sut, tpt from tdi_type";
            model.TypeList = new DAO<type>().RetrieveDataBySQL(strsql);

            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Merk Motor'";
            model.MerkList = new DAO<merk>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'";
            model.JenisList = new DAO<jenis>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'";
            model.ModelList = new DAO<model>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'";
            model.TahunList = new DAO<tahun>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'";
            model.SilinderList = new DAO<silinder>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'";
            model.WarnaList = new DAO<warna>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'";
            model.BahanBakarList = new DAO<bahan_bakar>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'";
            model.TptList = new DAO<tpt>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'";
            model.SutList = new DAO<sut>().RetrieveDataBySQL(strsql);
            //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'
            strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'";
            model.SrutList = new DAO<srut>().RetrieveDataBySQL(strsql);

            return View(model);
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
        public ActionResult UpdateIKB(IKBInputViewModel model, FormCollection formCollection, HttpPostedFileBase postedFile)
        {
            string path = Server.MapPath("~/Uploads/");
            string fileName = "";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            byte[] bytes = null;
            if (postedFile != null)
            {
               
                fileName = Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(path + fileName);
                CompressImage.Compressimage(postedFile.InputStream, path + fileName);
                //using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                //{
                //    bytes = br.ReadBytes(postedFile.ContentLength);
                //}

                //Image img = Image.FromFile(path);
                //MemoryStream tmpStream = new MemoryStream();
                //img.Save(tmpStream, ImageFormat.jpg); // change to other format
                //tmpStream.Seek(0, SeekOrigin.Begin);
                //byte[] imgBytes = new byte[MAX_IMG_SIZE];
                //tmpStream.Read(imgBytes, 0, MAX_IMG_SIZE);
                bytes = ReadFile(path + fileName);
            }
            string strsql = "";
            //if (
            //    formCollection["alamat"] != ""
            //    && formCollection["atas_nama"] != ""
            //    && formCollection["bahan_bakar"] != ""
            //    && formCollection["description"] != ""
            //    && formCollection["formulir_AB"] != ""
            //    && formCollection["item"] != ""
            //    && formCollection["jenis"] != ""
            //    && formCollection["merk"] != ""
            //    && formCollection["model"] != ""
            //    && formCollection["no_ktp"] != ""
            //    && formCollection["pib"] != ""
            //    && formCollection["silinder"] != ""
            //    && formCollection["srut"] != ""
            //    && formCollection["sut"] != ""
            //    && formCollection["tahun"] != ""
            //    && formCollection["tpt"] != ""
            //    && formCollection["type"] != ""
            //    && formCollection["warna"] != ""
            //    )
            //{
            //    try
            //    {
            //        model = new IKBInputViewModel();
            //        model.alamat = formCollection["alamat"];
            //        model.atas_nama = formCollection["atas_nama"];
            //        model.bahan_bakar = formCollection["bahan_bakar"];
            //        model.description = formCollection["description"];
            //        model.formulir_AB = formCollection["formulir_AB"];
            //        model.item = formCollection["item"];
            //        model.Item_price = Convert.ToDecimal(formCollection["Item_price"]);
            //        model.jenis = formCollection["jenis"];
            //        model.merk = formCollection["merk"];
            //        model.model = formCollection["model"];
            //        model.name = formCollection["name"];
            //        model.no_faktur = formCollection["no_faktur"];
            //        model.no_ktp = formCollection["no_ktp"];
            //        model.pib = formCollection["pib"];
            //        model.seq = Convert.ToInt32(formCollection["seq"]);
            //        model.silinder = formCollection["silinder"];
            //        model.srut = formCollection["srut"];
            //        model.sut = formCollection["sut"];
            //        model.tahun = Convert.ToInt32(formCollection["tahun"]);
            //        model.tgl_faktur = DateTime.ParseExact(formCollection["tgl_faktur"] ?? DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //        model.tpt = formCollection["tpt"];
            //        model.type = formCollection["type"];
            //        model.warna = formCollection["warna"];
            //        model.uf_harga_revisi = Convert.ToDecimal(formCollection["uf_harga_revisi"]);
            //        model.uf_revisi = Convert.ToInt32(formCollection["uf_revisi"]);
            //        model.no_rangka = formCollection["no_rangka"];
            //        model.no_mesin = formCollection["no_mesin"];

            //        strsql = "Update tdi_identitas_motor_mst set " +
            //        "  alamat='" + model.alamat + "'" +
            //        " ,atas_nama='" + model.atas_nama + "'" +
            //        " ,bahan_bakar='" + model.bahan_bakar + "'" +
            //        " ,formulir_AB='" + model.formulir_AB + "'" +
            //        " ,Item_price='" + model.Item_price + "'" +
            //        " ,jenis='" + model.jenis + "'" +
            //        " ,merk='" + model.merk + "'" +
            //        " ,model='" + model.model + "'" +
            //        " ,name='" + model.name + "'" +
            //        " ,no_faktur='" + model.no_faktur + "'" +
            //        " ,no_ktp='" + model.no_ktp + "'" +
            //        " ,pib='" + model.pib + "'" +
            //        " ,seq='" + model.seq + "'" +
            //        " ,silinder='" + model.silinder + "'" +
            //        " ,srut='" + model.srut + "'" +
            //        " ,sut='" + model.sut + "'" +
            //        " ,tahun='" + model.tahun + "'" +
            //        " ,tgl_faktur='" + model.tgl_faktur + "'" +
            //        " ,tpt='" + model.tpt + "'" +
            //        " ,type='" + model.type + "'" +
            //        " ,warna='" + model.warna + "'" +
            //        " ,uf_harga_revisi='" + model.uf_harga_revisi + "'" +
            //        " ,uf_revisi='" + model.uf_revisi + "'" +
            //        " where no_rangka='" + model.no_rangka + "' and no_mesin='" + model.no_mesin + "'";

            //        int retVal = new DAO<IKBInputViewModel>().UpdateDataBySP(strsql);

            //    }
            //    catch (Exception e)
            //    {
            //        ModelState.AddModelError("", e.Message);
            //    }

            //}
            //else
            //{
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Merk Motor'";
            //    model.MerkList = new DAO<merk>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Jenis Motor'";
            //    model.JenisList = new DAO<jenis>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Model Motor'";
            //    model.ModelList = new DAO<model>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Tahun Pembuatan Motor'";
            //    model.TahunList = new DAO<tahun>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Silinder Motor'";
            //    model.SilinderList = new DAO<silinder>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Warna Motor'";
            //    model.WarnaList = new DAO<warna>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-Bahan Bakar Motor'";
            //    model.BahanBakarList = new DAO<bahan_bakar>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-TPT Motor'";
            //    model.TptList = new DAO<tpt>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SUT Motor'";
            //    model.SutList = new DAO<sut>().RetrieveDataBySQL(strsql);
            //    //select userdefinedtypevalues.value from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'
            //    strsql = "select userdefinedtypevalues.value as ID, userdefinedtypevalues.value as name from userdefinedtypevalues where TYPENAME = 'UDT-SRUT Motor'";
            //    model.SrutList = new DAO<srut>().RetrieveDataBySQL(strsql);

            //    if (formCollection["alamat"] != "" || formCollection["alamat"] is null) ModelState.AddModelError("", "Please input Alamat");
            //    if (formCollection["atas_nama"] != "" || formCollection["atas_nama"] is null) ModelState.AddModelError("", "Please input Atas nama");
            //    if (formCollection["bahan_bakar"] != "" || formCollection["bahan_bakar"] is null) ModelState.AddModelError("", "Please input Bahan bakar");
            //    if (formCollection["formulir_AB"] != "" || formCollection["formulir_AB"] is null) ModelState.AddModelError("", "Please input Formulir A/B");
            //    if (formCollection["item"] != "" || formCollection["item"] is null) ModelState.AddModelError("", "Please input Item");
            //    if (formCollection["jenis"] != "" || formCollection["jenis"] is null) ModelState.AddModelError("", "Please input Jenis");
            //    if (formCollection["merk"] != "" || formCollection["merk"] is null) ModelState.AddModelError("", "Please input Merk");
            //    if (formCollection["model"] != "" || formCollection["model"] is null) ModelState.AddModelError("", "Please input Model");
            //    if (formCollection["no_ktp"] != "" || formCollection["no_ktp"] is null) ModelState.AddModelError("", "Please input No. KTP/TDP");
            //    if (formCollection["pib"] != "" || formCollection["pib"] is null) ModelState.AddModelError("", "Please input PIB");
            //    if (formCollection["silinder"] != "" || formCollection["silinder"] is null) ModelState.AddModelError("", "Please input Isi Silinder");
            //    if (formCollection["srut"] != "" || formCollection["srut"] is null) ModelState.AddModelError("", "Please input SRUT");
            //    if (formCollection["sut"] != "" || formCollection["sut"] is null) ModelState.AddModelError("", "Please input SUT");
            //    if (formCollection["tahun"] != "" || formCollection["tahun"] is null) ModelState.AddModelError("", "Please input Tahun");
            //    if (formCollection["tpt"] != "" || formCollection["tpt"] is null) ModelState.AddModelError("", "Please input TPT");
            //    if (formCollection["type"] != "" || formCollection["type"] is null) ModelState.AddModelError("", "Please input Type");
            //    if (formCollection["warna"] != "" || formCollection["warna"] is null) ModelState.AddModelError("", "Please input Warna");

            //    return View("EditIKB", model);
            //}
            try
            {
                
                model = new IKBInputViewModel();

                strsql = "select code, description, pib, formulir_AB, srut, sut, tpt from tdi_type where code='" + formCollection["type"] + "'";
                model.TypeList = new DAO<type>().RetrieveDataBySQL(strsql);

                model.alamat = formCollection["alamat"];
                model.atas_nama = formCollection["atas_nama"];
                model.bahan_bakar = formCollection["bahan_bakar"];
                model.description = formCollection["description"];
                model.formulir_AB = model.TypeList[0].formulir_AB;
                model.item = formCollection["item"];
                model.Item_price = Convert.ToDecimal(formCollection["Item_price"]);
                model.jenis = formCollection["jenis"];
                model.merk = formCollection["merk"];
                model.model = formCollection["model"];
                model.name = formCollection["name"];
                model.no_faktur = formCollection["no_faktur"];
                model.no_ktp = formCollection["no_ktp"];
                model.pib = model.TypeList[0].pib;
                model.seq = Convert.ToInt32(formCollection["seq"]);
                model.silinder = formCollection["silinder"];
                model.srut = model.TypeList[0].srut;
                model.sut = model.TypeList[0].sut;
                model.tahun = Convert.ToInt32((formCollection["tahun"] == "" ? DateTime.Today.Year.ToString() : formCollection["tahun"]));
                model.tgl_faktur = DateTime.ParseExact(formCollection["tgl_faktur"] ?? DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model.tpt = model.TypeList[0].tpt;
                model.type = formCollection["type"];
                model.warna = formCollection["warna"];
                model.uf_harga_revisi = Convert.ToDecimal(formCollection["uf_harga_revisi"]);
                model.uf_claim_cashback = Convert.ToInt32(formCollection["uf_claim_cashback"]);
                model.uf_claim_sepeda = Convert.ToInt32(formCollection["uf_claim_sepeda"]);
                model.uf_revisi = Convert.ToInt32(formCollection["uf_revisi"]);
                model.no_rangka = formCollection["no_rangka"];
                model.no_mesin = formCollection["no_mesin"];
                model.provinsi = formCollection["provinsi"];
                model.kabupaten = formCollection["kabupaten"];
                model.kecamatan = formCollection["kecamatan"];
                model.desa = formCollection["desa"];
                model.provinsiId = Convert.ToInt32(formCollection["provinsiId"]);
                model.kabupatenId = Convert.ToInt32(formCollection["kabupatenId"]);
                model.kecamatanId = Convert.ToInt64(formCollection["kecamatanId"]);
                model.desaId = Convert.ToInt64(formCollection["desaId"]);
                model.SubsidiClaimStatus = formCollection["SubsidiClaimStatus"];

                strsql = "Update tdi_identitas_motor_mst set " +
                "  alamat='" + model.alamat + "'" +
                " ,atas_nama='" + model.atas_nama + "'" +
                " ,bahan_bakar='" + model.bahan_bakar + "'" +
                " ,formulir_AB='" + model.formulir_AB + "'" +
                " ,Item_price='" + model.Item_price + "'" +
                " ,jenis='" + model.jenis + "'" +
                " ,merk='" + model.merk + "'" +
                " ,model='" + model.model + "'" +
                " ,name='" + model.name + "'" +
                //" ,no_faktur='" + model.no_faktur + "'" +
                " ,no_ktp='" + model.no_ktp + "'" +
                " ,pib='" + model.pib + "'" +
                " ,seq='" + model.seq + "'" +
                " ,silinder='" + model.silinder + "'" +
                " ,srut='" + model.srut + "'" +
                " ,sut='" + model.sut + "'" +
                " ,tahun='" + model.tahun + "'" +
                //" ,tgl_faktur='" + model.tgl_faktur + "'" +
                " ,tpt='" + model.tpt + "'" +
                " ,type='" + model.type + "'" +
                " ,warna='" + model.warna + "'" +
                " ,uf_harga_revisi='" + model.uf_harga_revisi + "'" +
                " ,uf_claim_cashback='" + model.uf_claim_cashback + "'" +
                " ,uf_claim_sepeda='" + model.uf_claim_sepeda + "'" +
                " ,uf_revisi='" + model.uf_revisi + "'" +
                " ,provinsi='" + model.provinsi + "'" +
                " ,kabupaten='" + model.kabupaten + "'" +
                " ,kecamatan='" + model.kecamatan + "'" +
                " ,desa='" + model.desa + "'" +
                " ,provinsiId='" + model.provinsiId + "'" +
                " ,kabupatenId='" + model.kabupatenId + "'" +
                " ,kecamatanId='" + model.kecamatanId + "'" +
                " ,desaId='" + model.desaId + "'" +
                " ,SubsidiClaimStatus='" + model.SubsidiClaimStatus + "'";
                if (fileName != "") {
                    strsql += " ,ImageIdentity=@bytes" +
                    " ,ImageIdentityPath='" + fileName + "'";
                }
                strsql += " where no_rangka='" + model.no_rangka + "' and no_mesin='" + model.no_mesin + "'";

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
                        //cmd.Parameters.AddWithValue("@bytes", bytes);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
               
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult SaveIKB(IKBInputViewModel model, FormCollection formCollection)
        {
            string strsql = "";
            try
            {
                model = new IKBInputViewModel();
                model.alamat = formCollection["alamat"];
                model.atas_nama = formCollection["atas_nama"];
                model.bahan_bakar = formCollection["bahan_bakar"];
                model.co_line = Convert.ToInt32(formCollection["co_line"]);
                model.co_line_qty = Convert.ToDecimal(formCollection["co_line_qty"]);
                model.co_num = formCollection["co_num"];
                model.cust_num = formCollection["cust_num"];
                model.description = formCollection["description"];
                model.formulir_AB = formCollection["formulir_AB"];
                model.identity_line = Convert.ToInt32(formCollection["identity_line"]);
                model.item = formCollection["item"];
                model.Item_price = Convert.ToDecimal(formCollection["Item_price"]);
                model.jenis = formCollection["jenis"];
                model.merk = formCollection["merk"];
                model.model = formCollection["model"];
                model.name = formCollection["name"];
                model.no_faktur = formCollection["no_faktur"];
                model.no_ktp = formCollection["no_ktp"];
                model.no_mesin = formCollection["no_mesin"];
                model.no_rangka = formCollection["no_rangka"];
                model.pib = formCollection["pib"];
                model.seq = Convert.ToInt32(formCollection["seq"]);
                model.silinder = formCollection["silinder"];
                model.site_ref = formCollection["site_ref"];
                model.srut = formCollection["srut"];
                model.sut = formCollection["sut"];
                model.tahun = Convert.ToInt32(formCollection["tahun"]);
                model.tgl_faktur = DateTime.ParseExact(formCollection["tgl_faktur"]??DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model.tpt = formCollection["tpt"];
                model.type = formCollection["type"];
                model.warna = formCollection["warna"];
                model.uf_harga_revisi = Convert.ToDecimal(formCollection["uf_harga_revisi"]);
                model.uf_revisi = Convert.ToInt32(formCollection["uf_revisi"]);

                strsql = "INSERT INTO tdi_identitas_motor_mst (" +
                " alamat" +
                " ,atas_nama" +
                " ,bahan_bakar" +
                " ,co_line" +
                " ,co_line_qty" +
                " ,co_num" +
                " ,cust_num" +
                " ,description" +
                " ,formulir_AB" +
                " ,identity_line" +
                " ,item" +
                " ,Item_price" +
                " ,jenis" +
                " ,merk" +
                " ,model" +
                " ,name" +
                " ,no_faktur" +
                " ,no_ktp" +
                " ,no_mesin" +
                " ,no_rangka" +
                " ,pib" +
                " ,seq" +
                " ,silinder" +
                " ,site_ref" +
                " ,srut" +
                " ,sut" +
                " ,tahun" +
                " ,tgl_faktur" +
                " ,tpt" +
                " ,type" +
                " ,warna" +
                " ,uf_harga_revisi" +
                " ,uf_revisi)" +
                " VALUES" +
                " (" +
                "'" + model.alamat + "'" +
                ",'" + model.atas_nama + "'" +
                ",'" + model.bahan_bakar + "'" +
                ",'" + model.co_line + "'" +
                ",'" + model.co_line_qty + "'" +
                ",'" + model.co_num + "'" +
                ",'" + model.cust_num + "'" +
                ",'" + model.description + "'" +
                ",'" + model.formulir_AB + "'" +
                ",'" + model.identity_line + "'" +
                ",'" + model.item + "'" +
                ",'" + model.Item_price + "'" +
                ",'" + model.jenis + "'" +
                ",'" + model.merk + "'" +
                ",'" + model.model + "'" +
                ",'" + model.name + "'" +
                ",'" + model.no_faktur + "'" +
                ",'" + model.no_ktp + "'" +
                ",'" + model.no_mesin + "'" +
                ",'" + model.no_rangka + "'" +
                ",'" + model.pib + "'" +
                ",'" + model.seq + "'" +
                ",'" + model.silinder + "'" +
                ",'" + model.site_ref + "'" +
                ",'" + model.srut + "'" +
                ",'" + model.sut + "'" +
                ",'" + model.tahun + "'" +
                ",'" + model.tgl_faktur + "'" +
                ",'" + model.tpt + "'" +
                ",'" + model.type + "'" +
                ",'" + model.warna + "'" +
                ",'" + model.uf_harga_revisi + "'" +
                ",'" + model.uf_revisi + "'" +
                ")";

                int retVal = new DAO<IKBInputViewModel>().UpdateDataBySP(strsql);

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("CreateIKB", new { model.co_num, model.co_line, model });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult getProvinsiById(string provinsiId)
        {
            List<provinsi> lProvinsi = new WilayahServices<provinsi>().GetWilayah("provinsi", provinsiId);
            return Json(lProvinsi);
        }

        [HttpPost]
        public ActionResult getKabupatenById(string kabupatenId)
        {
            List<kabupaten> lKabupaten = new WilayahServices<kabupaten>().GetWilayah("kabupaten", kabupatenId);
            return Json(lKabupaten);
        }
        [HttpPost]
        public ActionResult getKecamatanById(string kecamatanId)
        {
            List<kecamatan> lKecamatan = new WilayahServices<kecamatan>().GetWilayah("kecamatan", kecamatanId);
            return Json(lKecamatan);
        }

        [HttpPost]
        public ActionResult getDesaById(string desaId)
        {
            List<desa> lDesa = new WilayahServices<desa>().GetWilayah("desa", desaId);
            return Json(lDesa);
        }

        [HttpPost]
        public ActionResult getKabupaten(string provinsiId)
        {
            List<kabupaten> lKabupaten = new WilayahServices<kabupaten>().GetWilayah("kabupaten/getByProvinsi", provinsiId);
            kabupaten nkabupaten = new kabupaten() { id = "", description = "" };
            lKabupaten.Insert(0, nkabupaten);
            var kab = lKabupaten.Select(a => "<option value=" + a.id + ">" + a.description + "</option>");
            ContentResult retVal = Content(String.Join("", kab));
            return retVal;
        }

        [HttpPost]
        public ActionResult getKecamatan(string kabupatenId)
        {
            List<kecamatan> lKecamatan = new WilayahServices<kecamatan>().GetWilayah("kecamatan/getByKabupaten", kabupatenId);
            kecamatan nkecamatan = new kecamatan() { id = "", description = "" };
            lKecamatan.Insert(0, nkecamatan);
            var kec = lKecamatan.Select(a => "<option value=" + a.id + ">" + a.description + "</option>");
            ContentResult retVal = Content(String.Join("", kec));
            return retVal;
        }

        [HttpPost]
        public ActionResult getDesa(string kecamatanId)
        {
            List<desa> lDesa = new WilayahServices<desa>().GetWilayah("desa/getByKecamatan", kecamatanId);
            desa ndesa = new desa() { id = "", description = "" };
            lDesa.Insert(0, ndesa);
            var des = lDesa.Select(a => "<option value=" + a.id + ">" + a.description + "</option>");
            ContentResult retVal = Content(String.Join("", des));
            return retVal;
        }
    }
}