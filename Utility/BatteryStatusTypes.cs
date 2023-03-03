using Eco.Shared.Serialization;

namespace Eco.RM.Utility
{
    [Serialized] public enum BatteryStatusTypes
    {
        Empty = 0,
        Full = 1,
        Discharged = 2,
        Charged = 3,
        Idle = 4,
        Active = 5,
        LowChargeRate = 6,
        LowDischargeRate = 7,
    }
}
