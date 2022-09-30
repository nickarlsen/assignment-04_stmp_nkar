namespace Assignment.Infrastructure.Tests;

public class WorkItemRepositoryTests
{

private readonly KanbanContext _context;
    private readonly WorkItemRepository _repository;

    public WorkItemRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Items.AddRange(new WorkItem("itemTitle"), new WorkItem("itemTitle2"));
        context.Tags.AddRange(new Tag("tag1"), new Tag("tag2"));
        context.SaveChanges();

        _context = context;
        _repository = new WorkItemRepository(_context);
    }

    [Fact]
    public void Create_WorkItem_returns_response_and_id() 
    {
        //Arrange
        var listOfTagIds = new[]{"1", "2"};

        //Act
        var (response, created) = _repository.Create(new Core.WorkItemCreateDTO("itemTitle3", null, null, listOfTagIds));

        //Assert
        response.Should().Be(Response.Created);
        created.Should().Be(new WorkItemDTO(3, "itemTitle3", null, listOfTagIds, State.New).Id);
    }


    [Fact]
    public void Delete_WorkItem_returns_response() 
    {
        //Arrange

        //Act
        var response = _repository.Delete(2);

        //Assert
        response.Should().Be(Response.Deleted);
        
    }

    [Fact]
    public void Read_given_WorkItemId_returns_WorkItemDTO() 
    {
        //Arrange
        //Happens in database creation

        //Act
        var response = _repository.Find(2);
        var tagList = new List<string>();

        //Assert
        response.Should().BeEquivalentTo(new WorkItemDetailsDTO(2, "itemTitle2",null, new DateTime(), null, tagList, State.New, new DateTime()));

    }

    [Fact]
    public void Update_given_WorkItemUpdateDTO_returns_Response() 
    {
        //Arrange
        //Happens in database creation
        var tagList = new List<string>();

        //Act
        var response = _repository.Update(new WorkItemUpdateDTO(2, "nytNavn", null, null, tagList, State.Active));

        //Assert
        response.Should().Be(Core.Response.Updated);

    }

    [Fact]
    public void ReadAll_returns_all_WorkItemDTOs() 
    {
        //Arrange
        //Happens in database creation
        var tagList = new List<string>();

        //Act
        var response = _repository.Read();
        var output = new[] {new WorkItemDTO(1, "itemTitle", null, tagList, State.New), new WorkItemDTO(2, "itemTitle2", null, tagList, State.New)};

        //Assert
        response.Should().BeEquivalentTo(output);
    }

}
