using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface ICommentRepository
{
    Task<CommentEntity> GetByIdAsync(Guid commentId);
    Task CreateAsync(CommentEntity comment);
    Task UpdateAsync(CommentEntity comment);
    Task DeleteAsync(Guid commentId);
}
