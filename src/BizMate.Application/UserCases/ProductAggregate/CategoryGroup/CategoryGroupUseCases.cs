using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Requests;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;
using _CategoryGroup = BizMate.Domain.Entities.CategoryGroup;

namespace BizMate.Application.UserCases.ProductAggregate.CategoryGroup
{
    public sealed class GetCategoryGroupsRequest : SearchCore, IRequest<GetCategoryGroupsResponse>
    {
        public bool? IsActive { get; set; }
    }

    public sealed class GetAllCategoryGroupsRequest : IRequest<GetCategoryGroupsResponse>
    {
    }

    public sealed class GetCategoryGroupRequest : IRequest<GetCategoryGroupResponse>
    {
        public Guid Id { get; set; }

        public GetCategoryGroupRequest(Guid id)
        {
            Id = id;
        }
    }

    public sealed class CreateCategoryGroupRequest : IRequest<CategoryGroupMutationResponse>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public List<Guid> ProductCategoryIds { get; set; } = new();
    }

    public sealed class UpdateCategoryGroupRequest : IRequest<CategoryGroupMutationResponse>
    {
        public Guid Id { get; set; }
        public Guid RowVersion { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> ProductCategoryIds { get; set; } = new();
    }

    public sealed class AssignCategoryGroupCategoriesRequest : IRequest<CategoryGroupMutationResponse>
    {
        public Guid Id { get; set; }
        public List<Guid> ProductCategoryIds { get; set; } = new();
    }

    public sealed class DeleteCategoryGroupRequest : IRequest<CategoryGroupMutationResponse>
    {
        public Guid Id { get; set; }

        public DeleteCategoryGroupRequest(Guid id)
        {
            Id = id;
        }
    }

    public sealed class GetCategoryGroupsResponse : BaseResponse
    {
        public IEnumerable<CategoryGroupCoreDto> CategoryGroups { get; }
        public int TotalCount { get; }

        public GetCategoryGroupsResponse(IEnumerable<CategoryGroupCoreDto> categoryGroups, int totalCount, bool success = true, string? message = null)
            : base(success, message)
        {
            CategoryGroups = categoryGroups;
            TotalCount = totalCount;
        }

        public GetCategoryGroupsResponse(bool success, string message)
            : base(success, message)
        {
            CategoryGroups = Array.Empty<CategoryGroupCoreDto>();
        }
    }

    public sealed class GetCategoryGroupResponse : BaseResponse
    {
        public CategoryGroupCoreDto? CategoryGroup { get; }

        public GetCategoryGroupResponse(CategoryGroupCoreDto categoryGroup, bool success = true, string? message = null)
            : base(success, message)
        {
            CategoryGroup = categoryGroup;
        }

        public GetCategoryGroupResponse(bool success, string message)
            : base(success, message)
        {
        }
    }

    public sealed class CategoryGroupMutationResponse : BaseResponse
    {
        public CategoryGroupCoreDto? CategoryGroup { get; }

        public CategoryGroupMutationResponse(CategoryGroupCoreDto? categoryGroup, bool success, string? message = null)
            : base(success, message)
        {
            CategoryGroup = categoryGroup;
        }
    }

    public sealed class GetCategoryGroupsHandler :
        IRequestHandler<GetCategoryGroupsRequest, GetCategoryGroupsResponse>,
        IRequestHandler<GetAllCategoryGroupsRequest, GetCategoryGroupsResponse>,
        IRequestHandler<GetCategoryGroupRequest, GetCategoryGroupResponse>
    {
        private readonly ICategoryGroupRepository _categoryGroupRepository;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCategoryGroupsHandler> _logger;

        public GetCategoryGroupsHandler(
            ICategoryGroupRepository categoryGroupRepository,
            IUserSession userSession,
            IMapper mapper,
            ILogger<GetCategoryGroupsHandler> logger)
        {
            _categoryGroupRepository = categoryGroupRepository;
            _userSession = userSession;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetCategoryGroupsResponse> Handle(GetCategoryGroupsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
                var (groups, totalCount) = await _categoryGroupRepository.SearchAsync(
                    _userSession.StoreId,
                    request.KeySearch,
                    pageIndex,
                    pageSize,
                    request.IsActive,
                    cancellationToken);

                return new GetCategoryGroupsResponse(_mapper.Map<List<CategoryGroupCoreDto>>(groups), totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách nhóm loại sản phẩm.");
                return new GetCategoryGroupsResponse(false, "Không thể tải danh sách nhóm loại sản phẩm. Vui lòng thử lại.");
            }
        }

        public async Task<GetCategoryGroupsResponse> Handle(GetAllCategoryGroupsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var groups = await _categoryGroupRepository.GetAllAsync(_userSession.StoreId, cancellationToken);
                return new GetCategoryGroupsResponse(_mapper.Map<List<CategoryGroupCoreDto>>(groups), groups.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn toàn bộ nhóm loại sản phẩm.");
                return new GetCategoryGroupsResponse(false, "Không thể tải danh sách nhóm loại sản phẩm. Vui lòng thử lại.");
            }
        }

        public async Task<GetCategoryGroupResponse> Handle(GetCategoryGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var group = await _categoryGroupRepository.GetByIdAsync(_userSession.StoreId, request.Id, cancellationToken);
                if (group is null)
                    return new GetCategoryGroupResponse(false, ValidationMessage.LocalizedStrings.DataNotExist);

                return new GetCategoryGroupResponse(_mapper.Map<CategoryGroupCoreDto>(group));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn chi tiết nhóm loại sản phẩm {CategoryGroupId}.", request.Id);
                return new GetCategoryGroupResponse(false, "Không thể tải chi tiết nhóm loại sản phẩm. Vui lòng thử lại.");
            }
        }
    }

    public sealed class CategoryGroupMutationHandler :
        IRequestHandler<CreateCategoryGroupRequest, CategoryGroupMutationResponse>,
        IRequestHandler<UpdateCategoryGroupRequest, CategoryGroupMutationResponse>,
        IRequestHandler<AssignCategoryGroupCategoriesRequest, CategoryGroupMutationResponse>,
        IRequestHandler<DeleteCategoryGroupRequest, CategoryGroupMutationResponse>
    {
        private readonly ICategoryGroupRepository _categoryGroupRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryGroupMutationHandler> _logger;

        public CategoryGroupMutationHandler(
            ICategoryGroupRepository categoryGroupRepository,
            IProductCategoryRepository productCategoryRepository,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IMapper mapper,
            ILogger<CategoryGroupMutationHandler> logger)
        {
            _categoryGroupRepository = categoryGroupRepository;
            _productCategoryRepository = productCategoryRepository;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryGroupMutationResponse> Handle(CreateCategoryGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var name = request.Name?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                    return new CategoryGroupMutationResponse(null, false, "Tên nhóm loại sản phẩm không được để trống.");

                var duplicate = await _categoryGroupRepository.GetByNameAsync(storeId, name, null, cancellationToken);
                if (duplicate is not null)
                    return new CategoryGroupMutationResponse(null, false, "Tên nhóm loại sản phẩm đã tồn tại trong cửa hàng hiện tại.");

                var invalidCategoryMessage = await ValidateProductCategoriesAsync(storeId, request.ProductCategoryIds, cancellationToken);
                if (invalidCategoryMessage is not null)
                    return new CategoryGroupMutationResponse(null, false, invalidCategoryMessage);

                var categoryGroup = new _CategoryGroup
                {
                    Id = Guid.NewGuid(),
                    Code = await _codeGeneratorService.GenerateCodeAsync("#NLO", 5),
                    Name = name,
                    Description = request.Description?.Trim(),
                    StoreId = storeId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = ParseCurrentUserId(),
                };

                await _categoryGroupRepository.AddAsync(categoryGroup, cancellationToken);
                await _categoryGroupRepository.AssignProductCategoriesAsync(storeId, categoryGroup.Id, request.ProductCategoryIds, cancellationToken);

                var savedGroup = await _categoryGroupRepository.GetByIdAsync(storeId, categoryGroup.Id, cancellationToken);
                return new CategoryGroupMutationResponse(
                    _mapper.Map<CategoryGroupCoreDto>(savedGroup),
                    true,
                    "Tạo nhóm loại sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo nhóm loại sản phẩm.");
                return new CategoryGroupMutationResponse(null, false, "Không thể tạo nhóm loại sản phẩm. Vui lòng thử lại.");
            }
        }

        public async Task<CategoryGroupMutationResponse> Handle(UpdateCategoryGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var name = request.Name?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                    return new CategoryGroupMutationResponse(null, false, "Tên nhóm loại sản phẩm không được để trống.");

                var categoryGroup = await _categoryGroupRepository.GetByIdAsync(storeId, request.Id, cancellationToken);
                if (categoryGroup is null)
                    return new CategoryGroupMutationResponse(null, false, ValidationMessage.LocalizedStrings.DataNotExist);

                if (categoryGroup.RowVersion != request.RowVersion)
                    return new CategoryGroupMutationResponse(null, false, ValidationMessage.LocalizedStrings.NotValidRowversion);

                var duplicate = await _categoryGroupRepository.GetByNameAsync(storeId, name, request.Id, cancellationToken);
                if (duplicate is not null)
                    return new CategoryGroupMutationResponse(null, false, "Tên nhóm loại sản phẩm đã tồn tại trong cửa hàng hiện tại.");

                var invalidCategoryMessage = await ValidateProductCategoriesAsync(storeId, request.ProductCategoryIds, cancellationToken);
                if (invalidCategoryMessage is not null)
                    return new CategoryGroupMutationResponse(null, false, invalidCategoryMessage);

                categoryGroup.Name = name;
                categoryGroup.Description = request.Description?.Trim();
                categoryGroup.IsActive = request.IsActive;
                categoryGroup.UpdatedDate = DateTime.UtcNow;
                categoryGroup.UpdatedBy = ParseCurrentUserId();
                categoryGroup.RowVersion = Guid.NewGuid();

                await _categoryGroupRepository.UpdateAsync(categoryGroup, cancellationToken);
                await _categoryGroupRepository.AssignProductCategoriesAsync(storeId, categoryGroup.Id, request.ProductCategoryIds, cancellationToken);

                var savedGroup = await _categoryGroupRepository.GetByIdAsync(storeId, categoryGroup.Id, cancellationToken);
                return new CategoryGroupMutationResponse(
                    _mapper.Map<CategoryGroupCoreDto>(savedGroup),
                    true,
                    "Cập nhật nhóm loại sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật nhóm loại sản phẩm {CategoryGroupId}.", request.Id);
                return new CategoryGroupMutationResponse(null, false, "Không thể cập nhật nhóm loại sản phẩm. Vui lòng thử lại.");
            }
        }

        public async Task<CategoryGroupMutationResponse> Handle(AssignCategoryGroupCategoriesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var categoryGroup = await _categoryGroupRepository.GetByIdAsync(storeId, request.Id, cancellationToken);
                if (categoryGroup is null)
                    return new CategoryGroupMutationResponse(null, false, ValidationMessage.LocalizedStrings.DataNotExist);

                var invalidCategoryMessage = await ValidateProductCategoriesAsync(storeId, request.ProductCategoryIds, cancellationToken);
                if (invalidCategoryMessage is not null)
                    return new CategoryGroupMutationResponse(null, false, invalidCategoryMessage);

                await _categoryGroupRepository.AssignProductCategoriesAsync(storeId, request.Id, request.ProductCategoryIds, cancellationToken);

                var savedGroup = await _categoryGroupRepository.GetByIdAsync(storeId, request.Id, cancellationToken);
                return new CategoryGroupMutationResponse(
                    _mapper.Map<CategoryGroupCoreDto>(savedGroup),
                    true,
                    "Cập nhật danh sách loại sản phẩm trong nhóm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gán loại sản phẩm vào nhóm {CategoryGroupId}.", request.Id);
                return new CategoryGroupMutationResponse(null, false, "Không thể cập nhật danh sách loại sản phẩm trong nhóm. Vui lòng thử lại.");
            }
        }

        public async Task<CategoryGroupMutationResponse> Handle(DeleteCategoryGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var categoryGroup = await _categoryGroupRepository.GetByIdAsync(storeId, request.Id, cancellationToken);
                if (categoryGroup is null)
                    return new CategoryGroupMutationResponse(null, false, ValidationMessage.LocalizedStrings.DataNotExist);

                var hasCategories = await _categoryGroupRepository.HasProductCategoriesAsync(storeId, request.Id, cancellationToken);
                if (hasCategories)
                    return new CategoryGroupMutationResponse(null, false, "Nhóm này đang có loại sản phẩm, không thể xóa.");

                await _categoryGroupRepository.DeleteAsync(request.Id, cancellationToken);
                return new CategoryGroupMutationResponse(null, true, "Xóa nhóm loại sản phẩm thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa nhóm loại sản phẩm {CategoryGroupId}.", request.Id);
                return new CategoryGroupMutationResponse(null, false, "Không thể xóa nhóm loại sản phẩm. Vui lòng thử lại.");
            }
        }

        private async Task<string?> ValidateProductCategoriesAsync(Guid storeId, IReadOnlyCollection<Guid>? productCategoryIds, CancellationToken cancellationToken)
        {
            if (productCategoryIds is null || !productCategoryIds.Any())
                return null;

            var distinctIds = productCategoryIds.Distinct().ToList();
            var categories = await _productCategoryRepository.GetByIdsAsync(storeId, distinctIds, cancellationToken);
            return categories.Count == distinctIds.Count
                ? null
                : "Một hoặc nhiều loại sản phẩm không tồn tại hoặc không thuộc cửa hàng hiện tại.";
        }

        private Guid? ParseCurrentUserId()
        {
            return Guid.TryParse(_userSession.UserId, out var userId) ? userId : null;
        }
    }
}
