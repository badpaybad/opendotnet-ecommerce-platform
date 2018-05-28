using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Ef;

namespace Core.FrontEnd.Models
{
    public class FeTreeNodeBuilder
    {
        public List<FeTreeNode> GetAllCategoryForSideMenu(bool? isMenuCms, Guid langId, List<Enums.CategoryType> types = null, bool inclueLink = true, bool isFrontEnd = false)
        {
            List<FeTreeNode> child;
            List<ContentLanguage> contentLang;
            using (var db = new DomainDrivenDesign.CoreCms.Ef.CoreCmsDbContext())
            {
                Expression<Func<Category, bool>> cp = p => p.Deleted == false;

                if (types != null && types.Count > 0)
                {
                    var typeValue = types.Select(i => (short)i);
                    cp = cp.And(p => typeValue.Contains(p.Type));
                }
                if (isFrontEnd)
                {
                    cp = cp.And(p => p.ShowInFrontEnd);
                }

                child = db.Categories.Where(cp)
                .OrderBy(i => i.DisplayOrder)
                .AsEnumerable().Select(c => new FeTreeNode()
                {
                    id = c.Id.ToString(),
                    ctype=c.Type,
                    parent = c.ParentId.ToString(),
                    a_attr = inclueLink == false ? new FeTreeNode.LinkAttribute() : new FeTreeNode.LinkAttribute()
                    {
                        href = isMenuCms==null?"#":(isMenuCms.Value ? ("/Admin/AdminNews/Index/" + c.Id) : ("/Admin/AdminProduct/Index/" + c.Id))
                    },
                    isSinglePage = c.IsSinglePage,
                    showInFrontEnd = c.ShowInFrontEnd
                }).ToList();

                contentLang = db.ContentLanguages.Join(db.Categories, l => l.Id, c => c.Id,
                         (l, c) => new { L = l, C = c })
                     .Where(i => i.L.ColumnName.Equals("title", StringComparison.OrdinalIgnoreCase))
                     .Select(m => m.L).ToList();
            }

            var data = new List<FeTreeNode>();
            data.Add(new FeTreeNode()
            {
                id = Guid.Empty.ToString(),
                text = "root",
                parent = "#",
                state=new FeTreeNode.State() { opened = true}
            });

            data.AddRange(child);

            foreach (var d in data)
            {
                if (!d.id.Equals(Guid.Empty.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var title = contentLang.FirstOrDefault(i => i.LanguageId == langId && i.Id.ToString().Equals(d.id, StringComparison.OrdinalIgnoreCase))
                                ?? contentLang.FirstOrDefault(i => i.LanguageId == EngineeCurrentContext.DefaultLanguageId && i.Id.ToString().Equals(d.id, StringComparison.OrdinalIgnoreCase));
                    if (isFrontEnd)
                    {
                        d.text = title.ColumnValue;
                    }
                    else
                    {
                        d.text = title.ColumnValue + (d.isSinglePage ? " -Sp" : string.Empty) 
                            + (d.showInFrontEnd ? " -Fe" : string.Empty);
                        switch ((Enums.CategoryType) d.ctype)
                        {
                                case Enums.CategoryType.News:
                                    d.text = d.text + " -n";
                                    break;
                            case Enums.CategoryType.Product:
                                d.text = d.text + " -p";
                                break;
                            case Enums.CategoryType.NewsAndProduct:
                                d.text = d.text + " -np";
                                break;
                        }

                    }

                }
             
            }

            return data;
        }


    }
}