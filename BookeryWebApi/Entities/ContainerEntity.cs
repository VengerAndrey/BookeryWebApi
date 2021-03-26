using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookeryWebApi.Entities
{
    public class ContainerEntity
    {
        [Key] 
        public Guid Id { get; set; }
        [Required] 
        public string Name { get; set; }

        public ICollection<BlobEntity> Blobs { get; set; }
    }
}
