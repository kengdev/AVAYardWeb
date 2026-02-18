using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TaxAddress
{
    public string TaxId { get; set; } = null!;

    public string Acronym { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string BranchType { get; set; } = null!;

    public string? BranchName { get; set; }

    public bool IsActived { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual ICollection<TaxTransportation> TaxTransportations { get; set; } = new List<TaxTransportation>();
}
