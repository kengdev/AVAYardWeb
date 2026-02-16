using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceiptOverstay
{
    public string ReceiptCode { get; set; } = null!;

    public decimal ServiceCharge { get; set; }

    public int StayDays { get; set; }

    public int Overstay7Days { get; set; }

    public int Overstay10Days { get; set; }

    public decimal Cost7Days { get; set; }

    public decimal Cost10Days { get; set; }

    public virtual OrderReceipt ReceiptCodeNavigation { get; set; } = null!;
}
