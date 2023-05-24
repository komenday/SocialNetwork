using SQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class RemoveCommentComand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string Username { get; set; }
}
