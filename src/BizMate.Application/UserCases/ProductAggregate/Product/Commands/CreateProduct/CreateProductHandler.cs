using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using AutoMapper;
using _Product = BizMate.Domain.Entities.Product;
using BizMate.Application.Common.Security;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Resources;

namespace BizMate.Application.UserCases.ProductAggregate.Product.Commands.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IProductRepository _productRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<CreateProductHandler> _logger;
        private readonly IStringLocalizer<MessageUtils> _localizer;
        private readonly IMapper _mapper;

        #region constructor
        public CreateProductHandler(
            IAppMessageService messageService,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IProductRepository productRepository,
            QueryFactory db,
            ILogger<CreateProductHandler> logger,
            IStringLocalizer<MessageUtils> localizer,
            IMapper mapper)
        {
            _messageService = messageService;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _productRepository = productRepository;
            _db = db;
            _logger = logger;
            _localizer = localizer;
            _mapper = mapper;
        }
        #endregion
        public async Task<CreateProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;
                #region check product duplicate
                var existingProduct = await _productRepository.SearchProducts(
                    storeId,
                    request.SupplierId,
                    request.Name,
                    _db);

                if (existingProduct.Any())
                {
                    var message = _messageService.DuplicateData(request.Name, _localizer);
                    _logger.LogWarning(message);
                    return new CreateProductResponse(false, message);
                }
                #endregion

                #region create new product
                // Generate product code
                var productCode = await _codeGeneratorService.GenerateCodeAsync("SP");

                var newProduct = new _Product
                {
                    Id = Guid.NewGuid(),
                    Code = productCode,
                    Name = request.Name,
                    Unit = request.Unit,
                    ImageUrl = request.ImageUrl,
                    StoreId = storeId,
                    ProductCategoryId = request.ProductCategoryId,
                    SupplierId = request.SupplierId,
                    Description = request.Description,
                    CreatedBy = Guid.Parse(userId),
                    RowVersion = 1
                };

                await _productRepository.AddAsync(newProduct, cancellationToken);
                #endregion

                var productDto = _mapper.Map<ProductCoreDto>(newProduct);

                return new CreateProductResponse(productDto, true, _localizer["Tạo sản phẩm thành công."]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm.");
                return new CreateProductResponse(false, _localizer["Không thể tạo sản phẩm. Vui lòng thử lại."]);
            }
        }
    }
}
