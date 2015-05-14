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

        [KSPAction("Toggle", KSPActionGroup.None)]
        public void ActionToggle(KSPActionParam act)
        {
            deploy = !deploy;
            AeroSurfaceModule.deploy = deploy;
        }
        [KSPAction("Extend", KSPActionGroup.Brakes)]
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

        private PartModule GetModule(string moduleName)
        {
            for (int indexModules = 0; indexModules < part.Modules.Count; indexModules++)
            {
                if (moduleName == part.Modules[indexModules].moduleName)
                {
                    return (part.Modules[indexModules]);
                }
            }

            return (null);

        }  // GetModule

        private void SetupStockPlus()
        {
            if (StockPlusController.plusActive == false || plusEnabled == false)
            {
                plusEnabled = false;
                return;
            }

            deflectionLiftCoeff = AeroSurfaceModule.deflectionLiftCoeff;
        }

        public override void OnStart(StartState state)
        {
            Debug.Log(moduleName + ".Start(): v01.00");

            base.OnStart(state);

            AeroSurfaceModule = (ModuleAeroSurface)GetModule("ModuleAeroSurface");

            if (null == AeroSurfaceModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find Aero Surface Module.");
                return;
            }

            AeroSurfaceModule.deploy = deploy;

            // Wipe out the default actions, which always sets to the "Brakes" action group OnStart
            AeroSurfaceModule.Actions.Clear();

            SetupStockPlus();
        }

        public void FixedUpdate()
        {
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
