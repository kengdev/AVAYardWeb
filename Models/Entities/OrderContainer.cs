using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderContainer
{
    public string OrderCode { get; set; } = null!;

    public string IssueType { get; set; } = null!;

    public string ContainerStatus { get; set; } = null!;

    public string ContainerNo { get; set; } = null!;

    public string? BookingNo { get; set; }

    public string SealNo { get; set; } = null!;

    public string ContainerSizeCode { get; set; } = null!;

    public string TruckLicense { get; set; } = null!;

    public string TransportationCode { get; set; } = null!;

    public string PaymentStageCode { get; set; } = null!;

    public string AgentCode { get; set; } = null!;

    public int TareWeight { get; set; }

    public DateTime IssueDate { get; set; }

    public string? Remark { get; set; }

    public bool IsExchange { get; set; }

    public bool IsApprove { get; set; }

    public bool IsReceipt { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual TransAgent AgentCodeNavigation { get; set; } = null!;

    public virtual TransContainerSize ContainerSizeCodeNavigation { get; set; } = null!;

    public virtual ICollection<OrderContainerLocation> OrderContainerLocations { get; set; } = new List<OrderContainerLocation>();

    public virtual OrderContainerMatchdetail? OrderContainerMatchdetail { get; set; }

    public virtual OrderContainerRepair? OrderContainerRepair { get; set; }

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();

    public virtual ICollection<OrderReceipt> OrderReceipts { get; set; } = new List<OrderReceipt>();

    public virtual TransTransportation TransportationCodeNavigation { get; set; } = null!;
}
