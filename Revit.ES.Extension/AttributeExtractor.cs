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
using System.Reflection;

namespace Revit.ES.Extension
{
    /// <summary>
    /// Helper class which helps to find value if the TAttribute of a member info
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    internal class AttributeExtractor<TAttribute> where TAttribute : Attribute
    {
        public TAttribute GetAttribute(MemberInfo memberInfo)
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), false);

            if (attributes.Length == 0)
                throw new InvalidOperationException(string.Format("MemberInfo {0} does not have a {1}", memberInfo, typeof(TAttribute)));

            var atribute = attributes[0] as TAttribute;
            if (atribute == null)
                throw new InvalidOperationException(string.Format("MemberInfo {0} does not have a {1}", memberInfo, typeof(TAttribute)));

            return atribute;
        }

        
    }
}
