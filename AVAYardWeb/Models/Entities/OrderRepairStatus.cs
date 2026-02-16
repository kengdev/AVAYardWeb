using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderRepairStatus
{
    public string RepairStatusCode { get; set; } = null!;

    public string RepairStatusName { get; set; } = null!;

    public virtual ICollection<OrderContainerRepair> OrderContainerRepairs { get; set; } = new List<OrderContainerRepair>();
}
