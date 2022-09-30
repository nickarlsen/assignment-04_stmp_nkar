namespace Assignment.Infrastructure;

public class UserRepository
{

    private readonly KanbanContext _context;

    public UserRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        Response response;

        if (entity is null)
        {
            entity = new User(user.Name, user.Email);

            _context.Users.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);

    }

    public IReadOnlyCollection<UserDTO> ReadAll()
    {
        return _context.Users.Select(u => new UserDTO(u.Id, u.Name, u.Email)).ToList();
    }

    public UserDTO Read(int userId)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (entity is null)
        {
            return null;
        }

        return new UserDTO(entity.Id, entity.Name, entity.Email);
    }

    public Response Update(UserUpdateDTO user)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Id == user.Id);

        if (entity is null)
        {
            return Response.NotFound;
        }

        entity.Name = user.Name;
        entity.Email = user.Email;

        _context.SaveChanges();

        return Response.Updated;
    }

    public Response Delete(int userId, bool force = false)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Id == userId);


        if (entity is null)
        {
            return Response.NotFound;
        }

        if (force && entity.Items.Any())
        {
            _context.Users.Remove(entity);
        }
        else if (!force && entity.Items.Any())
        {
            return Response.Conflict;
        }

        _context.SaveChanges();

        return Response.Deleted;
    }
}
