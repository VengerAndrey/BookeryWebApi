using System.Collections.Generic;
using Domain.Models.DTOs;

namespace Domain.Models
{
    public class Node : DomainObject
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int? OwnerId { get; set; }
        public User Owner { get; set; }
        public Node Parent { get; set; }
        public ICollection<Node> SubNodes { get; } = new List<Node>();

        public NodeDto ToDto()
        {
            return new NodeDto {Id = Id, Name = Name, OwnerId = OwnerId, ParentId = ParentId};
        }
    }
}
