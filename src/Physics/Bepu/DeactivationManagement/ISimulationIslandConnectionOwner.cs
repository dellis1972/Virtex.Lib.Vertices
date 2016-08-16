using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtex.Lib.Vrtc.Physics.BEPU.DeactivationManagement
{
    /// <summary>
    /// Denotes a class which owns a simulation island connection.
    /// </summary>
    public interface ISimulationIslandConnectionOwner
    {
        /// <summary>
        /// Gets the connection associated with the object.
        /// </summary>
        SimulationIslandConnection SimulationIslandConnection
        {
            get;
        }
    }
}
