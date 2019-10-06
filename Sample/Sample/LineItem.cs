using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample
{

    public abstract class LineItem
    {
        public decimal Price { get; set; }
        public int EstimateId { get; internal set; }
        public string Description { get; internal set; }
        public decimal Quantity { get; internal set; } = 0;
        public decimal? Depth { get; internal set; }
        public decimal? RValuePerInch { get; internal set; }
        public virtual decimal? RValue { get; internal set; }
        public decimal? Yield { get; internal set; } = 0;
        public decimal LaborRate { get; internal set; } = 0;
        public decimal Discount { get; internal set; } = 0;
        public decimal MarkUp { get; internal set; } = 0;
        public decimal TaxRate { get; internal set; } = .1M;
        public bool Taxable { get; internal set; } = true;
        public bool SurchargeExempt { get; internal set; } = false;
        public bool DoNotShowOnEstimate { get; internal set; } = false;
        public bool DoNotShowOnWorkOrder { get; internal set; } = false;
        public LineItemStatusEnum Status { get; internal set; } = LineItemStatusEnum.Normal;
        public int? MaterialRecordId { get; internal set; }
        public MaterialRecord MaterialRecord { get; internal set; }
        public int? WorkAreaId { get; internal set; }
        public WorkArea WorkArea { get; internal set; }
        public List<ReplacedLineItem> ReplacedLineItems { get; internal set; }
        public decimal TaxAmount => MaterialCost * TaxRate;
        public LineItemCategoryEnum LineItemCategory { get; internal set; }
        public MaterialCategoryEnum Category { get; internal set; }
        public int? WorkOrderId { get; internal set; }

        private decimal amountPerContainer;
        public decimal AmountPerContainer
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Accessory:
                        return 1;
                    case MaterialCategoryEnum.Atticblow:
                        return Settings.Instance.AtticBlowSqftBase / MaterialRecord.GetRValue((RValueEnum)(RValue ?? 0));
                    case MaterialCategoryEnum.Fiberglass:
                        return MaterialRecord.AmountPerContainer.Value;
                    case MaterialCategoryEnum.Foam:
                        return (Quantity * Depth.Value) / Yield.Value;
                    case MaterialCategoryEnum.Paint:
                        return Yield.Value;
                    default:
                        return 0;

                }
            }
            internal set
            {
                amountPerContainer = value;
            }
        }

        public decimal ContainersRequired
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Accessory:
                        return 1;
                    case MaterialCategoryEnum.Atticblow:
                        return Quantity / AmountPerContainer;
                    case MaterialCategoryEnum.Fiberglass:
                        return Quantity / AmountPerContainer;
                    case MaterialCategoryEnum.Foam:
                        return AmountPerContainer;
                    case MaterialCategoryEnum.Paint:
                        return Quantity / AmountPerContainer;
                    default:
                        return 0;
                }
            }
        }
        public string ContainerName
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Atticblow:
                        return "Bags";
                    case MaterialCategoryEnum.Fiberglass:
                        return "Bags";
                    case MaterialCategoryEnum.Foam:
                        return "Sets";
                    case MaterialCategoryEnum.Paint:
                        return "Gallons";
                    default:
                        return "";
                };
            }
        }

        public virtual decimal MaterialCost
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Fee:
                        return 0;
                    case MaterialCategoryEnum.Removal:
                        return 0;
                    case MaterialCategoryEnum.Accessory:
                        return Price * Quantity;
                    case MaterialCategoryEnum.Atticblow:
                        return Price * ContainersRequired;
                    case MaterialCategoryEnum.Fiberglass:
                        return Price * Quantity;
                    case MaterialCategoryEnum.Foam:
                        return Price * AmountPerContainer;
                    case MaterialCategoryEnum.Paint:
                        return Price * (Quantity / Yield.Value);
                    default:
                        throw new InvalidOperationException("Invalid Material Type");
                }
            }
        }
        public decimal Profit
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Fee:
                        return Total;
                    case MaterialCategoryEnum.Removal:
                        return Total - TotalLabor - -(TotalLabor * Settings.Instance.TotalLaborSurchargePercent);
                    default:
                        return Total - (TotalLabor + MaterialCost) - (MaterialCost * TaxRate) - (TotalLabor * Settings.Instance.TotalLaborSurchargePercent);
                }
            }
        }
        public decimal UnitPrice => Total / (Quantity == 0 ? 1 : Quantity);
        public decimal TotalLabor => LaborRate * Quantity;

        public bool IsOptionItem =>
            new Enum[] {
                LineItemStatusEnum.OptionItem,
                LineItemStatusEnum.RejectedOptionItem,
                LineItemStatusEnum.AcceptedOptionItem
            }.Contains(Status);

        public decimal Total
        {
            get
            {
                switch (Category)
                {
                    case MaterialCategoryEnum.Fee:
                        return Price + LaborRate;
                    default:
                        decimal markup = MarkUp != 0 ? MarkUp : 1;
                        decimal adjustedLaborRate = (LaborRate * Quantity) / markup;
                        decimal adjustedMaterialCost = MaterialCost / markup;
                        decimal tax = MaterialCost * TaxRate;
                        decimal laborRateSurcharge = (LaborRate * Quantity * Settings.Instance.TotalLaborSurchargePercent);
                        return Helpers.RoundUp(adjustedMaterialCost + adjustedLaborRate + tax + laborRateSurcharge, 2);
                }
            }
        }


        public bool IsVentChute => MaterialRecord?.Material?.Id == (int)MaterialEnum.VentChute;
        public bool IsSlopedCeilingLineItem => WorkAreaId == (int)WorkAreaEnum.SlopedCeiling && !IsVentChute;
        public bool AffectsSlopedCeilingLineItems => IsSlopedCeilingLineItem || (ReplacedLineItems?.Where(l => l.LineItem?.IsSlopedCeilingLineItem ?? false).Any() ?? false);


        public void UpdateEstimatedFields(decimal containersRequired, decimal taxRate)
        {
            UpdateEstimatedQuantityForNewContainersRequired(containersRequired);
            TaxRate = taxRate;
        }

        protected abstract void UpdateEstimatedQuantityForNewContainersRequired(decimal containersRequired);

        internal void MarkReplaced()
        {
            if (Status == LineItemStatusEnum.Normal && MaterialRecord.Material.Id != (int)MaterialEnum.VentChute)
            {
                Status = LineItemStatusEnum.Replaced;
            }
        }

        internal void MarkNotReplaced()
        {
            if (Status == LineItemStatusEnum.Replaced)
            {
                Status = LineItemStatusEnum.Normal;
            }
        }

        public bool IsMatch(LineItem lineItem)
        {
            return lineItem.Quantity == Quantity
                   && lineItem.MaterialRecordId == MaterialRecordId
                   && lineItem.WorkAreaId == WorkAreaId
                   && lineItem.Depth == Depth
                   && lineItem.Status == Status
                   && lineItem.Taxable == Taxable
                   && lineItem.RValue == RValue
                   && lineItem.RValuePerInch == RValuePerInch
                   && lineItem.Yield == Yield
                   && Category == Category;
        }

        private string BaseDescription => $"{WorkArea?.Name} {MaterialRecord?.Material?.Name}";

        public string Name
        {
            get
            {
                if (Category == MaterialCategoryEnum.Atticblow)
                {
                    return $"R{Math.Round(RValue ?? 0, 0)} {BaseDescription}";
                }
                else if (Category == MaterialCategoryEnum.Foam)
                {
                    return $"R{Math.Round(RValue ?? 0, 0)} {MaterialRecord?.Material?.Name} {Math.Round(Depth ?? 0, 2)}in.";
                }

                return BaseDescription;
            }
        }
    }
}
