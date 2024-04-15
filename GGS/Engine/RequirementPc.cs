using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class RequirementPc
{
    public int ReqId { get; set; }

    public int? GameId { get; set; }

    public string? ReqType { get; set; }

    public string? ReqCpu { get; set; }

    public string? ReqCard { get; set; }

    public int? ReqRam { get; set; }

    public int? ReqSpace { get; set; }

    public virtual Game? Game { get; set; }
}
