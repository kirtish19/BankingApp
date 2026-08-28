namespace BankingApp.Data.DocumentDb
{
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext(
            DbContextOptions<DocumentDbContext> options)
            : base(options)
        {
        }

        public DbSet<KycDocument> KycDocuments =>
            Set<KycDocument>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KycDocument>(entity =>
            {
                entity.ToContainer("KycDocuments");

                entity.HasKey(x => x.DocumentId);

                entity.HasPartitionKey(x => x.CustomerId);
            });
        }
    }
}