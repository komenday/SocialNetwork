using Post.Common.Events;
using SQRS.Core.Domain;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author = null!;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active 
    {
        get => _active; 
        set => _active = value;
    }

    public PostAggregate()
    {
    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.Now,  
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        Id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!_active) 
        {
            throw new InvalidOperationException("You cannot edit the message of an inactive post");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException($"The value of {nameof(message)} cannot be empty. Please provide a {nameof(message)}");
        }

        RaiseEvent(new MessageUpdatedEvent 
        { 
            Id = Id, 
            Message = message 
        });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        Id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot like inactive post");
        }

        RaiseEvent(new PostLikedEvent
        {
            Id = Id
        });
    }

    public void Apply(PostLikedEvent @event)
    {
        Id = @event.Id;
    }

    public void AddComment(string comment, string username)
    {
        if (!_active) 
        {
            throw new InvalidOperationException("You cannot comment inactive post");    
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be empty. Please provide a {nameof(comment)}");
        }

        RaiseEvent(new CommentAddedEvent
        {
            Id = Id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.Now,
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        Id = @event.Id;
        _comments.Add(@event.CommentId, Tuple.Create(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot edit an comment of an inactive post");
        }

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to edit a comment which was made by another user");
        }

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = Id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.Now,
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        Id = @event.Id;
        _comments[@event.CommentId] = Tuple.Create(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("You cannot remove an comment of an inactive post");
        }

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to remove a comment which was made by another user");
        }

        RaiseEvent(new CommentRemovedEvent
        {
            Id = Id,
            CommentId = commentId
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        Id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
        {
            throw new InvalidOperationException("The post has already een removed");
        }

        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to delete a post which was made by another user");
        }

        RaiseEvent(new PostRemovedEvent
        {
            Id = Id
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        Id = @event.Id;
        _active = false;
    }
}
