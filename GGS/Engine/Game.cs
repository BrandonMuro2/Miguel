using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class Game
{
    public int GameId { get; set; }

    public string? GameName { get; set; }

    public string? GameImg { get; set; }

    public string? GameDev { get; set; }

    public DateTime? GameReleaseDate { get; set; }

    public string? GameDescription { get; set; }

    public byte? GameRating { get; set; }

    public virtual ICollection<RequirementConsole> RequirementConsoles { get; set; } = new List<RequirementConsole>();

    public virtual ICollection<RequirementPc> RequirementPcs { get; set; } = new List<RequirementPc>();

    public virtual ICollection<UserGameRating> UserGameRatings { get; set; } = new List<UserGameRating>();

    public virtual ICollection<Catalog> Cats { get; set; } = new List<Catalog>();

    public virtual ICollection<List> Lists { get; set; } = new List<List>();
}
