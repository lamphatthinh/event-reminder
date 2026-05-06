using System.Threading;
using System.Threading.Tasks;
using EventReminder.Application.Abstractions.Notifications;
using EventReminder.Application.Users.CreateUser;
using EventReminder.BackgroundTasks.Abstractions.Messaging;
using EventReminder.Contracts.Emails;
using EventReminder.Domain.Core.Errors;
using EventReminder.Domain.Core.Exceptions;
using EventReminder.Domain.Core.Primitives.Maybe;
using EventReminder.Domain.Users;

namespace EventReminder.BackgroundTasks.IntegrationEvents.Users.UserCreated
{
    /// <summary>
    /// Represents the <see cref="UserCreatedIntegrationEvent"/> handler.
    /// </summary>
    internal sealed class SendWelcomeEmailOnUserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailNotificationService _emailNotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendWelcomeEmailOnUserCreatedIntegrationEventHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="emailNotificationService">The email notification service.</param>
        public SendWelcomeEmailOnUserCreatedIntegrationEventHandler(IEmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        /// <inheritdoc />
        public async Task Handle(UserCreatedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            WelcomeEmail welcomeEmail = new WelcomeEmail(notification.Email, notification.FullName);
            await _emailNotificationService.SendWelcomeEmail(welcomeEmail);
        }
    }
}
