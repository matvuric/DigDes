using Api.Models.User;
using System.Xml.Linq;

namespace Api.Models.Attachment
{
    public class AttachmentModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }

    public class AttachmentWithLinkModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string? AttachmentLink { get; set; }

        public AttachmentWithLinkModel(AttachmentModel model, Func<AttachmentModel, string?>? linkGenerator)
        {
            Id = model.Id;
            Name = model.Name;
            MimeType = model.MimeType;
            AttachmentLink = linkGenerator?.Invoke(model);
        }
    }
}
