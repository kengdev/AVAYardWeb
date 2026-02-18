using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderContainerLocation
{
    public string ContainerNo { get; set; } = null!;

    public string ContainerSizeCode { get; set; } = null!;

    public string OrderCode { get; set; } = null!;

    public string LocationStatus { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public virtual TransContainerSize ContainerSizeCodeNavigation { get; set; } = null!;

    public virtual OrderContainer OrderCodeNavigation { get; set; } = null!;
}
