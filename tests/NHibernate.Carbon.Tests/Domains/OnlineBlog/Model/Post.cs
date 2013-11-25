using System;
using System.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domains.OnlineBlog.Model
{
	public class Post : Entity<Post>
	{
		private Blog _blog;
		public virtual Blog Blog
		{
			get { return _blog; }
			private set { _blog = value; }
		}

		//private int _id;

		///// <summary>
		///// Gets or sets the instance identifier of the entity
		///// </summary>
		//public virtual int Id
		//{
		//    get { return _id; }
		//    private set { _id = value; }
		//}

		private string _title;

		/// <summary>
		/// Gets or sets the tile of the post in the blog
		/// </summary>
		public virtual string Title
		{
			get { return _title; }
			private set { _title = value; }
		}


		private string _body;

		/// <summary>
		/// Gets or sets the body of the blog posting
		/// </summary>
		public virtual string Body
		{
			get { return _body; }
			private set { _body = value; }
		}

		private DateTime? _createdon;

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
			this.CreatedOn = System.DateTime.Now;
		}
		/// <summary>
		/// This will create a comment for the given posting
		/// </summary>
		public virtual Comment CreateComment(string text)
		{
			var comment = new Comment(this, text);
			return comment;
		}

		/// <summary>
		/// This will associate the comment with the current blog posting.
		/// </summary>
		/// <param name="text"></param>
		public virtual void RecordComment(string text)
		{
			var comment = new Comment(this, text);

			if (Comments.Contains(comment)) return;
			Comments.Add(comment);
		}

		/// <summary>
		/// This will allow for the title of the post to be changed.
		/// </summary>
		/// <param name="newTitle"></param>
		public virtual void ChangeTitle(string newTitle)
		{
			this.Title = newTitle;
		}
	}
}