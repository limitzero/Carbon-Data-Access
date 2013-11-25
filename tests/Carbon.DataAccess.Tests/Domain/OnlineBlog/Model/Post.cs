using System;
using System.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domain.OnlineBlog.Model
{
	public class Post
	{
		private int _id = 0;

		/// <summary>
		/// Gets or sets the instance identifier of the entity
		/// </summary>
		public virtual int Id
		{
			get { return _id; }
		}

		private Blog _blog = null;
		/// <summary>
		/// Gets the parent object that created the child item.
		/// </summary>
		public virtual Blog Blog
		{
			get { return _blog; }
			private set { _blog = value; }
		}

		private string _title = string.Empty;

		/// <summary>
		/// Gets or sets the tile of the post in the blog
		/// </summary>
		public virtual string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private string _body = string.Empty;

		/// <summary>
		/// Gets or sets the body of the blog posting
		/// </summary>
		public virtual string Body
		{
			get { return _body; }
			set { _body = value; }
		}

		private DateTime? _createdon = null;

		/// <summary>
		/// Gets or sets the date and time of the blog posting
		/// </summary>
		public virtual DateTime? CreatedOn
		{
			get { return _createdon; }
			private set { _createdon = value; }
		}

		private IList<Comment> _comments = new List<Comment>();
		/// <summary>
		/// Gets the set of comments for a particular blog post.
		/// </summary>
		public virtual IList<Comment> Comments
		{
			get { return _comments; }
			private set { _comments = value; }
		}

		protected Post()
		{
		}

		public Post(Blog blog)
		{
			Blog = blog;
			this.CreatedOn = DateTime.UtcNow;
		}

		/// <summary>
		/// This will create a comment for the given posting
		/// </summary>
		public virtual Comment CreateComment()
		{
			var comment = new Comment(this);
			return comment;
		}

		/// <summary>
		/// This will associate the comment with the current blog posting.
		/// </summary>
		/// <param name="comment"></param>
		public virtual void RecordComment(Comment comment)
		{
			if (Comments.Contains(comment)) return;
			Comments.Add(comment);
		}
	}
}