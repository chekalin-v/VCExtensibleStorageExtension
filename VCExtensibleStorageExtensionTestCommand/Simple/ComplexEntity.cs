using System.Collections.Generic;
using VCExtensibleStorageExtension;
using VCExtensibleStorageExtension.Attributes;

namespace VCExtensibleStorageExtensionTestCommand.Simple
{
[Schema("93CC69FC-AB6F-451A-B075-A5D9467569C2",
    "ComplexSchema")]
public class ComplexEntity : IRevitEntity
{
    [Field]
    public string SimpleField { get; set; }

    [Field]
    public List<IntEntity> ArrayField { get; set; }

    [Field]
    public Dictionary<int, IntEntity> MapField { get; set; }
}
}