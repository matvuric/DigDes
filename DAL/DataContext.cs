using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<Avatar>().ToTable(nameof(Avatars));
            modelBuilder.Entity<PostAttachment>().ToTable(nameof(PostAttachments));
            modelBuilder.Entity<PostCommentAttachment>().ToTable(nameof(PostCommentAttachments));
            modelBuilder.Entity<PostLike>().ToTable(nameof(PostLikes));
            modelBuilder.Entity<PostCommentLike>().ToTable(nameof(PostCommentLikes));
            
            modelBuilder.Entity<Relation>()
                .HasKey(x => new { x.FollowerId, x.FollowingId });
            modelBuilder.Entity<Relation>()
                .HasOne(x => x.FollowerUser)
                .WithMany(x => x.Following)
                .HasForeignKey(x => x.FollowerId);
            modelBuilder.Entity<Relation>()
                .HasOne(x => x.FollowingUser)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowingId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(builder => builder.MigrationsAssembly("Api"));

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostAttachment> PostAttachments => Set<PostAttachment>();
        public DbSet<PostComment> PostComments => Set<PostComment>();
        public DbSet<PostCommentAttachment> PostCommentAttachments => Set<PostCommentAttachment>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<PostCommentLike> PostCommentLikes => Set<PostCommentLike>();
        public DbSet<Relation> Relations => Set<Relation>();
    }
}