namespace Assignment.Infrastructure.Tests;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class TagRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Tags.AddRange(new Tag("High") { Id = 1 }, new Tag("Low") { Id = 2 });
        context.Items.Add(new WorkItem("Make")
        {
            Id = 1,
            Created = DateTime.Now,
            Updated = DateTime.Now,
            State = State.New,
            Tags = context.Tags.ToArray()
            
        }); 
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);

    }

    [Fact]
    public void Create_given_Tag_returns_Created_with_Tag()
    {

        var (response, tagId) = _repository.Create(new TagCreateDTO("Medium"));


        Assert.Equal(Response.Created, response);
        Assert.Equal(3, tagId);
    }

    [Fact]
    public void Create_given_existing_Tag_returns_Conflict()
    {
        var (response, tagId) = _repository.Create(new TagCreateDTO("High"));
        Assert.Equal(Response.Conflict, response);

    }

    [Fact]
    public void Find_given_non_existing_id_returns_null()
    {
        var tag = _repository.Read(42);
        Assert.Null(tag);
    }

    [Fact]
    public void Read_given_existing_id_returns_Tag()
    {
        var tag = _repository.Read(1);
        Assert.Equal("High", tag.Name);
    }

    [Fact]
    public void ReadAll_given_existing_id_returns_Tag()
    {
        var tags = _repository.ReadAll();
        Assert.Equal(2, tags.Count());
    }
}
