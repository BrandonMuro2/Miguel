using System;
using System.Collections.Generic;

namespace GGS.Engine;

public partial class UserGameRating
{
    public int RatingId { get; set; }

    public int? UserId { get; set; }

    public int? GameId { get; set; }

    public decimal? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Game? Game { get; set; }

    public virtual User? User { get; set; }
}
