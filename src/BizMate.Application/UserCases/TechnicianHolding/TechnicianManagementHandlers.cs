using BizMate.Application.Common.Interfaces;
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

    public class CreateTechnicianRequest : IRequest<TechnicianMutationResponse>
    {
        public string Name { get; set; } = default!;
        public string? Phone { get; set; }
        public string? ZaloPhone { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateTechnicianRequest : IRequest<TechnicianMutationResponse>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Phone { get; set; }
        public string? ZaloPhone { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class TechnicianMutationResponse : BaseResponse
    {
        public Guid? TechnicianId { get; set; }

        public TechnicianMutationResponse(Guid technicianId, string message) : base(true, message)
        {
            TechnicianId = technicianId;
        }

        public TechnicianMutationResponse(bool success = false, string message = "")
            : base(success, message) { }
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

    public class CreateTechnicianHandler : IRequestHandler<CreateTechnicianRequest, TechnicianMutationResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;
        private readonly ICodeGeneratorService _codeGenerator;
        private readonly IUnitOfWork _uow;

        public CreateTechnicianHandler(
            ITechnicianHoldingRepository repo,
            IUserSession userSession,
            ICodeGeneratorService codeGenerator,
            IUnitOfWork uow)
        {
            _repo = repo;
            _userSession = userSession;
            _codeGenerator = codeGenerator;
            _uow = uow;
        }

        public async Task<TechnicianMutationResponse> Handle(CreateTechnicianRequest request, CancellationToken ct)
        {
            var name = request.Name?.Trim();
            var phone = request.Phone?.Trim();
            var zaloPhone = request.ZaloPhone?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return new TechnicianMutationResponse(false, "Ten ky thuat khong duoc de trong.");
            if (!string.IsNullOrWhiteSpace(phone)
                && await _repo.ExistsTechnicianPhoneAsync(_userSession.StoreId, phone, null, ct))
                return new TechnicianMutationResponse(false, "So dien thoai ky thuat da ton tai.");

            var userId = Guid.TryParse(_userSession.UserId, out var parsedUserId) ? parsedUserId : (Guid?)null;
            var technician = new Technician
            {
                Id = Guid.NewGuid(),
                StoreId = _userSession.StoreId,
                Code = await _codeGenerator.GenerateCodeAsync("#KT", 5),
                Name = name,
                Phone = phone,
                ZaloPhone = zaloPhone,
                IsActive = request.IsActive,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            _repo.AddTechnician(technician);
            await _uow.SaveChangesAsync(ct);

            return new TechnicianMutationResponse(technician.Id, "Tao ky thuat thanh cong.");
        }
    }

    public class UpdateTechnicianHandler : IRequestHandler<UpdateTechnicianRequest, TechnicianMutationResponse>
    {
        private readonly ITechnicianHoldingRepository _repo;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _uow;

        public UpdateTechnicianHandler(
            ITechnicianHoldingRepository repo,
            IUserSession userSession,
            IUnitOfWork uow)
        {
            _repo = repo;
            _userSession = userSession;
            _uow = uow;
        }

        public async Task<TechnicianMutationResponse> Handle(UpdateTechnicianRequest request, CancellationToken ct)
        {
            if (request.Id == Guid.Empty)
                return new TechnicianMutationResponse(false, "TechnicianId khong hop le.");

            var name = request.Name?.Trim();
            var phone = request.Phone?.Trim();
            var zaloPhone = request.ZaloPhone?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return new TechnicianMutationResponse(false, "Ten ky thuat khong duoc de trong.");

            var technician = await _repo.GetTechnicianAsync(request.Id, _userSession.StoreId, ct);
            if (technician is null)
                return new TechnicianMutationResponse(false, "Khong tim thay ky thuat trong store hien tai.");

            if (!string.IsNullOrWhiteSpace(phone)
                && await _repo.ExistsTechnicianPhoneAsync(_userSession.StoreId, phone, request.Id, ct))
                return new TechnicianMutationResponse(false, "So dien thoai ky thuat da ton tai.");

            technician.Name = name;
            technician.Phone = phone;
            technician.ZaloPhone = zaloPhone;
            technician.IsActive = request.IsActive;
            technician.UpdatedDate = DateTime.UtcNow;
            technician.UpdatedBy = Guid.TryParse(_userSession.UserId, out var userId) ? userId : null;

            await _uow.SaveChangesAsync(ct);
            return new TechnicianMutationResponse(technician.Id, "Cap nhat ky thuat thanh cong.");
        }
    }
}
