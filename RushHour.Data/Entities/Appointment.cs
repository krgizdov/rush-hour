namespace RushHour.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Appointment
    {
        public Appointment()
        {
            AppointmentActivities = new HashSet<AppointmentActivity>();
            IsDeleted = false;
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsDeleted { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<AppointmentActivity> AppointmentActivities { get; set; }
    }
}
