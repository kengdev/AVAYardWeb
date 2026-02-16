using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class AccAccount
{
    public string AccUsername { get; set; } = null!;

    public string AccPassword { get; set; } = null!;

    public string AccFullname { get; set; } = null!;

    public string? Remark { get; set; }

    public bool IsActived { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;
}
