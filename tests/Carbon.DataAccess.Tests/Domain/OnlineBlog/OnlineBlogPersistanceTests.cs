using NHibernate.Carbon.ForTesting;
using NHibernate.Carbon.Tests.Domain.OnlineBlog.Model;
using Xunit;

namespace NHibernate.Carbon.Tests.Domain.OnlineBlog
{
	public class OnlineBlogPersistanceTests : BaseAutoPersistanceTestFixture
	{
		public OnlineBlogPersistanceTests()
		{
			// dyanmically creates the schema and mappings to conform to domain model:
			OneTimeInitalizeWithModel(new OnlineBlogModelConventions().GetPersistanceModel());
			CreateUnitOfWork();
		}

		[Fact]
		public void can_save_blog()
		{
			var blog = new Blog();
			blog.Title = "My Blog";
			blog.Description = "My Blog Description";

			using (var session = OpenTransactedSession())
			{
				session.Save(blog);
				var fromDB = session.FindById<Blog>(blog.Id);
				Assert.Equal(blog.Id, fromDB.Id);
			}
		}

		[Fact]
		public void can_create_post_from_blog()
		{
			var blog = new Blog();
			blog.Title = "My Blog";
			blog.Description = "My First Blog";

			var post = blog.CreatePost("My First Post");
			post.Body = "This is about using the auto-persistance model for NHibernate.Carbon";
			blog.RecordPost(post);

			using (var session = OpenTransactedSession())
			{
				session.Save(blog);
				var fromDB = session.FindById<Blog>(blog.Id);
				Assert.Equal(1, fromDB.Posts.Count);
			}
		}
	}
}