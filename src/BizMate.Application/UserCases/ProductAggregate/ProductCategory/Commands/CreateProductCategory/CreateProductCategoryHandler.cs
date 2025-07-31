using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using _ProductCategory = BizMate.Domain.Entities.ProductCategory;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.CreateProductCategory
{
    public class CreateProductCategoryHandler : IRequestHandler<CreateProductCategoryRequest, CreateProductCategoryResponse>
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IAppMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateProductCategoryHandler> _logger;
        private readonly IUserSession _userSession;
        private readonly IStringLocalizer<MessageUtils> _localizer;
        private readonly ICodeGeneratorService _codeGeneratorService;
        #region constructor
        public CreateProductCategoryHandler(IMapper mapper, IProductCategoryRepository productCategoryRepository, IStringLocalizer<MessageUtils> localizer,
          ILogger<CreateProductCategoryHandler> logger, IUserSession userSession, ICodeGeneratorService codeGeneratorService, IAppMessageService messageService)
        {
            _mapper = mapper;
            _productCategoryRepository = productCategoryRepository;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _productCategoryRepository = productCategoryRepository;
            _messageService = messageService;
            _localizer = localizer;
            _logger = logger;
        }

        #endregion

        public async Task<CreateProductCategoryResponse> Handle(CreateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            return await CreateProductCategory(request, cancellationToken);
        }

        private async Task<CreateProductCategoryResponse> CreateProductCategory(CreateProductCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var nameProductCategory = request.Name.Trim();
                var description = request.Description?.Trim();
                var storeId = _userSession.StoreId;

                #region check product category duplicate
                var categoryDb = await _productCategoryRepository.GetByNameAsync(storeId, nameProductCategory, cancellationToken);

                if (categoryDb != null)
                {
                    var message = _messageService.DuplicateData(nameProductCategory, _localizer);
                    _logger.LogWarning(message);
                    return new CreateProductCategoryResponse(null, false, message);
                }
                #endregion
                #region create product category
                string codeProductCategory = await _codeGeneratorService.GenerateCodeAsync("#NSP", 5);
                var productCategory = new _ProductCategory
                {
                    Code = codeProductCategory,
                    Name = nameProductCategory,
                    Description = description,
                    StoreId = storeId,
                };
                await _productCategoryRepository.AddAsync(productCategory, cancellationToken);
                #endregion
                var productCategoryDto = _mapper.Map<ProductCategoryCoreDto>(productCategory);

                return new CreateProductCategoryResponse(productCategoryDto, true, "Tạo loại sản phẩm thành công");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo loại sản phẩm.");
                return new CreateProductCategoryResponse(null, false, _localizer["Không thể tạo loại sản phẩm. Vui lòng thử lại."]);
            }
        }
    }
}
