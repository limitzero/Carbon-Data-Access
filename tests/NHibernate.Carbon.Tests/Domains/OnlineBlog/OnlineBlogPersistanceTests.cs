using NHibernate.Carbon.ForTesting;
using NHibernate.Carbon.Tests.Domains.OnlineBlog.Model;
using Xunit;

namespace NHibernate.Carbon.Tests.Domains.OnlineBlog
{
	public class PersistanceTests : BaseAutoPersistanceTestFixture
	{
		public PersistanceTests()
		{
			// dyanmically creates the schema and mappings to conform to domain model:
			OneTimeInitalizeWithModel(new OnlineBlogModelConventions().GetPersistanceModel());
			CreateUnitOfWork();
		}

		[Fact]
		public void can_save_blog()
		{
			var blog = CreateBlog();

			using (var session = base.OpenTransactedSession())
			{
				session.Save<Blog>(blog);

				var fromDB = session.FindById<Blog>(blog.Id);
				Assert.Equal(blog.Id, fromDB.Id);
			}			
		}

		[Fact]
		public void can_record_post_to_blog()
		{
			var blog = this.CreateBlog();
			var post = blog.CreatePost();

			post.ChangeTitle("My First Post");
			blog.RecordPost(post);

			using (var session = base.OpenTransactedSession())
			{
				session.Save<Blog>(blog);

				var fromDB = session.FindById<Blog>(blog.Id);

				Assert.Equal(blog.Id, fromDB.Id);
				Assert.Equal(1, fromDB.Posts.Count);
			}
			
		}

		[Fact]
		public void can_record_comment_to_post_for_blog()
		{
			var blog = this.CreateBlog();
			var post = blog.CreatePost();

			post.ChangeTitle("My First Post");
			post.RecordComment("This is a comment");

			blog.RecordPost(post);

			using (var session = base.OpenTransactedSession())
			{
				session.Save<Blog>(blog);

				var fromDB = session.FindById<Blog>(blog.Id);

				Assert.Equal(blog.Id, fromDB.Id);
				Assert.Equal(1, fromDB.Posts.Count);
				//Assert.Equal(1, fromDB.Posts[0].Comments.Count);
			}
		}

		private Blog CreateBlog()
		{
			var blog = new Blog
			           	{
			           		Title = "My Blog",
			           		Description = "My Blog Description"
			           	};
			return blog;
		}
	}
}