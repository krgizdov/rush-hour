namespace RushHour.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using RushHour.Data.Entities;
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class AppointmentRepository : BaseRepository<Appointment>
    {
        public AppointmentRepository(ApplicationDbContext context)
            : base(context)
        { }

        public override Task<Appointment> GetEntityAsync(Expression<Func<Appointment, bool>> filter)
        {
            return _dbSet
                .Include(a => a.AppointmentActivities)
                .ThenInclude(apac => apac.Activity)
                .FirstOrDefaultAsync(filter);
        }
    }
}
