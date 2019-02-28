using Sitecore.Commerce.Plugin.SQL;

namespace Feature.Catalog.Engine
{
    public class SitecoreMasterSqlPolicy : EntityStoreSqlPolicy
    {
        public SitecoreMasterSqlPolicy()
        {
            Database = "habitathome_Master";
        }
    }
}
