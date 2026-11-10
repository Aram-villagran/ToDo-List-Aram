using Microsoft.EntityFrameworkCore.Metadata.Builders;
using todoExample.domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace todoExample.MysqlDAL.Data.Mappings;

public class TodoTaskMap
{
    public TodoTaskMap(EntityTypeBuilder<TodoTaskModel> entityBuilder)
    {
        entityBuilder.HasKey(e => e.Id);
        entityBuilder.HasQueryFilter(e => e.Enabled);
        entityBuilder.HasOne(e => e.TodoList)
            .WithMany(e => e.TodoTasks)
            .HasForeignKey(e => e.TodoListId)
            .HasPrincipalKey(e => e.Id);

    }
}