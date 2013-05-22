/* 
 * Copyright 2012 © Victor Chekalin
 * 
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 * 
 */

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB;
using VCExtensibleStorageExtension.Attributes;
using System.Collections.Generic;
using VCExtensibleStorageExtension.ElementExtensions;

namespace VCExtensibleStorageExtension
{
    class EntityConverter : IEntityConverter
    {
        
        private readonly ISchemaCreator _schemaCreator;

        public EntityConverter(ISchemaCreator schemaCreator)
        {
            _schemaCreator = schemaCreator;
        }

        #region Implementation of IEntityCreator

        /// <summary>
        /// Convert object from IRevitEntity to a ExStorage.Entity object
        /// </summary>
        /// <param name="revitEntity">IRevitEntity object to convert</param>
        /// <returns>Converted ExStorage.Entity</returns>
        public Entity Convert(IRevitEntity revitEntity)
        {
            var entityType = revitEntity.GetType();

            //Create the schema for IRevitEntity object
            var schema = _schemaCreator.CreateSchema(entityType);

            Entity entity = new Entity(schema);

            /* Iterate all of the schema field and 
             * get IRevitEntity object property value 
             * for each field
             */
            var schemaFields = schema.ListFields();
            foreach (var field in schemaFields)
            {
                /*Get the property of the IRevitEntity with the 
                 * same name as FieldName
                 */

                var property = entityType.GetProperty(field.FieldName);

                //Get the property value
                dynamic propertyValue = property.GetValue(revitEntity, null);                                               
                   
                /*We don't need to write null value to
                 * the ExStorage.Entity
                 * So we just skip this property
                 */
                if (propertyValue == null)
                    continue;

                Type propertyValueType =
                    propertyValue.GetType();

                switch (field.ContainerType)
                {
                    case ContainerType.Simple:        
               
                        propertyValue = ConvertSimpleProperty(propertyValue, field);

                        if (field.UnitType == UnitType.UT_Undefined)
                        {
                            entity.Set(field, propertyValue);
                        }
                        else
                        {
                            var firstCompatibleDUT = GetFirstCompatibleDUT(field);

                            entity.Set(field, propertyValue, firstCompatibleDUT);
                        }

                        break;
                    case ContainerType.Array:    
                   
                        /* If we have a deal with null IList or with  
                         * empty IList we must skip this field.                         
                         */

                        if (propertyValue.Count == 0)
                            continue;

                        var convertedIListFieldValue =
                            ConvertIListProperty(propertyValue, field);

                        /* convertedArrayFieldValue is an List<T> object.                        
                         * Entity.Set method throws an exception if I do not pass 
                         * an IList interface as value. 
                         * Even if the type implements IList<T> interface
                         * With this method which do nothing except takes a 
                         * IList parameter instead FieldType, it works propoerly
                         */

                        if (field.UnitType == UnitType.UT_Undefined)
                        {
                            EntityExtension.SetWrapper(entity, field, convertedIListFieldValue);
                        }
                        else
                        {
                            var firstCompatibleDUT = GetFirstCompatibleDUT(field);

                            EntityExtension.SetWrapper(entity,
                                field,
                                convertedIListFieldValue,
                                firstCompatibleDUT);
                        }

                        break;

                    case ContainerType.Map:
                        var convertedMapFieldValue =
                            ConvertIDictionaryProperty(propertyValue, field);

                        if (field.UnitType == UnitType.UT_Undefined)
                        {
                            EntityExtension.SetWrapper(entity, field, convertedMapFieldValue);
                        }
                        else
                        {
                            var firstCompatibleDUT = GetFirstCompatibleDUT(field);

                            EntityExtension.SetWrapper(entity, 
                                field, 
                                convertedMapFieldValue, 
                                firstCompatibleDUT);
                        }
                        break;
                    default:
                        throw new NotSupportedException("Unknown Field.ContainerType");
                }

                
            }

            return entity;
        }

        private object ConvertIDictionaryProperty(dynamic propertyValue, Field field)
        {
            Type propertyValueType =
                propertyValue.GetType();

            /* An ExStorage.Entity MAp field stores an IDictionary<,>
             * So, it is need to sure, that property value type
             * supports generic IDictionary<,> interface.
             * As far as I create array field only if the property type 
             * implements IDictionary<,> in the SchemaCreateor, this condition is always
             * true.
             */

