using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderPaymentStage
{
    public string PaymentStageCode { get; set; } = null!;

    public string PaymentStageName { get; set; } = null!;
}
