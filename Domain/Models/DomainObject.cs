using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class DomainObject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
