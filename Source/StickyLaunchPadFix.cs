/*
 * This module written by Claw. For more details please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * StickyLaunchPadFix - Written for KSP v1.0
 * 
 * Change Log:
 * - v01.03  (1 Jul 15)   - Recompiled and tested for KSP v1.0.4
 * - v01.02  (14 May 15)  - Reworked loading procedures and fixed a quicksave bug
 * - v01.01  (3 May 15)   - Recompiled for KSP v1.0.1
 * - v01.00  (27 Apr 15)  - Recompiled for KSP v1.0
 * - v0.1.0a (29 Dec 14)  - Reduced log spam
 * - v0.1.0  (23 Dec 14)  - Initial Release
 */


using UnityEngine;
using KSP;

namespace ClawKSP
{

    public class MSLPFix : PartModule
    {

        //public ModuleEngines EngineModule = null;

        private bool ColliderDisabled = false;

        public void FixedUpdate()
        {

            /* Collider - Tier 1 Launch Pad Collider Name
             * LP_main, LP_barsAlpha - Tier 2 Launch Pad Collider Names
             * Launch Pad - Tier 3 Launch Pad Collider Name
             */ 

            // Debug.LogWarning("MSLPFix.FixedUpdate()");

            if (vessel.situation != Vessel.Situations.PRELAUNCH)
            {
                part.RemoveModule(this);
                return;
            }

            if (true == ColliderDisabled) { return; }

            for (int CollIndex = 0; CollIndex < part.currentCollisions.Count; CollIndex++)
            {
                //Debug.LogWarning("-- " + CollIndex + " || " + part.currentCollisions[CollIndex].name
                //    + " || " + part.currentCollisions[CollIndex].enabled);

                // Check for mesh grating collision
                if ("LP_barsAlpha" == part.currentCollisions[CollIndex].name)
                {
                    part.currentCollisions[CollIndex].enabled = false;
                    ColliderDisabled = true;
                }
            }

        } // Update

    }

} // ClawKSP
