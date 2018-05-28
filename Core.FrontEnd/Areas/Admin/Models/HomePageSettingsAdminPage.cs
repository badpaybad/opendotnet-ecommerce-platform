using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.FrontEnd.Models;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class HomePageSettingsAdminPage
    {
        public List<FeCategory> Categories;
        public List<string> ListViewName;

        public class Section
        {
            public Guid Id;
            public Guid CategoryId;
            public string CategoryTitle;
            public short DisplayOrder;
            public string Title;
            public string HomePageSectionViewName;
            public bool Published;
        }
    }
}