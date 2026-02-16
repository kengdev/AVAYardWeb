using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderTransaction
{
    public string TransactionCode { get; set; } = null!;

    public string OrderCode { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public string TransactionStatus { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;

    public virtual ICollection<OrderTransactionDetail> OrderTransactionDetails { get; set; } = new List<OrderTransactionDetail>();

    public virtual ICollection<OrderTransactionFile> OrderTransactionFiles { get; set; } = new List<OrderTransactionFile>();
}
