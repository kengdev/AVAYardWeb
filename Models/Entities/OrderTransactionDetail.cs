using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderTransactionDetail
{
    public int Id { get; set; }

    public string CheckedResult { get; set; } = null!;

    public string TransactionCode { get; set; } = null!;

    public virtual OrderTransaction TransactionCodeNavigation { get; set; } = null!;
}
