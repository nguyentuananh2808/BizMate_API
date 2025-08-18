using BizMate.Application.UserCases.DealerLevel.Commands.CreateDealerLevel;

namespace BizMate.Api.UserCases.DealerLevel.CreateDealerLevel
{
    public class CreateDealerLevelResponseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;

        public CreateDealerLevelResponseViewModel(CreateDealerLevelResponse response)
        {
            Id = response.DealerLevel.Id;
            Name = response.DealerLevel.Name;
        }

    }
}
