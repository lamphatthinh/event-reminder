using System.Runtime.CompilerServices;

namespace EventReminder.Services.Api.Contracts
{
    /// <summary>
    /// Contains the API endpoint routes.
    /// </summary>
    public static class ApiRoutes
    {
        /// <summary>
        /// Contains the authentication routes.
        /// </summary>
        public static class Authentication
        {
            public const string Login = "authentication/login";

            public const string Register = "authentication/register";
        }

        /// <summary>
        /// Contains the attendee routes.
        /// </summary>
        public static class Attendees
        {
            public const string Get = GroupEvents.Base + "/{groupEventId:guid}/attendees";
        }

        /// <summary>
        /// Contains the friendship requests routes.
        /// </summary>
        public static class FriendshipRequests
        {
            public const string GetById = "friendship-requests/{friendshipRequestId:guid}";
            
            public const string GetPending = "friendship-requests/pending";

            public const string GetSent = "friendship-requests/sent";

            public const string Accept = "friendship-requests/{friendshipRequestId:guid}/accept";

            public const string Reject = "friendship-requests/{friendshipRequestId:guid}/reject";
        }

        /// <summary>
        /// Contains the friendships routes.
        /// </summary>
        public static class Friendships
        {
            public const string Get = "friendships/{userId:guid}/{friendId:guid}";

            public const string GetMyFriendShips = "friendships/me";

            public const string Base = "friendships";
        }

        /// <summary>
        /// Contains the group events routes.
        /// </summary>
        public static class GroupEvents
        {
            public const string Base = "group-events";

            public const string GetMyOwn = "group-events/me";

            public const string GetById = "group-events/{groupEventId:guid}";

            public const string GetMostRecentAttending = "group-events/most-recent-attending";
            
            public const string Create = "group-events";

            public const string Update = "group-events/{groupEventId:guid}";

            public const string Cancel = "group-events/{groupEventId:guid}";

            public const string InviteFriend = "group-events/{groupEventId:guid}/invite";
        }

        /// <summary>
        /// Contains the group invitations routes.
        /// </summary>
        public static class Invitations
        {
            public const string GetById = "invitations/{invitationId:guid}";

            public const string GetPending = "invitations/pending";

            public const string GetSent = "invitations/sent";

            public const string Accept = "invitations/{invitationId:guid}/accept";

            public const string Reject = "invitations/{invitationId:guid}/reject";
        }

        /// <summary>
        /// Contains the personal events routes.
        /// </summary>
        public static class PersonalEvents
        {
            public const string Base = "personal-events";

            public const string GetMyOwn = PersonalEvents.Base + "/me";

            public const string GetById = PersonalEvents.Base + "/{personalEventId:guid}";

            public const string Update = PersonalEvents.Base + "/{personalEventId:guid}";

            public const string Cancel = PersonalEvents.Base + "/{personalEventId:guid}";
        }

        /// <summary>
        /// Contains the users routes.
        /// </summary>
        public static class Users
        {
            public const string GetMyInfo = "users/me";

            public const string Update = "users/me";

            public const string ChangePassword = "users/me/change-password";

            public const string SendFriendshipRequest = "users/me/send-friendship-request";
        }
    }
}
