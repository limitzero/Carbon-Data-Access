using System;
using System.Reflection;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Persistance;
using NHibernate.Cfg;

namespace NHibernate.Carbon.ForTesting
{
	/// <summary>
	/// Local set up fixture for testing interactions  with a database via NHibernate settings for auto persistance.
	/// </summary>
	/// <example>
	/// Example (using Xunit):
	/// 
	/// public class DomainModelPersistanceTests : BaseAutoPersistanceTestFixture
	/// {
	///       public DomainModelPersistanceTests()
	///       {
	///           OneTimeInitializeWithModel(GetModel());
	///           CreateUnitOfWork();
	///       }
	/// 
	///       [Fact]
	///       public void Can_save_order()
	///       {
	///           var order = new Order("123","This is a sample order");
	/// 
	///           Repository.Save{Order}(order);
	/// 
	///           var fromDb = Repository.FindById{Order}(order.Id);
	///           Assert.Equal(order.Id, fromDb.Id);
	///       }
	/// 
	///        private  AutoPersistanceModel GetModel()
	///       {
	///           var model = new AutoPersistanceModel(GetConventions());
	///
	///          // or use (sparingly as these conventions are not exspansive): 
	///          // AutoPersistanceModel model  = new AutoPeristanceModel(); //uses defaults for conventions that are the same as below
	///
	///          //set the assembly for the domain model to be auto-persisted:
	///          model.Assembly("{your domain model assembly name here}");
	///          model.Namespace.EndsWith("{any subfolder name where the entities are located}");
	///
	///          model.WriteMappingsTo("{your location for the *.hbm.xml files}");
	///          // when  you are satisfied with the mapping files, be sure to remove this setting from the configuration.
	///
	///          return model;
	///      }
	///     
	///       // listing of conventions used for our model:
	///       private  Convention GetConventions()
	///      {
	///          Convention convention = new Convention();
	///          convention.PluralizeTableNames();  // pluralize the table names in the database
	///          convention.MemberAccess.NoSetterLowerCaseUnderscore(); // how to access the members on the entities
	/// 
	///          convention.PrimaryKey.PropertyName("Id"); //exclude primary keys in property definition.
	///          convention.PrimaryKey.RenderAsLowerCaseEntityNameFollowedByID(); // how to describe the primary key in the table
	///          convention.PrimaryKey.MemberAccess.NoSetterCamelCaseUnderscore();  // how to access the primary key member
	///
	///           convention.ForeignKey.RenderAsParentEntityHasInstancesOfChildEntity(); 
	///           convention.ManyToManyTableName.RenderAsParentEntityNameConcatenatedWithChildEntityNamePluralized();
	/// 
	///            convention.Property.SetLargeTextFieldLengthsAndNames(10000, "Comments", "Notes", "Body", "History", "Message", "Event", "Text");
	///            convention.Property.SetDefaultTextFieldLength(200);
	///            convention.Property.RenderAsLowerCaseInRepository();
	///
	///            return convention;
	///        }
	/// }
	/// </example>
	public abstract class BaseAutoPersistanceTestFixture : IDisposable
	{

		private IPersistanceStrategy _strategy;
		private static ISessionFactory _sessionFactory;
		private static AutoPersistanceModel _model;

		public void Dispose()
		{
			DisposeUnitOfWork();

			_strategy = null;
			_sessionFactory = null;
			_model = null;
		}

		/// <summary>
		/// Gets the underlying persistance model created based on conventions.
		/// </summary>
		protected AutoPersistanceModel Model { get; private set; }

		/// <summary>
		/// Sets the persistance strategy to use while testing interaction with the persistance store.
		/// </summary>
		/// <param name="strategy"></param>
		public void SetPersistanceStrategyUsing(IPersistanceStrategy strategy)
		{
			_strategy = strategy;
		}

		#region -- public methods --
		/// <summary>
		/// This will configure NHibernate with the settings as kept in the configuration file for NHibernate.
		/// </summary>
		public void OneTimeInitialize(IPersistanceStrategy strategy, string assembly)
		{
			Assembly asm = Assembly.Load(assembly);
			OneTimeInitialize(strategy, asm);
		}

