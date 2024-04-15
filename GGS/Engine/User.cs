using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class User
{
    public int UsId { get; set; }

    public string? UsName { get; set; }

    public virtual ICollection<List> Lists { get; set; } = new List<List>();

    public virtual ICollection<UserGameRating> UserGameRatings { get; set; } = new List<UserGameRating>();
}
