using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using ProjectBase.Models;
using ProjectBase.Models.DAO;

namespace ProjectBase.Helpers

{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<BlogsModel> Blogs { get; set; }
        public DbSet<SliderModel> Slider { get; set; }
        public DbSet<SubjectsModel> Subjects { get; set; }
        public DbSet<Subject_CategoryModel> Subject_Category { get; set; }
        public DbSet<PricePackageModel> Price_package { get; set; }
        public DbSet<CategoryModel> Category { get; set; }
        public DbSet<Blogs_CategoryModel> Blogs_Category { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PracticeModel> Practice { get; set; }
        public DbSet<RecipeModel> Recipe { get; set; }
        public DbSet<QuizHandleModel> QuizHandle { get; set; }
        public DbSet<SubjectTopicModel> SubjectTopic {  get; set; }
        public DbSet<PracticeLevel> PracticeLevel { get; set; }
        public DbSet<QuizBankModel> QuizBank { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject_CategoryModel>()
                .HasKey(sc => new { sc.SubjectID, sc.CategoryID });

            modelBuilder.Entity<Subject_CategoryModel>()
                .HasOne(sc => sc.Subjects)
                .WithMany(s => s.Subject_Category)
                .HasForeignKey(sc => sc.SubjectID);
            modelBuilder.Entity<Subject_CategoryModel>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.Subject_Category)
                .HasForeignKey(sc => sc.CategoryID);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID);
            modelBuilder.Entity<PracticeModel>()
                .HasOne(u => u.User)
                .WithMany(p => p.Practice)
                .HasForeignKey(u => u.UserID);
            modelBuilder.Entity<PracticeModel>()
                .HasOne(s => s.Subject)
                .WithMany(p => p.Practice)
                .HasForeignKey(s => s.SubjectID);
            modelBuilder.Entity<PracticeModel>()
                .HasOne(s => s.Level)
                .WithMany(p => p.Practice)
                .HasForeignKey(l => l.levelID);
            modelBuilder.Entity<PracticeModel>()
                .HasOne(t => t.Topic)
                .WithMany(p => p.Practice)
                .HasForeignKey(tp => tp.topicID);
            modelBuilder.Entity<PricePackageModel>()
                .HasOne(pp => pp.Subjects)
                .WithMany(s => s.Price_package)
                .HasForeignKey(pp => pp.SubjectID);
            modelBuilder.Entity<RecipeModel>()
            .HasOne(r => r.PricePackage)
            .WithMany(pp => pp.Recipe)
            .HasForeignKey(r => r.PricePackage_ID);

            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Recipes)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserID);

            modelBuilder.Entity<RecipeModel>()
                .HasOne(s => s.Subjects)
                .WithMany(r => r.Recipes)
                .HasForeignKey(s=>s.SubjectID);

            modelBuilder.Entity<SimulationExam>()
                .HasOne(s => s.Subjects)
                .WithMany(e => e.Exams)
                .HasForeignKey(s => s.SubjectID);
            modelBuilder.Entity<SimulationExam>()
                .HasOne(s => s.Level)
                .WithMany(e => e.Exams)
                .HasForeignKey(l => l.LevelID);
            modelBuilder.Entity<SliderModel>()
                .HasOne(u => u.User)
                .WithMany(s => s.Sliders)
                .HasForeignKey(s => s.userID);
            modelBuilder.Entity<QuizHandleModel>()
                .HasOne(q => q.QuizBank)
                .WithMany(s => s.QuizHandle)
                .HasForeignKey(s => s.QuizID);
            base.OnModelCreating(modelBuilder);
           

        }
           

    }
}
