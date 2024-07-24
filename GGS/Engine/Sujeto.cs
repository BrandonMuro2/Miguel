using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class Sujeto
{
    public int SubjectId { get; set; }

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public int? RecordingYear { get; set; }

    public double? NumberOfSubtractions { get; set; }

    public int? CountQuality { get; set; }

    public virtual ICollection<Sesione> Sesiones { get; set; } = new List<Sesione>();
}
