using DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Context;

public class UnadeskDbContext : DbContext
{
    // Набор данных для документов PDF
    public DbSet<PdfDocument> Documents { get; set; }

    // Конструктор контекста базы данных
    public UnadeskDbContext(DbContextOptions<UnadeskDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация сущности PdfDocument
        modelBuilder.Entity<PdfDocument>(entity =>
        {
            // Установка первичного ключа
            entity.HasKey(d => d.Id);

            // Настройка свойства OriginalFileName: обязательно и ограничение длины
            entity.Property(d => d.OriginalFileName)
                  .IsRequired()
                  .HasMaxLength(255);

            // Настройка свойства TextContent: использование типа TEXT для хранения содержимого
            entity.Property(d => d.TextContent)
                  .HasColumnType("TEXT"); 

            // Настройка свойства ProcessedOn: использование типа timestamp с часовым поясом
            entity.Property(d => d.ProcessedOn)
                  .HasColumnType("timestamp with time zone");
        });

        base.OnModelCreating(modelBuilder);
    }
}




