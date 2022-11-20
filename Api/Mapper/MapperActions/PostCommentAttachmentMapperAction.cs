using Api.Models.Attachment;
using Api.Services;
using AutoMapper;
using DAL.Entities;

namespace Api.Mapper.MapperActions
{
    public class PostCommentAttachmentMapperAction : IMappingAction<PostCommentAttachment, AttachmentExternalModel>
    {
        private readonly LinkGeneratorService _linkGeneratorService;

        public PostCommentAttachmentMapperAction(LinkGeneratorService linkGeneratorService)
        {
            _linkGeneratorService = linkGeneratorService;
        }

        public void Process(PostCommentAttachment source, AttachmentExternalModel destination, ResolutionContext context)
        {
            _linkGeneratorService.FixPostCommentAttachment(source, destination);
        }
    }
}