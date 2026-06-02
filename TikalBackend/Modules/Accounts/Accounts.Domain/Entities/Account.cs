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
        public void ValidAccountUserId()
        {
            ruleBuilder
                .NotEmpty()
                .WithMessage("UserId cannot be empty")
                .MaximumLength(100)
                .WithMessage("UserId cannot exceed 100 characters");
        }

        public void ValidAccountName()
        {
            ruleBuilder
                .NotEmpty()
                .WithMessage("Name cannot be empty")
                .MaximumLength(30)
                .WithMessage("Name cannot exceed 30 characters");
        }
    }
}