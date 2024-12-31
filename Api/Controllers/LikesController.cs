using Api.Dto;
using Api.Entities;
using Api.Extensions;
using Api.Helpers;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class LikesController(ILikesRepository likesRepository) : BaseApiController
{

    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();
        if (sourceUserId == targetUserId) return BadRequest("You can not like yourself");

        var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);
        if (existingLike == null)
        {
            var like = new UserLike { SourceUserId = sourceUserId, TargetUserId = targetUserId };
            likesRepository.AddLike(like);
        }
        else
        {
            likesRepository.DeleteLike(existingLike);
        }

        if (await likesRepository.SaveChanges()) return Ok();

        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likeParams)
    {
        likeParams.UserId=User.GetUserId();
        var users = await likesRepository.GetUserLikes(likeParams);

        return (Ok(users));
    }

}
