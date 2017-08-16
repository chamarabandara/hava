using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WebMVC.ModelViews;

namespace WebMVC.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
            : base()
        {
            this.Groups = new HashSet<ApplicationUserGroup>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<ApplicationUserGroup> Groups { get; set; }
    }

    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) { Name = name; }
    }

    public class ApplicationUserRole : IdentityUserRole<int>
    {
    }

    public class ApplicationUserLogin : IdentityUserLogin<int>
    {
    }

    public class ApplicationUserClaim : IdentityUserClaim<int>
    {
    }

    public class ApplicatonUserStore :
        UserStore<ApplicationUser, ApplicationRole, int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicatonUserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class ApplicatonRoleStore : RoleStore<ApplicationRole, int, ApplicationUserRole>
    {
        public ApplicatonRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    //Group Based Role Changes - Start

    public class ApplicationRoleGroup
    {
        public virtual int RoleId { get; set; }
        public virtual int GroupId { get; set; }

        public virtual ApplicationRole Role { get; set; }
        public virtual ApplicationGroup Group { get; set; }
    }

    public class ApplicationGroup
    {
        public ApplicationGroup() { }


        public ApplicationGroup(string name)
            : this()
        {
            this.Roles = new List<ApplicationRoleGroup>();
            this.Name = name;
        }


        [Key]
        [Required]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
        public virtual ICollection<ApplicationRoleGroup> Roles { get; set; }


    }

    public class ApplicationUserGroup
    {
        [Required]
        public virtual int UserId { get; set; }
        [Required]
        public virtual int GroupId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationGroup Group { get; set; }
    }

    //Group Based Role Changes - End


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserLogin,
                                                            ApplicationUserRole, ApplicationUserClaim>
    {
        // ADD THIS:
        public virtual IDbSet<ApplicationGroup> ApplicationGroups { get; set; }

        public ApplicationDbContext()
            : base("IdentityConnection")
        {
        }
        public DbSet<AspNetClient> Clients { get; set; }
        public DbSet<AspNetRefreshToken> RefreshTokens { get; set; }
        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            // Add the group stuff here:
            modelBuilder.Entity<ApplicationUser>().HasMany<ApplicationUserGroup>((ApplicationUser u) => u.Groups);
            modelBuilder.Entity<ApplicationUserGroup>().HasKey((ApplicationUserGroup r) =>
                new { UserId = r.UserId, GroupId = r.GroupId }).ToTable("ApplicationUserGroups");
            // And here:
            modelBuilder.Entity<ApplicationGroup>().HasMany<ApplicationRoleGroup>((ApplicationGroup g) => g.Roles);
            modelBuilder.Entity<ApplicationRoleGroup>().HasKey((ApplicationRoleGroup gr) =>
                new { RoleId = gr.RoleId, GroupId = gr.GroupId }).ToTable("ApplicationRoleGroups");
            // And Here:
            EntityTypeConfiguration<ApplicationGroup> groupsConfig = modelBuilder.Entity<ApplicationGroup>().ToTable("ApplicationGroups");
            groupsConfig.Property((ApplicationGroup r) => r.Name).IsRequired();

        }
    }

    //New Group Methods
}