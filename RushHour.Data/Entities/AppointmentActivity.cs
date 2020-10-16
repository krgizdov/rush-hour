namespace RushHour.Data.Entities
{
    using System;

    public class AppointmentActivity
    {
        public AppointmentActivity()
        {
            IsDeleted = false;
        }

        public Guid AppointmentId { get; set; }

        public Appointment Appointment { get; set; }

        public Guid ActivityId { get; set; }

        public Activity Activity { get; set; }

        public bool IsDeleted { get; set; }
    }
}
