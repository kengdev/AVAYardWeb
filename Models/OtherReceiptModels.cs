using AVAYardWeb.Models.Entities;
namespace AVAYardWeb.Models;
public class ContainerHandlingViewModel
{
    public string doc_type { get; set; }
    public string code { get; set; }
    public string tax_id { get; set; }
    public string tax_name { get; set; }
    public string tax_address { get; set; }
    public string type { get; set; }
    public string session { get; set; }
    public string container_no { get; set; }
    public string container_size { get; set; }
    public decimal total { get; set; }
    public decimal vat { get; set; }
    public decimal net_total { get; set; }
    public string receipt_date { get; set; }
    public int overstay_7_days { get; set; }
    public int overstay_10_days { get; set; }
    public decimal service_charge { get; set; }
    public decimal cost_7_days { get; set; }
    public decimal cost_10_days { get; set; }
    public decimal price { get; set; }
    public string payment_type { get; set; }
    public string bank_name { get; set; }
    public string branch { get; set; }
    public string cheque_no { get; set; }
    public DateOnly? cheque_date { get; set; }
    public DateTime create_time { get; set; }
    public int rate_id { get; set; }
}