            var implementIDictionaryInterface =
                            propertyValueType
                                .GetInterfaces()
                                .Any(x => x.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (!implementIDictionaryInterface)
            {
                throw new NotSupportedException("Unsupported type");
            }

            /* ExStorage.Entity supports primitive generic types, described 
             * here
             * http://wikihelp.autodesk.com/Revit/enu/2013/Help/00006-API_Developer's_Guide/0135-Advanced135/0136-Storing_136/0141-Extensib141
             * And also generic type can be an ExStorage.Entity, i.e. IDictionary<T, Entity>.
             * So, need to check whether generic type is Entity or not.
             * If true, need to convert IList<IRevitEntity> to the IDictionary<T, Entity>.
             */


            if (field.ValueType == typeof(Entity))
            {
                /* Create new list
                 * As property value is a IList<IRevitEntity>, I can get
                 * size of the list and pass it to the new list
                 */

                var dictionaryType = typeof (Dictionary<,>)
                    .MakeGenericType(field.KeyType, typeof (Entity));

                var mapArray =
                    Activator
                    .CreateInstance(dictionaryType, new object[] {propertyValue.Count}) 
                        as IDictionary;

                foreach (var keyValuePair in propertyValue)
                {
                    //convert each IRevitEntity to the ExStorage.Entity
                    var convertedEntity =
                        Convert(keyValuePair.Value);
                    mapArray.Add(keyValuePair.Key, convertedEntity);
                }

                return mapArray;
            }

            return propertyValue;
        }

        /// <summary>
        /// Convert IRevitEntity property value to the
        /// ExStorage.Entity property value
        /// </summary>
        /// <param name="propertyValue">Value of the IRevitEntity property</param>
        /// <param name="field">Field to be converted</param>
        /// <returns></returns>
        private object ConvertSimpleProperty(dynamic propertyValue, Field field)
        {
            if (field.ContainerType != ContainerType.Simple)
            {
                throw new InvalidOperationException("Field is not a simple type");
            }

            /* If field value type is Entity,
             * the Property value type is IRevitEntity type.
             * So it is need to convert IRevitEntity to the 
             * ExStorageEntity
             */
            if (field.ValueType == typeof (Entity))
            {
                propertyValue = Convert(propertyValue);
            }
            return propertyValue;
        }       

        /// <summary>
        /// Convert IRevitEntity property of IList type
        /// to the ExStorage.Entity type
        /// </summary>
        /// <param name="propertyValue">Value of the IRevitEntity property</param>
        /// <param name="field">Field to be converted</param>
        /// <returns></returns>
        private object ConvertIListProperty(dynamic propertyValue, Field field)
        {
            Type propertyValueType = 
                propertyValue.GetType();

            /* An ExStorage.Entity Array field stores an IList
             * So, it is need to sure, that property value type
             * supports generic IList<> interface.
             * As far as I create array field only if the property type 
             * implements IList<> in the SchemaCreateor, this condition is always
             * true.
             */

            var implementIListInterface =
                            propertyValueType
                                .GetInterfaces()                                
                                .Any(x => x.GetGenericTypeDefinition() == typeof(IList<>));
            if (!implementIListInterface)
            {
                throw new NotSupportedException("Unsupported type");
            }

            /* ExStorage.Entity supports primitive generic types, described 
             * here
             * http://wikihelp.autodesk.com/Revit/enu/2013/Help/00006-API_Developer's_Guide/0135-Advanced135/0136-Storing_136/0141-Extensib141
             * And also generic type can be an ExStorage.Entity, i.e. IList<Entity>.
             * So, need to check whether generic type is Entity or not.
             * If true, need to convert IList<IRevitEntity> to the IList<Entity>.
             */


            if (field.ValueType == typeof(Entity))
            {
                /* Create new list
                 * As property value is a IList<IRevitEntity>, I can get
                 * size of the list and pass it to the new list
                 */

                IList<Entity> entityList =
                    new List<Entity>(propertyValue.Count);

                foreach (IRevitEntity revitEntity in propertyValue)
                {
                    //convert each IRevitEntity to the ExStorage.Entity
                    var convertedEntity =
                        Convert(revitEntity);
                    entityList.Add(convertedEntity);
                }

                return entityList;
            }

            /* If generic type is a primitive types,
             * just return source property value as IList<>
             */
            return propertyValue;
        }


