using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    public class MaterialRecord
    {
        private MaterialRecord()
        {
        }

        public decimal Price { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public int MaterialId { get; private set; }
        public bool Priority { get; private set; } = false;
        public decimal? AmountPerContainer { get; private set; }
        public string SKU { get; private set; }
        public string Manufacturer { get; private set; }
        public RValue RValue { get; private set; }
        public LineItemPreset LineItemPreset { get; private set; }
        public Material Material { get; private set; }

        public decimal GetRValue(RValueEnum rvalue)
        {
            decimal returnValue;
            switch (rvalue)
            {
                case RValueEnum.R1:
                    returnValue = RValue.R1 ?? 1;
                    break;
                case RValueEnum.R11:
                    returnValue = RValue.R11 ?? 1;
                    break;
                case RValueEnum.R13:
                    returnValue = RValue.R13 ?? 1;
                    break;
                case RValueEnum.R19:
                    returnValue = RValue.R19 ?? 1;
                    break;
                case RValueEnum.R22:
                    returnValue = RValue.R22 ?? 1;
                    break;
                case RValueEnum.R26:
                    returnValue = RValue.R26 ?? 1;
                    break;
                case RValueEnum.R30:
                    returnValue = RValue.R30 ?? 1;
                    break;
                case RValueEnum.R38:
                    returnValue = RValue.R38 ?? 1;
                    break;
                case RValueEnum.R44:
                    returnValue = RValue.R44 ?? 1;
                    break;
                case RValueEnum.R49:
                    returnValue = RValue.R49 ?? 1;
                    break;
                case RValueEnum.R85:
                    returnValue = RValue.R85 ?? 1;
                    break;
                case RValueEnum.R127:
                    returnValue = RValue.R127 ?? 1;
                    break;
                default:
                    throw new InvalidOperationException("Invalid RValue selected");
            }

            return returnValue == 0 ? 1 : returnValue;
        }
    }
}
