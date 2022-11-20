using Api.Models.Attachment;
using Api.Services;
using AutoMapper;
using DAL.Entities;

namespace Api.Mapper.MapperActions
{
    public class PostAttachmentMapperAction : IMappingAction<PostAttachment, AttachmentExternalModel>
    {
        private readonly LinkGeneratorService _linkGeneratorService;

        public PostAttachmentMapperAction(LinkGeneratorService linkGeneratorService)
        {
            _linkGeneratorService = linkGeneratorService;
        }

        public void Process(PostAttachment source, AttachmentExternalModel destination, ResolutionContext context)
        {
            _linkGeneratorService.FixPostAttachment(source, destination);
        }
    }
}
