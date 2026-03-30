using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceiptVoucher
{
    public string ReceiptCode { get; set; } = null!;

    public string IssueSession { get; set; } = null!;

    public string ContainerNo { get; set; } = null!;

    public string ContainerSizeCode { get; set; } = null!;

    public decimal NetTotal { get; set; }

    public DateOnly VoucherDate { get; set; }

    public string PaymentType { get; set; } = null!;

    public string? BankName { get; set; }

    public string? BankBranchName { get; set; }

    public string? ChequeNo { get; set; }

    public DateOnly? ChequeDate { get; set; }

    public string OrderCode { get; set; } = null!;

    public string PaymentCode { get; set; } = null!;

    public string? Remark { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransContainerSize ContainerSizeCodeNavigation { get; set; } = null!;

    public virtual OrderContainer OrderCodeNavigation { get; set; } = null!;

    public virtual OrderPaymentVoucher PaymentCodeNavigation { get; set; } = null!;
}
