using Microsoft.EntityFrameworkCore;
using RandomSongSearchEngine.DBEntities;

namespace RandomSongSearchEngine.DBContext
{
    /// <summary>
    /// Контекст (таблицы) базы данных
    /// </summary>
    public class RsseContext : DbContext
    {
        /// <summary>
        /// Конструктор, конфигурирующий контекст базы данных
        /// </summary>
        /// <param name="option"></param>
        public RsseContext(DbContextOptions<RsseContext> option) : base(option)
        {
        }

        /// <summary>
        /// Таблица бд с текстами песен
        /// </summary>
        public DbSet<TextEntity> Text { get; set; }

        /// <summary>
        /// Таблица бд с жанрами песен
        /// </summary>
        public DbSet<GenreEntity> Genre { get; set; }

        /// <summary>
        /// Таблица бд, связывающая песни и их жанры
        /// </summary>
        public DbSet<GenreTextEntity> GenreText { get; set; }

        /// <summary>
        /// Создание связей для модели "многие-ко-многим"
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TextEntity>()
                .HasKey(k => k.TextID);

            modelBuilder.Entity<GenreEntity>()
                .HasKey(k => k.GenreID);

            modelBuilder.Entity<GenreTextEntity>()
                .HasKey(k => new { k.GenreID, k.TextID });
            modelBuilder.Entity<GenreTextEntity>()
                .HasOne(g => g.GenreInGenreText)
                .WithMany(m => m.GenreTextInGenre)
                .HasForeignKey(k => k.GenreID);
            modelBuilder.Entity<GenreTextEntity>()
                .HasOne(t => t.TextInGenreText)
                .WithMany(m => m.GenreTextInText)
                .HasForeignKey(k => k.TextID);
        }
    }
}