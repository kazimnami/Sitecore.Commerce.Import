using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using System;
using System.Linq;

namespace Feature.Catalog.Engine
{
    public class ProductExtensionComponent : Component
    {
        public string Style { get; set; }
        public string FuelType { get; set; }
        public string NaturalGasConversionAvailable { get; set; }
        public string DimensionsHeightHoodOpen { get; set; }
        public string DimensionsHeightHoodClosed { get; set; }
        public string DimensionsWidth { get; set; }
        public string DimensionsDepth { get; set; }

        public ProductExtensionComponent Clone()
        {
            return new ProductExtensionComponent
            {
                Style = Style,
                FuelType = FuelType,
                NaturalGasConversionAvailable = NaturalGasConversionAvailable,
                DimensionsHeightHoodOpen = DimensionsHeightHoodOpen,
                DimensionsHeightHoodClosed = DimensionsHeightHoodClosed,
                DimensionsWidth = DimensionsWidth,
                DimensionsDepth = DimensionsDepth
            };
        }

        public void CopyTo(ProductExtensionComponent to)
        {
            to.Style = Style;
            to.FuelType = FuelType;
            to.NaturalGasConversionAvailable = NaturalGasConversionAvailable;
            to.DimensionsHeightHoodOpen = DimensionsHeightHoodOpen;
            to.DimensionsHeightHoodClosed = DimensionsHeightHoodClosed;
            to.DimensionsWidth = DimensionsWidth;
            to.DimensionsDepth = DimensionsDepth;
        }

        #region Entity View 

        public void AddPropertiesToView(EntityView entityView, bool isReadOnly)
        {
            entityView.Properties.Add(new ViewProperty { Name = nameof(Style), RawValue = Style, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(FuelType), RawValue = FuelType, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(NaturalGasConversionAvailable), RawValue = NaturalGasConversionAvailable, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(DimensionsHeightHoodOpen), RawValue = DimensionsHeightHoodOpen, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(DimensionsHeightHoodClosed), RawValue = DimensionsHeightHoodClosed, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(DimensionsWidth), RawValue = DimensionsWidth, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(DimensionsDepth), RawValue = DimensionsDepth, IsReadOnly = isReadOnly });
        }

        public void GetPropertiesFromView(EntityView arg)
        {
            Style = GetEntityViewProperty<string>(arg, nameof(Style));
            FuelType = GetEntityViewProperty<string>(arg, nameof(FuelType));
            NaturalGasConversionAvailable = GetEntityViewProperty<string>(arg, nameof(NaturalGasConversionAvailable));
            DimensionsHeightHoodOpen = GetEntityViewProperty<string>(arg, nameof(DimensionsHeightHoodOpen));
            DimensionsHeightHoodClosed = GetEntityViewProperty<string>(arg, nameof(DimensionsHeightHoodClosed));
            DimensionsWidth = GetEntityViewProperty<string>(arg, nameof(DimensionsWidth));
            DimensionsDepth = GetEntityViewProperty<string>(arg, nameof(DimensionsDepth));
        }

        private static T GetEntityViewProperty<T>(EntityView arg, string propertyName)
        {
            return (T)Convert.ChangeType(arg.Properties.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))?.Value, typeof(T));
        }

        #endregion Entity View 
        #region Sellable-Item Comparer 

        public static bool MemberEquality(ProductExtensionComponent x, ProductExtensionComponent y)
        {
            if (x == null || y == null) return false;

            return x.Style == y.Style
                && x.FuelType == y.FuelType
                && x.NaturalGasConversionAvailable == y.NaturalGasConversionAvailable
                && x.DimensionsHeightHoodOpen == y.DimensionsHeightHoodOpen
                && x.DimensionsHeightHoodClosed == y.DimensionsHeightHoodClosed
                && x.DimensionsWidth == y.DimensionsWidth
                && x.DimensionsDepth == y.DimensionsDepth;
        }

        internal static int GetHashCodeMembers(ProductExtensionComponent obj)
        {
            if (obj == null) return 0;

            // https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked
            {
                int hash = 17;
                if (obj.Style != null) hash = hash * 23 + obj.Style.GetHashCode();
                if (obj.FuelType != null) hash = hash * 23 + obj.FuelType.GetHashCode();
                if (obj.NaturalGasConversionAvailable != null) hash = hash * 23 + obj.NaturalGasConversionAvailable.GetHashCode();
                if (obj.DimensionsHeightHoodOpen != null) hash = hash * 23 + obj.DimensionsHeightHoodOpen.GetHashCode();
                if (obj.DimensionsHeightHoodClosed != null) hash = hash * 23 + obj.DimensionsHeightHoodClosed.GetHashCode();
                if (obj.DimensionsWidth != null) hash = hash * 23 + obj.DimensionsWidth.GetHashCode();
                if (obj.DimensionsDepth != null) hash = hash * 23 + obj.DimensionsDepth.GetHashCode();
                return hash;
            }
        }

        #endregion Sellable-Item Comparer 
    }
}
