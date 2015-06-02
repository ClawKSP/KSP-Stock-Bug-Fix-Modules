/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleAeroSurfaceFix - Written for KSP v1.0
 * 
 * - Fixes deployment of aero surfaces on launch and in the editor (loading, cloning, etc)
 * - Fixes Action Groups
 * - (Plus) When stowed, the airbrake doesn't generate drag.
 * 
 * Change Log:
 * - v01.02  (1 Jun 15)    Changed brake default action to toggle
 * - v01.01  (14 May 15)   Added some error checking to make it more compatible with other mods
 * - v01.00  (12 May 15)   Initial Release
 * 
 */


using UnityEngine;
using KSP;

namespace ClawKSP
{
    public class MASFix : PartModule
    {
        [KSPField(isPersistant = true)]
        public bool deploy = false;

        [KSPField(isPersistant = false)]
        public bool plusEnabled = false;

        [KSPField(isPersistant = false)]
        private float deflectionLiftCoeff;

        private ModuleAeroSurface AeroSurfaceModule;

        [KSPAction("Toggle", KSPActionGroup.Brakes)]
        public void ActionToggle(KSPActionParam act)
        {
            if (act.type == KSPActionType.Activate)
            {
                deploy = true;
            }
            else
            {
                deploy = false;
            }
            AeroSurfaceModule.deploy = deploy;
        }
        [KSPAction("Extend", KSPActionGroup.None)]
        public void ActionExtend(KSPActionParam act)
        {
            deploy = true;
            AeroSurfaceModule.deploy = deploy;
        }
        [KSPAction("Retract", KSPActionGroup.None)]
        public void ActionRetract(KSPActionParam act)
        {
            deploy = false;
            AeroSurfaceModule.deploy = deploy;
        }

        private void SetupStockPlus()
        {
            if (StockPlusController.plusActive == false || plusEnabled == false)
            {
                plusEnabled = false;
                return;
            }

            Debug.Log(moduleName + " StockPlus Enabled");

            deflectionLiftCoeff = AeroSurfaceModule.deflectionLiftCoeff;
        }

        public override void OnStart(StartState state)
        {
            Debug.Log(moduleName + ".Start(): v01.02");

            base.OnStart(state);

            AeroSurfaceModule = part.FindModuleImplementing<ModuleAeroSurface>();
            if (null == AeroSurfaceModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find Aero Surface Module.");
                return;
            }

            // Wipe out the default actions, which always sets to the "Brakes" action group OnStart
            AeroSurfaceModule.Actions.Clear();
            AeroSurfaceModule.deploy = deploy;

            SetupStockPlus();
        }

        public void FixedUpdate()
        {
            AeroSurfaceModule = part.FindModuleImplementing<ModuleAeroSurface>();
            if (null == AeroSurfaceModule) { return; }

            deploy = AeroSurfaceModule.deploy;

            if (plusEnabled)
            {
                if (false == deploy)
                {
                    AeroSurfaceModule.deflectionLiftCoeff = 0f;
                }
                else
                {
                    AeroSurfaceModule.deflectionLiftCoeff = deflectionLiftCoeff;
                }
            }
        }

    }
}
