using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAppTDIPortalE_Motor.Models
{

    public class AllocGraphModel
    {
        public AllocGraphModel()
        {

        }

        public string item { get; set; }
        public string Spec { get; set; }
        public string Model { get; set; }
        public Decimal Qty_Alloc { get; set; }
        public Decimal Qty { get; set; }

    }

    public class StockInfoModel
    {
        public StockInfoModel()
        {

        }

        public string Spec { get; set; }
        public string Model { get; set; }
        public int SoldQty { get; set; }
        public int BeginningStock { get; set; }
        public int AvailableQty { get; set; }

    }


    public class COWarnModel
    {
        public COWarnModel()
        {

        }

        public string co_num { get; set; }
        public DateTime order_date { get; set; }
        public DateTime due_date { get; set; }
        public string item { get; set; }
        public Decimal qty_ordered { get; set; }
        public int outs_day { get; set; }

    }

    public class CustModel
    {
        public CustModel()
        {

        }

        public string cust_num { get; set; }
        public string name { get; set; }
        public Decimal ARBalance { get; set; }
        public Decimal OnOrderBalance { get; set; }
        public Decimal CreditLimit { get; set; }
    }

    public class CustAllocModel
    {
        public CustAllocModel()
        {

        }
        public string cust_num { get; set; }
        public string model { get; set; }
        public string spec { get; set; }
        public Decimal qty { get; set; }
        public Decimal qty_ordered { get; set; }
        public Decimal qty_outs { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }

    public class OutstandingARModel
    {
        public OutstandingARModel()
        {

        }
        public string cust_po { get; set; }
        public string co_num { get; set; }
        public string Customer { get; set; }
        public string Faktur { get; set; }
        public int NoBukti { get; set; }
        public Decimal Kredit { get; set; }
        public Decimal Debit { get; set; }
        public Decimal Sisa { get; set; }
        public DateTime Tanggal { get; set; }
        public DateTime TanggalDueDate { get; set; }
        public DateTime TanggalBayar { get; set; }
    }

    public class COModel
    {
        public COModel()
        {

        }

        public string co_num { get; set; }
        public string cust_num { get; set; }
        public string cust_name { get; set; }
        public string cust_po { get; set; }
        public int cust_seq { get; set; }
        public string whse { get; set; }
        public DateTime order_date { get; set; }

    }

    public class COLinesModel
    {
        public COLinesModel()
        {

        }

        public string co_num { get; set; }
        public int co_line { get; set; }
        public string item { get; set; }
        public string description { get; set; }
        public Decimal qty_ordered { get; set; }
        public string u_m { get; set; }
        public DateTime order_date { get; set; }
        public DateTime due_date { get; set; }
        public string cust_po { get; set; }
        public string stat { get; set; }
        public string cust_num { get; set; }
        public int credit_hold { get; set; }
    }

    public class CustAddrModel
    {
        public CustAddrModel()
        {

        }

        public string cust_num { get; set; }
        public string name { get; set; }
        public int cust_seq { get; set; }
        public string city { get; set; }

    }

    public class WhseModel
    {
        public WhseModel()
        {

        }

        public string whse { get; set; }
        public string name { get; set; }

    }

    public class CustomerCurrencyModel
    {
        public CustomerCurrencyModel()
        {

        }

        public string cust_num { get; set; }
        public string curr_code { get; set; }
        public Decimal exch_rate { get; set; }
        public Byte fixed_rate { get; set; }
        public string description { get; set; }
        public Byte is_default { get; set; }

    }

    public class COAddViewModel
    {

        [Required]
        [Display(Name = "CO Number")]
        public string co_num { get; set; }

        [Required]
        [Display(Name = "Customer Number")]
        public string cust_num { get; set; }
        public string cust_name { get; set; }
        public string cust_po { get; set; }
        public int cust_seq { get; set; }
        public string ship_name { get; set; }
        public string whse { get; set; }
        public bool sparepart { get; set; }
        public DateTime order_date { get; set; }
        public DateTime Uf_date_pengambilan { get; set; }
        public string Uf_StartTime { get; set; }
        public string Uf_EndTime { get; set; }
    }

    public class COItemModel
    {

        public COItemModel()
        {

        }

        public int co_line { get; set; }
        public string item { get; set; }
        public string description { get; set; }
        public Decimal qty_ordered { get; set; }
        public string u_m { get; set; }
        public Decimal price { get; set; }
        public Decimal net_price { get; set; }
        public DateTime due_date { get; set; }
        public Decimal unit_cost { get; set; }
        public Decimal matl_cost { get; set; }
    }

    public class ItemModel
    {
        public ItemModel()
        {

        }

        public string item { get; set; }
        public string description { get; set; }
        public string u_m { get; set; }
        public string Uf_Model { get; set; }
        public string Uf_spec { get; set; }
        public Decimal unit_price { get; set; }
        public Decimal qty { get; set; }
        public Decimal unit_cost { get; set; }
        public Decimal matl_cost { get; set; }

    }

    public class COReservationModel
    {
        public COReservationModel()
        {

        }

        public DateTime order_date { get; set; }
        public string cust_num { get; set; }
        public string name { get; set; }
        public string ref_num { get; set; }
        public int ref_line { get; set; }
        public string item { get; set; }
        public string u_m { get; set; }
        public Decimal qty_rsvd_conv { get; set; }
        public Decimal qty_rsvd { get; set; }
        public string whse { get; set; }
        public string No_Rangka { get; set; }
        public string No_Mesin { get; set; }
        public DateTime RecordDate { get; set; }
        
    }

    public class CustInfoModel
    {
        public CustInfoModel()
        {

        }

        public string contact { get; set; }
        public string phone { get; set; }
        public string terms_code { get; set; }
        public string tax_code { get; set; }
        public string end_user_type { get; set; }
        public int include_tax_in_price { get; set; }

    }

    public class KeyIDModel
    {
        public KeyIDModel()
        {

        }
        public long KeyID { get; set; }
    }
}