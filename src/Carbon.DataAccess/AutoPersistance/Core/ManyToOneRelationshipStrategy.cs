using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Carbon.Repository.AutoPersistance.Core
{
    public class ManyToOneRelationshipStrategy : BaseRelationshipStrategy, IRelationship
    {
        private Convention _convention = null;
        private Type _entity = null;

        public ManyToOneRelationshipStrategy(Convention convention, Type entity)
        {
            _convention = convention;
            _entity = entity;
        }

        public string Build()
        {
            string results = string.Empty;

            //only examine the properties that are currently domain model objects;
            IList<PropertyInfo> modelAsProperties = GetPropertiesDefinedAsModelEntitiesFor(_entity);
                                                                                                                                             
            IList<Type> entitiesInConstructor = GetModelEntitiesDefinedInConstructorFor(_entity, ORMUtils.FindAllEntities(_convention,_entity.Assembly));

            foreach (Type constructorEntity in entitiesInConstructor)
            {
                results += GetManyToOneRelationshipDefinitonFor(constructorEntity, _entity);
            }

            /*
            Type parentEntity = null;
            if (IsModelEntityDefinedInConstructorFor(_entity, GetModelEntitiesAsTypes(_entity.Assembly), out parentEntity))
            {
                results = GetManyToOneRelationshipDefinitonFor(parentEntity, _entity);
            }
             */

            return results;
        }

        private IList<Type> GetModelEntitiesDefinedInConstructorFor(Type entity, IList<Type> modelEntities)
        {
            IList<Type> results = new List<Type>(); 

            ConstructorInfo[] constructors = entity.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo theConstructor = null; 

            foreach (ConstructorInfo info in constructors)
            {
                if(theConstructor == null)
                {
                    theConstructor = info;    
                }

                // find the greediest constructor for basing this relationship on:
                if(theConstructor.GetParameters().Length < info.GetParameters().Length)
                {
                    theConstructor = info;
                }
            }

            foreach (ParameterInfo param in theConstructor.GetParameters())
            {
                if (modelEntities.Contains(param.ParameterType) && ORMUtils.FindIdentityFieldFor(_convention, param.ParameterType) != null)
                {
                    if (!results.Contains(param.ParameterType))
                        results.Add(param.ParameterType);
                }
            }

            return results;
        }

        private string GetManyToOneRelationshipDefinitonFor(Type parentEntity, Type _entity)
        {
            StringBuilder results = new StringBuilder();

            IList<PropertyInfo> parentProperties = GetPropertiesDefinedAsModelEntitiesFor(_entity);
            PropertyInfo parentProperty = null;

            //scan all of the properties on the "many" side of the relationship looking for 
            //a property defined as the parent type from the entity model:
            foreach (PropertyInfo property in parentProperties)
            {
                if (property.PropertyType == parentEntity)
                {
                    parentProperty = property;
                    break;
                }
            }

            results.Append(string.Format("<!-- many \"{0}\" are associated with one instance of a \"{1}\" -->", _entity.Name, parentEntity.Name));
            results.Append("\r\n");
            results.Append(string.Format("<many-to-one name=\"{0}\" class=\"{1}\" cascade=\"all\" access=\"{2}\"  column=\"{3}\" foreign-key=\"{4}\" fetch=\"join\" not-found=\"ignore\" />",
                                         parentProperty.Name,
                                         parentEntity.Name,
                                         _convention.MemberAccess.Strategy,
                                         base.BuildPrimaryKeyColumnName(_convention, parentEntity),
                                         base.BuildForeignKeyName(_convention, parentEntity, _entity)));
            results.Append("\r\n");

            return results.ToString();
        }

        private bool IsModelEntityDefinedInConstructorFor(Type entity, IList<Type> modelEntityTypes, out Type parentEntityType)
        {
            bool results = false;
            Type parentEntity = null;
            
            ConstructorInfo[] constructors = entity.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach(ConstructorInfo info in constructors)
            {
                foreach(ParameterInfo param in info.GetParameters())
                {
                    if (modelEntityTypes.Contains(param.ParameterType))
                    {
                        parentEntity = param.ParameterType;
                        results = true;
                        break;
                    }
                }

                if (results == true)
                    break;
            }

            parentEntityType = parentEntity;
            return results;
        }

        private IList<PropertyInfo> GetPropertiesDefinedAsModelEntitiesFor(Type entity)
        {
            IList<PropertyInfo> results = new List<PropertyInfo>();
            IList<Type> modelTypes = GetModelEntitiesAsTypes(entity.Assembly); 

            foreach (PropertyInfo property in _entity.GetProperties())
            {
                if (modelTypes.Contains(property.PropertyType))
                {
                    results.Add(property);
                }
            }

            return results;
        }

        private IList<Type> GetModelEntitiesAsTypes(Assembly asm)
        {
            IList<Type> results = new List<Type>();
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                    results.Add(type);
            }

            return results;
        }


    }
}