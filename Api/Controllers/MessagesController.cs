using Api.Dto;
using Api.Entities;
using Api.Extensions;
using Api.Helpers;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper) : BaseApiController
{

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUserName.ToLower()) return BadRequest("Cannot sent message to yourself");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUserName);
        if (recipient == null || sender == null) return BadRequest("Cannopt send message");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        messageRepository.AddMessage(message);
        if (await messageRepository.SaveAllAsync())
        {
            return Ok(mapper.Map<MessageDto>(message));
        }

        return BadRequest("Failed to add message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username){
        var currentUsername=User.GetUsername();

        return Ok(await messageRepository.GetMessageThread(currentUsername,username));
    }

}
