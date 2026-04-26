using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using _ImportReceipt = BizMate.Domain.Entities.ImportReceipt;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using BizMate.Public.Message;
using BizMate.Application.Common.Extensions;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.CreateImportReceipt
{
    public class CreateImportReceiptHandler : IRequestHandler<CreateImportReceiptRequest, CreateImportReceiptResponse>
    {
        private readonly IImportReceiptRepository _ImportReceiptRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly IProductItemRepository _productItemRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateImportReceiptHandler> _logger;

        #region constructor
        public CreateImportReceiptHandler(
            IStatusRepository statusRepository,
            IProductRepository productRepository,
            IProductItemRepository productItemRepository,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IUnitOfWork unitOfWork,
            IImportReceiptRepository ImportReceiptRepository,
            ILogger<CreateImportReceiptHandler> logger)
        {
            _productRepository = productRepository;
            _productItemRepository = productItemRepository;
            _statusRepository = statusRepository;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _unitOfWork = unitOfWork;
            _ImportReceiptRepository = ImportReceiptRepository;
            _logger = logger;
        }
        #endregion
        public async Task<CreateImportReceiptResponse> Handle(CreateImportReceiptRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;
                var parsedUserId = Guid.Parse(userId);


                #region create new ImportReceipt
                var productIds = request.Details.Select(d => d.ProductId).ToList();

                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

                var productDict = products.ToDictionary(p => p.Id);

                var validationMessage = await ValidateSerialNumbersAsync(request.Details, productDict, cancellationToken);
                if (validationMessage is not null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return new CreateImportReceiptResponse(false, validationMessage);
                }

                //get status for ImportReceipt
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync("NEW", "ImportReceipt");
                if (statusId == Guid.Empty)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogError(message);
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return new CreateImportReceiptResponse(false, message);
                }
                var receiptCode = await _codeGeneratorService.GenerateCodeAsync("#NK");

                var receiptId = Guid.NewGuid();
                var newImportReceipt = new _ImportReceipt
                {
                    Id = receiptId,
                    Code = receiptCode,
                    StoreId = storeId,
                    CreatedBy = parsedUserId,
                    SupplierName = request.SupplierName,
                    DeliveryAddress = request.DeliveryAddress,
                    StatusId = statusId,
                    Description = request.Description,
                    TotalAmount=0,
                    Details = request.Details.Select(d =>
                    {
                        var product = productDict[d.ProductId];

                        return new ImportReceiptDetail
                        {
                            Id = Guid.NewGuid(),
                            ImportReceiptId = receiptId,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            ProductName = product.Name,
                            ProductCode = product.Code,
                            Unit = product.Unit,
                        };
                    }).ToList()
                };

                var productItems = BuildPendingProductItems(newImportReceipt, request.Details, productDict, storeId, parsedUserId);

                await _ImportReceiptRepository.AddAsync(newImportReceipt, cancellationToken);

                if (productItems.Count > 0)
                {
                    await _productItemRepository.AddRangeAsync(productItems, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                await _unitOfWork.CommitAsync(cancellationToken);
                #endregion

                return new CreateImportReceiptResponse(true, "Tạo phiếu nhập hàng thành công.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi khi tạo phiếu nhập hàng.");
                return new CreateImportReceiptResponse(false, "Không thể tạo phiếu nhập hàng. Vui lòng thử lại.");
            }
        }

        private async Task<string?> ValidateSerialNumbersAsync(
            IEnumerable<CreateImportReceiptDetail> details,
            IReadOnlyDictionary<Guid, Product> productDict,
            CancellationToken cancellationToken)
        {
            var allSerials = new List<string>();

            foreach (var detail in details)
            {
                if (!productDict.TryGetValue(detail.ProductId, out var product))
                {
                    return $"San pham {detail.ProductId} khong ton tai.";
                }

                var duplicates = SerialNumberNormalizer.FindDuplicates(detail.SerialNumbers);
                if (duplicates.Count > 0)
                {
                    return $"SN bi trung trong dong san pham {product.Code}: {string.Join(", ", duplicates)}";
                }

                var serials = SerialNumberNormalizer.Normalize(detail.SerialNumbers);

                if (product.IsSerialTracked && serials.Count != detail.Quantity)
                {
                    return $"San pham {product.Code} bat buoc co so SN bang so luong nhap ({detail.Quantity}).";
                }

                if (!product.IsSerialTracked && serials.Count > 0)
                {
                    return $"San pham {product.Code} khong bat quan ly SN.";
                }

                allSerials.AddRange(serials);
            }

            var requestDuplicates = allSerials
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (requestDuplicates.Count > 0)
            {
                return $"SN bi trung trong phieu nhap: {string.Join(", ", requestDuplicates)}";
            }

            var existingItems = await _productItemRepository.GetBySerialNumbersAsync(allSerials, cancellationToken);
            if (existingItems.Count > 0)
            {
                return $"SN da ton tai: {string.Join(", ", existingItems.Select(x => x.SerialNumber))}";
            }

            return null;
        }

        private static List<ProductItem> BuildPendingProductItems(
            _ImportReceipt receipt,
            IEnumerable<CreateImportReceiptDetail> requestDetails,
            IReadOnlyDictionary<Guid, Product> productDict,
            Guid storeId,
            Guid userId)
        {
            var detailsByProductId = receipt.Details
                .GroupBy(x => x.ProductId)
                .ToDictionary(x => x.Key, x => new Queue<ImportReceiptDetail>(x));

            var items = new List<ProductItem>();

            foreach (var detailRequest in requestDetails)
            {
                var product = productDict[detailRequest.ProductId];
                if (!product.IsSerialTracked)
                    continue;

                var receiptDetail = detailsByProductId[detailRequest.ProductId].Dequeue();
                var serials = SerialNumberNormalizer.Normalize(detailRequest.SerialNumbers);

                items.AddRange(serials.Select(serial => new ProductItem
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    ProductId = detailRequest.ProductId,
                    SerialNumber = serial,
                    Status = ProductItemStatus.PendingImport,
                    ImportReceiptDetailId = receiptDetail.Id,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                }));
            }

            return items;
        }
    }
}
