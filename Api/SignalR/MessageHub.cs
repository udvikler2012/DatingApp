using Api.Dtos;
using Api.Entities;
using Api.Extensions;
using Api.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Api.SignalR;

public class MessageHub(IMessageRepository messageRepository, IMemberRepository memberRepository, IHubContext<PresenceHub> presenceHub) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request?.Query["userId"].ToString()
         ?? throw new HubException("Other user not found");

        var groupName = GetGroupName(GetUserId(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await messageRepository.GetMessageThread(GetUserId(), otherUser!);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await messageRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(GetUserId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
        if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
            throw new HubException("Cannot send message");

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        // Updating Delivered date
        var groupName = GetGroupName(sender.Id, recipient.Id);
        var group = await messageRepository.GetMessageGroup(groupName);
        var userInGroup = group != null && group.Connections.Any(x => x.UserId == message.RecipientId);
        if (userInGroup)
        {
            message.DateRead = DateTime.UtcNow;
        }

        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());

            var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id);
            if (connections != null && connections.Count > 0 && !userInGroup)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToDto());
            }
        }
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, GetUserId());
        if (group == null)
        {
            group = new Group(groupName);
            messageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);
        return await messageRepository.SaveAllAsync();
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        // var group = await unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        // var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        // if (connection != null && group != null)
        // {
        //     unitOfWork.MessageRepository.RemoveConnection(connection);
        //     if (await unitOfWork.Complete()) return group;
        // }

        throw new Exception("Failed to remove from group");
    }



    private static string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
    }
}
