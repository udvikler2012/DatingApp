using Api.Data;
using Api.Dtos;
using Api.Entities;
using Api.Extensions;
using Api.Helpers;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class MessagesController(IMessageRepository messageRepository, MemberRepository memberRepository) : BaseApiController
{

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(User.GetMemberId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
        if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
            return BadRequest("Cannopt send message");

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };
        messageRepository.AddMessage(message);
        if (await messageRepository.SaveAllAsync()) return Ok(message.ToDto());


        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer([FromQuery] MessageParams messageParams)
    {
        messageParams.MemberId = User.GetMemberId();
        return await messageRepository.GetMessagesForMember(messageParams);
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        // var currentUsername = User.GetUsername();

        // return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
        return BadRequest("Failed to send message");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        // var username = User.GetUsername();

        // var message = await unitOfWork.MessageRepository.GetMessage(id);
        // if (message == null) return BadRequest("Cannot delete this message");

        // if (message.SenderUsername != username && message.RecipientUsername != username) return Forbid();

        // if (message.SenderUsername == username) message.SenderDeleted = true;
        // if (message.RecipientUsername == username) message.RecipientDeleted = true;
        // if (message is { SenderDeleted: true, RecipientDeleted: true })
        // {
        //     unitOfWork.MessageRepository.DeleteMessage(message);
        // }

        // if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting message");
    }
}
