using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class Catalog
{
    public int CatId { get; set; }

    public string? CatName { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
