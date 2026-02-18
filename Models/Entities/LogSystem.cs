using System;
using System.Collections.Generic;

namespace AVAYardWeb.Models.Entities;

public partial class LogSystem
{
    public int Id { get; set; }

    public string LogReferenceCode { get; set; } = null!;

    public string LogAction { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public string CreateBy { get; set; } = null!;
}
