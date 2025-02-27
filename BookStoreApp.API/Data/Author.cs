using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace BookStoreApp.API.Data;

public partial class Author
{

    //In older.NET versions, EF Core scaffolding used the constructor approach. Så är det i kursen:
    //public Author()
    //{
    //    Books = new HashSet<Book>();
    //}

    public int Id { get; set; } // primary key

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Bio { get; set; }

    // Skillnad mot kursen. This directly initializes Books at the property level, instead of using a constructor.
    // The = new List<Book>(); ensures that Books is never null when a new Author object is created.
    // It makes the constructor unnecessary because the property is initialized immediately.
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
