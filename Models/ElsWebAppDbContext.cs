using ElsWebApp.Models.Entitiy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using NLog.LayoutRenderers;

namespace ElsWebApp.Models
{
    public class ElsWebAppDbContext : DbContext
    {
        public DbSet<MSysCode> MSysCode { get; set; }
        public DbSet<MUser> MUser { get; set; }
        public DbSet<MCourse> MCourse { get; set; }
        public DbSet<MChapter> MChapter { get; set; }
        public DbSet<MovieContents> MovieContents { get; set; }
        public DbSet<TestContents> TestContents { get; set; }
        public DbSet<ExamList> ExamList { get; set; }
        public DbSet<QuestionCatalog> QuestionCatalog { get; set; }
        public DbSet<AnswerGroup> AnswerGroup { get; set; }

        public DbSet<UserCourse> UserCourse { get; set; }
        public DbSet<UserChapter> UserChapter { get; set; }
        public DbSet<UserExam> UserExam { get; set; }
        public DbSet<UserScore> UserScore { get; set; }


        public ElsWebAppDbContext(DbContextOptions<ElsWebAppDbContext> options) :
            base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(msg => System.Diagnostics.Debug.WriteLine(msg));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MSysCode>().HasKey("ClassId", "ClassCd");

            modelBuilder.Entity<MUser>().HasIndex("LoginId").IsUnique();
            modelBuilder.Entity<MCourse>().HasIndex("BegineDateTime");
            modelBuilder.Entity<MChapter>().HasIndex("CourseId", "OrderNo");
            modelBuilder.Entity<MovieContents>().HasIndex("ChapterId").IsUnique();
            modelBuilder.Entity<TestContents>().HasIndex("ChapterId").IsUnique();
            modelBuilder.Entity<QuestionCatalog>().HasIndex("MajorCd", "MiddleCd", "MinorCd", "SeqNo").IsUnique();

            modelBuilder.Entity<UserCourse>().HasIndex("UserId", "CourseId").IsUnique();
            modelBuilder.Entity<UserChapter>().HasIndex("UserId", "CourseId", "ChapterId").IsUnique();
            modelBuilder.Entity<UserExam>().HasIndex("UserChapterId", "NthTime", "QuestionId").IsUnique();
            modelBuilder.Entity<UserScore>().HasIndex("UserChapterId", "NthTime", "QuestionId", "AnswerId").IsUnique();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetCreatedAndUpdatedAt();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            SetCreatedAndUpdatedAt();
            return base.SaveChanges();
        }

        private void SetCreatedAndUpdatedAt()
        {
            var dt = DateTime.Now;
            var entries = ChangeTracker.Entries()
                    .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var entry in entries)
            {
                var createdAt = entry.Properties.Any(x => x.Metadata.Name == "CreatedAt") ? entry.Property("CreatedAt") : null;
                var updatedAt = entry.Properties.Any(x => x.Metadata.Name == "UpdatedAt") ? entry.Property("UpdatedAt") : null;

                if (entry.State == EntityState.Added && createdAt != null)
                {
                    createdAt.CurrentValue = dt;
                }

                if (updatedAt != null)
                {
                    updatedAt.CurrentValue = dt;
                }
            }
        }

        /// <summary>
        /// トランザクションの開始
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BeginTans()
        {
            IDbContextTransaction? context = null;
            try
            {
                context = await this.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return context != null;
        }

        /// <summary>
        /// トランザクションの終了
        /// </summary>
        /// <param name="commit">
        ///     コミットフラグ：
        ///     　true  - コミット
        ///     　false - ロールバック
        /// </param>
        public async Task EndTans(bool commit = true)
        {
            try
            {
                if (commit)
                {
                    await this.Database.CommitTransactionAsync();
                }
                else
                {
                    await this.Database.RollbackTransactionAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
