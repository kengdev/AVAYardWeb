using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceiptDetail
{
    public int Id { get; set; }

    public string? ContainerNo { get; set; }

    public string? Description { get; set; }

    public string? ContainerSize { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Total { get; set; }

    public string ReceiptCode { get; set; } = null!;

    public virtual OrderReceipt ReceiptCodeNavigation { get; set; } = null!;
}