        /// <summary>
        /// Convert ExStorage.Entity to the IRevitEntity object
        /// </summary>
        /// <typeparam name="TRevitEntity">The type of the IRevitEntity</typeparam>
        /// <param name="entity">Entity to convert</param>
        /// <returns>Converted IRevitEntity</returns>
        public TRevitEntity Convert<TRevitEntity>(Entity entity) where TRevitEntity : class, IRevitEntity
        {
            Type revitEntityType = typeof(TRevitEntity);

            /* Create new instance of the TRevitEntity which is a class
             * that implements IRevitEntity interface
            */
            var revitEntity = Activator.CreateInstance<TRevitEntity>();

            var schema = entity.Schema;

            Type entityType = typeof(Entity);

            /* Iterate all of the schema fields
             * and set the IRevit entity property, with 
             * FieldName name, with value of fieldValue
             */
            var schemaFields = schema.ListFields();            
            foreach (var field in schemaFields)
            {
                // Get the property of the IRevitEntity class
                var property = revitEntityType.GetProperty(field.FieldName);

                //does not work
                //var entityValue = entity.Get<dynamic>(field);

                /*
                 * Get the field value of the entity
                 * I.e. call Entity.Get<FieldTypeValue>
                 * As we don't know the FieldTypeValue at 
                 * the compile time, we invoke the 
                 * Entity.Get<> method via reflection
                 */
                object entityValue = null;
                switch (field.ContainerType)
                {
                    case ContainerType.Simple:
                        
                        entityValue = 
                            GetEntityFieldValue(entity, 
                                field,
                                field.ValueType);

                        if (entityValue == null)
                        {
                            continue;
                        }

                        if (entityValue is Entity)
                        {                            
                            entityValue =
                                Convert(property.PropertyType, entityValue as Entity);
                        }
                        break;
                    case ContainerType.Array:

                        /*
                         * Call Entity.Get<FieldType>(Field field) method
                        */
                        var iListType = typeof (IList<>);
                        var genericIlistType = iListType.MakeGenericType(field.ValueType);                                     

                        /* Get the field value from entity.
                         * As Field.Container type is an Array,
                         * the entity value has IList<T> type.
                         */
                        entityValue =
                            GetEntityFieldValue(entity,
                                field,
                                genericIlistType);

                        if (entityValue == null)
                        {
                            continue;
                        }

                        var listEntityValues = entityValue as IList;

                        /* create a new instance of a property of the IRevitEntity
                         * object which implements IList<T> interface.
                         */
                        IList listProperty;

                        /* property type which implements IList<T> interface,
                         * may have constructor with capacity parameter.
                         * If have, pass as capacity. If not - create 
                         * instance with default constructor
                         */

                        if (property
                            .PropertyType
                            .GetConstructor(new Type[] { typeof(int) }) != null)
                        {
                            listProperty =
                                Activator
                                .CreateInstance(property.PropertyType,
                                         new object[] { listEntityValues.Count }) as IList;
                        }
                        else
                        {
                            listProperty =
                                Activator
                                    .CreateInstance(property.PropertyType) as IList;
                        }
                        

                        /* if field.ValueType is Entity
                         * We must convert all of the Entity 
                         * to the IRevitEntity
                         * I.e. get IList<IRevitEntity> from
                         * IList<Entity>
                         */
                        if (field.ValueType == typeof(Entity))
                        {
                                                        
                            /* Get the generic type of the IList
                             * it should be IRevitEntity.
                             * So, we convert an Entity to an IRevitEntity
                             */
                            var iRevitEntityType =
                                property.PropertyType.GetGenericArguments()[0];

                            foreach (Entity listEntityValue in listEntityValues)
                            {
                                var convertedEntity = Convert(iRevitEntityType,
                                                              listEntityValue);                                    
                                listProperty.Add(convertedEntity);
                            }

                            
                        }
                        else
                        {
                            foreach (var value in listEntityValues)
                            {
                                listProperty.Add(value);
                            }
                        }

                        entityValue = listProperty;

                        break;

                    // IDictionary<,>
                    case ContainerType.Map:
                        /*
                        * Call Entity.Get<FieldType>(Field field) method
                       */
                        var iDicitonaryType = typeof(IDictionary<,>);
                        var genericIDicitionaryType = 
                            iDicitonaryType.MakeGenericType(field.KeyType, field.ValueType);

                        /* Get the field value from entity.
                         * As Field.Container type is an Array,
                         * the entity value has IList<T> type.
                         */
                        entityValue =
                            GetEntityFieldValue(entity,
                                field,
                                genericIDicitionaryType);

                        if (entityValue == null)
                        {
                            continue;
                        }

                        var mapEntityValues = entityValue as IDictionary;

                        /* create a new instance of a property of the IRevitEntity
                        * object which implements IList<T> interface.
                        */
                        IDictionary dictProperty;

                        if (property.PropertyType.GetConstructor(new[] { typeof(int) })!=null)
                        {
                            dictProperty =
                            Activator
                            .CreateInstance(property.PropertyType,
                                        new object[] { mapEntityValues.Count }) as IDictionary;
                        }
                        else
                        {
                            dictProperty =
                                Activator
                                    .CreateInstance(property.PropertyType) as IDictionary;
                        }


                        /* if field.ValueType is Entity
                         * We must convert all of the Entity 
                         * to the IRevitEntity
                         * I.e. get IDictionary<T, IRevitEntity> from
                         * IDictionary<T, Entity>
                         */
                        if (field.ValueType == typeof(Entity))
                        {
                            
                            /* Get the generic type of the IList
                             * it should be IRevitEntity.
                             * So, we convert an Entity to an IRevitEntity
                             */
                            var iRevitEntityType =
                                property.PropertyType.GetGenericArguments()[1];

                            foreach (dynamic keyValuePair in mapEntityValues)
                            {
                                var convertedEntity = Convert(iRevitEntityType,
                                                              keyValuePair.Value);

                                dictProperty.Add(keyValuePair.Key, convertedEntity);
                            }

                            
                        }
                        else
                        {
                            foreach (dynamic keyValuePair in mapEntityValues)
                            {
                                dictProperty.Add(keyValuePair.Key, keyValuePair.Value);
                            }
                        }

                        entityValue = dictProperty;

                        break;
                }

                if (entityValue !=null)
                    property.SetValue(revitEntity, entityValue, null);                
            }

            return revitEntity;
        }

