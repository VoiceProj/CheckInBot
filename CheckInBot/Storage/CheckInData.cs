namespace CheckInBot.Storage
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CheckInData : DbContext
    {
        public CheckInData()
            : base("name=CheckInData")
        {
        }

        public virtual DbSet<CheckIn> CheckInDB { get; set; }
        public virtual DbSet<Registration> RegistrationDB { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CheckIn>()
                .Property(e => e.ID)
                .HasPrecision(10, 0);

            modelBuilder.Entity<CheckIn>()
                .Property(e => e.UserID)
                .HasPrecision(10, 0);

            modelBuilder.Entity<CheckIn>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<CheckIn>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<CheckIn>()
                .Property(e => e.UserName)
                .IsUnicode(false);
        }
    }
}
