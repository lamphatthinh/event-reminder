using EventReminder.Application.Abstractions.Authentication;
using EventReminder.Application.Abstractions.Cryptography;
using EventReminder.Application.Abstractions.Data;
using EventReminder.Application.Abstractions.Messaging;
using EventReminder.Domain.Core.Errors;
using EventReminder.Domain.Core.Primitives.Maybe;
using EventReminder.Domain.Core.Primitives.Result;
using EventReminder.Domain.Users;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventReminder.Application.Users.ChangePassword
{
    /// <summary>
    /// Represents the <see cref="ChangePasswordCommand"/> handler.
    /// </summary>
    internal sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Result>
    {
        private readonly IUserIdentifierProvider _userIdentifierProvider;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordHashChecker _passwordHashChecker;


        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordCommandHandler"/> class.
        /// </summary>
        /// <param name="userIdentifierProvider">The user identifier provider.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="passwordHasher">The password hasher.</param>
        public ChangePasswordCommandHandler(
            IUserIdentifierProvider userIdentifierProvider,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IPasswordHashChecker passwordHashChecker)
        {
            _userIdentifierProvider = userIdentifierProvider;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _passwordHashChecker = passwordHashChecker;
        }

        /// <inheritdoc />
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            Maybe<User> maybeUser = await _userRepository.GetByIdAsync(request.UserId);

            if (maybeUser.HasNoValue)
            {
                return Result.Failure(DomainErrors.User.NotFound);
            }

            User user = maybeUser.Value;

            bool isPasswordValid = user.VerifyPasswordHash(request.Password, _passwordHashChecker);

            if (!isPasswordValid)
            {
                return Result.Failure(DomainErrors.Authentication.InvalidPassword);
            }

            Result<Password> passwordResult = Password.Create(request.NewPassword);

            if (passwordResult.IsFailure)
            {
                return Result.Failure(passwordResult.Error);
            }

            string passwordHash = _passwordHasher.HashPassword(passwordResult.Value);

            Result result = user.ChangePassword(passwordHash);

            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
