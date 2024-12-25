using Microsoft.EntityFrameworkCore;
using radioCabs.Models;

namespace radioCabs.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : base(opt)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }
        public virtual DbSet<Advertise> Advertisements { get; set; }
        public virtual DbSet<Feedback> Feedback { get; set; }
        public virtual DbSet<PaymentPlan> PaymentPlans { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