        #endregion

        private object GetEntityFieldValue(Entity entity, 
            Field field,
            Type fieldValueType)
        {

            /*
             * When we save entity to an element and 
             * entity has an SubEntity we should ommit
             * set Subentity. And there is a cse would happen
             * when there is no subschema loaded into the memory.
             * In this case, Revit throws exception about 
             * "There is no Schema with id in memory"
             */
            if (field.SubSchemaGUID != Guid.Empty &&
                field.SubSchema == null)
            {
                return null;
            }

            object entityValue;
            if (field.UnitType == UnitType.UT_Undefined)
            {
                MethodInfo entityGetMethod =
                    entity
                        .GetType()
                        .GetMethod("Get", new[] {typeof (Field)});
                MethodInfo entityGetMethodGeneric =
                    entityGetMethod
                        .MakeGenericMethod(fieldValueType);

                entityValue = entityGetMethodGeneric.Invoke(entity, new[] {field});
            }
            else
            {
                var firstCompatibleDUT = GetFirstCompatibleDUT(field);

                MethodInfo entityGetMethod =
                    entity
                        .GetType()
                        .GetMethod("Get", new[] { typeof(Field), typeof(DisplayUnitType) });
                MethodInfo entityGetMethodGeneric =
                    entityGetMethod
                        .MakeGenericMethod(fieldValueType);

                entityValue = 
                    entityGetMethodGeneric
                        .Invoke(entity, new object[] {field, firstCompatibleDUT});
            }
            return entityValue;
        }

        private object Convert(Type irevitEntityType, Entity entity)
        {
            MethodInfo convertMethod =
                    GetType()
                    .GetMethod("Convert", new[] { typeof(Entity) });
            MethodInfo convertMethodGeneric =
                convertMethod.MakeGenericMethod(irevitEntityType);
            var iRevitEntity = convertMethodGeneric.Invoke(this, new[] { entity });

            return iRevitEntity;
        }

        private DisplayUnitType GetFirstCompatibleDUT(Field field)
        {
            var firstCompatibleDUT = Enum
                    .GetValues(typeof(DisplayUnitType))
                    .OfType<DisplayUnitType>()
                    .FirstOrDefault(field.CompatibleDisplayUnitType);

            return firstCompatibleDUT;
        }

    }


}