using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderContainerStatus
{
    public string ContainerStatusCode { get; set; } = null!;

    public string ContainerStatusName { get; set; } = null!;
}
