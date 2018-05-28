using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Reflection
{
    public static class AssemblyExtesions
    {
        public static Type FindType(string typeFullName)
        {
            var t = Type.GetType(typeFullName);
            if (t != null) return t;

            var allAss = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAss)
            {
                var allTypes = assembly.GetTypes();
                foreach (var type in allTypes)
                {
                    if (type.FullName != null && type.FullName.Equals(typeFullName, StringComparison.OrdinalIgnoreCase))
                        return type;
                }
            }

            return null;
        }
    }
}
