using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
