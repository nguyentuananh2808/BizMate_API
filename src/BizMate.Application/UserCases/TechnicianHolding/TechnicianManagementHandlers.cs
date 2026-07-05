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

    public class GetBorrowableEmployeesRequest : IRequest<GetBorrowableEmployeesResponse>
    {
        public string? Keyword { get; set; }
        public bool IncludeInactive { get; set; }
        public int PageSize { get; set; } = 100;
    }

    public class GetBorrowableEmployeesResponse : BaseResponse
    {
        public List<BorrowableEmployeeDto> Employees { get; set; } = new();

        public GetBorrowableEmployeesResponse(List<BorrowableEmployeeDto> employees)
            : base(true)
        {
            Employees = employees;
        }

        public GetBorrowableEmployeesResponse(bool success = false, string message = "")
            : base(success, message) { }
    }

    public class BorrowableEmployeeDto
    {
        public Guid UserId { get; set; }
        public Guid? TechnicianId { get; set; }
        public string Code { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
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

    public class GetBorrowableEmployeesHandler(
        IUserRepository userRepository,
        ITechnicianHoldingRepository technicianRepository,
        IUserSession userSession)
        : IRequestHandler<GetBorrowableEmployeesRequest, GetBorrowableEmployeesResponse>
    {
        public async Task<GetBorrowableEmployeesResponse> Handle(
            GetBorrowableEmployeesRequest request,
            CancellationToken ct)
        {
            try
            {
                var pageSize = request.PageSize <= 0
                    ? 100
                    : Math.Min(request.PageSize, 200);

                var (users, _) = await userRepository.SearchUsersWithPagingAsync(
                    userSession.StoreId,
                    request.Keyword,
                    0,
                    pageSize,
                    request.IncludeInactive ? null : true,
                    ct);

                var userIds = users.Select(x => x.Id).ToList();
                var technicians = await technicianRepository.GetTechniciansByUserIdsAsync(
                    userSession.StoreId,
                    userIds,
                    ct);
                var technicianByUserId = technicians
                    .Where(x => x.UserId.HasValue)
                    .ToDictionary(x => x.UserId!.Value);

                var employees = users
                    .OrderBy(x => x.FullName)
                    .Select(user =>
                    {
                        technicianByUserId.TryGetValue(user.Id, out var technician);
                        return new BorrowableEmployeeDto
                        {
                            UserId = user.Id,
                            TechnicianId = technician?.Id,
                            Code = user.Code,
                            FullName = user.FullName,
                            Email = user.Email,
                            Role = user.Role,
                            IsActive = user.IsActive
                        };
                    })
                    .ToList();

                return new GetBorrowableEmployeesResponse(employees);
            }
            catch (Exception ex)
            {
                return new GetBorrowableEmployeesResponse(
                    false,
                    ex.Message);
            }
        }
    }

}
