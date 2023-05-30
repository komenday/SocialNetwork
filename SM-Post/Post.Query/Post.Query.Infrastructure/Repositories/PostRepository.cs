using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _databaseContextFactory;

    public PostRepository(DatabaseContextFactory databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    public async Task<List<PostEntity>> GetAllAsync()
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .Include(p => p.Comments)
            .FirstAsync(p => p.PostId == postId);
    }

    public async Task<List<PostEntity>> GetByAuthorAsync(string author)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .AsNoTracking()
            .Where(p => p.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<List<PostEntity>> GetWithCommentsAsync()
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .AsNoTracking()
            .Where(p => p.Comments != null && p.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> GetWithLikesAsync(int numberOfLikes)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Comments)
            .AsNoTracking()
            .Where(p => p.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task CreateAsync(PostEntity post)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        context.Posts.Add(post);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        context.Posts.Update(post);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using var context = _databaseContextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);

        if (post is null) return;

        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }
}
