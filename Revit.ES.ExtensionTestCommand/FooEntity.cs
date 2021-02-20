using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Revit.ES.Extension;
using Revit.ES.Extension.Attributes;
using Autodesk.Revit.DB;

namespace Revit.ES.ExtensionTestCommand
{
    [Schema("675551F3-04D4-4A33-94CA-0C2E34B2A5BF", "FooEntity",
        Documentation = "The class I want to save in the project")]
    public class FooEntity : IRevitEntity
    {
        #region Simple properties
        
        [Field(Documentation = "Int32 Property")]
        public int IntProperty { get; set; }

        [Field(Documentation = "Int16 Property")]
        public short ShortProperty { get; set; }

        [Field(Documentation = "Byte Property")]
        public byte ByteProperty { get; set; }

        [Field(Documentation = "Double Property", UnitType = UnitType.UT_Electrical_CableTraySize)]
        public double DoubleProperty { get; set; }

        [Field(Documentation = "Float Property", UnitType = UnitType.UT_Number)]
        public float FloatProperty { get; set; }

        [Field(Documentation = "Boolean Property")]
        public bool BoolProperty { get; set; }

        [Field(Documentation = "String Property")]
        public string StringProperty { get; set; }

        [Field(Documentation = "Guid Property")]
        public Guid GuidProperty { get; set; }

        [Field(Documentation = "ElementId Property")]
        public ElementId ElementIdProperty { get; set; }

        [Field(Documentation = "XYZ Property", UnitType = UnitType.UT_Volume)]
        public XYZ XyzProperty { get; set; }

        [Field(Documentation = "UV Property", UnitType =  UnitType.UT_HVAC_DuctInsulationThickness)]
        public UV UvProperty { get; set; }

        [Field]
        public DeepEntity DeepEntityProperty { get; set; }

        #endregion

        #region ArrayProperties
        

        [Field(Documentation = "Int32 Collection Property")]
        public Collection<int> IntArrayProperty { get; set; }

        [Field(Documentation = "Int16 List Property")]
        public List<short> ShortArrayProperty { get; set; }

        [Field(Documentation = "BindingList of Byte Property")]
        public BindingList<byte> ByteArrayProperty { get; set; }

        [Field(Documentation = "ObservableCollection of Double Property", 
            UnitType = UnitType.UT_HVAC_Temperature)]
        public ObservableCollection<double> DoubleArrayProperty { get; set; }

        [Field(Documentation = "Float List Property",
            UnitType = UnitType.UT_Force)]
        public List<float> FloatArrayProperty { get; set; }

        [Field(Documentation = "Boolean Property")]
        public Collection<bool> BoolArrayProperty { get; set; }

        [Field(Documentation = "String List Property")]
        public List<string> StringArrayProperty { get; set; }

        [Field(Documentation = "Guid List Property")]
        public List<Guid> GuidArrayProperty { get; set; }

        [Field(Documentation = "ElementId List Property")]
        public List<ElementId> ElementIdArrayProperty { get; set; }

        [Field(Documentation = "XYZ Property",
            UnitType = UnitType.UT_Piping_Temperature)]
        public Collection<XYZ> XyzArrayProperty { get; set; }

        [Field(Documentation = "UV BindingList Property",
            UnitType = UnitType.UT_HVAC_Energy)]
        public BindingList<UV> UvArrayProperty { get; set; }

        [Field]
        public List<BarEntity> BarEntityArray { get; set; }

        #endregion


        #region Map properties
        
        /* The supported types for the keys are 
         * Boolean, Byte, Int16, Int32, ElementId, GUID and String. 
         */

        [Field(UnitType = UnitType.UT_HVAC_DuctLiningThickness)]
        public Dictionary<bool, XYZ> BoolXyzMap { get; set; }

        [Field]
        public SortedDictionary<Byte, Guid> ByteGuidMap { get; set; }

        [Field]
        public Dictionary<short, ElementId> ShortElementIdMap { get; set; }

        [Field]
        public SortedDictionary<int, BarEntity> IntBarEntityMap { get; set; }

        [Field]
        public Dictionary<ElementId, string> ElementIdStringMap { get; set; }

        [Field]
        public Dictionary<Guid, DeepEntity> GuidDeepEntityMap { get; set; }

        [Field(UnitType = UnitType.UT_Electrical_Demand_Factor)]
        public SortedDictionary<string, double> StringDoubleMap { get; set; }

        #endregion

        /// <summary>
        /// This property won't save in an Entity
        /// </summary>
        //public string NonFieldProperty { get; set; }
    }
}
