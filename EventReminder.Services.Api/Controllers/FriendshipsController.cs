using EventReminder.Application.Abstractions.Authentication;
using EventReminder.Application.Friendships.GetFriendship;
using EventReminder.Application.Friendships.GetFriendshipsForUserId;
using EventReminder.Application.Friendships.RemoveFriendship;
using EventReminder.Contracts.Common;
using EventReminder.Contracts.Friendships;
using EventReminder.Domain.Core.Primitives.Maybe;
using EventReminder.Domain.Core.Primitives.Result;
using EventReminder.Services.Api.Contracts;
using EventReminder.Services.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventReminder.Services.Api.Controllers
{
    public sealed class FriendshipsController : ApiController
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;
        public FriendshipsController(IMediator mediator, IUserIdentifierProvider userIdentifierProvider)
            : base(mediator)
        {
            _userIdentifierProvider = userIdentifierProvider;
        }

        [HttpGet(ApiRoutes.Friendships.GetMyFriendShips)]
        [ProducesResponseType(typeof(PagedList<FriendshipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyFriendShips(int page, int pageSize) =>
            await Maybe<GetFriendshipsForUserIdQuery>
                .From(new GetFriendshipsForUserIdQuery(_userIdentifierProvider.UserId, page, pageSize))
                .Bind(query => Mediator.Send(query))
                .Match(Ok, NotFound);

        [HttpGet(ApiRoutes.Friendships.Get)]
        [ProducesResponseType(typeof(FriendshipResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid userId, Guid friendId) =>
            await Maybe<GetFriendshipQuery>
                .From(new GetFriendshipQuery(userId, friendId))
                .Bind(query => Mediator.Send(query))
                .Match(Ok, NotFound);

        [HttpDelete(ApiRoutes.Friendships.Base)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove(Guid friendId) =>
            await Result.Success(new RemoveFriendshipCommand(_userIdentifierProvider.UserId, friendId))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);
    }
}
