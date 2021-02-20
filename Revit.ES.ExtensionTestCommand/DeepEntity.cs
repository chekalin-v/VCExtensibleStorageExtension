using Revit.ES.Extension;
using Revit.ES.Extension.Attributes;
using Autodesk.Revit.DB;

namespace Revit.ES.ExtensionTestCommand
{
    [Schema("6C0EEADC-8B38-4CDF-A5C9-28296D37EE23",
        "SubEntity", Documentation = "Sub entity for test command")]
    public class DeepEntity : IRevitEntity
    {
        [Field]
        public ElementId ElementId { get; set; }

        [Field]
        public int Count { get; set; }
    }
}
