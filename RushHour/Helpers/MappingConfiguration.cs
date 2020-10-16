namespace RushHour.Helpers
{
    using AutoMapper;
    using RushHour.Data.Entities;
    using RushHour.Domain.Entities;
    using RushHour.ViewModels.Activity;
    using RushHour.ViewModels.Appointment;
    using System.Linq;

    public class MappingConfiguration : Profile
    {
        public MappingConfiguration()
        {
            CreateMap<ActivityDto, ActivityViewModel>();
            CreateMap<ActivityInput, ActivityDto>(); 
            CreateMap<ActivityUpdate, ActivityDto>();
            CreateMap<Activity, ActivityDto>().ForMember(dest => dest.AppointmentDtos, opt => opt.MapFrom(src => src.AppointmentActivities.Where(aa => !aa.IsDeleted).Select(aa => aa.Appointment)));
            CreateMap<ActivityDto, Activity>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AppointmentDto, AppointmentViewModel>().ForMember(dest => dest.Activities, opt => opt.MapFrom(src => src.ActivityDtos.Select(a => a.Name)));
            CreateMap<AppointmentDto, AppointmentAdminViewModel>().ForMember(dest => dest.Activities, opt => opt.MapFrom(src => src.ActivityDtos.Select(a => a.Name)));
            CreateMap<Appointment, AppointmentDto>().ForMember(dest => dest.ActivityDtos, opt => opt.MapFrom(src => src.AppointmentActivities.Where(aa => !aa.IsDeleted).Select(aa => aa.Activity)));
            CreateMap<AppointmentInput, AppointmentDto>();
            CreateMap<AppointmentDto, Appointment>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
