namespace Assignment.Infrastructure;

public class WorkItemRepository : Assignment.Core.IWorkItemRepository
{

    private readonly KanbanContext _context;

    public WorkItemRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int ItemId) Create(WorkItemCreateDTO item)
    {
        var entity = _context.Items.FirstOrDefault(i => i.Title == item.Title);
        Response response;
        
        if (entity is null)
        {
            entity = new WorkItem(item.Title);

            _context.Items.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else 
        {
            response =  Response.Conflict;
        }

        return (response, entity.Id);
    }

    public Response Delete(int itemId)
    {
        var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
        Response response;

        if (item is null) 
        {
            response = Response.NotFound;
        }
        else if (item.State == State.Active)
        {
            item.State = State.Removed;
            _context.SaveChanges();
            response = Response.Deleted; //Updated?
        }
        else if (item.State == State.New)
        {
            _context.Items.Remove(item);
            _context.SaveChanges();

            response = Response.Deleted;
            
        }
        else //Only land here if State is one of the conflicting types
        {
            response = Response.Conflict;
        }

        return response;
    }

    public WorkItemDetailsDTO Find(int itemId)
    {
        var item = from i in _context.Items
                    where i.Id == itemId
                    select new WorkItemDetailsDTO(i.Id, i.Title, null, i.Created, i.AssignedTo!.Name, i.Tags.Select(i => i.Name).ToList(), i.State, i.Updated);
       
        if (item is null) return null;
        else return item.FirstOrDefault(); //This will never be null here
    }

    public IReadOnlyCollection<WorkItemDTO> Read()
    {
        var item = from t in _context.Items
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return item.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByState(State state)
    {
        var item = from t in _context.Items
                    where t.State == state
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return item.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByTag(string tag)
    {
        var specificTag = _context.Tags.Find(tag);
        var item = from t in _context.Items
                    where t.Tags == specificTag
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return item.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadByUser(int userId)
    {
        var item = from t in _context.Items
                    where t.AssignedTo!.Id == userId
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return item.ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadRemoved()
    {
        var item = from t in _context.Items
                    where t.State == State.Removed
                    select new WorkItemDTO(t.Id, t.Title, t.AssignedTo!.Name, t.Tags.Select(t => t.Name).ToList(), t.State);

        return item.ToArray();
    }

    public Response Update(WorkItemUpdateDTO item)
    {
        var entity = _context.Items.Find(item.Id);
        Response response;

        if (entity is null) 
        {
            response = Response.NotFound;
        }
        else if (_context.Items.FirstOrDefault(t => t.Id != item.Id && t.Title == item.Title) != null)
        {
            response = Response.Conflict;
        }
        else 
        {
            entity.Title = item.Title;
            _context.SaveChanges();
            response = Response.Updated;
        }
        return response;
    }
}
