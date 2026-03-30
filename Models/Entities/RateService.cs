using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class RateService
{
    public int Id { get; set; }

    public string AgentCode { get; set; } = null!;

    public string ContainerTypeCode { get; set; } = null!;

    public string ContainerLoadType { get; set; } = null!;

    public decimal RateServiceCharge { get; set; }

    public decimal Limit7Days { get; set; }

    public decimal Limit10Days { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransAgent AgentCodeNavigation { get; set; } = null!;

    public virtual TransContainerType ContainerTypeCodeNavigation { get; set; } = null!;
}
