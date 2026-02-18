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
    }
}
