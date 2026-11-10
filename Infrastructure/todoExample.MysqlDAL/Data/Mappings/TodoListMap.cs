using Microsoft.EntityFrameworkCore.Metadata.Builders;
using todoExample.domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace todoExample.MysqlDAL.Data.Mappings;

public class TodoListMap
{
    public TodoListMap(EntityTypeBuilder<TodoListModel> entityBuilder)
    {
        entityBuilder.HasKey(e => e.Id);
        entityBuilder.HasQueryFilter(e => e.Enabled);
        entityBuilder.HasOne(e => e.User)
            .WithMany(e => e.TodoList)
            .HasForeignKey(e => e.UserId);

    }
}