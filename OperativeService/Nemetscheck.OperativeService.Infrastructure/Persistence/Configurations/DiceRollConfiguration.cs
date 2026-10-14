using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nemetscheck.OperativeService.Domain.DiceRolls;

namespace Nemetscheck.OperativeService.Infrastructure.Persistence.Configurations;

internal sealed class DiceRollConfiguration : IEntityTypeConfiguration<DiceRoll>
{
    public void Configure(EntityTypeBuilder<DiceRoll> builder)
    {
        builder.ToTable("DiceRolls", t =>
        {
            t.HasCheckConstraint("CK_DiceRolls_FirstDie", $"FirstDie BETWEEN {DiceRoll.MinFace} AND {DiceRoll.MaxFace}");
            t.HasCheckConstraint("CK_DiceRolls_SecondDie", $"SecondDie BETWEEN {DiceRoll.MinFace} AND {DiceRoll.MaxFace}");
            t.HasCheckConstraint("CK_DiceRolls_Sum", "Sum = FirstDie + SecondDie");
        });

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.FirstDie).IsRequired();
        builder.Property(r => r.SecondDie).IsRequired();
        builder.Property(r => r.Sum).IsRequired();
        builder.Property(r => r.RolledAtUtc).IsRequired();

        // every query is scoped to one user and filtered/sorted by date or sum
        builder.HasIndex(r => new { r.UserId, r.RolledAtUtc });
        builder.HasIndex(r => new { r.UserId, r.Sum });
    }
}
