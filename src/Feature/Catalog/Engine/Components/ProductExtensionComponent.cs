using System;
using System.Linq;
using System.Collections.Generic;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;

namespace Feature.Catalog.Engine
{
    public class ProductExtensionComponent : Component
    {
        public string RelatedProductId { get; set; }
        public string RelatedProductNumber { get; set; }
        public string BomId { get; set; }
        public string BomName { get; set; }
        public string AutoIdCapability { get; set; } // Multiple Choice(Industrial, Desktop, Mobility)
        public string HardwareType { get; set; } // Multiple Choice(MFD, Printer, Auto ID, Accessory, Consumable, Scanner)
        public string Extract { get; set; }
        public string ColorCapability { get; set; } // Multiple Choice(Colour, Monochrome)
        public string PaperSize { get; set; } // Multiple Choice(A4, A3)
        public string SpeedPpmMin { get; set; }
        public string SpeedPpmMax { get; set; }
        public string SoftwareCategory { get; set; } //   Multiple Choice(Printing Solution, Document Management, Mobility Solution, Tracking & Auditing)
        public bool SoftwareCostManagement { get; set; }
        public bool SoftwareCloudTechnology { get; set; }
        public bool SoftwareDocumentArchive { get; set; }
        public bool SoftwareDocumentCapture { get; set; }
        public bool SoftwareDocumentManagement { get; set; }
        public bool SoftwareDocumentWorkflow { get; set; }
        public bool SoftwareEcoStrategies { get; set; }
        public bool SoftwareMobility { get; set; }
        public bool SoftwareSecurity { get; set; }
        public bool SoftwareVariableData { get; set; }
        public bool SoftwareDocumentAutomation { get; set; }
        public bool SoftwarePrintOutput { get; set; }
        public bool SoftwarePrintManagement { get; set; }

        public ProductExtensionComponent Copy()
        {
            return new ProductExtensionComponent
            {
                RelatedProductId = this.RelatedProductId,
                RelatedProductNumber = this.RelatedProductNumber,
                BomId = this.BomId,
                BomName = this.BomName,
                AutoIdCapability = this.AutoIdCapability,
                HardwareType = this.HardwareType,
                Extract = this.Extract,
                ColorCapability = this.ColorCapability,
                PaperSize = this.PaperSize,
                SpeedPpmMin = this.SpeedPpmMin,
                SpeedPpmMax = this.SpeedPpmMax,
                SoftwareCategory = this.SoftwareCategory,
                SoftwareCostManagement = this.SoftwareCostManagement,
                SoftwareCloudTechnology = this.SoftwareCloudTechnology,
                SoftwareDocumentArchive = this.SoftwareDocumentArchive,
                SoftwareDocumentCapture = this.SoftwareDocumentCapture,
                SoftwareDocumentManagement = this.SoftwareDocumentManagement,
                SoftwareDocumentWorkflow = this.SoftwareDocumentWorkflow,
                SoftwareEcoStrategies = this.SoftwareEcoStrategies,
                SoftwareMobility = this.SoftwareMobility,
                SoftwareSecurity = this.SoftwareSecurity,
                SoftwareVariableData = this.SoftwareVariableData,
                SoftwareDocumentAutomation = this.SoftwareDocumentAutomation,
                SoftwarePrintOutput = this.SoftwarePrintOutput,
                SoftwarePrintManagement = this.SoftwarePrintManagement
            };
        }

        #region Entity View 

