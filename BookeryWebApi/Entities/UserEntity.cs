using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookeryWebApi.Entities
{
    public class UserEntity
    {
        [Key]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<ContainerEntity> Containers { get; set; }
    }
}
