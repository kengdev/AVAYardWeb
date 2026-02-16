using AVAYardWeb.Models.Entities;

namespace AVAYardWeb.Models
{
    public class OrderContainerModel
    {
        public string order_code { get; set; }
        public string detention_date { get; set; }
        public string agent_name { get; set; }
        public string container_no { get; set; }
        public string seal_no { get; set; }
        public string container_size_code { get; set; }
        public string container_size { get; set; }
        public string container_type { get; set; }
        public string truck_license { get; set; }
        public string trailer_license { get; set; }
        public string transportation_name { get; set; }
        public string consignee_code { get; set; }
        public string consignee_name { get; set; }
        public string trans_tax_no { get; set; }
        public string trans_tax_address { get; set; }
        public string cus_tax_no { get; set; }
        public string cus_tax_address { get; set; }
        public string match_type { get; set; }
        public int aging { get; set; }
        public int days_ago { get; set; }
        public int overstay { get; set; }
        public int overstay_short { get; set; }
        public int overstay_long { get; set; }
        public string container_status { get; set; }
        public string issue_type { get; set; }
        public DateTime issue_date { get; set; }
        public string issue_date_str { get; set; }
        public string transaction_code { get; set; }
        public int gross_weight { get; set; }
        public int tare_weight { get; set; }
        public decimal service_rate { get; set; }
        public decimal gate_charge { get; set; }
        public decimal lift_charge { get; set; }
        public decimal empty_container_rate { get; set; }
        public decimal loader_container_rate { get; set; }
        public decimal limit_7_days { get; set; }
        public decimal limit_10_days { get; set; }

        public decimal total_7_charge { get; set; }
        public decimal total_10_charge { get; set; }
        public string remark { get; set; }
        public bool is_exchange { get; set; }
        public bool is_receipt { get; set; }
        public string payment_stage { get; set; }
        public string repair_stage { get; set; }
        public List<OrderTransactionDetail> check_result { get; set; }
        public List<OrderTransactionFile> file_result { get; set; }
    }

    public class ContainerReceipt
    {
        public string issue_type { get; set; }
        public string order_code { get; set; }
        public string container_no { get; set; }
        public string customer_code { get; set; }
        public string customer_name { get; set; }
        public string tax_id { get; set; }
        public string address { get; set; }
        public decimal service_charge { get; set; }
        public int stay_days { get; set; }
        public int overstay_7_days { get; set; }
        public int overstay_10_days { get; set; }
        public decimal cost_7_days { get; set; }
        public decimal cost_10_days { get; set; }

        public decimal gate_charge { get; set; }
        public decimal lift_charge { get; set; }

        public string payment_type { get; set; }
        public string bank_name { get; set; }
        public string bank_branch_name { get;set; }
        public string cheque_no { get; set; }
        public DateTime cheque_date { get; set; }   
        public string create_by { get; set; }
        public string remark { get; set; }
        public List<OtherContainerCost> other_cost { get; set; }
    }

    public class OtherContainerCost
    {
        public string cost_detail { get; set; }
        public decimal cost_value { get; set; }
    }
}
