using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using todoExample.domain.Models.Identity;

namespace todoExample.MysqlDAL.Data.Mappings;

    public class TagMap
    {
        public TagMap(EntityTypeBuilder<TagModel> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasQueryFilter(e => e.Enabled);
        entityBuilder.HasOne(e => e.User)
                .WithMany(e => e.Tags)
                .HasForeignKey(e => e.UserId);

            entityBuilder.HasMany(e => e.TodoTaskTags)
                .WithOne(e => e.Tag)
                .HasForeignKey(e => e.TagId);

        }
    }
