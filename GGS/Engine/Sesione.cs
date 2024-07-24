using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class Sesione
{
    public int SessionId { get; set; }

    public int? SubjectId { get; set; }

    public string? SessionType { get; set; }

    public DateTime? RecordingDate { get; set; }

    public virtual ICollection<MuestrasDeSeñale> MuestrasDeSeñales { get; set; } = new List<MuestrasDeSeñale>();

    public virtual Sujeto? Subject { get; set; }

    public virtual ICollection<Canale> Channels { get; set; } = new List<Canale>();
}
