using System.ComponentModel.DataAnnotations;

namespace Medals_API.Authentication
{
    public class UserTokenDTO
    {
      [Required]
      public string Token { get; set; }
      [Required]
      public string Expiration { get; set; }
    }
}
