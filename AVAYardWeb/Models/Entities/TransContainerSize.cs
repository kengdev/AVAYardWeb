using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class TransContainerSize
{
    public string ContainerSizeCode { get; set; } = null!;

    public string ContainerSizeName { get; set; } = null!;

    public string ContainerTypeCode { get; set; } = null!;

    public bool IsActived { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransContainerType ContainerTypeCodeNavigation { get; set; } = null!;

    public virtual ICollection<OrderContainerLocation> OrderContainerLocations { get; set; } = new List<OrderContainerLocation>();

    public virtual ICollection<OrderContainer> OrderContainers { get; set; } = new List<OrderContainer>();

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual ICollection<OrderReceipt> OrderReceipts { get; set; } = new List<OrderReceipt>();
}
