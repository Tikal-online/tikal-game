using Accounts.Application.DataAccess;
using Accounts.Application.UseCases.CreateAccount;
using Accounts.Contracts.Commands;
using Accounts.Contracts.Errors;
using Accounts.Contracts.Models;
using Accounts.Domain.Entities;
using Moq;

namespace Accounts.Application.Tests.UseCases.CreateAccount;

public sealed class CreateAccountCommandHandlerTests
{
    // dependencies
    private readonly Mock<AccountRepository> accountRepository;
    private readonly Mock<UnitOfWork> unitOfWork;

    // under test
    private CreateAccountCommandHandler handler;

    public CreateAccountCommandHandlerTests()
    {
        accountRepository = new Mock<AccountRepository>();
        unitOfWork = new Mock<UnitOfWork>();

        handler = new CreateAccountCommandHandler(accountRepository.Object, unitOfWork.Object);
    }

    [Theory]
    [ClassData(typeof(ValidCreateAccountCommands))]
    public async Task GivenExistingAccountForUserId_WhenHandle_ThenReturnsDuplicateUserIdError(
        CreateAccountCommand command
    )
    {
        // given
        var existingAccount = new Account
        {
            UserId = command.UserId,
            Name = "AccountName"
        };

        accountRepository
            .Setup(r => r.GetByUserId(command.UserId))
            .ReturnsAsync(existingAccount);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.IsType<DuplicateUserId>(result.Value),
            () => Assert.Equal(existingAccount.UserId, result.AsT1.UserId)
        );
    }

    [Theory]
    [ClassData(typeof(ValidCreateAccountCommands))]
    public async Task GivenNoAccountForUserId_WhenHandle_ThenReturnsCreatedAccountAndPersistsAccount(
        CreateAccountCommand command
    )
    {
        // given
        accountRepository
            .Setup(r => r.GetByUserId(command.UserId))
            .ReturnsAsync(default(Account));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        Assert.Multiple(
            () => Assert.IsType<AccountModel>(result.Value),
            () => Assert.Equal(command.UserId, result.AsT0.UserId),
            () => Assert.Equal(command.Name, result.AsT0.Name)
        );

        accountRepository.Verify(r => r.Create(It.IsAny<Account>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}