        public void AddPropertiesToView(EntityView entityView, bool isReadOnly)
        {
            entityView.Properties.Add(new ViewProperty { Name = nameof(RelatedProductId), RawValue = this.RelatedProductId, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(RelatedProductNumber), RawValue = this.RelatedProductNumber, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(BomId), RawValue = this.BomId, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(BomName), RawValue = this.BomName, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(AutoIdCapability), RawValue = this.AutoIdCapability, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(HardwareType), RawValue = this.HardwareType, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(Extract), RawValue = this.Extract, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(ColorCapability), RawValue = this.ColorCapability, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(PaperSize), RawValue = this.PaperSize, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SpeedPpmMin), RawValue = this.SpeedPpmMin, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SpeedPpmMax), RawValue = this.SpeedPpmMax, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareCategory), RawValue = this.SoftwareCategory, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareCostManagement), RawValue = this.SoftwareCostManagement, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareCloudTechnology), RawValue = this.SoftwareCloudTechnology, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareDocumentArchive), RawValue = this.SoftwareDocumentArchive, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareDocumentCapture), RawValue = this.SoftwareDocumentCapture, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareDocumentManagement), RawValue = this.SoftwareDocumentManagement, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareDocumentWorkflow), RawValue = this.SoftwareDocumentWorkflow, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareEcoStrategies), RawValue = this.SoftwareEcoStrategies, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareMobility), RawValue = this.SoftwareMobility, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareSecurity), RawValue = this.SoftwareSecurity, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareVariableData), RawValue = this.SoftwareVariableData, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwareDocumentAutomation), RawValue = this.SoftwareDocumentAutomation, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwarePrintOutput), RawValue = this.SoftwarePrintOutput, IsReadOnly = isReadOnly });
            entityView.Properties.Add(new ViewProperty { Name = nameof(SoftwarePrintManagement), RawValue = this.SoftwarePrintManagement, IsReadOnly = isReadOnly });
        }

        public void GetPropertiesFromView(EntityView arg)
        {
            this.RelatedProductId = GetEntityViewProperty<string>(arg, nameof(RelatedProductId));
            this.RelatedProductNumber = GetEntityViewProperty<string>(arg, nameof(RelatedProductNumber));
            this.BomId = GetEntityViewProperty<string>(arg, nameof(BomId));
            this.BomName = GetEntityViewProperty<string>(arg, nameof(BomName));
            this.AutoIdCapability = GetEntityViewProperty<string>(arg, nameof(AutoIdCapability));
            this.HardwareType = GetEntityViewProperty<string>(arg, nameof(HardwareType));
            this.Extract = GetEntityViewProperty<string>(arg, nameof(Extract));
            this.ColorCapability = GetEntityViewProperty<string>(arg, nameof(ColorCapability));
            this.PaperSize = GetEntityViewProperty<string>(arg, nameof(PaperSize));
            this.SpeedPpmMin = GetEntityViewProperty<string>(arg, nameof(SpeedPpmMin));
            this.SpeedPpmMax = GetEntityViewProperty<string>(arg, nameof(SpeedPpmMax));
            this.SoftwareCategory = GetEntityViewProperty<string>(arg, nameof(SoftwareCategory));
            this.SoftwareCostManagement = GetEntityViewProperty<bool>(arg, nameof(SoftwareCostManagement));
            this.SoftwareCloudTechnology = GetEntityViewProperty<bool>(arg, nameof(SoftwareCloudTechnology));
            this.SoftwareDocumentArchive = GetEntityViewProperty<bool>(arg, nameof(SoftwareDocumentArchive));
            this.SoftwareDocumentCapture = GetEntityViewProperty<bool>(arg, nameof(SoftwareDocumentCapture));
            this.SoftwareDocumentManagement = GetEntityViewProperty<bool>(arg, nameof(SoftwareDocumentManagement));
            this.SoftwareDocumentWorkflow = GetEntityViewProperty<bool>(arg, nameof(SoftwareDocumentWorkflow));
            this.SoftwareEcoStrategies = GetEntityViewProperty<bool>(arg, nameof(SoftwareEcoStrategies));
            this.SoftwareMobility = GetEntityViewProperty<bool>(arg, nameof(SoftwareMobility));
            this.SoftwareSecurity = GetEntityViewProperty<bool>(arg, nameof(SoftwareSecurity));
            this.SoftwareVariableData = GetEntityViewProperty<bool>(arg, nameof(SoftwareVariableData));
            this.SoftwareDocumentAutomation = GetEntityViewProperty<bool>(arg, nameof(SoftwareDocumentAutomation));
            this.SoftwarePrintOutput = GetEntityViewProperty<bool>(arg, nameof(SoftwarePrintOutput));
            this.SoftwarePrintManagement = GetEntityViewProperty<bool>(arg, nameof(SoftwarePrintManagement));
        }

        private static T GetEntityViewProperty<T>(EntityView arg, string propertyName)
        {
            return (T)Convert.ChangeType(arg.Properties.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))?.Value, typeof(T));
        }

        #endregion Entity View 
    }
}
