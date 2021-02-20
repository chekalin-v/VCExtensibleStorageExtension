using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit.ES.ExtensionTestCommand.Simple;
using Revit.ES.Extension.ElementExtensions;

namespace Revit.ES.ExtensionTestCommand
{
    [Transaction(TransactionMode.Manual)]
    public class Command2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc =
                commandData.Application.ActiveUIDocument;

            Reference r;

            try
            {
                r = uiDoc.Selection.PickObject(ObjectType.Element);
            }
            catch (Exception ex)
            {
                return Result.Cancelled;
            }

            var element =
                uiDoc
                    .Document
                    .GetElement(r.ElementId);
            // Create a new instance of the class
            IntEntity intEntity =
                new IntEntity();

            // Set property value
            intEntity.SomeValue = 777;

            // attach to the element
            element.SetEntity(intEntity);


            //read entity
            var intEntity2 =
                element.GetEntity<IntEntity>();

            if (intEntity2 != null)
            {
                TaskDialog.Show(intEntity2.GetType().Name, intEntity2.SomeValue.ToString());
            }


            // 1. Looking for the schema in the memory
            Schema schema = Schema.Lookup(new Guid("4E5B6F62-B8B3-4A2F-9B06-DDD953D4D4BC"));

            // 2. Check if schema exists in the memory or not
            if (schema == null)
            {
                //3. Create it, if not
                schema = CreateSchema();
            }

            // 4. Create entity of the specific schema
            var entity = new Entity(schema);

            // 5. Set the value for the Field.
            // HERE WE HAVE TO REMEMEBER THE NAME OF THE SCHEMA FIELD
            entity.Set("SomeValue", 888);

            // 6. Attach entity to the element
            element.SetEntity(entity);

            // read

            var schema2 =
                Schema.Lookup(new Guid("4E5B6F62-B8B3-4A2F-9B06-DDD953D4D4BC"));
            if (schema2 != null)
            {
                var entity2 = element.GetEntity(schema2);
                var someValue =
                    entity2.Get<int>("SomeValue");
                TaskDialog.Show("Entity value", someValue.ToString());
            }


            //write entity with map and array field

            // Check if schema exists in the memory.
            var schema3 =
                Schema.Lookup(new Guid("1899FD3C-7046-4B53-945A-AA1370B8C577"));

            if (schema3 == null)
            {
                // create if not
                schema3 = CreateComplexSchema();
            }

            var entity4 =
                new Entity(schema3);

            //Map fields
            IDictionary<int, Entity> mapOfEntities =
                new Dictionary<int, Entity>();

            // create sub-entity 1
            var entity7 =
                new Entity(new Guid("4E5B6F62-B8B3-4A2F-9B06-DDD953D4D4BC"));
            entity7.Set("SomeValue", 7);

            // create sub-entity 2
            var entity8 =
                new Entity(new Guid("4E5B6F62-B8B3-4A2F-9B06-DDD953D4D4BC"));
            entity8.Set("SomeValue", 8);

            mapOfEntities.Add(7, entity7);
            mapOfEntities.Add(8, entity8);

            entity4.Set("MapField", mapOfEntities);

            element.SetEntity(entity4);


            //Change value in map field

            var entity10 =
                element.GetEntity(schema3);

            var mapField =
                entity10.Get<IDictionary<int, Entity>>("MapField");
            if (mapField != null)
            {
                if (mapField.ContainsKey(8))
                {
                    var entity11 = mapField[8];
                    entity11.Set("SomeValue", 999);

                    // write changes = 
                    entity10.Set("MapField", mapField);

                    element.SetEntity(entity10);
                }
            }


            // the same with Extension
            ComplexEntity complexEntity =
                new ComplexEntity();

            complexEntity.MapField =
                new Dictionary<int, IntEntity>
    {
        {9, new IntEntity(){SomeValue = 9}},
        {10, new IntEntity(){SomeValue = 10}}
    };

            element.SetEntity(complexEntity);

            //Change value in map field
            var complexEntity2 =
                element.GetEntity<ComplexEntity>();
            if (complexEntity2 != null)
            {
                if (complexEntity.MapField.ContainsKey(9))
                {
                    var entityInMapField =
                        complexEntity.MapField[9];
                    entityInMapField.SomeValue = 9898;

                    element.SetEntity(complexEntity2);
                }
            }

            return Result.Succeeded;
        }

        private Schema CreateComplexSchema()
        {
            SchemaBuilder schemaBuilder =
                new SchemaBuilder(new Guid("1899FD3C-7046-4B53-945A-AA1370B8C577"));
            schemaBuilder.SetSchemaName("ComplexSchema");

            var mapField = schemaBuilder.AddMapField("MapField", typeof(int), typeof(Entity));
            mapField.SetSubSchemaGUID(new Guid("4E5B6F62-B8B3-4A2F-9B06-DDD953D4D4BC"));
            mapField.SetDocumentation("Map field documentation");

            return schemaBuilder.Finish();
        }

        private Schema CreateSchema()
        {
            SchemaBuilder schemaBuilder =
                new SchemaBuilder(new Guid("4E5B6F62-B8B3-4A2F-9B06-DDD953D4D4BC"));

            schemaBuilder.SetSchemaName("SimpleIntSchema");

            // have to define the field name as string and set the type using typeof method
            schemaBuilder.AddSimpleField("SomeValue", typeof(int));

            return schemaBuilder.Finish();
        }
    }
}
