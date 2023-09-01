using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookDb>(opt => opt.UseInMemoryDatabase("BookDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

// app.MapGet (Get command)
app.MapGet("/", async (BookDb db) =>
    await db.Books.ToListAsync());

// app.MapGet(id) (Get command)
app.MapGet("/{id}", async (int id, BookDb db) =>
    await db.Books.FindAsync(id)
        is Book book
            ? Results.Ok(book)
            : Results.NotFound());

// app.MapPut(id) (Put command)
app.MapPut("/{id}", async (int id, Book inputBook, BookDb db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null) return Results.NotFound();

    book.Name = inputBook.Name;
    book.Id = inputBook.Id;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

// app.MapDelete(id) (Delete command)
app.MapDelete("/{id}", async (int id, BookDb db) =>
{
    if (await db.Books.FindAsync(id) is Book book)
    {
        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NoContent();
});

// app.MapPost (Post command, for adding data)
app.MapPost("/", async (Book book, BookDb db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();

    return Results.Created($"/{book.Id}", book);
});

app.Run();

public class Book
{
    public int Id { set; get; }
    public String Name { set; get; }
}

class BookDb : DbContext
{
    public BookDb(DbContextOptions<BookDb> options)
        : base(options) { }

    public DbSet<Book> Books => Set<Book>();
}