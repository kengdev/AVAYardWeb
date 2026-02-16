using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TransTransportationTax
{
    public string TransportationCode { get; set; } = null!;

    public string TaxId { get; set; } = null!;

    public string TaxName { get; set; } = null!;

    public string TaxPhone { get; set; } = null!;

    public string TaxAddress { get; set; } = null!;

    public string BranchType { get; set; } = null!;

    public string? BranchName { get; set; }

    public bool IsTaxTransportation { get; set; }

    public virtual TransTransportation TransportationCodeNavigation { get; set; } = null!;
}
