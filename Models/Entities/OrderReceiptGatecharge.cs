using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceiptGatecharge
{
    public string ReceiptCode { get; set; } = null!;

    public decimal GateCharge { get; set; }

    public virtual OrderReceipt ReceiptCodeNavigation { get; set; } = null!;
}
