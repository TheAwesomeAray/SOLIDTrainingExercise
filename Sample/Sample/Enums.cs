using System;
using System.Collections.Generic;
using System.Text;

namespace Sample
{
    public enum LineItemStatusEnum
    {
        Error = 0,
        Normal = 1,
        Replaced = 2,
        OptionItem = 3,
        AcceptedOptionItem = 4,
        RejectedOptionItem = 5
    }

    public enum LineItemCategoryEnum
    {
        Error = 0,
        Install = 1,
        Removal = 2,
        Fee = 3,
        SystemManaged = 4
    }

    public enum MaterialCategoryEnum
    {
        NotApplicable = 0,
        Foam = 1,
        Fiberglass = 2,
        Atticblow = 3,
        Paint = 4,
        Accessory = 5,
        Removal = 30,
        Fee = 40
    }

    public enum RValueEnum
    {
        NotApplicable = 0,
        R1 = 1,
        R11 = 11,
        R13 = 13,
        R19 = 19,
        R22 = 22,
        R26 = 26,
        R30 = 30,
        R38 = 38,
        R44 = 44,
        R49 = 49,
        R85 = 85,
        R127 = 127
    }

    public enum WorkAreaEnum
    {
        SlopedCeiling = 57
    }

    public enum MaterialEnum
    {
        VentChute = 65
    }
}
