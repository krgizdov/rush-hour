namespace RushHour.ViewModels.Activity
{
    using System.ComponentModel.DataAnnotations;

    public class ActivityUpdate
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
