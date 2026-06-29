using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.User.Commands.UserManagement;
using BizMate.Domain.Entities;
using FluentAssertions;
using Moq;

namespace BizMate.UnitTest.UserCase.User.UserManagement
{
    public class CreateStoreUserHandlerTests
    {
        [Fact]
        public async Task CreateStoreUser_ShouldRejectOwnerRole()
        {
            var fixture = new CreateStoreUserFixture();
            fixture.RoleRepository
                .Setup(repository => repository.GetByIdAsync(
                    fixture.RoleId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Role
                {
                    Id = fixture.RoleId,
                    Name = "Owner",
                    DisplayName = "Chủ công ty",
                    IsSystem = true
                });

            var response = await fixture.CreateHandler().Handle(
                fixture.CreateRequest(),
                CancellationToken.None);

            response.Success.Should().BeFalse();
            response.Message.Should().Contain("chủ công ty");
            fixture.UserRepository.Verify(
                repository => repository.AddAsync(
                    It.IsAny<BizMate.Domain.Entities.User>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateStoreUser_ShouldCreateUserAndPrimaryRoleInCurrentStore()
        {
            var fixture = new CreateStoreUserFixture();
            fixture.RoleRepository
                .Setup(repository => repository.GetByIdAsync(
                    fixture.RoleId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Role
                {
                    Id = fixture.RoleId,
                    Name = "Staff",
                    DisplayName = "Nhân viên"
                });

            var response = await fixture.CreateHandler().Handle(
                fixture.CreateRequest(),
                CancellationToken.None);

            response.Success.Should().BeTrue();
            fixture.UserRepository.Verify(
                repository => repository.AddAsync(
                    It.Is<BizMate.Domain.Entities.User>(user =>
                        user.StoreId == fixture.StoreId &&
                        user.Role == "Staff" &&
                        user.IsActive),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            fixture.UserRoleRepository.Verify(
                repository => repository.AddAsync(
                    It.Is<UserRole>(userRole =>
                        userRole.StoreId == fixture.StoreId &&
                        userRole.RoleId == fixture.RoleId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            fixture.UnitOfWork.Verify(
                unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private sealed class CreateStoreUserFixture
        {
            public Guid StoreId { get; } = Guid.NewGuid();
            public Guid RoleId { get; } = Guid.NewGuid();
            public Mock<IUserRepository> UserRepository { get; } = new();
            public Mock<IRoleRepository> RoleRepository { get; } = new();
            public Mock<IUserRoleRepository> UserRoleRepository { get; } = new();
            public Mock<IUserSession> UserSession { get; } = new();
            public Mock<ICodeGeneratorService> CodeGeneratorService { get; } = new();
            public Mock<IUnitOfWork> UnitOfWork { get; } = new();

            public CreateStoreUserFixture()
            {
                UserRepository
                    .Setup(repository => repository.ExistsByEmailAsync(
                        It.IsAny<string>(),
                        null,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);
                UserSession.SetupGet(session => session.StoreId).Returns(StoreId);
                UserSession.SetupGet(session => session.UserId).Returns(Guid.NewGuid().ToString());
                CodeGeneratorService
                    .Setup(service => service.GenerateCodeAsync("#U", 5))
                    .ReturnsAsync("#U00001");
            }

            public CreateStoreUserHandler CreateHandler() =>
                new(
                    UserRepository.Object,
                    RoleRepository.Object,
                    UserRoleRepository.Object,
                    UserSession.Object,
                    CodeGeneratorService.Object,
                    UnitOfWork.Object);

            public CreateStoreUserRequest CreateRequest() =>
                new()
                {
                    FullName = "Nhân viên mới",
                    Email = "employee@bizmate.vn",
                    Password = "Password@123",
                    RoleId = RoleId,
                    IsActive = true
                };
        }
    }
}
