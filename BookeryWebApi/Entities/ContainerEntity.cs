using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookeryWebApi.Entities
{
    public class ContainerEntity
    {
        [Key] 
        public Guid Id { get; set; }
        [Required] 
        public string Name { get; set; }
        [Required] 
        [ForeignKey("Owner")]
        public string OwnerLogin { get; set; }
        
        public UserEntity Owner { get; set; }
        public ICollection<BlobEntity> Blobs { get; set; }
    }
}
