using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceipt
{
    public string ReceiptCode { get; set; } = null!;

    public string IssueSession { get; set; } = null!;

    public string ContainerNo { get; set; } = null!;

    public string ContainerSizeCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string TaxId { get; set; } = null!;

    public string TaxName { get; set; } = null!;

    public string TaxAddress { get; set; } = null!;

    public decimal Total { get; set; }

    public decimal Vat { get; set; }

    public decimal NetTotal { get; set; }

    public DateOnly ReceiptDate { get; set; }

    public string PaymentType { get; set; } = null!;

    public string? BankName { get; set; }

    public string? BankBranchName { get; set; }

    public string? ChequeNo { get; set; }

    public DateOnly? ChequeDate { get; set; }

    public string OrderCode { get; set; } = null!;

    public string PaymentCode { get; set; } = null!;

    public string? Remark { get; set; }

    public bool IsTaxInvoice { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransContainerSize ContainerSizeCodeNavigation { get; set; } = null!;

    public virtual OrderContainer OrderCodeNavigation { get; set; } = null!;

    public virtual OrderPayment PaymentCodeNavigation { get; set; } = null!;
}
