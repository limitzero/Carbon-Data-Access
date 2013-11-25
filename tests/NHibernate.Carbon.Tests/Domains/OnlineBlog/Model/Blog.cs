using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domains.OnlineBlog.Model
{
	public class Blog : Entity<Blog>
	{
		private int _version;
		public virtual int Version
		{
			get { return _version; }
			private set { _version = value; }
		}

		private Iesi.Collections.Generic.ISet<Post> _posts = new HashedSet<Post>();

		/// <summary>
		/// Gets the collection of unique posts for a given blog
		/// </summary>
		public virtual Iesi.Collections.Generic.ISet<Post> Posts
		{
			get { return _posts; }
			private set { _posts = value; }
		}

		private string _title;

		/// <summary>
		/// Gets or sets the title of the blog
		/// </summary>
		public virtual string Title
		{
			get { return _title; }
			set { _title = value; }
		}


		private string _description;

		/// <summary>
		/// Gets or sets the description of the blog
		/// </summary>
		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// This will create a post for a given blog
		/// </summary>
		public virtual Post CreatePost()
		{
			return new Post(this);
		}

		public virtual void Change(string title,string description)
		{
			this.Title = title;
			this.Description = description;
		}

		/// <summary>
		/// This will record the current post to the blog.
		/// </summary>
		/// <param name="post"></param>
		public virtual void RecordPost(Post post)
		{
			if (Posts.Contains(post)) return;
			Posts.Add(post);
		}
	}
}