//using BizMate.Application.Common.Requests.Validators;
//using FluentValidation;
//using Microsoft.Extensions.Localization;

//namespace BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipts.Validators
//{
//    public class GetInventoryReceiptsRequestValidator : AbstractValidator<GetInventoryReceiptsRequest>
//    {
//        public GetInventoryReceiptsRequestValidator(IStringLocalizer<GetInventoryReceiptsRequest> localizer)
//        {
//            Include(new SearchCoreValidator(localizer));
//            RuleFor(x => x.Type)
//                .Must(type => type == null || type == 1 || type == 2)
//                .WithMessage("Type must be null, 1 (Import), or 2 (Export).");
//        }

//    }
//}
