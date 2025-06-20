﻿namespace PersonalFinanceManager.Application.Data.Configurations;

internal class TransactionCategoryConfiguration : IEntityTypeConfiguration<TransactionCategory>
{
    public void Configure(EntityTypeBuilder<TransactionCategory> builder)
    {
        builder.ToTable("TransactionCategories");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(30);
        builder.Property(m => m.Description).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.Created).IsRequired().ValueGeneratedOnAdd();
        builder.Property(m => m.LastModified).IsRequired().ValueGeneratedOnUpdate();

        builder.HasIndex(m => m.Name);

        builder
            .HasMany(tc => tc.TransactionTypes)
            .WithOne(tt => tt.TransactionCategory)
            .HasForeignKey(tt => tt.TransactionCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
