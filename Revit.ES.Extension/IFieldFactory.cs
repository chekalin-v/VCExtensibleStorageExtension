/* 
 * Copyright 2021 © Victor Chekalin
 * 
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autodesk.Revit.DB.ExtensibleStorage;
using Revit.ES.Extension.Attributes;

namespace Revit.ES.Extension
{
    public interface IFieldFactory
    {
        FieldBuilder CreateField(SchemaBuilder schemaBuilder,
            PropertyInfo propertyInfo);
    }

    class FieldFactory : IFieldFactory
    {
        public FieldBuilder CreateField(SchemaBuilder schemaBuilder, 
            PropertyInfo propertyInfo)
        {
            IFieldFactory fieldFactory = null;

            var fieldType = propertyInfo.PropertyType;

            /* Check whether fieldType is generic or not.
             * Only IList<> and IDictionary are supported.            
             */
            if (fieldType.IsGenericType)
            {
                foreach (var interfaceType in fieldType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType &&
                        interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        fieldFactory = new ArrayFieldCreator();
                        break;
                    }
                    else if (interfaceType.IsGenericType &&
                        interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        fieldFactory = new MapFieldCreator();
                        break;                        
                    }
                }
                /*
                if (fieldType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    fieldFactory = new ArrayFieldCreator();
                }
                else if (fieldType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    fieldFactory = new MapFieldCreator();
                }
                else 
                 */
                if (fieldFactory == null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Format("Type {0} does not supported.", fieldType));
                    sb.AppendLine("Only IList<T> and IDictionary<TKey, TValue> generic types are supproted");
                    throw new NotSupportedException(sb.ToString());
                }
            }
            else
            {
                fieldFactory = new SimpleFieldCreator();
            }
            return fieldFactory.CreateField(schemaBuilder, propertyInfo);
        }
    }

    class SimpleFieldCreator : IFieldFactory
    {
        public FieldBuilder CreateField(SchemaBuilder schemaBuilder, PropertyInfo propertyInfo)
        {
            FieldBuilder fieldBuilder;

            var iRevitEntity = propertyInfo.PropertyType.GetInterface("IRevitEntity");
            if (iRevitEntity != null)
            {
                AttributeExtractor<SchemaAttribute> schemaAttributeExtractor =
                    new AttributeExtractor<SchemaAttribute>();

                fieldBuilder = schemaBuilder
                    .AddSimpleField(propertyInfo.Name, typeof(Entity));
                var subSchemaAttribute = 
                    schemaAttributeExtractor
                    .GetAttribute(propertyInfo.PropertyType);
                fieldBuilder
                    .SetSubSchemaGUID(subSchemaAttribute.GUID);
            }
            else
            {
                fieldBuilder =
                    schemaBuilder.AddSimpleField(propertyInfo.Name, propertyInfo.PropertyType);
            }

            return fieldBuilder;
        }
    }    

    class ArrayFieldCreator : IFieldFactory
    {
        public FieldBuilder CreateField(SchemaBuilder schemaBuilder, 
            PropertyInfo propertyInfo)
        {
            FieldBuilder fieldBuilder;

            // Check whether generic type is subentity or not
            var genericType = propertyInfo.PropertyType.GetGenericArguments()[0];

            var iRevitEntity = genericType.GetInterface("IRevitEntity");

            if (iRevitEntity != null)
            {
                fieldBuilder =
                    schemaBuilder.AddArrayField(propertyInfo.Name, typeof(Entity));

                AttributeExtractor<SchemaAttribute> schemaAttributeExtractor =
                    new AttributeExtractor<SchemaAttribute>();               
                var subSchemaAttribute =
                    schemaAttributeExtractor
                    .GetAttribute(genericType);
                fieldBuilder
                    .SetSubSchemaGUID(subSchemaAttribute.GUID);
            }
            else
            {
                fieldBuilder =
                    schemaBuilder.AddArrayField(propertyInfo.Name, genericType);
            }
            return fieldBuilder;
        }
    }

    class MapFieldCreator : IFieldFactory
    {
        public FieldBuilder CreateField(SchemaBuilder schemaBuilder, 
            PropertyInfo propertyInfo)
        {
            FieldBuilder fieldBuilder;

            var genericKeyType = propertyInfo.PropertyType.GetGenericArguments()[0];
            var genericValueType = propertyInfo.PropertyType.GetGenericArguments()[1];

            if (genericValueType.GetInterface("IRevitEntity") != null)
            {
                fieldBuilder =
                schemaBuilder.AddMapField(propertyInfo.Name,
                                          genericKeyType, typeof(Entity));

                AttributeExtractor<SchemaAttribute> schemaAttributeExtractor =
                   new AttributeExtractor<SchemaAttribute>();
                var subSchemaAttribute =
                    schemaAttributeExtractor
                    .GetAttribute(genericValueType);
                fieldBuilder
                    .SetSubSchemaGUID(subSchemaAttribute.GUID);
            }
            else
            {
                fieldBuilder =
                schemaBuilder.AddMapField(propertyInfo.Name,
                                          genericKeyType, genericValueType);                
            }

            var needSubschemaId = fieldBuilder.NeedsSubSchemaGUID();

            return fieldBuilder;
        }
    }
}
