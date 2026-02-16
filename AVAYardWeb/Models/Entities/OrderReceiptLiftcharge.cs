using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceiptLiftcharge
{
    public string ReceiptCode { get; set; } = null!;

    public decimal LiftCharge { get; set; }

    public virtual OrderReceipt ReceiptCodeNavigation { get; set; } = null!;
}
