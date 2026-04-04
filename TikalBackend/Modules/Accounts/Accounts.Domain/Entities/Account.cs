using FluentValidation;

namespace Accounts.Domain.Entities;

public sealed class Account
{
    public long Id { get; set; }

    public required string UserId { get; set; }

    public required string Name { get; set; }
}

public static class AccountValidationRules
{
    extension<T>(IRuleBuilder<T, string> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string> ValidAccountUserId()
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("UserId cannot be empty")
                .MaximumLength(100)
                .WithMessage("UserId cannot exceed 100 characters");
        }

        public IRuleBuilderOptions<T, string> ValidAccountName()
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("Name cannot be empty")
                .MaximumLength(30)
                .WithMessage("Name cannot exceed 30 characters");
        }
    }
}