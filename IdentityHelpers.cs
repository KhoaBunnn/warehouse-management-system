using System;

namespace Microsoft.AspNetCore.Identity.UI
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class UIFrameworkAttribute : Attribute
    {
        public UIFrameworkAttribute(string framework) { }
    }
}
