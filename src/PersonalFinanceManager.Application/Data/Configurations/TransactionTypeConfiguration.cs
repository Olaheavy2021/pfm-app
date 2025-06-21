namespace PersonalFinanceManager.Application.Data.Configurations;

internal class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
{
    public void Configure(EntityTypeBuilder<TransactionType> builder)
    {
        builder.ToTable("TransactionTypes");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(30);
        builder.Property(m => m.Description).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.Created).IsRequired().ValueGeneratedOnAdd();
        builder.Property(m => m.LastModified).IsRequired().ValueGeneratedOnUpdate();
        builder.Property(m => m.TransactionCategoryId).IsRequired();

        builder.HasIndex(m => m.Name);

        //builder
        //    .HasOne(m => m.TransactionCategory)
        //    .WithMany()
        //    .HasForeignKey(m => m.TransactionCategoryId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}
