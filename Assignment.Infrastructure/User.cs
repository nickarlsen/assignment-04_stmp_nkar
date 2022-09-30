namespace Assignment.Infrastructure;

public class User
{
    public int Id { get; set; }
    [StringLength(100)]
    [Required]
    public string Name { get; set; }
    [StringLength(100)]
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public ICollection<WorkItem> Items { get; set; }

    public User(string name, string email)
    {
        Name = name;
        Email = email;
        Items = new HashSet<WorkItem>();
    }
}
