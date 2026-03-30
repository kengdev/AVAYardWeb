using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TransTransportation
{
    public string TransportationCode { get; set; } = null!;

    public string TransportationName { get; set; } = null!;

    public string TransportationAcronym { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Remark { get; set; }

    public bool IsActived { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual ICollection<OrderContainer> OrderContainers { get; set; } = new List<OrderContainer>();

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual ICollection<TaxTransportation> TaxTransportations { get; set; } = new List<TaxTransportation>();
}
