using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.Core
{
    public class EngineeCurrentContext
    {
        public static readonly Guid DefaultLanguageId = Guid.Parse("4A2E940C-E109-4640-9044-4B2C3D0114BE");

        public static readonly Guid AdminRoleId = Guid.Parse("D45CA6E8-473B-466C-B1E5-BEC3AA7BA534");

        public static readonly DateTime SystemMinDate = DateTime.Parse("1900-01-01");

        static EngineeCurrentContext()
        {
           
        }

        public static Guid LanguageId
        {
            get
            {
                try
                {
                    var httpContext = System.Web.HttpContext.Current;
                    var temp = httpContext.Request.QueryString["langid"];
                    if (!string.IsNullOrEmpty(temp))
                        return string.IsNullOrEmpty(temp) ? DefaultLanguageId : Guid.Parse(temp);

                    var requestCookie = httpContext.Request.Cookies["langid"];
                    if (requestCookie != null) temp = requestCookie.Value;
                    return string.IsNullOrEmpty(temp) ? DefaultLanguageId : Guid.Parse(temp);

                }
                catch (Exception ex)
                {
                    return DefaultLanguageId;
                }
            }
        }

        private static void InitLanguage()
        {
            using (var db = new Implements.CoreDbContext())
            {
                var temp = db.Languages.FirstOrDefault(i => i.Id == DefaultLanguageId);
                if (temp == null)
                {
                    db.Languages.Add(new Language()
                    {
                        Id = DefaultLanguageId,
                        Code = "En-us",
                        Title = "English",
                        CurrencyCode = "USD",
                        CurrencyExchangeRate = 1
                    });
                    db.SaveChanges();
                }
            }
        }

        public static List<Language> ListSiteLanguage()
        {
            return CacheManager.GetOrSetIfNull("SiteListLanguage", () =>
            {
                using (var db = new Implements.CoreDbContext())
                {
                    return db.Languages.ToList();
                }
            });
        }

        public static void RefreshListLanguage()
        {
            List<Language> listLang;
            using (var db = new Implements.CoreDbContext())
            {
                listLang= db.Languages.ToList();
            }
            foreach (var l in listLang)
            {
                CacheManager.Set("CurrentSiteLanguage_"+l.Id, l);
            }
            CacheManager.Set("SiteListLanguage", listLang);
        }

        public static void Init()
        {
            InitLanguage();
        }

        public static string GoogleMapApiKey
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["GoogleMapApiKey"] ??
                      "AIzaSyDnOGMcuAY4pJxanbNW3I3gGRTZcAJUbDg";
            }
        }

        public static string GetEmailForContactUs()
        {
            return System.Configuration.ConfigurationManager.AppSettings["EmailContactUs"] ?? "badpaybad@gmail.com";
        }

        public static string GetPhoneForContactUs()
        {
            return System.Configuration.ConfigurationManager.AppSettings["PhoneContactUs"] ?? "01228384839";
        }

        public static string GetCurrencyCode()
        {
            return GetCurrentLanguage().CurrencyCode;
        }

        public static double GetCurrencyExchangeRate()
        {
            return GetCurrentLanguage().CurrencyExchangeRate;
        }

        public static Language GetCurrentLanguage()
        {
            var languageId = LanguageId;

            return CacheManager.GetOrSetIfNull("CurrentSiteLanguage_"+ languageId, () =>
            {
                return ListSiteLanguage().FirstOrDefault(i => i.Id == languageId);
            });
        }

        public static Language GetDefaultLanguage()
        {
            var languageId = DefaultLanguageId;

            return CacheManager.GetOrSetIfNull("CurrentSiteLanguage_" + languageId, () =>
            {
                return ListSiteLanguage().FirstOrDefault(i => i.Id == languageId);
            });
        }
    }
}
