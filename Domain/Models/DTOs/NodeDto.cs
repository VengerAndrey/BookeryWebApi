namespace Domain.Models.DTOs
{
    public class NodeDto : DomainObject
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int? OwnerId { get; set; }
    }
}
