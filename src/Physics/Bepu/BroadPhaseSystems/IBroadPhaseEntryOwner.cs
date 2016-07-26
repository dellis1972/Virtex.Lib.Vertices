using Virtex.Lib.Vertices.Physics.BEPU.BroadPhaseEntries;

namespace Virtex.Lib.Vertices.Physics.BEPU.BroadPhaseSystems
{
    ///<summary>
    /// Requires that a class own a BroadPhaseEntry.
    ///</summary>
    public interface IBroadPhaseEntryOwner
    {
        ///<summary>
        /// Gets the broad phase entry associated with this object.
        ///</summary>
        BroadPhaseEntry Entry { get; }
    }
}
