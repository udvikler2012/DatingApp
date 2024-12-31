namespace Api.Entities;

public class UserLike
{
    public int SourceUserId { get; set; }
    public int TargetUserId { get; set; }

    public AppUser SourceUser { get; set; } = null!;
    public AppUser TargetUser { get; set; } = null!;

}
