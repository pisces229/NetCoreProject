using NetCoreProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.Common;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NetCoreProject.Domain.DatabaseContext
{
    public partial class DefaultDbContext : DbContext, IDbContext
    {
        public int ConnectionTimeout { get; protected set; }
        protected DbTransaction _dbTransaction { get; set; }
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options)
        {
            // default TrackAll
            this.ChangeTracker.QueryTrackingBehavior
                = QueryTrackingBehavior.NoTracking;
            //context.ChangeTracker.QueryTrackingBehavior
            //    = QueryTrackingBehavior.TrackAll;
        }
        #region DbSet
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestMaster> TestMaster { get; set; }
        public virtual DbSet<TestDetail> TestDetail { get; set; }
        public virtual DbSet<Grade> Grade { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentAddress> StudentAddress { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            #region Builder Entity

            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasComment("Test Table");

                entity.HasKey(e => e.ROW).IsClustered(false);

                entity.Property(e => e.NAME)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MAKE_DATE)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.SALE_AMT)
                    .HasColumnType("int");

                entity.Property(e => e.SALE_DATE)
                    .HasColumnType("datetime");

                entity.Property(e => e.TAX)
                    .HasColumnType("decimal(10, 6)");

                entity.Property(e => e.REMARK)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(true);

                entity.Property(e => e.UPDATE_DATE_TIME)
                    .HasColumnType("datetime");

                entity.Property(e => e.UPDATE_PROG_CD)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UPDATE_USER_ID)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TestMaster>(entity =>
            {
                entity.HasKey(e => e.ROW).IsClustered(false);

                entity.Property(e => e.ID)
                    .IsRequired()
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.NAME)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.BORN_DATE)
                    .IsRequired()
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<TestDetail>(entity =>
            {
                entity.HasKey(e => e.ROW).IsClustered(false);

                entity.Property(e => e.MASTER_ID)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ID)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.NAME)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.BORN_DATE)
                    .IsRequired()
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasKey(e => e.GradeId);

                entity.Property(e => e.GradeId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.Property(e => e.GradeId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StudentId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StudentAddress>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.Property(e => e.StudentId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            #endregion

            modelBuilder.Entity<Student>()
                .HasOne(h => h.Grade)
                .WithMany(w => w.Students)
                .HasForeignKey(k => k.GradeId);

            modelBuilder.Entity<Student>()
                .HasOne(h => h.Address)
                .WithOne(w => w.Student)
                .HasForeignKey<StudentAddress>(k => k.StudentId);

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        protected void UpdateInfomation()
        {
            foreach (var entrie in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                //switch (entrie.State)
                //{
                //    case EntityState.Added:
                //        entrie.Property("CREATE_USER").CurrentValue = _userService.Userid();
                //        entrie.Property("CREATE_DATETIME").CurrentValue = DateTime.Now;
                //        entrie.Property("UPDATE_USER").CurrentValue = _userService.Userid();
                //        entrie.Property("UPDATE_DATETIME").CurrentValue = DateTime.Now;
                //        entrie.Property("PROG_CD").CurrentValue = _userService.Proid();
                //        break;
                //    case EntityState.Modified:
                //        entrie.Property("UPDATE_USER").CurrentValue = _userService.Userid();
                //        entrie.Property("UPDATE_DATETIME").CurrentValue = DateTime.Now;
                //        entrie.Property("PROG_CD").CurrentValue = _userService.Proid();
                //        break;
                //}
            }
        }
        #region Expansion
        public override int SaveChanges()
        {
            UpdateInfomation();
            return base.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            UpdateInfomation();
            return base.SaveChangesAsync();
        }
        public DatabaseFacade GetDatabase() => this.Database;
        public async Task<DbConnection> GetDbConnection()
        {
            await OpenConnection();
            return this.Database.GetDbConnection();
        }
        private async Task OpenConnection()
        {
            if (!this.Database.IsInMemory())
            {
                if (this.Database.GetDbConnection().State == ConnectionState.Closed)
                {
                    await this.Database.OpenConnectionAsync();
                }
            }
        }
        public async Task<DbTransaction> GetDbTransaction()
        {
            return await Task.FromResult(_dbTransaction);
        }
        public async Task BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (!this.Database.IsInMemory())
            {
                if (!this.Database.IsInMemory())
                {
                    await OpenConnection();
                    _dbTransaction = await this.Database.GetDbConnection().BeginTransactionAsync(isolationLevel);
                    await this.Database.UseTransactionAsync(_dbTransaction);
                }
            }
        }
        public void Commit()
        {
            if (!this.Database.IsInMemory())
            {
                if (_dbTransaction != null)
                {
                    this.Database.CommitTransaction();
                    _dbTransaction = null;
                }
            }
        }
        public void Rollback()
        {
            if (!this.Database.IsInMemory())
            {
                if (_dbTransaction != null)
                {
                    this.Database.RollbackTransaction();
                    _dbTransaction = null;
                }
            }
        }
        public override void Dispose()
        {
            try
            {
                Rollback();
            }
            catch
            {
                // pass
            }
            base.Dispose();
        }
        #endregion
    }
}
