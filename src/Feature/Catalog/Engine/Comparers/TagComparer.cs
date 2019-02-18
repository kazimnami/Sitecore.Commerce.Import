using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Feature.Catalog.Engine
{
    public class TagComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag x, Tag y)
        {
            if (x == null || y == null) return false;

            return x.Name == y.Name
                && x.Excluded == y.Excluded;
        }

        public int GetHashCode(Tag obj)
        {
            if (obj == null) return base.GetHashCode();

            // https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked
            {
                int hash = 17;
                if (obj.Name != null) hash = hash * 23 + obj.Name.GetHashCode();
                hash = hash * 23 + obj.Excluded.GetHashCode();
                return hash;
            }
        }
    }
}
