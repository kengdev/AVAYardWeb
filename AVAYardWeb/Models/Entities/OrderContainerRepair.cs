using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderContainerRepair
{
    public string OrderCode { get; set; } = null!;

    public string ContainerNo { get; set; } = null!;

    public string RepairStatusCode { get; set; } = null!;

    public virtual OrderContainer OrderCodeNavigation { get; set; } = null!;

    public virtual OrderRepairStatus RepairStatusCodeNavigation { get; set; } = null!;
}
