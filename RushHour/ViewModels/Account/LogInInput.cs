namespace RushHour.ViewModels.Account
{
    using System.ComponentModel.DataAnnotations;

    public class LogInInput
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
