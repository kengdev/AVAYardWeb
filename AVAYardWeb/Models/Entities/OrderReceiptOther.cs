using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceiptOther
{
    public int Id { get; set; }

    public string CostDetail { get; set; } = null!;

    public decimal CostValue { get; set; }

    public string ReceiptCode { get; set; } = null!;

    public virtual OrderReceipt ReceiptCodeNavigation { get; set; } = null!;
}
