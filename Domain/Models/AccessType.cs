namespace Domain.Models
{
    public class AccessType
    {
        public AccessTypeId AccessTypeId { get; set; }
        public string Name { get; set; }
    }

    public enum AccessTypeId : int
    {
        Read = 0,
        Write = 1
    }
}
