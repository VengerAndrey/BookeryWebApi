using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookeryWebApi.Entities
{
    public class BlobEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [ForeignKey("Container")]
        public Guid IdContainer { get; set; }

        public ContainerEntity Container { get; set; }
    }
}
