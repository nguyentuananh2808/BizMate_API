using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using BizMate.Domain.Entities;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateImportReceipt
{
    public class UpdateImportReceiptHandler : IRequestHandler<UpdateImportReceiptRequest, UpdateImportReceiptResponse>
    {
        private readonly IImportReceiptDetailRepository _detailRepository;
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UpdateImportReceiptHandler> _logger;

        #region constructor
        public UpdateImportReceiptHandler(
            IImportReceiptDetailRepository detailRepository,
            IProductRepository productRepository,
            IUserSession userSession,
            IImportReceiptRepository ImportReceiptRepository,
            ILogger<UpdateImportReceiptHandler> logger)
        {
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
                var importReceipt = await _importReceiptRepository.GetByIdAsync(request.Id);
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

                #region update data
                // Cập nhật phiếu
                importReceipt.SupplierName = request.SupplierName;
                importReceipt.DeliveryAddress = request.DeliveryAddress;
                importReceipt.Description = request.Description;
                importReceipt.UpdatedDate = DateTime.UtcNow;
                importReceipt.UpdatedBy = Guid.Parse(userId);
                importReceipt.RowVersion = Guid.NewGuid();

                // Xóa và thêm lại chi tiết
                var existingDetails = await _detailRepository.GetByReceiptIdAsync(request.Id);

                // So sánh
                var requestProductIds = request.Details.Select(d => d.ProductId).ToList();

                // Update hoặc Add
                foreach (var item in request.Details)
                {
                    var existing = existingDetails.FirstOrDefault(d => d.ProductId == item.ProductId);
                    if (existing != null)
                    {
                        existing.Quantity = item.Quantity;
                        await _detailRepository.UpdateAsync(existing);
                    }
                    else
                    {
                        var product = await _productRepository.GetByIdAsync(item.ProductId);
                        await _detailRepository.AddAsync(new ImportReceiptDetail
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            ImportReceiptId = request.Id,
                            ProductName = product.Name,
                            ProductCode = product.Code,
                            Unit = product.Unit
                        });
                    }
                }

                // Xóa
                var toDelete = existingDetails.Where(d => !requestProductIds.Contains(d.ProductId)).ToList();
                await _detailRepository.DeleteRangeAsync(toDelete.Select(x => x.Id).ToList());



                await _importReceiptRepository.UpdateAsync(importReceipt);
                #endregion

                return new UpdateImportReceiptResponse(true, "Cập nhật phiếu nhập kho thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật phiếu nhập kho.");
                return new UpdateImportReceiptResponse(false, "Không thể cập nhật phiếu nhập kho. Vui lòng thử lại.");
            }
        }
    }
}
