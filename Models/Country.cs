using System.ComponentModel.DataAnnotations;

namespace Medals_API.Models
{
    public class Country
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze{ get; set; }
    }
}
