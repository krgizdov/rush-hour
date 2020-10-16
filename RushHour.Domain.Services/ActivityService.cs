namespace RushHour.Domain.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using RushHour.Data.Entities;
    using RushHour.Domain.Entities;
    using RushHour.Domain.Repositories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ActivityService : IActivityService
    {
        private readonly IBaseRepository<Activity> _activityRepo;
        private readonly IMapper _mapper;

        public ActivityService(IBaseRepository<Activity> activityRepo, IMapper mapper)
        {
            _activityRepo = activityRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActivityDto>> GetPaginatedActivitiesAsync(int page, int size)
        {
            return await _activityRepo.GetAllPaginated(page, size)
                .Where(a => !a.IsDeleted)
                .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ActivityDto> GetActivityByIdAsync(string id)
        {
            var activity = await GetActivityEntityAsync(id);

            if (activity == null)
            {
                return null;
            }

            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task AddActivityAsync(ActivityDto activityDto)
        {
            var activity = _mapper.Map<Activity>(activityDto);

            await _activityRepo.AddAsync(activity);

            await _activityRepo.SaveChangesAsync();

            activityDto.Id = activity.Id.ToString();
        }

        public async Task UpdateActivityAsync(ActivityDto activityDto)
        {
            var activity = await GetActivityEntityAsync(activityDto.Id);

            _mapper.Map(activityDto, activity);

            _activityRepo.Update(activity);

            await _activityRepo.SaveChangesAsync();
        }

        public async Task DeleteActivityAsync(string id)
        {
            var activity = await GetActivityEntityAsync(id);

            activity.IsDeleted = true;

            await _activityRepo.SaveChangesAsync();
        }

        private async Task<Activity> GetActivityEntityAsync(string id)
        {
            return await _activityRepo.GetEntityAsync(a => a.Id.ToString() == id && !a.IsDeleted);
        }
    }
}
