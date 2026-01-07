using Api.Dtos;
using Api.Entities;
using Api.Extensions;
using Api.Helpers;
using Api.Interfaces;

namespace Api.Data;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    // public void AddGroup(Group group)
    // {
    //     context.Groups.Add(group);
    // }

    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    // public async Task<Connection?> GetConnection(string connectionId)
    // {
    //     return await context.Connections.FindAsync(connectionId);
    // }

    // public async Task<Group?> GetGroupForConnection(string connectionId)
    // {
    //     return await context.Groups
    //     .Include(x=>x.Connections)
    //     .Where(x=>x.Connections.Any(c=>c.ConnectionId==connectionId))
    //     .FirstOrDefaultAsync();
    // }

    public async Task<Message?> GetMessage(string messageId)
    {
        return await context.Messages.FindAsync(messageId);
    }

    // public async Task<Group?> GetMessageGroup(string groupName)
    // {
    //     return await context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
    // }

    public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.RecipientId == messageParams.MemberId),
            "Outbox" => query.Where(x => x.SenderId == messageParams.MemberId),
            _ => query.Where(x => x.RecipientId == messageParams.MemberId)
        };
        var messageQuery = query.Select(MessageExtensions.ToDtoProjection());

        return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
    {
        // var query = context.Messages
        // .Where(x =>
        //         (x.RecipientUsername == currentMemberId
        //             && x.RecipientDeleted == false
        //             && x.SenderUsername == recipientId)
        //             ||
        //         (x.SenderUsername == currentMemberId
        //             && x.SenderDeleted == false
        //             && x.RecipientUsername == recipientId)
        // )
        // .OrderBy(x => x.MessageSent)
        // .AsQueryable();

        // var unreadMessages = query.Where(x => x.DateRead == null && x.RecipientUsername == currentMemberId).ToList();
        // if (unreadMessages.Count != 0)
        // {
        //     unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
    
        // }

        // return await query.ProjectTo<MessageDto>(mapper.ConfigurationProvider).ToListAsync();
         throw new NotImplementedException();
    }

    // public void RemoveConnection(Connection connection)
    // {
    //     context.Connections.Remove(connection);
    // }

    public async Task<bool> SaveAllAsync()
    {
       return await context.SaveChangesAsync()>0;
    }
}
