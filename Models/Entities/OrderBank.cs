using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderBank
{
    public string BankCode { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }
}
