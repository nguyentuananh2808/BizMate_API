using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using _ImportReceipt = BizMate.Domain.Entities.ImportReceipt;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.CreateImportReceipt
{
    public class CreateImportReceiptHandler : IRequestHandler<CreateImportReceiptRequest, CreateImportReceiptResponse>
    {
        private readonly IImportReceiptRepository _ImportReceiptRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ILogger<CreateImportReceiptHandler> _logger;

        #region constructor
        public CreateImportReceiptHandler(
            IStatusRepository statusRepository,
            IProductRepository productRepository,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IImportReceiptRepository ImportReceiptRepository,
            ILogger<CreateImportReceiptHandler> logger)
        {
            _productRepository = productRepository;
            _statusRepository = statusRepository;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _ImportReceiptRepository = ImportReceiptRepository;
            _logger = logger;
        }
        #endregion
        public async Task<CreateImportReceiptResponse> Handle(CreateImportReceiptRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;


                #region create new ImportReceipt
                var productIds = request.Details.Select(d => d.ProductId).ToList();

                var products = await _productRepository.GetByIdsAsync(productIds);

                var productDict = products.ToDictionary(p => p.Id);

                //get status for ImportReceipt
                var statusId = await _statusRepository.GetStatusByCode("NEW", "ImportReceipt");
                if (statusId == Guid.Empty)
                {
                    _logger.LogError("Không tìm thấy trạng thái 'tạo mới' cho phiếu nhập hàng.");
                    return new CreateImportReceiptResponse(false, "Không thể tạo phiếu nhập hàng. Trạng thái không hợp lệ.");
                }
                var receiptCode = await _codeGeneratorService.GenerateCodeAsync("#NK");

                var receiptId = Guid.NewGuid();
                var newImportReceipt = new _ImportReceipt
                {
                    Id = receiptId,
                    Code = receiptCode,
                    StoreId = storeId,
                    CreatedBy = Guid.Parse(userId),
                    SupplierName = request.SupplierName,
                    DeliveryAddress = request.DeliveryAddress,
                    StatusId = statusId,
                    Description = request.Description,
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

                await _ImportReceiptRepository.AddAsync(newImportReceipt, cancellationToken);
                #endregion

                return new CreateImportReceiptResponse(true, "Tạo phiếu nhập hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phiếu nhập hàng.");
                return new CreateImportReceiptResponse(false, "Không thể tạo phiếu nhập hàng. Vui lòng thử lại.");
            }
        }
    }
}
