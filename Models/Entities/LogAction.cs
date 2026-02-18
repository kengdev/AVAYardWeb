using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class LogAction
{
    public long Id { get; set; }

    public string Action { get; set; } = null!;

    public string? Module { get; set; }

    public string? RefCode { get; set; }

    public string? BeforeData { get; set; }

    public string? AfterData { get; set; }

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;
}
