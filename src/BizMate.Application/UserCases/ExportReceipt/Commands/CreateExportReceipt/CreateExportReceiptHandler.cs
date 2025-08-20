using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _ExportReceipt = BizMate.Domain.Entities.ExportReceipt;

namespace BizMate.Application.UserCases.ExportReceipt.Commands.CreateExportReceipt
{
    public class CreateExportReceiptHandler : IRequestHandler<CreateExportReceiptRequest, CreateExportReceiptResponse>
    {
        private readonly IExportReceiptRepository _exportReceiptRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ILogger<CreateExportReceiptHandler> _logger;

        #region constructor
        public CreateExportReceiptHandler(
            IStatusRepository statusRepository,
            IProductRepository productRepository,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IExportReceiptRepository ExportReceiptRepository,
            ILogger<CreateExportReceiptHandler> logger)
        {
            _productRepository = productRepository;
            _statusRepository = statusRepository;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _exportReceiptRepository = ExportReceiptRepository;
            _logger = logger;
        }
        #endregion
        public async Task<CreateExportReceiptResponse> Handle(CreateExportReceiptRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;


                #region create new ExportReceipt
                var productIds = request.Details.Select(d => d.ProductId).ToList();

                var products = await _productRepository.GetByIdsAsync(productIds);

                var productDict = products.ToDictionary(p => p.Id);

                //get status for ExportReceipt
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync("NEW", "ExportReceipt");
                if (statusId == Guid.Empty)
                {
                    _logger.LogError("Không tìm thấy trạng thái 'tạo mới' cho phiếu xuất hàng.");
                    return new CreateExportReceiptResponse(false, "Không thể tạo phiếu xuất hàng. Trạng thái không hợp lệ.");
                }
                var receiptCode = await _codeGeneratorService.GenerateCodeAsync("#NK");

                var receiptId = Guid.NewGuid();
                var newExportReceipt = new _ExportReceipt
                {
                    Id = receiptId,
                    Code = receiptCode,
                    StoreId = storeId,
                    CreatedBy = Guid.Parse(userId),
                    CustomerPhone = request.CustomerPhone,
                    CustomerName = request.CustomerName,
                    DeliveryAddress = request.DeliveryAddress,
                    StatusId = statusId,
                    Description = request.Description,
                    Details = request.Details.Select(d =>
                    {
                        var product = productDict[d.ProductId];

                        return new ExportReceiptDetail
                        {
                            Id = Guid.NewGuid(),
                            ExportReceiptId = receiptId,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            ProductName = product.Name,
                            ProductCode = product.Code,
                            Unit = product.Unit,
                        };
                    }).ToList()
                };

                await _exportReceiptRepository.AddAsync(newExportReceipt, cancellationToken);
                #endregion

                return new CreateExportReceiptResponse(true, "Tạo phiếu xuất hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phiếu xuất hàng.");
                return new CreateExportReceiptResponse(false, "Không thể tạo phiếu xuất hàng. Vui lòng thử lại.");
            }
        }
    }
}
