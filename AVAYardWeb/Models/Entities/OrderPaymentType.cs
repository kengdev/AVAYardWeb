using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class OrderPaymentType
{
    public string PaymentTypeCode { get; set; } = null!;

    public string PaymentTypeName { get; set; } = null!;

    public virtual ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
}
