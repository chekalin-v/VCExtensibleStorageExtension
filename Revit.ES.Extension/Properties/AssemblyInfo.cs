using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Extensible Storage Extensions for Revit")]
[assembly: AssemblyDescription("Useful utility for easy work with Revit Extensible storage")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Victor Chekalin")]
[assembly: AssemblyProduct("Revit.ES.Extension")]
[assembly: AssemblyCopyright("Copyright © Victor Chekalin 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("fd00b88e-9bce-4c71-836c-dd3036656b64")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#if REVIT2019
[assembly: AssemblyVersion("2019.0.*")]
#elif REVIT2020
[assembly: AssemblyVersion("2020.0.*")]
#elif REVIT2021
[assembly: AssemblyVersion("2021.0.*")]
#elif REVIT2022
[assembly: AssemblyVersion("2022.0.*")]
#elif REVIT2023
[assembly: AssemblyVersion("2023.0.*")]
#endif

