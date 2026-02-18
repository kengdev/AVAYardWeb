using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderContainerMatchdetail
{
    public string OrderCode { get; set; } = null!;

    public DateOnly DetentionDate { get; set; }

    public string MatchType { get; set; } = null!;

    public virtual OrderContainer OrderCodeNavigation { get; set; } = null!;
}
