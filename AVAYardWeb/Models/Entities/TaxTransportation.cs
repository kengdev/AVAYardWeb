using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TaxTransportation
{
    public string TaxId { get; set; } = null!;

    public string TrasportationCode { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TaxAddress Tax { get; set; } = null!;

    public virtual TransTransportation TrasportationCodeNavigation { get; set; } = null!;
}
