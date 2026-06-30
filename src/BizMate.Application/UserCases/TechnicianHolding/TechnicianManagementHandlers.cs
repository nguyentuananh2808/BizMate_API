using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Responses;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;

namespace BizMate.Application.UserCases.TechnicianHolding
{
    public class GetTechniciansRequest : IRequest<GetTechniciansResponse>
    {
        public string? Keyword { get; set; }
        public bool IncludeInactive { get; set; }
    }

    public class GetTechniciansResponse : BaseResponse
    {
        public List<TechnicianDto> Technicians { get; set; } = new();

        public GetTechniciansResponse(List<TechnicianDto> technicians) : base(true)
        {
            Technicians = technicians;
        }

        public GetTechniciansResponse(bool success = false, string message = "")
            : base(success, message) { }
    }

    public class TechnicianDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Phone { get; set; }
        public string? ZaloPhone { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetTechniciansHandler : IRequestHandler<GetTechniciansRequest, GetTechniciansResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;

        public GetTechniciansHandler(ITechnicianHoldingRepository repo, IUserSession userSession)
        {
            _repo = repo;
            _userSession = userSession;
        }

        public async Task<GetTechniciansResponse> Handle(GetTechniciansRequest request, CancellationToken ct)
        {
            var technicians = await _repo.SearchTechniciansAsync(
                _userSession.StoreId,
                request.Keyword,
                request.IncludeInactive,
                ct);

            return new GetTechniciansResponse(technicians.Select(ToDto).ToList());
        }

        private static TechnicianDto ToDto(Technician technician)
            => new()
            {
                Id = technician.Id,
                Code = technician.Code,
                Name = technician.Name,
                Phone = technician.Phone,
                ZaloPhone = technician.ZaloPhone,
                IsActive = technician.IsActive
            };
    }

}
