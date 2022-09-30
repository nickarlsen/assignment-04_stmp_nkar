namespace Assignment.Core;

public interface ITagRepository
{
    (Response Response, int WorkItemId) Create(TagCreateDTO tag);
    IReadOnlyCollection<TagDTO> Read();
    TagDTO Find(int WorkItemId);
    Response Update(TagUpdateDTO tag);
    Response Delete(int WorkItemId, bool force = false);
}