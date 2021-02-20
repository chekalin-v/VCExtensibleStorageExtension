using Revit.ES.Extension;
using Revit.ES.Extension.Attributes;

namespace Revit.ES.ExtensionTestCommand.Simple
{
// Set schema Id and Schema name as class attributes
[Schema("4E5B6F62-B8B3-4A2F-9B06-DDD953D4D4BB",
    "SimpleIntSchema")]
public class IntEntity : IRevitEntity 
{
    // Mark the property as Schema field using attributes
    [Field]
    public int SomeValue { get; set; }
}
}   
