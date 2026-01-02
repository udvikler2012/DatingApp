using Api.Entities;

namespace Api.Interfaces;

public interface IMemberRepository
{
    void Update(Member user);
    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<Member>> GetMembersAsync();
    Task<Member?> GetMemberByIdAsync(string id);
    Task<Member?> GetMemberForUpdate(string id);

    Task<IReadOnlyList<Photo>> GetPhotoForMemberAsync(string memberId);
}
