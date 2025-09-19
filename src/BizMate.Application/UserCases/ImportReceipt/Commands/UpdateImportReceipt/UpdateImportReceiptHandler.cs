using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using BizMate.Domain.Entities;
using BizMate.Public.Message;
using BizMate.Application.Common.Interfaces;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateImportReceipt
{
    public class UpdateImportReceiptHandler : IRequestHandler<UpdateImportReceiptRequest, UpdateImportReceiptResponse>
    {
        private readonly IImportReceiptDetailRepository _detailRepository;
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UpdateImportReceiptHandler> _logger;

        #region constructor
        public UpdateImportReceiptHandler(
            IUnitOfWork unitOfWork,
            IImportReceiptDetailRepository detailRepository,
            IProductRepository productRepository,
            IUserSession userSession,
            IImportReceiptRepository ImportReceiptRepository,
            ILogger<UpdateImportReceiptHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _detailRepository = detailRepository;
            _productRepository = productRepository;
            _userSession = userSession;
            _importReceiptRepository = ImportReceiptRepository;
            _logger = logger;
        }
        #endregion
        public async Task<UpdateImportReceiptResponse> Handle(UpdateImportReceiptRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userSession.UserId;

                #region check ImportReceipt exist
                var importReceipt = await _importReceiptRepository.GetByIdAsync(request.Id, cancellationToken);
                if (importReceipt == null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new UpdateImportReceiptResponse(false, message);
                }
                #endregion

                #region Check rowversion
                if (importReceipt.RowVersion != request.RowVersion)
                {
                    var message = ValidationMessage.LocalizedStrings.NotValidRowversion;
                    _logger.LogWarning(message);
                    return new UpdateImportReceiptResponse(false, message);
                }
                #endregion

                #region Update via Stored Procedure
                importReceipt.SupplierName = request.SupplierName;
                importReceipt.DeliveryAddress = request.DeliveryAddress;
                importReceipt.Description = request.Description;
                importReceipt.UpdatedDate = DateTime.UtcNow;
                importReceipt.UpdatedBy = Guid.Parse(userId);
                // RowVersion sẽ do store tự sinh

                // Map details để truyền vào store
                var details = new List<ImportReceiptDetail>();
                foreach (var item in request.Details)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    details.Add(new ImportReceiptDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = product.Name,
                        ProductCode = product.Code,
                        Unit = product.Unit
                    });
                }

                await _importReceiptRepository.UpdateWithDetailsAsync(importReceipt, details, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                return new UpdateImportReceiptResponse(true, "Cập nhật phiếu nhập kho thành công.");
                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật phiếu nhập kho.");
                return new UpdateImportReceiptResponse(false, "Không thể cập nhật phiếu nhập kho. Vui lòng thử lại.");
            }
        }

    }
}