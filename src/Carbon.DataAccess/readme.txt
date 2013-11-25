NHibernate.Carbon - Libary for building and testing data persistance using NHibernate (with or without the hbm files!!)

Conventions used by this library:

Associations: Parent to Child (Many to One)
===========================================
In order to maintain relationships from one entity to the next, the basic thinking here is to introduce the dependency on the constructor 
as the starting point for determining what object "has a relationship" with another object. The process for relating entities together is based
on what parent entity has child entities (normally denoted by a property of "Id"). Also, when asking for instances of the "child" object we go 
through the parent with a factory method to bind the relationship from a schema perspective.

Example:

A Loan Application "has a" Borrower

Code:

public class LoanApplication
{
    private Borrower _borrower;

	public virtual Borrower Borrower
	{
		get {return _borrower;}
		private set {_borrower = value;}
	}

	public virtual void CreateBorrower()
	{
	   // create the "child" (borrower) and bind to "parent" (loan application)
	   this.Borrower = new Borrower(this); 
	}
}

// the borrower can not exist without an application so the constructor enforces this relationship...
public class Borrower
{
	private LoanApplication _loanapplication

	public virtual LoanApplication Application 
	{
		get {return _loanapplication;}
		private set {_loanapplication = value;}
	}

	// Needed for NHibernate to create the entity..
	private Borrower()
	{}

	public Borrower(LoanApplication application)
	{
		this.LoanApplication = application;
	}
}

Mapping (on LoanApplication mapping file):

 <many-to-one name="Borrower" 
	class="Borrower" cascade="all" 
	access="nosetter.lowercase-underscore" column="borrowerID" 
	foreign-key="fk_LoanApplication_has_instances_of_Borrower" 
	fetch="join" not-found="ignore" />


Associations: Many to One (as collection)
==========================================
Many to one relationships are denoted by having a collection of a child entity on the parent. 

Example:

An Loan Application "has" Processing Comments

Code:

public class LoanApplication
{
    private List<ProcessingComment> _comments = new List<ProcessingComment>();

	public virtual IList<ProcessingComment> Comments
	{
		get {return _comments;}
		private set {_comments = value;}
	}

	// here is the "factory" method that will control access
	// update the  child collection on the parent. this 
	// is the point to control uniqueness and business rules 
	// for addition...
	public virtual void CreateComment(string comment)
	{
	   // create the "child" and bind to "parent"
	   var processingComment = new ProcessingComment(this, comment); 

	   // add the comment to the collection (parent controls the relationship and content):
	   if(this.Comments.Contains(processingComment) == false)
	   {
			this.Comments.Add(processingComment);
		}
	}
}

// the comments can not exist without an application...
public class ProcessingComment
{
	private LoanApplication _loanapplication
	private string _comment; 
	private DateTime _timestamp; 

	public virtual LoanApplication Application 
	{
		get {return _loanapplication;}
		private set {_loanapplication = value;}
	}

	public virtual string Comment
	{
		get {return _comment;}
		private set {_comment = value;}
	}

	public virtual DateTime Timestamp
	{
		get {return _timestamp;}
		private set {_timestamp = value;}
	}

	// Needed for NHibernate to create the entity..
	private ProcessingComment()
	{}

	public ProcessingComment(LoanApplication application, string comment)
	{
		this.Application = application;
		this.Comment = comment;
		this.Timestamp = DateTime.Now();
	}
}

Mapping (on Loan Application mapping file):

<set||bag access="nosetter.lowercase-underscore" name="ProcessingComment" inverse="true" cascade="all" generic="true">
      <key column="processingCommentID" foreign-key="fk_LoanApplication_has_instances_of_ProcessingComment" />
      <one-to-many class="ProcessingComment" />
</set||bag>

**set : collection of non-repeating elements 
  bag : collection of possible repeating elements

Note that the "inverse=false" setting says that the Loan Application controls the relationship and not the "Processing Comments" object

What about domain driven design?
==========================
This follows pretty closely to the principles of DDD for the domain model for persistance. The dependencies are managed on the constructor 
to clearly delineate entity relationships for life-cycle events. As for business rules, domain services, repositories and application services, these 
are created as you need them and used in the appropriate place. 


Hints: 
http://www.archfirst.org/books/service-injection-entities
http://nhforge.org/blogs/nhibernate/archive/2008/12/12/entities-behavior-injection.aspx (preferred)

Limitations:
==========================
1. Base classes to support common entity attributes and operations : Since the library uses the 
inheritance tree of the entity to determine the mapping structure, using common base 
classes for managing the ID, Equals(), and GetHashCode() operations (i.e. Entity<T>)  is not supported. 
These core operations are recommeneded to be done on an entity by entity case.

2. Collections of value objects (i.e. components) is not supported for persistance : The reason for this is the library is concerned 
primarly with persisting entities for common usage and it is quite an edge case scenario to persist multiple value 
objects for an entity (think about how they will be reconsituted on the object from the persistance store if they
do not have anything to uniquely identify them -- value objects by definition do not have an identity).

3. Production worthiness: Just use the library to help create the persistance associated with the domain from a write-perspective and 
use native DI and NHibernate ISession as normal, this is used primarily for evolution based testing of the domain without using mapping files 
or mapping classes. Develop the model, hook in your conventions, test, repeat...
