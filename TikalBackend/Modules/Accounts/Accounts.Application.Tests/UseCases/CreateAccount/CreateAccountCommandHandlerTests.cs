using Accounts.Application.DataAccess;
using Accounts.Application.UseCases.CreateAccount;
using Accounts.Contracts.Commands;
using Accounts.Contracts.Errors;
using Accounts.Contracts.Models;
using Accounts.Domain.Entities;
using Moq;

namespace Accounts.Application.Tests.UseCases.CreateAccount;

internal sealed class CreateAccountCommandHandlerTests
{
    // dependencies
    private Mock<AccountRepository> accountRepository;
    private Mock<UnitOfWork> unitOfWork;

    // under test
    private CreateAccountCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        accountRepository = new Mock<AccountRepository>();
        unitOfWork = new Mock<UnitOfWork>();

        handler = new CreateAccountCommandHandler(accountRepository.Object, unitOfWork.Object);
    }

    [TestCaseSource(
        typeof(CreateAccountCommandTestCases),
        nameof(CreateAccountCommandTestCases.ValidCreateAccountCommands)
    )]
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
            .Setup(r => r.GetByUserIdAsync(command.UserId))
            .ReturnsAsync(existingAccount);

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.InstanceOf<DuplicateUserId>());
            Assert.That(result.AsT1.UserId, Is.EqualTo(existingAccount.UserId));
        }
    }

    [TestCaseSource(
        typeof(CreateAccountCommandTestCases),
        nameof(CreateAccountCommandTestCases.ValidCreateAccountCommands)
    )]
    public async Task GivenNoAccountForUserId_WhenHandle_ThenReturnsCreatedAccountAndPersistsAccount(
        CreateAccountCommand command
    )
    {
        // given
        accountRepository
            .Setup(r => r.GetByUserIdAsync(command.UserId))
            .ReturnsAsync(default(Account));

        // when
        var result = await handler.Handle(command, CancellationToken.None);

        // then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Value, Is.InstanceOf<AccountModel>());
            Assert.That(result.AsT0.Name, Is.EqualTo(command.Name));
            Assert.That(result.AsT0.UserId, Is.EqualTo(command.UserId));
        }

        accountRepository.Verify(r => r.Create(It.IsAny<Account>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}