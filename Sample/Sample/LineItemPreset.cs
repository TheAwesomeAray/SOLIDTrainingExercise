using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    public class LineItemPreset
    {
        public LineItemPreset(string description, decimal? depth, decimal? rValuePerInch, int? rValue, decimal? yield, decimal? laborRate, decimal? discount, decimal? markUp,
                                    decimal? taxRate, bool taxable, bool surchargeExempt, Material material)
        {
            Description = description;
            Depth = depth;
            RValuePerInch = rValuePerInch;
            RValue = rValue;
            Yield = yield;
            LaborRate = laborRate ?? GetDefaultLaborRate(material.Category);
            Discount = discount;
            MarkUp = markUp ?? GetDefaultMarkUp(material.Category);
            TaxRate = taxRate ?? .1M;
            Taxable = taxable;
            SurchargeExempt = surchargeExempt;
        }

        public LineItemPreset(Material material)
        {
            LaborRate = GetDefaultLaborRate(material.Category);
            MarkUp = GetDefaultMarkUp(material.Category);
        }

        //private constructor for EF
        private LineItemPreset()
        {
        }

        public string Description { get; set; }
        public decimal? Depth { get; private set; }
        public decimal? RValuePerInch { get; private set; }
        public int? RValue { get; private set; }
        public decimal? Yield { get; private set; }
        public decimal? LaborRate { get; private set; }
        public decimal? Discount { get; private set; }
        public decimal? MarkUp { get; private set; }
        public decimal? TaxRate { get; private set; } = .1M;
        public bool Taxable { get; private set; } = true;
        public bool SurchargeExempt { get; private set; } = false;

        public decimal GetDefaultMarkUp(MaterialCategoryEnum categoryId)
        {
            switch (categoryId)
            {
                //TODO: Remove Magic Values
                case MaterialCategoryEnum.Foam:
                    return .6M;

                case MaterialCategoryEnum.Fiberglass:
                    return .57M;

                case MaterialCategoryEnum.Atticblow:
                    return 0M;

                default:
                    return 0M;
            }
        }

        public decimal GetDefaultLaborRate(MaterialCategoryEnum categoryId)
        {
            switch (categoryId)
            {
                case MaterialCategoryEnum.Foam:
                    return .23M;

                case MaterialCategoryEnum.Fiberglass:
                    return .06M;

                case MaterialCategoryEnum.Atticblow:
                    return .05M;

                default:
                    return 0;
            }
        }
    }
}
