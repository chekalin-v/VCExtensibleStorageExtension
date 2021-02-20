#region Namespaces
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.ES.Extension;
using Revit.ES.Extension.ElementExtensions;

#endregion

namespace Revit.ES.ExtensionTestCommand
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var projectInfo = doc.ProjectInformation;

            FooEntity fooEntity = new FooEntity();

            fooEntity.IntProperty = int.MaxValue;
            fooEntity.ShortProperty = short.MinValue;
            fooEntity.ByteProperty = 0;
            fooEntity.DoubleProperty = 8.23123456789;
            fooEntity.FloatProperty = 0.23345F;
            fooEntity.BoolProperty = true;
            fooEntity.StringProperty = "The quick brown fox jumps over the lazy dog";
            fooEntity.GuidProperty = new Guid("DFCD07E5-7218-4052-8731-1F8B74ABFCF3");
            fooEntity.ElementIdProperty = new ElementId(9872);
            fooEntity.XyzProperty = new XYZ(10.01, 20.02, 30.03);
            fooEntity.UvProperty = new UV(1001, 2222.333);
            fooEntity.DeepEntityProperty =
                new DeepEntity
                    {
                        Count = 789,
                        ElementId = new ElementId(7777)
                    };

            fooEntity.IntArrayProperty =
                new Collection<int> {1, 5, 8, 4, 37, 183403853, -243512, -4122345};
            fooEntity.ShortArrayProperty = 
                new List<short>() {-23,13456,4236,125,752,246,-234};
            fooEntity.ByteArrayProperty = 
                new BindingList<byte>() {0,1,2,3, 255};
            fooEntity.DoubleArrayProperty =
                new ObservableCollection<double>() {-23.45,34.56};
            fooEntity.FloatArrayProperty = 
                new List<float>() {99.8877665544332211F};
            fooEntity.BoolArrayProperty = 
                new Collection<bool>() {true,true, false, true, false, false};
            fooEntity.StringArrayProperty = 
                new List<string>() {"QWERTY", "ASDFGH","ZxCvBN"};
            fooEntity.GuidArrayProperty =
                new List<Guid>(){new Guid("9E7941F8-03EE-48AC-90B7-4352911F06F7"),
                new Guid("78304C8D-B904-47A2-BDF6-C52A6B569D86"),
                new Guid("8B1ADB16-4974-4820-A0E2-129F16620331")};
            fooEntity.ElementIdArrayProperty = 
                new List<ElementId>() {new ElementId(1),
                new ElementId(2),
                new ElementId(3)};
            fooEntity.XyzArrayProperty = 
                new Collection<XYZ>()
                {
                    XYZ.Zero,
                    XYZ.BasisX,
                    XYZ.BasisY,
                    XYZ.BasisZ
                };
            fooEntity.UvArrayProperty = 
                new BindingList<UV>()
                    {
                        UV.Zero,
                        UV.BasisU,
                        UV.BasisV,
                    };

            fooEntity.BarEntityArray =
                new List<BarEntity>()
                    {
                        new BarEntity()
                            {
                                ArrayField = new List<int>() {123,456},
                                Property1 = "Hello, world!",
                                Property2 = 99,
                                Property3 = 0.0000000056,
                                SubEntities = new List<DeepEntity>()
                                    {
                                        new DeepEntity() {Count = 1, ElementId = new ElementId(43)},
                                        new DeepEntity() {Count = 589, ElementId = new ElementId(55)}
                                    },
                                SubEntity = new DeepEntity() {Count = 0, ElementId = ElementId.InvalidElementId}
                            },
                        new BarEntity()
                            {
                                ArrayField = new List<int>() {789,101112},
                                Property1 = "Hello, again!",
                                Property2 = 88,
                                Property3 = -0.0000000056,
                                SubEntities = new List<DeepEntity>()
                                    {
                                        new DeepEntity() {Count = 100, ElementId = new ElementId(555)},
                                        new DeepEntity() {Count = 345, ElementId = new ElementId(666)}
                                    },
                                SubEntity = new DeepEntity() {Count = 12, ElementId = ElementId.InvalidElementId}
                            }
                    };

            fooEntity.BoolXyzMap =
                new Dictionary<bool, XYZ>()
                    {
                        {true, new XYZ(1,2,3)},
                        {false, new XYZ(-3,-2,-1)}
                    };
            fooEntity.ByteGuidMap =
                new SortedDictionary<byte, Guid>()
                    {
                        {0, new Guid("D2EF3FB3-0EF9-4F5A-BCBD-A1F84EA658B8")},
                        {255, new Guid("71DA88AA-6D47-4BF9-972A-DDB6F90BFAE0")},
                        {124, new Guid("1DDF733C-5AA1-4079-99E9-D621DBDFD928")}
                    };
            fooEntity.ShortElementIdMap =
                new Dictionary<short, ElementId>()
                    {
                        {-23, ElementId.InvalidElementId},
                        {124, new ElementId(245)},
                        {156, new ElementId(984534)},
                        {-145, new ElementId(991233516)}
                    };
            fooEntity.IntBarEntityMap =
                new SortedDictionary<int, BarEntity>()
                    {
                        {-1, new BarEntity()
                            {
                                ArrayField = new List<int>() {234,1112},
                                Property1 = "Hello from map!",
                                Property2 = 33,
                                Property3 = -0.0000200056,
                                SubEntities = new List<DeepEntity>()
                                    {
                                        new DeepEntity() {Count = 100, ElementId = new ElementId(555)},
                                        new DeepEntity() {Count = 345, ElementId = new ElementId(666)}
                                    },
                                SubEntity = new DeepEntity() {Count = 12, ElementId = ElementId.InvalidElementId}
                            }},
                        {775993884, new BarEntity()
                            {
                                ArrayField = new List<int>() {0,123, 345564,-31243, 51454},
                                Property1 = "Hello from map 2!",
                                Property2 = 33,
                                Property3 = -0.0000200056,
                                SubEntities = new List<DeepEntity>()
                                    {
                                        new DeepEntity() {Count = 100, ElementId = new ElementId(555)},
                                        new DeepEntity() {Count = 345, ElementId = new ElementId(666)}
                                    },
                                SubEntity = new DeepEntity() {Count = 12, ElementId = ElementId.InvalidElementId}
                            }}
                    };
            fooEntity.ElementIdStringMap =
                new Dictionary<ElementId, string>()
                    {
                        {new ElementId(BuiltInParameter.LEVEL_DATA_OWNING_LEVEL),
                            "LEVEL_DATA_OWNING_LEVEL" },
                            {ElementId.InvalidElementId,
                            "Invalid"}
                    };

            fooEntity.GuidDeepEntityMap =
                new Dictionary<Guid, DeepEntity>()
                    {
                        {new Guid("A85D94A3-162D-4611-BA9B-C268700ECDB1"), 
                        new DeepEntity() {Count = 23, ElementId = new ElementId(24)}}
                    };
            fooEntity.StringDoubleMap =
                new SortedDictionary<string, double>()
                    {
                        {"one point zero five", 0.05},
                        {"one hundred and sixty six point one two three", 166.123}
                    };

            //fooEntity.NonFieldProperty = "Non field property";

            using (Transaction transaction = new Transaction(doc, "Set entity"))
            {
                transaction.Start();
                projectInfo.SetEntity(fooEntity);
                transaction.Commit();
            }

            var extractedFooEntity =
                projectInfo.GetEntity<FooEntity>();

            message = GetEntityInfo(extractedFooEntity);

            //bool entitiesAreEqual =
            //    fooEntity.Equals(extractedFooEntity);

            TaskDialog
                .Show("Extract entity",
                      message);

            return Result.Succeeded;
        }

        private string GetEntityInfo (IRevitEntity revitEntity)
        {
            StringBuilder sb = new StringBuilder();

            Type type = revitEntity.GetType();

            sb.AppendLine(type.ToString());
            

            foreach (var property in type.GetProperties())
            {
                sb.Append("\t");
                sb.Append(property.Name);
                sb.Append(": ");

                var propertyValue = property.GetValue(revitEntity, null);

                IRevitEntity subRevitEntity = propertyValue as IRevitEntity;
                if (subRevitEntity == null)
                    sb.AppendLine(propertyValue == null? "null": propertyValue.ToString());
                else
                    sb.AppendLine(GetEntityInfo(subRevitEntity));
            }

            return sb.ToString();
        }
    }
}
