namespace RushHour.Data.Seeding
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using RushHour.Data.Entities;
    using RushHour.Domain;

    public class ApplicationDbContextSeeder
    {
        public void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            SeedRoles(roleManager);
            SeedUser(userManager, configuration, roleManager);
        }

        private void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }

            SeedRole(GlobalConstants.AdministratorRoleName, roleManager);
            SeedRole(GlobalConstants.UserRoleName, roleManager);
        }

        private void SeedRole(string roleName, RoleManager<ApplicationRole> roleManager)
        {
            var role = roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult();
            if (role == null)
            {
                var result = roleManager.CreateAsync(new ApplicationRole(roleName)).GetAwaiter().GetResult();

                if (!result.Succeeded)
                {
                    throw new ArgumentException(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }

        private void SeedUser(UserManager<ApplicationUser> userManager, IConfiguration configuration, RoleManager<ApplicationRole> roleManager)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            var defaultUser = configuration.GetSection(typeof(DefaultUserAdmin).Name).Get<DefaultUserAdmin>();

            var adminUser = new ApplicationUser
            {
                UserName = defaultUser.Username,
                Email = defaultUser.Email,
                FirstName = defaultUser.FirstName,
                LastName = defaultUser.LastName
            };

            var findUserResult = userManager.FindByNameAsync(adminUser.UserName).GetAwaiter().GetResult();

            if (findUserResult == null)
            {
                var userCreateResult = userManager.CreateAsync(adminUser, defaultUser.Password).GetAwaiter().GetResult();

                if (!userCreateResult.Succeeded)
                {
                    throw new ArgumentException(string.Join(Environment.NewLine, userCreateResult.Errors.Select(e => e.Description)));
                }

                foreach (var role in roleManager.Roles.Select(r => r.Name).ToList())
                {
                    var addToRole = userManager.AddToRoleAsync(adminUser, role).GetAwaiter().GetResult();

                    if (!addToRole.Succeeded)
                    {
                        throw new ArgumentException(string.Join(Environment.NewLine, addToRole.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
    }
}
