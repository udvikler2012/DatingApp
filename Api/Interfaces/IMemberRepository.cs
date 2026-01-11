using Api.Entities;
using Api.Helpers;

namespace Api.Interfaces;

public interface IMemberRepository
{
    void Update(Member user);
    Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
    Task<Member?> GetMemberByIdAsync(string id);
    Task<Member?> GetMemberForUpdate(string id);

    Task<IReadOnlyList<Photo>> GetPhotoForMemberAsync(string memberId);
}
