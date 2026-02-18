using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class AccAccountLine
{
    public string AccUsername { get; set; } = null!;

    public string LineId { get; set; } = null!;

    public bool IsActived { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;
}
