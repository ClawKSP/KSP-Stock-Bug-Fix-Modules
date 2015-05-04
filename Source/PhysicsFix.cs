/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * PhysicsFix - Written for KSP v1.0
 * 
 * - Fixes an error with convective heating values.
 * 
 * Change Log:
  * - v01.00  (3 May 15)   Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class PhysicsFix
    {
        public void Start()
        {
            // Fix Convective Heating
            PhysicsGlobals.FullConvectionAreaMin = -0.2f;
        }
    }
}
