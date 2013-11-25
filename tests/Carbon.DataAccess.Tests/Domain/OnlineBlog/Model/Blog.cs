using System;
using System.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domain.OnlineBlog.Model
{
	public class Blog
	{
		private IList<Post> _posts = new List<Post>();

		/// <summary>
		/// Gets the collection of unique posts for a given blog
		/// </summary>
		public virtual IList<Post> Posts
		{
			get { return _posts; }
			private set { _posts = value; }
		}

		private int _id = 0;
		/// <summary>
		/// Gets or sets the instance identifier of the entity
		/// </summary>
		public virtual int Id { get { return _id; } }

		private string _title = string.Empty;
		/// <summary>
		/// Gets or sets the title of the blog
		/// </summary>
		public virtual string Title
		{
			get { return _title; }
			set { _title = value; }
		}


		private string _description = string.Empty;
		/// <summary>
		/// Gets or sets the description of the blog
		/// </summary>
		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// This will create a post for a given blog.
		/// </summary>
		public virtual Post CreatePost(string title)
		{
			// every new post should have a title.
			Post post = new Post(this) {Title = title};
			return post;
		}

		/// <summary>
		/// This will record the current post to the blog.
		/// </summary>
		/// <param name="post"></param>
		public virtual void RecordPost(Post post)
		{
			// enforce uniqueness here on posts:
			if (Posts.Contains(post)) return;
			Posts.Add(post);
		}
	}
}