		/// <summary>
		/// This will configure NHibernate with the settings as kept in the configuration file for NHibernate.
		/// </summary>
		public void OneTimeInitialize(IPersistanceStrategy strategy, params Assembly[] assemblies)
		{
			// configure NHibernate based on the assemblies:
			try
			{
				_strategy = strategy;
				foreach (Assembly assembly in assemblies)
				{
					_strategy.Configuration.AddAssembly(assembly);
				}

				_model = null;
				_sessionFactory = _strategy.Configuration.BuildSessionFactory();
			}
			catch (Exception exc)
			{
				string msg = "An error has occurred while attempting to initialize the persistance strategy. Reason: " + exc.Message;
				throw new Exception(msg, exc);
			}
		}

		/// <summary>
		/// This will configure NHibernate for testing based on the configuration of the auto-persist model.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cacheModelDefinition">Allows the test fixture to cache the model definition</param>
		public void OneTimeInitalizeWithModel(AutoPersistanceModel model, bool cacheModelDefinition = false)
		{
			// configure NHibernate based on the assemblies:
			try
			{
				// configure the model then initialize the persistance strategy:
				if(cacheModelDefinition == true)
				{
					if (_model == null)
					{
						model.Build();
						_model = model;
					}
				}
				else
				{
					model.Build();
					_model = model;	
				}
				
				_sessionFactory = null;

				this.Model = model;

				var maps = model.GetMaps();
				System.Diagnostics.Debug.WriteLine("Entity Mapping(s): \r" + maps);

			}
			catch (Exception exc)
			{
				string msg = "An error has occurred while attempting to initialize the model based on the persistance strategy. Reason: " + exc.Message;
				throw new Exception(msg, exc);
			}
		}

		/// <summary>
		/// This will configure the data repository with the settings as kept in the configuration file by NHibernate.
		/// </summary>
		public void CreateUnitOfWork()
		{
			// configure the data repository based on the NHibernate mappings:
			try
			{
				if (GetConfiguration() != null)
				{
					NHibernate.Tool.hbm2ddl.SchemaExport exporter = new NHibernate.Tool.hbm2ddl.SchemaExport(GetConfiguration());
					exporter.Execute(true, true, false);
				}
			}
			catch (Exception exc)
			{
				string msg = "An error has occurred while attempting to begin the unit of work session. Reason: " + exc.Message;
				throw new Exception(msg, exc);
			}
		}

		/// <summary>
		/// This will destroy the data repository with the settings as kept in the configuration file by NHibernate.
		/// </summary>
		public void DisposeUnitOfWork()
		{
			try
			{
				if (GetConfiguration() != null)
				{
					NHibernate.Tool.hbm2ddl.SchemaExport exporter = new NHibernate.Tool.hbm2ddl.SchemaExport(GetConfiguration());
					exporter.Execute(true, true, true);
				}

				// if the schema is removed, the the persistance strategy should be de-initialized as well;
				_strategy = null;
				_sessionFactory = null;

			}
			catch (Exception exc)
			{
				string msg = "An error has occurred while attempting to end the unit of work session. Reason: " + exc.Message;
				throw new Exception(msg, exc);
			}
		}

		/// <summary>
		/// (Use for unit testing only). This will return a local instance of the NHibernate session from the underlying auto-persistance model
		/// and the transactions needed to persist the data to the persistance store.
		/// </summary>
		/// <returns></returns>
		public TransactedSession OpenTransactedSession()
		{
			return new TransactedSession();
		}

		private Configuration GetConfiguration()
		{
			Configuration configuration = null;

			if (_model == null && _strategy == null) return configuration; 

			if (_model != null)
			{
				configuration = _model.CurrentConfiguration;
			}
			else
			{
				configuration = _strategy.Configuration;
			}

			return configuration;
		}

		#endregion

		#region -- nested classes --
		/// <summary>
		/// This is the data persistance and retreival object for testing interactions via NHibernate (for testing only)
		/// </summary>
		public class TransactedSession : IDisposable
		{
			#region -- local variables --
			private static object[] _saveOperations = new object[] { };
			private static object[] _deleteOperations = new object[] { };
			private ISession _session;
			#endregion

			public ISession CurrentSession
			{
				get { return _session; }
			}

			public TransactedSession()
			{
				InitializeSession();
			}

