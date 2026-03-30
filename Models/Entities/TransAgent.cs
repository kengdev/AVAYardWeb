using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TransAgent
{
    public string AgentCode { get; set; } = null!;

    public string AgentName { get; set; } = null!;

    public bool IsActived { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual ICollection<OrderContainer> OrderContainers { get; set; } = new List<OrderContainer>();

    public virtual ICollection<RateMatchService> RateMatchServices { get; set; } = new List<RateMatchService>();
}
