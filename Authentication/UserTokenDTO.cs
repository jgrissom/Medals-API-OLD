using System.ComponentModel.DataAnnotations;

public class UserTokenDTO
{
    [Required]
    public string Token { get; set; }
    [Required]
    public string Expiration { get; set; }
}
