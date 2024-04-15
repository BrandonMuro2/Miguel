using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class List
{
    public int ListId { get; set; }

    public int? UsId { get; set; }

    public string? ListName { get; set; }

    public string? ListDesc { get; set; }

    public DateTime? ListCreatedAt { get; set; }

    public virtual User? Us { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
