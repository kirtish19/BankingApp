using BankingApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data.DocumentDb
{
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext(
            DbContextOptions<DocumentDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerKYCMessage> CustomerKYCMessages =>
            Set<CustomerKYCMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerKYCMessage>(entity =>
            {
                entity.ToContainer("CustomerKYC");// Please Confirm Container Name

                entity.HasKey(x => x.EventId);

                entity.HasPartitionKey(x => x.CustomerId);
            });
        }
    }
}