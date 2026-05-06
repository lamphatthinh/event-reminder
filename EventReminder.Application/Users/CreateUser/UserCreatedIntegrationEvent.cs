using System;
using EventReminder.Application.Abstractions.Messaging;
using EventReminder.Domain.Users.DomainEvents;
using Newtonsoft.Json;

namespace EventReminder.Application.Users.CreateUser
{
    /// <summary>
    /// Represents the integration event that is raised when a user is created.
    /// </summary>
    public sealed class UserCreatedIntegrationEvent : IIntegrationEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserCreatedIntegrationEvent"/> class.
        /// </summary>
        /// <param name="userCreatedDomainEvent">The user created domain event.</param>
        internal UserCreatedIntegrationEvent(UserCreatedDomainEvent userCreatedDomainEvent)
        {
            UserId = userCreatedDomainEvent.User.Id;
            Email = userCreatedDomainEvent.User.Email;
            FullName = userCreatedDomainEvent.User.FullName;
        }

        [JsonConstructor]
        private UserCreatedIntegrationEvent(Guid userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Gets the user's full name.
        /// </summary>
        public string FullName { get; }
    }
}
