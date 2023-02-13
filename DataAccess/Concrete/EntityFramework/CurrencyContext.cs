using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public partial class CurrencyContext : DbContext
    {
        public CurrencyContext()
            : base(@"Server=(localdb)\mssqllocaldb;Database=Currency;Trusted_Connection=true")
        {
        }

        public virtual DbSet<CurrencyReport> CurrencyReport { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyReport>()
                .Property(e => e.CurrencyName)
                .IsUnicode(false);

            modelBuilder.Entity<CurrencyReport>()
                .Property(e => e.ForexBuying)
                .HasPrecision(18, 5);

            modelBuilder.Entity<CurrencyReport>()
                .Property(e => e.ForexSelling)
                .HasPrecision(18, 5);

            modelBuilder.Entity<CurrencyReport>()
                .Property(e => e.BanknoteBuying)
                .HasPrecision(18, 5);

            modelBuilder.Entity<CurrencyReport>()
                .Property(e => e.BanknoteSelling)
                .HasPrecision(18, 5);

            modelBuilder.Entity<CurrencyReport>()
                .Property(e => e.CrossRateUSD)
                .HasPrecision(18, 5);
        }
    }
}
