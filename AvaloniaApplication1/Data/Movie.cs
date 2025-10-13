using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Data;

public partial class Movie
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Genre { get; set; }

    public string? Director { get; set; }
}
