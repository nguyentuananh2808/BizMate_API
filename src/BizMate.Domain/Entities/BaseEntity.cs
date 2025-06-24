namespace BizMate.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public uint RowVersion { get; set; }
    }
}
