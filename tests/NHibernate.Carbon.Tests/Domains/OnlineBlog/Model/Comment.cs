using System;

namespace NHibernate.Carbon.Tests.Domains.OnlineBlog.Model
{
	public class Comment : Entity<Comment>
	{
		protected Comment()
		{
		}

		public Comment(Post post, string text)
		{
			Post = post;
			Text = text;
			CreatedOn = System.DateTime.Now;
		}

		private string _text;
		/// <summary>
		/// Gets the text for the comment.
		/// </summary>
		public virtual string Text
		{
			get { return _text; }
			private set { _text = value; }
		}

		private DateTime? _createdon;

		/// <summary>
		/// Gets or sets the date and time of the comment for the blog post.
		/// </summary>
		public virtual DateTime? CreatedOn
		{
			get { return _createdon; }
			private set { _createdon = value; }
		}


		private Post _post;

		/// <summary>
		/// Gets the parent Post that the comment belongs to.
		/// </summary>
		public virtual Post Post
		{
			get { return _post; }
			private set { _post = value; }
		}

		public virtual void ChangeText(string text)
		{
			this.Text = text;
		}
	}
}