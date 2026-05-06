using EventReminder.Application.Users.ChangePassword;
using EventReminder.Application.Users.GetUserById;
using EventReminder.Application.Users.SendFriendshipRequest;
using EventReminder.Application.Users.UpdateUser;
using EventReminder.Application.Abstractions.Authentication;
using EventReminder.Contracts.Users;
using EventReminder.Domain.Core.Errors;
using EventReminder.Domain.Core.Primitives.Maybe;
using EventReminder.Domain.Core.Primitives.Result;
using EventReminder.Services.Api.Contracts;
using EventReminder.Services.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventReminder.Services.Api.Controllers
{
    public sealed class UsersController : ApiController
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;

        public UsersController(IMediator mediator, IUserIdentifierProvider userIdentifierProvider)
            : base(mediator)
        {
            _userIdentifierProvider = userIdentifierProvider;
        }

        [HttpGet(ApiRoutes.Users.GetById)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById() =>
            await Maybe<GetUserByIdQuery>
            .From(new GetUserByIdQuery(_userIdentifierProvider.UserId))
                .Bind(query => Mediator.Send(query))
                .Match(Ok, NotFound);

        [HttpPut(ApiRoutes.Users.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(UpdateUserRequest updateUserRequest) =>
            await Result.Create(updateUserRequest, DomainErrors.General.UnProcessableRequest)
            .Map(request => new UpdateUserCommand(_userIdentifierProvider.UserId, request.FirstName, updateUserRequest.LastName))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);

        [HttpPut(ApiRoutes.Users.ChangePassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest) =>
            await Result.Create(changePasswordRequest, DomainErrors.General.UnProcessableRequest)
            .Map(request => new ChangePasswordCommand(_userIdentifierProvider.UserId, request.Password))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);

        [HttpPost(ApiRoutes.Users.SendFriendshipRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendFriendshipRequest(SendFriendshipRequestRequest sendFriendshipRequestRequest) =>
            await Result.Create(sendFriendshipRequestRequest, DomainErrors.General.UnProcessableRequest)
                .Map(request => new SendFriendshipRequestCommand(_userIdentifierProvider.UserId, request.FriendId))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);
    }
}
