namespace Assignment.Infrastructure;

public class TagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response response, int WorkItemId) Create(TagCreateDTO tag)
    {
        var entity = _context.Tags.FirstOrDefault(t => t.Name == tag.Name);
        Response response;

        if (entity is null)
        {
            entity = new Tag(tag.Name);

            _context.Tags.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);
    }
    public IReadOnlyCollection<TagDTO> ReadAll()
    {
        var tags = from t in _context.Tags
                   orderby t.Name
                   select new TagDTO(t.Id, t.Name);

        return tags.ToArray();
    }
    public TagDTO? Read(int? WorkItemId)
    {
        if (WorkItemId == null)
        {
            return null;
        }
        var tags = from t in _context.Tags
                   where t.Id == WorkItemId
                   select new TagDTO(t.Id, t.Name);

        return tags.FirstOrDefault();

    }
    public Response Update(TagUpdateDTO tag)
    {
        var entity = _context.Tags.Find(tag.Id);
        Response response;

        if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.Tags.FirstOrDefault(t => t.Id != tag.Id && t.Name == tag.Name) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.Name = tag.Name;
            _context.SaveChanges();
            response = Response.Updated;
        }

        return response;

    }
    public Response Delete(int WorkItemId, bool force = false)
    {
        var tag = _context.Tags.Include(t => t.WorkItems).FirstOrDefault(t => t.Id == WorkItemId);
        Response response;
        if (tag == null)
        {
            response = Response.NotFound;
        }
        else if (force && tag.WorkItems.Any())
        {
            _context.Tags.Remove(tag);
            _context.SaveChanges();

            response = Response.Deleted;
            
        }
        else if (!force && tag.WorkItems.Any())
        {
            response = Response.Conflict;
        }
        else
        {
            _context.Tags.Remove(tag);
            _context.SaveChanges();

            response = Response.Deleted;
        }

        return response;
    }
}
