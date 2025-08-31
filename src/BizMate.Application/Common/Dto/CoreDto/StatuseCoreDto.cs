namespace BizMate.Application.Common.Dto.CoreDto
{
    public class StatuseCoreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Group { get; set; } = default!;
        public string? Description { get; set; }
        public string? Code { get; set; }
    }
}
