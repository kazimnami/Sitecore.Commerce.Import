namespace Feature.Catalog.Engine
{
    public static class Constants
    {
        public const string BaseName = "Feature.Catalog.";
        public static class Pipelines
        {
            public static class Blocks
            {
                public const string ProductExtensionDoActionEditBlock = BaseName + "Block.ProductExtension.DoActionEdit";
                public const string ProductExtensionGetViewBlock = BaseName + "Block.ProductExtension.GetView";
                public const string ProductExtensionPopulateActionsBlock = BaseName + "Block.ProductExtension.PopulateActions";
            }
        }
    }
}
