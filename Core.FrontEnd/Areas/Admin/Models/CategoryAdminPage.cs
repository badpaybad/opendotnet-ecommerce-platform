using System.Collections.Generic;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class CategoryAdminPage
    {
        public Dictionary<short, string> ListCategoryType=new Dictionary<short, string>();

        public List<string> ListCategoryViewName;

        public List<string> ListCategoryProductViewName { get; set; }
    }
}