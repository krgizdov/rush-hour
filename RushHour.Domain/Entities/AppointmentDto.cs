namespace RushHour.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class AppointmentDto
    {
        public string Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ApplicationUserId { get; set; }

        public ICollection<ActivityDto> ActivityDtos { get; set; }
    }
}
