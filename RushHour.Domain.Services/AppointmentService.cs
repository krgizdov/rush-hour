namespace RushHour.Domain.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using RushHour.Data.Entities;
    using RushHour.Domain.Entities;
    using RushHour.Domain.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AppointmentService : IAppointmentService
    {
        private readonly IBaseRepository<Appointment> _appointmentRepo;
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;

        public AppointmentService(IBaseRepository<Appointment> appointmentRepo, IMapper mapper, IActivityService activityService)
        {
            _appointmentRepo = appointmentRepo;
            _mapper = mapper;
            _activityService = activityService;
        }

        public async Task<IEnumerable<AppointmentDto>> GetPaginatedAppointmentsAsync(int page, int size)
        {
            return await _appointmentRepo.GetAllPaginated(page, size)
                .Where(a => !a.IsDeleted)
                .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentDto>> GetPaginatedAppointmentsAsync(string userId, int page, int size)
        {
            return await _appointmentRepo.GetAllPaginated(page, size)
                .Where(a => a.ApplicationUserId == userId && !a.IsDeleted)
                .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<AppointmentDto> GetAppointmentByIdAsync(string id)
        {
            var appointment = await GetAppointmentEntityAsync(id);

            if (appointment == null)
            {
                return null;
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto> GetAppointmentByIdAsync(string id, string userId)
        {
            var appointment = await _appointmentRepo.GetEntityAsync(a => a.Id.ToString() == id && a.ApplicationUserId == userId && !a.IsDeleted);

            if (appointment == null)
            {
                return null;
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task AddAppointmentAsync(string[] activityIds, AppointmentDto appointmentDto)
        {
            var activityDtos = new List<ActivityDto>();

            foreach (var activityId in activityIds)
            {
                var activityDto = await _activityService.GetActivityByIdAsync(activityId);

                if (activityDto == null)
                {
                    throw new ArgumentException($"Activity with id: {activityId} was not found!");
                }

                activityDtos.Add(activityDto);
            }

            appointmentDto.EndDate = appointmentDto.StartDate.AddMinutes(activityDtos.Sum(a => a.Duration));

            if (appointmentDto.EndDate.Hour < 9 || appointmentDto.EndDate.Hour > 21)
            {
                throw new ArgumentException("Please make an appointment for hours from 09:00 to 21:00!");
            }

            var appointment = _mapper.Map<Appointment>(appointmentDto);

            await _appointmentRepo.AddAsync(appointment);

            foreach (var activityDto in activityDtos)
            {
                CheckForOverlappingAppointments(appointment, activityDto);

                var appActivity = new AppointmentActivity
                {
                    AppointmentId = appointment.Id,
                    ActivityId = Guid.Parse(activityDto.Id)
                };

                appointment.AppointmentActivities.Add(appActivity);
            }

            await _appointmentRepo.SaveChangesAsync();

            _mapper.Map(appointment, appointmentDto);
        }

        public async Task UpdateAppointmentAsync(AppointmentDto appointmentDto)
        {
            var appointment = await GetAppointmentEntityAsync(appointmentDto.Id);

            _mapper.Map(appointmentDto, appointment);

            appointment.EndDate = appointment.StartDate.AddMinutes(appointment.AppointmentActivities.Sum(appAct => appAct.Activity.Duration));

            if (appointmentDto.EndDate.Hour < 9 || appointmentDto.EndDate.Hour > 21)
            {
                throw new ArgumentException("Please make an appointment for hours from 09:00 to 21:00!");
            }

            foreach (var appActivity in appointment.AppointmentActivities)
            {
                var activityDto = await _activityService.GetActivityByIdAsync(appActivity.ActivityId.ToString());

                CheckForOverlappingAppointments(appointment, activityDto);
            }

            _appointmentRepo.Update(appointment);

            await _appointmentRepo.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(string id)
        {
            var appointment = await GetAppointmentEntityAsync(id);

            appointment.IsDeleted = true;

            foreach (var appActivity in appointment.AppointmentActivities.Where(ap => !ap.IsDeleted))
            {
                appActivity.IsDeleted = true;
            }

            await _appointmentRepo.SaveChangesAsync();
        }

        public async Task ConfigureAppointmentsUponActivityChangeAsync(ActivityDto activityDto)
        {
            foreach (var appointmentDto in activityDto.AppointmentDtos)
            {
                var appointment = await GetAppointmentEntityAsync(appointmentDto.Id);

                var activitiesInAppointment = appointment.AppointmentActivities.Where(apac => !apac.IsDeleted).ToList();

                if (activitiesInAppointment.Count == 1)
                {
                    await DeleteAppointmentAsync(appointmentDto.Id);
                }
                else if (activitiesInAppointment.Count > 1)
                {
                    appointment.EndDate = appointment.EndDate.AddMinutes(-activityDto.Duration);

                    var appActivity = appointment.AppointmentActivities.FirstOrDefault(appAct => appAct.ActivityId.ToString() == activityDto.Id);

                    appActivity.IsDeleted = true;

                    await _appointmentRepo.SaveChangesAsync();
                }
            }
        }

        private void CheckForOverlappingAppointments(Appointment appointment, ActivityDto activityDto)
        {
            foreach (var appointmentDto in activityDto.AppointmentDtos.Where(apDto => apDto.Id != appointment.Id.ToString()))
            {
                if (appointment.StartDate < appointmentDto.EndDate && appointmentDto.StartDate < appointment.EndDate)
                {
                    throw new ArgumentException($"Cannot make an appointment for {activityDto.Name} for the specific date!");
                }
            }
        }

        private async Task<Appointment> GetAppointmentEntityAsync(string id)
        {
            return  await _appointmentRepo.GetEntityAsync(a => a.Id.ToString() == id && !a.IsDeleted);
        }
    }
}
