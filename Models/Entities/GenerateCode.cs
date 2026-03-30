using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class GenerateCode
{
    public string GenCode { get; set; } = null!;

    public string GenType { get; set; } = null!;

    public DateTime CreateDate { get; set; }
}