			public void Dispose()
			{
				_saveOperations = null;
				_deleteOperations = null;
			}

			/// <summary>
			/// Retrieves an object based on type as indicated by 
			/// the given object key.
			/// </summary>
			/// <typeparam name="T">Type of the object to be returned and retrieved</typeparam>
			/// <param name="key">Key used to fetch the object.</param>
			/// <returns></returns>
			public T FindById<T>(object key)
			{
				T retval = default(T);

				try
				{
					if (_session.IsConnected == false)
						_session.Reconnect();

					retval = _session.Get<T>(key);
				}
				catch (Exception exc)
				{
					string msg = "An error has occurred while attempting to retrieve object [" +
								 default(T).GetType().FullName + "] with the key of [" +
								 key.ToString() +
								 "] for the current unit of work session. Reason: " + exc.Message;
					throw;
				}
				finally
				{
					_session.Flush();
					_session.Disconnect();
				}

				return retval;
			}

			/// <summary>
			/// Saves a single object for persistance.
			/// </summary>
			/// <typeparam name="T">Type of object to be persisted</typeparam>
			/// <param name="currentObject">Current object to be persisted.</param>
			public void Save<T>(object currentObject)
			{
				RegisterSave(currentObject);
				Commit();
			}

			/// <summary>
			/// Saves a series of entities to the persistance store in 
			/// one transacted session.
			/// </summary>
			/// <param name="entities"></param>
			public void Save(params object[] entities)
			{
				RegisterSave(entities);
				Commit();
			}

			/// <summary>
			/// Deletes a single object from persistance store.
			/// </summary>
			/// <typeparam name="T">Type of object to be deleted</typeparam>
			/// <param name="currentObject">Current object to be deleted.</param>
			public void Delete<T>(object currentObject)
			{
				RegisterDelete(currentObject);
				Commit();
			}

			/// <summary>
			/// Exposes the NHibernate session factory.
			/// </summary>
			/// <returns></returns>
			public static ISessionFactory GetSessionFactory()
			{
				ISessionFactory factory = null;

				if (_model != null)
				{
					factory = _model.CurrentSessionFactory;
				}
				else
				{
					factory = _sessionFactory;
				}

				return factory;
			}

			private void InitializeSession()
			{
				_session = GetSessionFactory().OpenSession();
			}

			/// <summary>
			/// The Commit method persists all changes to the repository in order 
			/// as they were received.
			/// </summary>
			private void Commit()
			{
				try
				{
					if (_session.IsConnected == false)
						_session.Reconnect();

					using (ITransaction txn = _session.BeginTransaction())
					{

						if (_saveOperations.Length > 0)
						{
							foreach (object saveOperation in _saveOperations)
							{
								if (saveOperation != null)
									_session.SaveOrUpdate(saveOperation);
							}
						}

						if (_deleteOperations.Length > 0)
						{
							foreach (object deleteOperation in _deleteOperations)
							{
								if (deleteOperation != null)
									_session.Delete(deleteOperation);
							}
						}

						txn.Commit();
					}

				}
				catch (Exception exc)
				{
					string msg = "An error has occurred while attempting to commit the unit of work for the session for list of objects. Reason: " + exc.Message;
					throw new Exception(msg, exc);
				}
				finally
				{
					Clear();
					_session.Flush();
					_session.Disconnect();
				}
			}

			/// <summary>
			/// Registers a series of objects to be updated or inserted 
			/// into the repository based on the order supplied.
			/// </summary>
			/// <param name="objects">A list of objects to be persisted.</param>
			private static void RegisterSave(params object[] objects)
			{
				if (objects.Length > 0)
				{
					_saveOperations = objects;
				}
			}

			/// <summary>
			/// Registers a series of objects to be removed from 
			/// the repository based on the order supplied.
			/// </summary>
			/// <param name="objects">A list of objects to be removed.</param>
			private static void RegisterDelete(params object[] objects)
			{
				if (objects.Length > 0)
				{
					_deleteOperations = objects;
				}
			}

			/// <summary>
			/// The Clear method removes all registered save and delete operations
			/// from memory for persistance.
			/// </summary>
			private static void Clear()
			{
				_saveOperations = new object[] { };
				_deleteOperations = new object[] { };
			}

		}

		#endregion

	}
}