using System.ComponentModel.DataAnnotations;

namespace Api.Entities;

public class Group(string name)
{
    [Key]
    public string Name { get; set; } = name;

    // nav property
    public ICollection<Connection> Connections { get; set; } = [];
}
