using System;
using System.Collections.Generic;
using System.Text;
using BooksLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksLibrary.Infrastructure.Data.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property(l => l.LoanedAt).IsRequired();
            builder.Property(l => l.DueDate).IsRequired();
            builder.Property(l => l.ReturnedAt).IsRequired(false);

            builder.HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Reader)
                .WithMany(r => r.Loans)
                .HasForeignKey(l => l.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(l => l.IsOverdue);
            builder.Ignore(l => l.IsReturned);
        }
    }
}