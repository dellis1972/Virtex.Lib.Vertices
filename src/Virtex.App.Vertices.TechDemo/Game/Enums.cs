namespace VerticeEnginePort.Base
{
    public enum ShipController
    {
        Player,
        AI,
    }

    public enum WeaponTypes
    {
        None,
        Missle,
        HomingMissle,
        Bomb,
        Singularity,
        Virus,
        Shield,
    }

    public enum ItemAddMode
    {
        AddingRegularItem,
        AddingTrackItem
    }

    public enum vxTrackItemType
    {
        Tri,
        ItemPickUp,
        Boost,
        Rotator,
        FinishLineTrigger,
        MidTrackTrigger,
    }
}
