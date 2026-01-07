using Api.Dtos;
using Api.Entities;
using Api.Helpers;

namespace Api.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessage(string messageId);
    Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams);
    Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId);
    Task<bool> SaveAllAsync();
    // void AddGroup(Group group);
    // void RemoveConnection(Connection connection);
    // Task<Connection?> GetConnection(string connectionId);
    // Task<Group?> GetMessageGroup(string groupName);
    // Task<Group?> GetGroupForConnection(string connectionId);
}

