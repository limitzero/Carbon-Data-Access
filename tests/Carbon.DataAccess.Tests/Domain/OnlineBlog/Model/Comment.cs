using System;

namespace NHibernate.Carbon.Tests.Domain.OnlineBlog.Model
{
	public class Comment
	{
		protected Comment()
		{
		}

		public Comment(Post post)
		{
			Post = post;
			CreatedOn = DateTime.UtcNow;
		}

		private int _id = 0;
		/// <summary>
		/// Gets or sets the instance identifier of the entity
		/// </summary>
		public virtual int Id { get { return _id; } }


		private string _text = string.Empty;

		/// <summary>
		/// Gets or sets the text of the comment.
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		private DateTime? _createdon = null;
		/// <summary>
		/// Gets or sets the date and time of the comment for the blog post.
		/// </summary>
		public virtual DateTime? CreatedOn
		{
			get { return _createdon; }
			private set { _createdon = value; }
		}

		private Post _post = null;

		/// <summary>
		/// Gets the parent Post that the comment belongs to.
		/// </summary>
		public virtual Post Post
		{
			get { return _post; }
			private set { _post = value; }
		}
	}
}