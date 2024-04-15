using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class RequirementConsole
{
    public int ConId { get; set; }

    public int? GameId { get; set; }

    public string? ConName { get; set; }

    public int? ConSpace { get; set; }

    public virtual Game? Game { get; set; }
}
