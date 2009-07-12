using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// Stores method and parameter information parsed from the linq expression
    /// </summary>
    internal class MethodParamValue
    {
        List<MemberInfo> _paramPath = new List<MemberInfo>();
        public ParameterInfo Param { get; set; }
        public List<MemberInfo> ParamPath { get { return _paramPath; } }
        public object Value { get; set; }
        public string QueryVariableName { get; set; }
    }
}
