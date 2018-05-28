using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeHomePage
    {
        public List<Section> Sections;

        public class Section
        {
            public Guid Id;
            public string Title;
            public string ViewName;
            public FeCategory Data;
        }
    }
}