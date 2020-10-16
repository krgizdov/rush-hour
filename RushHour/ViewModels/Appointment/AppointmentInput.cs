namespace RushHour.ViewModels.Appointment
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AppointmentInput
    {
        [Required]
        public DateTime StartDate { get; set; }
    }
}
