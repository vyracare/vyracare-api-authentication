using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

public sealed class FirstAccessCheckHandler
{
    private readonly IUserRepository _userRepository;

    public FirstAccessCheckHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UseCaseResult<FirstAccessCheckResponse>> HandleAsync(FirstAccessCheckRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return UseCaseResult<FirstAccessCheckResponse>.Failure(UseCaseErrorType.Validation, "Email is required");
        }

        var email = request.Email.Trim();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            return UseCaseResult<FirstAccessCheckResponse>.Success(new FirstAccessCheckResponse(false, false));
        }

        var canSetPassword = string.IsNullOrWhiteSpace(user.PasswordHash);
        return UseCaseResult<FirstAccessCheckResponse>.Success(new FirstAccessCheckResponse(true, canSetPassword));
    }
}
