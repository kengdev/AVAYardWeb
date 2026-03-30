using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TransContainerType
{
    public string ContainerTypeCode { get; set; } = null!;

    public string ContainerTypeName { get; set; } = null!;

    public virtual ICollection<RateDropService> RateDropServices { get; set; } = new List<RateDropService>();

    public virtual ICollection<RateMatchService> RateMatchServices { get; set; } = new List<RateMatchService>();

    public virtual ICollection<TransContainerSize> TransContainerSizes { get; set; } = new List<TransContainerSize>();
}
