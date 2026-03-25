using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderPaymentVoucher
{
    public string PaymentCode { get; set; } = null!;

    public string IssueType { get; set; } = null!;

    public string OrderCode { get; set; } = null!;

    public string ContainerNo { get; set; } = null!;

    public string ContainerSizeCode { get; set; } = null!;

    public string TruckLicense { get; set; } = null!;

    public string TransportationName { get; set; } = null!;

    public string PaymentTypeCode { get; set; } = null!;

    public string BankCode { get; set; } = null!;

    public decimal NetTotal { get; set; }

    public bool IsPaid { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransContainerSize ContainerSizeCodeNavigation { get; set; } = null!;

    public virtual OrderContainer OrderCodeNavigation { get; set; } = null!;

    public virtual OrderPaymentVoucherDetail? OrderPaymentVoucherDetail { get; set; }

    public virtual ICollection<OrderReceiptVoucher> OrderReceiptVouchers { get; set; } = new List<OrderReceiptVoucher>();
}
