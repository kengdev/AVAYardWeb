using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class RateDropService
{
    public int Id { get; set; }

    public string ContainerTypeCode { get; set; } = null!;

    public decimal RateServiceCharge { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransContainerType ContainerTypeCodeNavigation { get; set; } = null!;
}
