using System.Collections.Generic;
using Virtex.Lib.Vrtc.Physics.BEPU.Entities;
using BEPUutilities.DataStructures;

namespace Virtex.Lib.Vrtc.Physics.BEPU.UpdateableSystems.ForceFields
{
    /// <summary>
    /// Defines the area in which a force field works using an entity's shape.
    /// </summary>
    public class InfiniteForceFieldShape : ForceFieldShape
    {
        private IList<Entity> boxedList;
        /// <summary>
        /// Determines the possibly involved entities.
        /// </summary>
        /// <returns>Possibly involved entities.</returns>
        public override IList<Entity> GetPossiblyAffectedEntities()
        {
            return boxedList ?? (boxedList = (ForceField as ISpaceObject).Space.Entities);
        }

        /// <summary>
        /// Determines if the entity is affected by the force field.
        /// </summary>
        /// <param name="testEntity">Entity to test.</param>
        /// <returns>Whether the entity is affected.</returns>
        public override bool IsEntityAffected(Entity testEntity)
        {
            return true;
        }
    }
}