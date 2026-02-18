using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderReceiptType
{
    public string ReceiptTypeCode { get; set; } = null!;

    public string ReceiptTypeName { get; set; } = null!;
}
