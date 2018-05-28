using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.CorePermission.Reflections
{
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class RightDescriptionAttribute : Attribute
    {
        private readonly string _description;
        public string Description
        {
            get { return _description; }
        }

        public RightDescriptionAttribute(string description)
        {
            _description = description;
        }
    }
}
