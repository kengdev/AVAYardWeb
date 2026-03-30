using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class CusConsignee
{
    public string ConsigneeCode { get; set; } = null!;

    public string ConsigneeName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string TaxId { get; set; } = null!;

    public bool IsActived { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual ICollection<OrderContainer> OrderContainers { get; set; } = new List<OrderContainer>();
}
