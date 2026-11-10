using Microsoft.EntityFrameworkCore.Metadata.Builders;
using todoExample.domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace todoExample.MysqlDAL.Data.Mappings;

public class TaskTagMap
{
    public TaskTagMap(EntityTypeBuilder<TodoTaskTagModel> entityBuilder)
    {
        entityBuilder.HasKey(e => new { e.TodoTaskId, e.TagId });

        entityBuilder.HasOne(e => e.TodoTask)
           .WithMany(e => e.TodoTaskTags)
           .HasForeignKey(e => e.TodoTaskId);

        entityBuilder.HasOne(e => e.Tag)
           .WithMany(e => e.TodoTaskTags)
           .HasForeignKey(e => e.TagId);
    }
}
