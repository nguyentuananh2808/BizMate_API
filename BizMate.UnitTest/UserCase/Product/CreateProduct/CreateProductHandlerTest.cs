using BizMate.Application.UserCases.Product.Commands.CreateProduct;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using _Product = BizMate.Domain.Entities.Product;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Resources;
using AutoMapper;
using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Security;
using SqlKata.Execution;

namespace BizMate.UnitTest.UserCase.Product.CreateProduct
{
    public class CreateProductHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock = new();
        private readonly Mock<ICodeGeneratorService> _codeGeneratorServiceMock = new();
        private readonly Mock<IUserSession> _userSessionMock = new();
        private readonly Mock<QueryFactory> _dbMock = new();
        private readonly Mock<ILogger<CreateProductHandler>> _loggerMock = new();
        private readonly Mock<IStringLocalizer<BizMate_Application_Resources_CommonAppResourceKeys>> _localizerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private readonly CreateProductHandler _handler;

        public CreateProductHandlerTests()
        {
            _handler = new CreateProductHandler(
                _codeGeneratorServiceMock.Object,
                _userSessionMock.Object,
                _productRepositoryMock.Object,
                _dbMock.Object,
                _loggerMock.Object,
                _localizerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenProductCreated()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                SupplierId = Guid.NewGuid(),
                Unit = 1,
                Description = "Test Desc",
                ImageUrl = "http://image.test"
            };

            var storeId = Guid.NewGuid();
            _userSessionMock.Setup(u => u.StoreId).Returns(storeId);

            _productRepositoryMock.Setup(r => r.SearchProducts(
                storeId,
                request.SupplierId,
                request.Name,
                It.IsAny<QueryFactory>()))
                .ReturnsAsync(new List<_Product>());

            _codeGeneratorServiceMock.Setup(c => c.GenerateCodeAsync("SP", 5))
                .ReturnsAsync("SP0001");

            _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<_Product>()))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<ProductCoreDto>(It.IsAny<_Product>()))
                .Returns(new ProductCoreDto { Name = request.Name });

            _localizerMock.Setup(l => l["Tạo sản phẩm thành công."])
                .Returns(new LocalizedString("Tạo sản phẩm thành công.", "Tạo sản phẩm thành công."));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Tạo sản phẩm thành công.", result.Message);
            Assert.Equal(request.Name, result.Product.Name);
        }
    }
}