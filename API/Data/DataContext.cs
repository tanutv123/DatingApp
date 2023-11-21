using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class DataContext :IdentityDbContext<AppUser, 
												AppRole, 
												int, 
												IdentityUserClaim<int>, 
												AppUserRole, 
												IdentityUserLogin<int>, 
												IdentityRoleClaim<int>, 
												IdentityUserToken<int>
												>
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<Connection> Connections{ get; set; }
		public DbSet<UserReport> UserReports { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<ReportType> ReportTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Report>().HasKey(x => new { x.ReportTypeId, x.UserReportId });
			modelBuilder.Entity<UserReport>()
				.HasMany(x => x.Reports)
				.WithOne(x => x.UserReport)
				.HasForeignKey(x => x.UserReportId)
				.IsRequired();
			modelBuilder.Entity<ReportType>()
				.HasMany(x => x.Reports)
				.WithOne(x => x.ReportType)
				.HasForeignKey(x => x.ReportTypeId)
				.IsRequired();
			modelBuilder.Entity<AppUser>()
				.HasMany(x => x.Reported)
				.WithOne(x => x.Reporter)
				.HasForeignKey(x => x.ReporterUserId)
				.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<AppUser>()
				.HasMany(x => x.IsReported)
				.WithOne(x => x.ReportedUser)
				.HasForeignKey(x => x.ReportedUserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<AppUser>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(ur => ur.User)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

			modelBuilder.Entity<AppRole>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(ur => ur.Role)
				.HasForeignKey(ur => ur.RoleId)
				.IsRequired();

			modelBuilder.Entity<UserLike>()
				.HasKey(k => new {k.SourceUserId, k.TargetUserId});


			//If using MSSQL server, you need to specify one of the DeleteBehavior to NoAction(DeleteBehavior.NoAction) 
			//Or else, it will cause an error
			//Other db is oke with this approach
			modelBuilder.Entity<UserLike>()
				.HasOne(s => s.SourceUser)
				.WithMany(l => l.LikedUsers)
				.HasForeignKey(s => s.SourceUserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<UserLike>()
				.HasOne(s => s.TargetUser)
				.WithMany(l => l.LikedByUsers)
				.HasForeignKey(s => s.TargetUserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Message>()
				.HasOne(u => u.Recipient)
				.WithMany(u => u.MessagesReceived)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Message>()
				.HasOne(u => u.Sender)
				.WithMany(u => u.MessagesSent)
				.OnDelete(DeleteBehavior.Restrict);


		}
	}
}
