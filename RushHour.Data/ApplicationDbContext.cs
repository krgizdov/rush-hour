namespace RushHour.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using RushHour.Data.Entities;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Property(u => u.UserName).HasMaxLength(100);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Appointments)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey(a => a.ApplicationUserId);

            builder.Entity<AppointmentActivity>()
                .HasKey(k => new { k.AppointmentId, k.ActivityId });

            builder.Entity<AppointmentActivity>()
                .HasOne(apac => apac.Appointment)
                .WithMany(a => a.AppointmentActivities)
                .HasForeignKey(a => a.AppointmentId);

            builder.Entity<AppointmentActivity>()
               .HasOne(apac => apac.Activity)
               .WithMany(a => a.AppointmentActivities)
               .HasForeignKey(a => a.ActivityId);
        }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<AppointmentActivity> AppointmentActivities { get; set; }
    }
}
