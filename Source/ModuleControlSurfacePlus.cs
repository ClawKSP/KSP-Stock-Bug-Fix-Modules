/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleControlSurfacePlus - Written for KSP v1.1.2
 * 
 * - (Plus) Adds tweakable actuator speed
 * - (Plus) Adds tweakable to independently invert deployment
 * 
 * Change Log:
 * - v01.07  ( 8 May 16)   Updated for KSP v1.1.2
 * - v01.06  (21 Apr 16)   Updated for KSP v1.1.0
 * - v01.05  (25 Jan 16)   Added actuator speed tweakable.
 * - v01.04  (10 Nov 15)   Updated for KSP v1.0.5. Renamed from ModuleControlSurfaceFix. Integrated into StockBugFixPlusController
 * - v01.03  (1 Jul 15)    Recompiled and tested for KSP v1.0.4
 * - v01.02  (18 May 15)   Fixed a bug that caused roll inputs to be reversed forward of CoM
 * - v01.01  (14 May 15)   Added some error checking to make it more compatible with other mods
 * - v01.00  (12 May 15)   Initial Release
 * 
 */


using UnityEngine;
using KSP;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MASPlusHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleControlSurface", "MCSPlus");
        }
    }

    public class MCSPlus : PartModule
    {
        private bool plusEnabled = true;

        // Plus options
        [KSPField(guiName = "Actuator Speed", isPersistant = true)]
        [UI_FloatRange(minValue = 1f, maxValue = 50f, stepIncrement = 0.5f, affectSymCounterparts = UI_Scene.All)]
        public float actuatorSpeed = -1f;

        private ModuleControlSurface ControlSurfaceModule;

        private void SetupStockPlus()
        {
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.controlSurfacePlus == false)
            {
                plusEnabled = false;
                Fields["actuatorSpeed"].guiActive = false;
                Fields["actuatorSpeed"].guiActiveEditor = false;
                ControlSurfaceModule.Fields["partDeployInvert"].guiActive = false;
                ControlSurfaceModule.Fields["partDeployInvert"].guiActiveEditor = false;
                return;
            }

            plusEnabled = true;
            Debug.Log(moduleName + " StockPlus Enabled");

            Fields["actuatorSpeed"].guiActive = true;
            Fields["actuatorSpeed"].guiActiveEditor = true;
            ControlSurfaceModule.Fields["partDeployInvert"].guiActive = true;
            ControlSurfaceModule.Fields["partDeployInvert"].guiActiveEditor = true;

            if (actuatorSpeed < 0) { actuatorSpeed = ControlSurfaceModule.actuatorSpeed; }
        }

        public override void OnStart(StartState state)
        {
            Debug.Log(moduleName + ".Start(): v01.07");

            base.OnStart(state);

            ControlSurfaceModule = part.FindModuleImplementing<ModuleControlSurface>();

            if (null == ControlSurfaceModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find Control Surface Module.");
                return;
            }

            ControlSurfaceModule.Fields["deploy"].guiActive = true;
            ControlSurfaceModule.Fields["deploy"].guiActiveEditor = true;

            ControlSurfaceModule.Fields["deployInvert"].guiActive = true;
            ControlSurfaceModule.Fields["deployInvert"].guiActiveEditor = true;

            SetupStockPlus();

        }

        public void FixedUpdate()
        {
            ControlSurfaceModule = part.FindModuleImplementing<ModuleControlSurface>();

            if (null == ControlSurfaceModule)
            {
                return;
            }

            if (plusEnabled)
            {
                ControlSurfaceModule.actuatorSpeed = actuatorSpeed;
            }
        }
    }
}