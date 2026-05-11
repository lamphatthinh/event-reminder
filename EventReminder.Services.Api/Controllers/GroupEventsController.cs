using EventReminder.Application.Abstractions.Authentication;
using EventReminder.Application.GroupEvents.CancelGroupEvent;
using EventReminder.Application.GroupEvents.CreateGroupEvent;
using EventReminder.Application.GroupEvents.Get10MostRecentAttendingGroupEvents;
using EventReminder.Application.GroupEvents.GetGroupEventById;
using EventReminder.Application.GroupEvents.GetGroupEvents;
using EventReminder.Application.GroupEvents.InviteFriendToGroupEvent;
using EventReminder.Application.GroupEvents.UpdateGroupEvent;
using EventReminder.Contracts.Common;
using EventReminder.Contracts.GroupEvents;
using EventReminder.Domain.Core.Errors;
using EventReminder.Domain.Core.Primitives.Maybe;
using EventReminder.Domain.Core.Primitives.Result;
using EventReminder.Services.Api.Contracts;
using EventReminder.Services.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventReminder.Services.Api.Controllers
{
    public sealed class GroupEventsController : ApiController
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;
        public GroupEventsController(IMediator mediator, IUserIdentifierProvider userIdentifiderProvider)
            : base(mediator)
        {
            _userIdentifierProvider = userIdentifiderProvider;
        }

        [HttpGet(ApiRoutes.GroupEvents.GetById)]
        [ProducesResponseType(typeof(DetailedGroupEventResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid groupEventId) =>
            await Maybe<GetGroupEventByIdQuery>
                .From(new GetGroupEventByIdQuery(groupEventId))
                .Bind(query => Mediator.Send(query))
                .Match(Ok, NotFound);

        [HttpGet(ApiRoutes.GroupEvents.GetMyOwn)]
        [ProducesResponseType(typeof(PagedList<GroupEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyOwn(
            string name,
            int? categoryId,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize) =>
            await Maybe<GetGroupEventsQuery>
                .From(new GetGroupEventsQuery(_userIdentifierProvider.UserId, name, categoryId, startDate, endDate, page, pageSize))
                .Bind(query => Mediator.Send(query))
                .Match(Ok, NotFound);

        [HttpGet(ApiRoutes.GroupEvents.GetMostRecentAttending)]
        [ProducesResponseType(typeof(IReadOnlyCollection<GroupEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMostRecentAttending() =>
            await Maybe<Get10MostRecentAttendingGroupEventsQuery>
                .From(new Get10MostRecentAttendingGroupEventsQuery(_userIdentifierProvider.UserId))
                .Bind(query => Mediator.Send(query))
                .Match(Ok, NotFound);

        [HttpPost(ApiRoutes.GroupEvents.Base)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateGroupEventRequest createGroupEventRequest) =>
            await Result.Create(createGroupEventRequest, DomainErrors.General.UnProcessableRequest)
                .Map(request => new CreateGroupEventCommand(_userIdentifierProvider.UserId, request.Name, request.CategoryId, request.DateTimeUtc))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);

        [HttpPut(ApiRoutes.GroupEvents.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid groupEventId, UpdateGroupEventRequest updateGroupEventRequest) =>
            await Result.Create(updateGroupEventRequest, DomainErrors.General.UnProcessableRequest)
                .Map(request => new UpdateGroupEventCommand(groupEventId, request.Name, request.DateTimeUtc))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);

        [HttpPost(ApiRoutes.GroupEvents.InviteFriend)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InviteFriend(Guid groupEventId, InviteFriendToGroupEventRequest inviteFriendToGroupEventRequest) =>
            await Result.Create(inviteFriendToGroupEventRequest, DomainErrors.General.UnProcessableRequest)
                .Map(request => new InviteFriendToGroupEventCommand(groupEventId, request.FriendId))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);

        [HttpDelete(ApiRoutes.GroupEvents.Cancel)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(Guid groupEventId) =>
            await Result.Success(new CancelGroupEventCommand(groupEventId))
                .Bind(command => Mediator.Send(command))
                .Match(Ok, BadRequest);
    }
}
