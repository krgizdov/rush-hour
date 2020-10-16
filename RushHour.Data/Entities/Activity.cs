namespace RushHour.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Activity
    {
        public Activity()
        {
            AppointmentActivities = new HashSet<AppointmentActivity>();
            IsDeleted = false;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<AppointmentActivity> AppointmentActivities { get; set; }
    }
}
