namespace RushHour.Domain.Services
{
    using RushHour.Domain.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IActivityService
    {
        Task<IEnumerable<ActivityDto>> GetPaginatedActivitiesAsync(int page, int size);

        Task<ActivityDto> GetActivityByIdAsync(string id);

        Task AddActivityAsync(ActivityDto activityDto);

        Task UpdateActivityAsync(ActivityDto activityDto);

        Task DeleteActivityAsync(string id);
    }
}
