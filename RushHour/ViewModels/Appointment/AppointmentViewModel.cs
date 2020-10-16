namespace RushHour.ViewModels.Appointment
{
    using System;
    using System.Collections.Generic;

    public class AppointmentViewModel
    {
        public string Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<string> Activities { get; set; }
    }
}
