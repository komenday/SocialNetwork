﻿using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DatabaseContextFactory _databaseContextFactory;

    public CommentRepository(DatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Comments.FirstAsync(c => c.CommentId == commentId);
    }

    public async Task CreateAsync(CommentEntity comment)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        var comment = await GetByIdAsync(commentId);

        if (comment is null) return;

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }
}
