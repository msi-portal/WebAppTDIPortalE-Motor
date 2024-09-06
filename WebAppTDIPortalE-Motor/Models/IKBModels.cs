using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace WebAppTDIPortalE_Motor.Models
{
    public class IKBModel
    {

        public IKBModel()
        {

        }

        public string alamat { get; set; }
        public string atas_nama { get; set; }
        public string bahan_bakar { get; set; }
        public int co_line { get; set; }
        public int co_line_qty { get; set; }
        public string co_num { get; set; }
        public string cust_num { get; set; }
        public string description { get; set; }
        public string formulir_AB { get; set; }
        public int identity_line { get; set; }
        public string item { get; set; }
        public decimal Item_price { get; set; }
        public string jenis { get; set; }
        public string merk { get; set; }
        public string model { get; set; }
        public string name { get; set; }
        public string no_faktur { get; set; }
        public string no_ktp { get; set; }
        public string no_mesin { get; set; }
        public string no_rangka { get; set; }
        public string pib { get; set; }
        public int seq { get; set; }
        public string silinder { get; set; }
        public string site_ref { get; set; }
        public string srut { get; set; }
        public string sut { get; set; }
        public int tahun { get; set; }
        public DateTime tgl_faktur { get; set; }
        public string tpt { get; set; }
        public string type { get; set; }
        public decimal uf_harga_revisi { get; set; }
        public int uf_revisi { get; set; }
        public int uf_claim_cashback { get; set; }
        public int uf_claim_sepeda { get; set; }
        public bool uf_claim_cashbacks { get; set; }
        public bool uf_claim_sepedas { get; set; }
        public string warna { get; set; }
        public int sreturn { get; set; }
        public string provinsi { get; set; }
        public string kabupaten { get; set; }
        public string kecamatan { get; set; }
        public string desa { get; set; }
        public int provinsiId { get; set; }
        public int kabupatenId { get; set; }
        public long kecamatanId { get; set; }
        public long desaId { get; set; }
        public byte[] ImageIdentity { get; set; }
        public string ImageIdentityPath { get; set; }
        public DateTime SubsidiApprovalDate { get; set; }
        public string SubsidiClaimStatus { get; set; }
        public bool ClaimStatus { get; set; }
        public DateTime SubsidiPaidDate { get; set; }
        public string notes { get; set; }
    }

    public class IKBHeaderModel
    {

        public IKBHeaderModel()
        {

        }

        public string no_faktur { get; set; }
        public string co_num { get; set; }
        public int co_line { get; set; }
        public int identity_line { get; set; }
        public string cust_num { get; set; }
        public string item { get; set; }
        public string description { get; set; }
        public Decimal qty_shipped { get; set; }
        public string u_m { get; set; }
        public DateTime tgl_faktur { get; set; }
        public DateTime order_date { get; set; }
        public DateTime ship_date { get; set; }
        public string no_mesin { get; set; }
        public string no_rangka { get; set; }
        public string merk { get; set; }
        public string alamat { get; set; }
        public string atas_nama { get; set; }
        public string SubsidiClaimStatus { get; set; }
        public int sreturn { get; set; }

    }

    public class IKBInputViewModel
    {
        [Required]
        [Display(Name = "Alamat")]
        public string alamat { get; set; }
        [Required]
        [Display(Name = "Atas Nama")]
        public string atas_nama { get; set; }
        [Required]
        [Display(Name = "Bahan Bakar")]
        public string bahan_bakar { get; set; }
        public int co_line { get; set; }
        public Decimal co_line_qty { get; set; }
        public string co_num { get; set; }
        public string cust_num { get; set; }
        public string description { get; set; }
        [Required]
        [Display(Name = "Formulir A/B")]
        public string formulir_AB { get; set; }
        public int identity_line { get; set; }
        public string item { get; set; }
        [Required]
        [Display(Name = "Item Price")]
        public decimal Item_price { get; set; }
        [Required]
        [Display(Name = "Jenis")]
        public string jenis { get; set; }
        [Required]
        [Display(Name = "Merk")]
        public string merk { get; set; }
        [Required]
        [Display(Name = "Model")]
        public string model { get; set; }
        public string name { get; set; }
        public string no_faktur { get; set; }
        [Required]
        [Display(Name = "No. KTP")]
        public string no_ktp { get; set; }
        public string no_mesin { get; set; }
        public string no_rangka { get; set; }
        [Required]
        [Display(Name = "PIB")]
        public string pib { get; set; }
        public int seq { get; set; }
        [Required]
        [Display(Name = "Isi Silinder")]
        public string silinder { get; set; }
        public string site_ref { get; set; }
        [Required]
        [Display(Name = "SRUT")]
        public string srut { get; set; }
        [Required]
        [Display(Name = "SUT")]
        public string sut { get; set; }
        [Required]
        [Display(Name = "Tahun")]
        public int tahun { get; set; }
        public DateTime tgl_faktur { get; set; }
        [Required]
        [Display(Name = "TPT")]
        public string tpt { get; set; }
        [Required]
        [Display(Name = "Type")]
        public string type { get; set; }
        public decimal uf_harga_revisi { get; set; }
        public int uf_revisi { get; set; }
        public int uf_claim_cashback { get; set; }
        public int uf_claim_sepeda { get; set; }
        public bool uf_claim_cashbacks { get; set; }
        public bool uf_claim_sepedas { get; set; }
        public string warna { get; set; }
        public string provinsi { get; set; }
        public string kabupaten { get; set; }
        public string kecamatan { get; set; }
        public string desa { get; set; }
        public string vprovinsi { get; set; }
        public string vkabupaten { get; set; }
        public string vkecamatan { get; set; }
        public string vdesa { get; set; }
        public int provinsiId { get; set; }
        public int kabupatenId { get; set; }
        public long kecamatanId { get; set; }
        public long desaId { get; set; }
        public byte[] ImageIdentity { get; set; }
        public string ImageIdentityPath { get; set; }
        public DateTime SubsidiApprovalDate { get; set; }
        public string SubsidiClaimStatus { get; set; }
        public bool ClaimStatus { get; set; }
        public DateTime SubsidiPaidDate { get; set; }
        [Required(ErrorMessage = "Pilih file gambar.")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Hanya file gambar yang diijinkan.")]
        public HttpPostedFileBase PostedFile { get; set; }

        public List<bahan_bakar> BahanBakarList { get; set; }
        public List<jenis> JenisList { get; set; }
        public List<merk> MerkList { get; set; }
        public List<model> ModelList { get; set; }
        public List<silinder> SilinderList { get; set; }
        public List<srut> SrutList { get; set; }
        public List<sut> SutList { get; set; }
        public List<tahun> TahunList { get; set; }
        public List<tpt> TptList { get; set; }
        public List<warna> WarnaList { get; set; }
        public List<type> TypeList { get; set; }
        public List<provinsi> ProvinsiList { get; set; }
        public List<kabupaten> KabupatenList { get; set; }
        public List<kecamatan> KecamatanList { get; set; }
        public List<desa> DesaList { get; set; }

    }

    public class UploadFakturModel
    {
        public string FilePath { get; set; }
        [Required(ErrorMessage = "Choose file.")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.xls|.xlxs|.csv)$", ErrorMessage = "Only for XLS, XLXS, CSV file.")]
        public HttpPostedFileBase PostedFile { get; set; }
    }

    public class type
    {
        public string code { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public string pib { get; set; }
        public string formulir_AB { get; set; }
        public string srut { get; set; }
        public string sut { get; set; }
        public string tpt { get; set; }

    }

    public class warna
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class bahan_bakar
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class tpt
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class tahun
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class sut
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class srut
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class silinder
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class model
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class merk
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class jenis
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class period
    {
        public string ID { get; set; }
        public string name { get; set; }
    }

    public class TDI_Notice
    {
        public TDI_Notice()
        {

        }
        public string cust_num { get; set; }
        public string notification { get; set; }
    }

    public class provinsi
    {
        public provinsi()
        {

        }

        public string id { get; set; }
        public string description { get; set; }
    }

    public class kabupaten
    {
        public kabupaten()
        {

        }

        public string provinsi_id { get; set; }
        public string id { get; set; }
        public string description { get; set; }
    }

    public class kecamatan
    {
        public kecamatan()
        {

        }

        public string kabupaten_id { get; set; }
        public string id { get; set; }
        public string description { get; set; }
    }

    public class desa
    {
        public desa()
        {

        }

        public string kecamatan_id { get; set; }
        public string id { get; set; }
        public string description { get; set; }
    }

    //public class FKModel
    //{
    //    public FKModel()
    //    {

    //    }

    //    public string [Alamat] { get; set; }
    //    public string [Atas Nama] { get; set; }
    //    public string [Bahan Bakar] { get; set; }
    //    public int [Co Line] { get; set; }
    //    public int [Co Line Qty] { get; set; }
    //    public string [Co Num] { get; set; }
    //    public string [Cust Num] { get; set; }
    //    public string [Description] { get; set; }
    //    public string [Formulir Ab] { get; set; }
    //    public int [Identity Line] { get; set; }
    //    public string [Item] { get; set; }
    //    public decimal [Item Price] { get; set; }
    //    public string [Jenis] { get; set; }
    //    public string [Merk] { get; set; }
    //    public string [Model] { get; set; }
    //    public string [Warna] { get; set; }
    //    public string [No Faktur] { get; set; }
    //    public string [No KTP] { get; set; }
    //    public string [No Mesin] { get; set; }
    //    public string [No Rangka] { get; set; }
    //    public string [PIB] { get; set; }
    //    public int [Seq] { get; set; }
    //    public string [Silinder] { get; set; }
    //    public string [Site Ref] { get; set; }
    //    public string [SRUT] { get; set; }
    //    public string [SUT] { get; set; }
    //    public int [Tahun] { get; set; }
    //    public DateTime [Tgl Faktur] { get; set; }
    //    public string [TPT] { get; set; }
    //    public string [Type] { get; set; }
    //    public decimal[Harga Revisi] { get; set; }
    //    public int [Revisi] { get; set; }
    //    public int [Claim Cashback] { get; set; }
    //    public int [Claim Sepeda] { get; set; }
    //    public string [SubsidiClaimStatus] { get; set; }
    //    public string [Provinsi] { get; set; }
    //    public string [Kabupaten] { get; set; }
    //    public string [Kecamatan] { get; set; }
    //    public string [Desa] { get; set; }

    //}

}