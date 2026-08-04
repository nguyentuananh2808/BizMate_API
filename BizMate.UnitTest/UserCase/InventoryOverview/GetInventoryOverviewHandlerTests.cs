using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.InventoryOverview;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BizMate.UnitTest.UserCase.InventoryOverview
{
    public class GetInventoryOverviewHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldGroupAndSortInventoryOverview()
        {
            var fixture = new InventoryOverviewFixture();
            var olderGroupId = Guid.NewGuid();
            var newerGroupId = Guid.NewGuid();
            var cameraCategoryId = Guid.NewGuid();
            var recorderCategoryId = Guid.NewGuid();

            fixture.Repository
                .Setup(repository => repository.GetOverviewRowsAsync(
                    fixture.StoreId,
                    null,
                    null,
                    false,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InventoryOverviewRowDto>
                {
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductCode = "#SP0002",
                        ProductName = "Z Camera",
                        Unit = 1,
                        ProductCategoryId = cameraCategoryId,
                        ProductCategoryName = "Camera IP",
                        ProductCategoryCreatedDate = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                        CategoryGroupId = olderGroupId,
                        CategoryGroupName = "CAMERA",
                        CategoryGroupCreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        StockQuantity = 10,
                        ReservedQuantity = 2,
                        AvailableQuantity = 8
                    },
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductCode = "#SP0001",
                        ProductName = "A Camera",
                        Unit = 1,
                        ProductCategoryId = cameraCategoryId,
                        ProductCategoryName = "Camera IP",
                        ProductCategoryCreatedDate = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                        CategoryGroupId = olderGroupId,
                        CategoryGroupName = "CAMERA",
                        CategoryGroupCreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        StockQuantity = 0,
                        ReservedQuantity = 0,
                        AvailableQuantity = 0
                    },
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductCode = "#SP0003",
                        ProductName = "NVR",
                        Unit = 1,
                        ProductCategoryId = recorderCategoryId,
                        ProductCategoryName = "Đầu ghi",
                        ProductCategoryCreatedDate = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                        CategoryGroupId = newerGroupId,
                        CategoryGroupName = "ĐẦU GHI",
                        CategoryGroupCreatedDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                        StockQuantity = 5,
                        ReservedQuantity = 1,
                        AvailableQuantity = 4
                    }
                });

            var response = await fixture.CreateHandler().Handle(
                new GetInventoryOverviewRequest(),
                CancellationToken.None);

            response.Success.Should().BeTrue();
            response.Overview.Should().NotBeNull();
            response.Overview!.TotalProducts.Should().Be(3);
            response.Overview.TotalStockQuantity.Should().Be(15);
            response.Overview.TotalReservedQuantity.Should().Be(3);
            response.Overview.TotalAvailableQuantity.Should().Be(12);
            response.Overview.OutOfStockProductCount.Should().Be(1);
            response.Overview.CategoryGroups.Select(x => x.CategoryGroupName)
                .Should().Equal("CAMERA", "ĐẦU GHI");
            response.Overview.CategoryGroups[0].Categories[0].Products.Select(x => x.ProductName)
                .Should().Equal("A Camera", "Z Camera");
        }

        [Fact]
        public async Task Handle_ShouldRejectMissingCategoryGroup()
        {
            var fixture = new InventoryOverviewFixture();
            var categoryGroupId = Guid.NewGuid();
            fixture.Repository
                .Setup(repository => repository.CategoryGroupExistsAsync(
                    fixture.StoreId,
                    categoryGroupId,
                    false,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var response = await fixture.CreateHandler().Handle(
                new GetInventoryOverviewRequest { CategoryGroupId = categoryGroupId },
                CancellationToken.None);

            response.Success.Should().BeFalse();
            response.Message.Should().Contain("Nhóm loại sản phẩm");
            fixture.Repository.Verify(
                repository => repository.GetOverviewRowsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<string?>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldPassTrimmedKeywordAndFilterToRepository()
        {
            var fixture = new InventoryOverviewFixture();
            var categoryGroupId = Guid.NewGuid();
            fixture.Repository
                .Setup(repository => repository.CategoryGroupExistsAsync(
                    fixture.StoreId,
                    categoryGroupId,
                    false,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            fixture.Repository
                .Setup(repository => repository.GetOverviewRowsAsync(
                    fixture.StoreId,
                    categoryGroupId,
                    "DH-H3AE",
                    false,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InventoryOverviewRowDto>());

            var response = await fixture.CreateHandler().Handle(
                new GetInventoryOverviewRequest
                {
                    CategoryGroupId = categoryGroupId,
                    Keyword = "  DH-H3AE  "
                },
                CancellationToken.None);

            response.Success.Should().BeTrue();
            fixture.Repository.Verify(
                repository => repository.GetOverviewRowsAsync(
                    fixture.StoreId,
                    categoryGroupId,
                    "DH-H3AE",
                    false,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private sealed class InventoryOverviewFixture
        {
            public Guid StoreId { get; } = Guid.NewGuid();
            public Mock<IInventoryOverviewRepository> Repository { get; } = new();
            public Mock<IUserSession> UserSession { get; } = new();
            public Mock<ILogger<GetInventoryOverviewHandler>> Logger { get; } = new();

            public InventoryOverviewFixture()
            {
                UserSession.SetupGet(session => session.StoreId).Returns(StoreId);
            }

            public GetInventoryOverviewHandler CreateHandler() =>
                new(
                    Repository.Object,
                    UserSession.Object,
                    Logger.Object);
        }
    }
}
