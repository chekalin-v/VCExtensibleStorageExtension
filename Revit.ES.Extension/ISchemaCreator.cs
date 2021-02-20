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
using System.Collections.Generic;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Revit.ES.Extension
{
    /// <summary>
    /// Create an Autodesk Extensible storage schema from a type
    /// </summary>
    public interface ISchemaCreator
    {
        Schema CreateSchema(Type type);
    }
}
