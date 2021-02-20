/* 
 * Copyright 2021 © Victor Chekalin
 * 
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 * 
 */

using Autodesk.Revit.DB.ExtensibleStorage;

namespace Revit.ES.Extension
{
    public interface IEntityConverter
    {
        Entity Convert(IRevitEntity revitEntity);
        TRevitEntity Convert<TRevitEntity>(Entity entity) where TRevitEntity : class, IRevitEntity;
    }
}
