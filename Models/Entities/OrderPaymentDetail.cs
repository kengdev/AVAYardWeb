using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderPaymentDetail
{
    public string PaymentCode { get; set; } = null!;

    public decimal ServiceCharge { get; set; }

    public decimal RepairCharge { get; set; }

    public int StayDays { get; set; }

    public int Overstay7Days { get; set; }

    public int Overstay10Days { get; set; }

    public decimal Cost7Days { get; set; }

    public decimal Cost10Days { get; set; }

    public virtual OrderPayment PaymentCodeNavigation { get; set; } = null!;
}
