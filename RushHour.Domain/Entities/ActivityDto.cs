namespace RushHour.Domain.Entities
{
    using System.Collections.Generic;

    public class ActivityDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Duration { get; set; }

        public decimal Price { get; set; }

        public ICollection<AppointmentDto> AppointmentDtos { get; set; }
    }
}
