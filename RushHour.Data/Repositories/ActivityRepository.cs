namespace RushHour.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using RushHour.Data.Entities;
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class ActivityRepository : BaseRepository<Activity>
    {
        public ActivityRepository(ApplicationDbContext context)
            : base(context)
        { }

        public override Task<Activity> GetEntityAsync(Expression<Func<Activity, bool>> filter)
        {
            return _dbSet
                .Include(a => a.AppointmentActivities)
                .ThenInclude(apac => apac.Appointment)
                .FirstOrDefaultAsync(filter);
        }
    }
}
