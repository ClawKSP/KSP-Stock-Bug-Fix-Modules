/*
 * This module written by Claw. Please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.25.0
 *
 * ChuteFixer v0.1.1
 * 
 * Change Log:
 * 
 * v0.1.1b - Modified slightly to be compatible with Kerbal Joint Reinforcement
 * v0.1.1 - Made the setup a little more robust to prevent inadvertantly saving changed
 *          indestructible facilities state.
 * v0.1.0 - Initial release
 * 
 */
using KSP;
using UnityEngine;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class LargeCraftLaunchFix : UnityEngine.MonoBehaviour
    {
        private bool UserSelection = true;
        private int CountdownTimer = 15;
        private bool isActive = false;

        public void Start()
        {
            //Debug.LogWarning("LargeCraftLaunchFix.Start()");
            if (false == HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities)
            {
                GameEvents.onVesselGoOffRails.Add(OffRails);
                UserSelection = false;
            }
        }

        public void OffRails (Vessel VesselToFix)
        {
            //Debug.LogWarning("LargeCraftLaunchFix.OffRails");
            HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = true;
            isActive = true;
            CountdownTimer = 15;
        }

        public void FixedUpdate ()
        {
            //Debug.LogWarning("LargeCraftLaunchFix.FixedUpdate()");
            if (false == isActive)
            {
                return;
            }
            CountdownTimer--;
            //Debug.LogWarning("Countdown : " + CountdownTimer);

            if (CountdownTimer <= 0)
            {
                isActive = false;
                HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = false;
                Debug.Log("LargeCraftLaunchFix Deactivating");
            }
        }

        public void OnDestroy()
        {
            Debug.Log("LargeCraftLaunchFix.OnDestroy() User Selection = " + UserSelection);
            GameEvents.onVesselGoOffRails.Remove(OffRails);
            HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = UserSelection;
        }
    }
}
