using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tasks.Persistence.Reading.Projections.ModelConfiguration;

public class TaskModelConfiguration : IEntityTypeConfiguration<TaskProjection>
{
  public void Configure(EntityTypeBuilder<TaskProjection> builder)
  {
    builder.ToTable("tasks");
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
    builder.Property(x => x.UserId).HasColumnName("user_id");
    builder.Property(x => x.At).HasColumnName("at");
    builder.Property(x => x.StartedAt).HasColumnName("started_at");
    builder.Property(x => x.CompletedAt).HasColumnName("completed_at");
    builder.Property(x => x.DueAt).HasColumnName("due_at");
    builder.Property(x => x.Subject).HasColumnName("subject");
    builder.Property(x => x.Description).HasColumnName("description");
    builder.Property(x => x.Status).HasColumnName("status");
  }
}
