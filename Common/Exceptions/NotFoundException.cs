﻿namespace Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"{Model} not found";
    }

    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
        {
            Model = "User";
        }
    }

    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException()
        {
            Model = "Post";
        }
    }

    public class PostCommentNotFoundException : NotFoundException
    {
        public PostCommentNotFoundException()
        {
            Model = "PostComment";
        }
    }

    public class FileNotFoundException : NotFoundException
    {
        public FileNotFoundException()
        {
            Model = "File";
        }
    }

    public class AttachmentNotFoundException : NotFoundException
    {
        public AttachmentNotFoundException()
        {
            Model = "Attachment";
        }
    }

    public class SessionNotFoundException : NotFoundException
    {
        public SessionNotFoundException()
        {
            Model = "Session";
        }
    }

    public class LikeNotFoundException : NotFoundException
    {
        public LikeNotFoundException()
        {
            Model = "Like";
        }
    }

    public class RelationNotFoundException : NotFoundException
    {
        public RelationNotFoundException()
        {
            Model = "Relation";
        }
    }
}
