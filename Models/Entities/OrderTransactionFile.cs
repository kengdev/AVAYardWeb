using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderTransactionFile
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string TransactionCode { get; set; } = null!;

    public virtual OrderTransaction TransactionCodeNavigation { get; set; } = null!;
}
