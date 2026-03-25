namespace AVAYardWeb.Models
{
    public class PaymentViewModel
    {
        public decimal Amount { get; set; }

        // Base64 ของรูป QR
        public string QrBase64 { get; set; }

        // เผื่อใช้ต่อ
        public string PromptPayPhone { get; set; }

        // เลขงาน/เลขบิลของลานตู้
        public string RefNo { get; set; }
        public string Payload { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSize { get; set; }
        public bool IsTaxInvoice { get; set; }
        public bool IsVat { get; set; }
        public string TaxId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string BankCode { get; set; }
    }
}
