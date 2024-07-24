using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class Canale
{
    public int ChannelId { get; set; }

    public string? ChannelName { get; set; }

    public double? SampleFrequency { get; set; }

    public virtual ICollection<MuestrasDeSeñale> MuestrasDeSeñales { get; set; } = new List<MuestrasDeSeñale>();

    public virtual ICollection<Sesione> Sessions { get; set; } = new List<Sesione>();
}
