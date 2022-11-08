using Api.Models.Attach;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class PostService
    {
        private readonly UserService _userService;
        private readonly DataContext _context;

        public PostService(UserService userService, DataContext context)
        {
            _userService = userService;
            _context = context;
        }

        public async Task UploadPostAttaches(List<MetaDataModel> attaches, Guid postId, Guid authorId)
        {
            foreach (var meta in attaches)
            {
                var tempFileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), meta.TempId.ToString()));
                if (!tempFileInfo.Exists)
                {
                    throw new Exception("File not found");
                }
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Attaches", meta.TempId.ToString());
                    var destFileInfo = new FileInfo(path);

                    if (destFileInfo.Directory == null)
                    {
                        throw new Exception("Directory is not defined");
                    }
                    else
                    {
                        if (!destFileInfo.Directory.Exists)
                        {
                            destFileInfo.Directory.Create();
                        }
                    }

                    File.Copy(tempFileInfo.FullName, path, true);
                    await AddAttaches(postId, meta, path, authorId);
                }
            }
        }

        private async Task AddAttaches(Guid postId, MetaDataModel meta, string filePath, Guid authorId)
        {
            var post = await GetPostByIdWithAttaches(postId);
            var user = await _userService.GetUserById(authorId);
            var postAttach = new PostAttach()
            {
                Name = meta.Name,
                MimeType = meta.MimeType,
                FilePath = filePath,
                Size = meta.Size,
                Author = user
            };
            post.Attaches.Add(postAttach);

            await _context.SaveChangesAsync();
        }

        private async Task<Post> GetPostByIdWithAttaches(Guid id)
        {
            var post = await _context.Posts.Include(post => post.Attaches).FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                throw new Exception("Post not found");
            }

            return post;
        }

        public async Task<Post> GetPostById(Guid id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                throw new Exception("Post not found");
            }

            return post;
        }
    }
}
