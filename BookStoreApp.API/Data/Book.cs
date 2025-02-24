using System;
using System.Collections.Generic;
using Humanizer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Data;

public partial class Book
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public int? Year { get; set; }

    public string Isbn { get; set; } = null!;

    public string? Summary { get; set; }

    public string? Image { get; set; }

    public decimal? Price { get; set; }

    public int? AuthorId { get; set; }

    //Author is a navigation property. It represents a relationship to another table(Authors).
    //By default, EF Core does NOT automatically load navigation properties unless you explicitly tell it to do so.
    // Because this just defines the relationship in C#, but EF Core decides when to actually load the data.
    // Use .Include(q => q.Author) to tell EF Core to fetch the related data.
    public virtual Author? Author { get; set; }
}
