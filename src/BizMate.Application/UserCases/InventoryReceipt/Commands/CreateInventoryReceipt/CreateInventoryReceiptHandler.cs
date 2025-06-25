using AutoMapper;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

public class CreateInventoryReceiptHandler : IRequestHandler<CreateInventoryReceiptRequest, CreateInventoryReceiptResponse>
{
    private readonly IInventoryReceiptRepository _inventoryReceiptRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ICodeGeneratorService _codeGeneratorService;
    private readonly IUserSession _userSession;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateInventoryReceiptHandler> _logger;
    private readonly IStringLocalizer<CreateInventoryReceiptHandler> _localizer;

    #region constructor
    public CreateInventoryReceiptHandler(
        IInventoryReceiptRepository inventoryReceiptRepository,
        IStockRepository stockRepository,
        ICodeGeneratorService codeGeneratorService,
        IUserSession userSession,
        IMapper mapper,
        ILogger<CreateInventoryReceiptHandler> logger,
        IStringLocalizer<CreateInventoryReceiptHandler> localizer)
    {
        _inventoryReceiptRepository = inventoryReceiptRepository;
        _stockRepository = stockRepository;
        _codeGeneratorService = codeGeneratorService;
        _userSession = userSession;
        _mapper = mapper;
        _logger = logger;
        _localizer = localizer;
    }
    #endregion

    public async Task<CreateInventoryReceiptResponse> Handle(CreateInventoryReceiptRequest request, CancellationToken cancellationToken)
    {
        return await CreateInventoryReceipt(request, cancellationToken);

    }

    private async Task<CreateInventoryReceiptResponse> CreateInventoryReceipt(CreateInventoryReceiptRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Begin transaction from repository
            await _inventoryReceiptRepository.BeginTransactionAsync();

            #region create inventory receipt
            var storeId = _userSession.StoreId;
            var userId = _userSession.UserId;

            var receiptCode = await _codeGeneratorService.GenerateCodeAsync(request.Type == 1 ? "NK" : "XK");

            var receipt = new InventoryReceipt
            {
                Id = Guid.NewGuid(),
                InventoryCode = receiptCode,
                Date = DateTime.UtcNow,
                Type = request.Type,
                StoreId = storeId,
                CreatedByUserId = userId,
                SupplierName = request.SupplierName,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                DeliveryAddress = request.DeliveryAddress,
                Description = request.Description,
                Details = request.Details.Select(d => new InventoryReceiptDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    ProductName = d.ProductName,
                    Unit = d.Unit
                }).ToList()
            };

            await _inventoryReceiptRepository.AddAsync(receipt);

            #endregion

            #region inventory handling
            foreach (var detail in receipt.Details)
            {
                var stock = await _stockRepository.GetByStoreAndProductAsync(storeId, detail.ProductId);

                if (stock == null)
                {
                    if (receipt.Type == 2)
                        throw new InvalidOperationException($"Tồn kho sản phẩm không đủ : {detail.ProductName}");

                    stock = new Stock
                    {
                        Id = Guid.NewGuid(),
                        StoreId = storeId,
                        ProductId = detail.ProductId,
                        Quantity = 0,
                        LastUpdated = DateTime.UtcNow
                    };
                    await _stockRepository.AddAsync(stock);
                }

                if (receipt.Type == 1) // Import
                {
                    stock.Quantity += detail.Quantity;
                }
                else if (receipt.Type == 2) // Export
                {
                    if (stock.Quantity < detail.Quantity)
                        throw new InvalidOperationException($"Tồn kho sản phẩm không đủ: {detail.ProductName}");

                    stock.Quantity -= detail.Quantity;
                }

                stock.LastUpdated = DateTime.UtcNow;
                await _stockRepository.UpdateAsync(stock);
            }
            #endregion
            await _inventoryReceiptRepository.CommitTransactionAsync();

            var response = _mapper.Map<CreateInventoryReceiptResponse>(receipt);
            return response;
        }
        catch (Exception ex)
        {
            await _inventoryReceiptRepository.RollbackTransactionAsync();
            _logger.LogError(ex, "Lỗi tạo phiếu.");
            return new CreateInventoryReceiptResponse
            {
                InventoryCode = string.Empty,
                Description = _localizer["Không thể tạo biên lai. Vui lòng thử lại."]
            };
        }
    }
}
