using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderPayment
{
    public string PaymentCode { get; set; } = null!;

    public string IssueType { get; set; } = null!;

    public string OrderCode { get; set; } = null!;

    public string ContainerNo { get; set; } = null!;

    public string ContainerSizeCode { get; set; } = null!;

    public string PaymentTypeCode { get; set; } = null!;

    public string TaxId { get; set; } = null!;

    public string TaxName { get; set; } = null!;

    public string TaxAddress { get; set; } = null!;

    public decimal Total { get; set; }

    public decimal Vat { get; set; }

    public decimal NetTotal { get; set; }

    public bool IsTaxInvoice { get; set; }

    public bool IsPaid { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransContainerSize ContainerSizeCodeNavigation { get; set; } = null!;

    public virtual OrderContainer OrderCodeNavigation { get; set; } = null!;

    public virtual OrderPaymentDetail? OrderPaymentDetail { get; set; }

    public virtual ICollection<OrderReceipt> OrderReceipts { get; set; } = new List<OrderReceipt>();

    public virtual OrderPaymentType PaymentTypeCodeNavigation { get; set; } = null!;
}
