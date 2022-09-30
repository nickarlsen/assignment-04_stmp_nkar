namespace Assignment.Infrastructure.Tests;

public class UserRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly UserRepository _repository;
    public UserRepositoryTests()
    {

        var connection = new SqliteConnection("filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.Users.AddRange(new User("Test", "test@test.dk"),
                               new User("Test2","test2@test.dk" ));
        context.SaveChanges();


        _context = context;
        _repository = new UserRepository(_context);

    }

    [Fact]
    public void Create_given_existing_Tag_returns_Conflict()
    {
        var (response, id) = _repository.Create(new UserCreateDTO("Test", "test@test.dk"));
        Assert.Equal(Response.Conflict, response);

    }

    [Fact]
    public void Find_given_non_existing_id_returns_null()
    {
        var user = _repository.Read(42);
        Assert.Null(user);
    }

    [Fact]
    public void Read_given_existing_id_returns_User()
    {
        var user = _repository.Read(1);
        Assert.Equal("Test", user.Name);
    }

    [Fact]
    public void ReadAll_given_existing_id_returns_User()
    {
        var users = _repository.ReadAll();
        Assert.Equal(2, users.Count());
    }

    [Fact]
    public void Update_given_existing_User_returns_Updated()
    {
        var user = _repository.Read(1);
        var response = _repository.Update(new UserUpdateDTO(user.Id, "Test1", user.Email));
        Assert.Equal(Response.Updated, response);
    }

    [Fact]
    public void Update_given_non_existing_User_returns_NotFound()
    {
        var user = _repository.Read(1);
        var response = _repository.Update(new UserUpdateDTO(42, "Test2", user.Email));
        Assert.Equal(Response.NotFound, response);
    }

    [Fact]
    public void Delete_given_existing_User_returns_Deleted()
    {
        var response = _repository.Delete(1);
        Assert.Equal(Response.Deleted, response);
    }

    [Fact]
    public void Delete_given_non_existing_User_returns_NotFound()
    {
        var response = _repository.Delete(42);
        Assert.Equal(Response.NotFound, response);
    }

}
