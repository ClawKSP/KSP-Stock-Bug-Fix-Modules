/*
 * This module written by Claw (with an incredible amount of help from NathanKell). Please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules for more details.
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.25.0
 *
 * EVAEjectonFix v0.1.1a
 * 
 * Change Log:
 * 
 * v0.1.1a - Nullifies ladder slide bug for initial EVA
 * v0.1.1 - Updated error handling to prevent log spam for incompatible mods
 * v0.1 - Initial Release
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{

    public class ModuleEVAEjectionFix : PartModule
    {
        private int TimesToTry = 5; // number of attempts before giving up, prevents locking up kerbal

        public override void OnUpdate()
        {
            if ("Ladder (Acquire)" != part.vessel.evaController.fsm.currentStateName
                && 0 <= TimesToTry)
            {
                Debug.LogWarning("ModuleEVAEjectionFix: Attempting to reset = " + vessel.evaController.fsm.currentStateName);

                TimesToTry--;

                try
                {
                    part.vessel.evaController.fsm.StartFSM("Ladder (Acquire)"); // Force the FSM to reacquire the ladder
                }
                catch
                {
                    Debug.LogError("ModuleEVAEjectionFix: Incompatible Module Found");
                    TimesToTry = 0;
                    part.RemoveModule(this);
                }
                return;
            }

            part.RemoveModule(this);  // remove this module after the fix is complete
            return;
        }
    }
    
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class EVAEjectionFix : UnityEngine.MonoBehaviour
    {
        void Start()
        {
            GameEvents.onCrewOnEva.Add(EjectionFixHook);
        }

        /* This gets called when a crew going on EVA is detected.
         * No need to hook this in if the vessel is loaded or off rails since
         * save/load and time warp are not permitted when a kerbal is on a ladder.
         */
        public void EjectionFixHook (GameEvents.FromToAction<Part, Part> EVAParts)
        {
            if (null == EVAParts.to)
            {
                return;
            }

            // tipping the kerbal back prevents collision induced ejection
            EVAParts.to.vessel.transform.Rotate(-30f, 0f, 0f);

            // nullify the ladder slide upon initially going EVA
            EVAParts.to.vessel.rigidbody.velocity = EVAParts.from.rigidbody.velocity;
            EVAParts.to.rigidbody.velocity = EVAParts.from.rigidbody.velocity;

            // hook in the module that reacquires the ladder
            EVAParts.to.AddModule("ModuleEVAEjectionFix");
        }

        void OnDestroy ()
        {
            GameEvents.onCrewOnEva.Remove(EjectionFixHook);
        }
    }
}
