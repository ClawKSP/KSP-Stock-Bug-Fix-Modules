/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * OverheatFix - Written for KSP v1.0
 * - Fixes overheat bug with small parts, causing a thermal feedback and exploding parts.
 * 
 * Change Log:
 * - v00.01  (22 Aug 15)  Initial Experimental Release.
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)] 
    public class OverheatFix : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("OverheatFix.Start(): v0.01 (Experimental)");

            GameEvents.onVesselGoOffRails.Add(CheckThermalMass);
        }

        public void OnDestroy()
        {
            GameEvents.onVesselGoOffRails.Remove(CheckThermalMass);
        }


        public void CheckThermalMass (Vessel VesselToFix)
        {
            if (null == VesselToFix) { return; }
            if (null == VesselToFix.parts) { return; }

            for (int IndexParts = VesselToFix.parts.Count - 1; IndexParts >= 0; IndexParts--)
            {
                Part tempPart = VesselToFix.parts[IndexParts];

                if (tempPart.thermalMass < 1.0)
                {
                    // Debug.LogError("Modifying Original: " + tempPart.thermalMassModifier);
                    tempPart.thermalMassModifier = ((tempPart.thermalMass + 1) + 0.5 - tempPart.resourceThermalMass) / (tempPart.mass * PhysicsGlobals.StandardSpecificHeatCapacity);
                    // Debug.LogError("Modifying New: " + tempPart.thermalMassModifier);
                }
            }
        }
    }
}
