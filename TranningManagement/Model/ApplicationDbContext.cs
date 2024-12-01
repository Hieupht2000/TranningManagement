using Microsoft.EntityFrameworkCore;

namespace TranningManagement.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Classes> Classes { get; set; }
        public DbSet<StudentClasses> student_classes { get; set; }
        public DbSet<LearningMaterials> learning_materials { get; set; }
        public DbSet<TuitionFees> tuition_fees {  get; set; }
        public DbSet<Promotions> promotions { get; set; }
        public DbSet<Schedules> schedules { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.user_id);
            modelBuilder.Entity<Student>()
                .HasKey(s => s.student_id);
            modelBuilder.Entity<Teacher>()
                .HasKey(h => h.teacher_id);
            modelBuilder.Entity<Staff>()
                .HasKey(i => i.staff_id);
            modelBuilder.Entity<Classes>()
                .HasKey(l => l.class_id);
            modelBuilder.Entity<StudentClasses>()
                .HasKey(k => k.student_id);
            modelBuilder.Entity<LearningMaterials>()
                .HasKey(h => h.material_id);
            modelBuilder.Entity<TuitionFees>()
                .HasKey(t => t.payment_id);
            modelBuilder.Entity<Promotions>()
                .HasKey(p=>p.promotion_id);
            modelBuilder.Entity<Schedules>()
                .HasKey(c => c.schedule_id);
        }

    }
}
