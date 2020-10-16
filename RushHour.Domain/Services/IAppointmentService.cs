namespace RushHour.Domain.Services
{
    using RushHour.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetPaginatedAppointmentsAsync(int page, int size);

        Task<IEnumerable<AppointmentDto>> GetPaginatedAppointmentsAsync(string userId, int page, int size);

        Task<AppointmentDto> GetAppointmentByIdAsync(string id, string userId);

        Task<AppointmentDto> GetAppointmentByIdAsync(string id);

        Task AddAppointmentAsync(string[] activityIds, AppointmentDto appointmentDto);

        Task UpdateAppointmentAsync(AppointmentDto appointmentDto);

        Task DeleteAppointmentAsync(string id);

        Task ConfigureAppointmentsUponActivityChangeAsync(ActivityDto activityDto);
    }
}
