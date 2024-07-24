using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class MuestrasDeSeñale
{
    public int SampleId { get; set; }

    public int? SessionId { get; set; }

    public int? ChannelId { get; set; }

    public int? TimeIndex { get; set; }

    public double? Amplitude { get; set; }

    public virtual Canale? Channel { get; set; }

    public virtual Sesione? Session { get; set; }
}
