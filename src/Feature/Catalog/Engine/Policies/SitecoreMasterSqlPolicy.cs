using Sitecore.Commerce.Plugin.SQL;

namespace Feature.Catalog.Engine
{
    public class SitecoreMasterSqlPolicy : EntityStoreSqlPolicy
    {
        public SitecoreMasterSqlPolicy()
        {
            Database = "sc902_Master";
        }
    }
}
