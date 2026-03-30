using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TaxTotalCost
{
    public int Id { get; set; }

    public string TaxId { get; set; } = null!;

    public decimal SizeShortCost { get; set; }

    public decimal SizeLongCost { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public virtual TaxAddress Tax { get; set; } = null!;
}
