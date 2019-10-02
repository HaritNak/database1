using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace database1.Data
{
    public class SampleData
    {
        public static async Task InitializeASync(IServiceProvider serviceProvider)
        {
           var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            string[] roles = new string[] { "Adminstrator", "User" };

            foreach (var role in roles)
            {
                var isExist = await roleManager.RoleExistsAsync(role);
                if(!isExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUser = new IdentityUser
            {
                Email = "xunxusvongola@gmail.com",
                UserName = "xunxusvongola@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var currentUser = await userManager.FindByEmailAsync(adminUser.Email);
            if (currentUser == null)
            {
                await userManager.CreateAsync(adminUser, "Secret123!");
                currentUser = await userManager.FindByNameAsync(adminUser.Email);
            }
            var isAdmin =  await userManager.IsInRoleAsync(currentUser, "Adminstrator");
            if (!isAdmin)
            {
                await userManager.AddToRolesAsync(currentUser, roles);
            }
            var containSamplebook = await dbContext.Books.AnyAsync(b => b.Name == "Sample Book");
            if (!containSamplebook)
            {
                dbContext.Books.Add(new Models.Book
                {
                    Name = "Sample Book",
                    Price = 100m
                });
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
