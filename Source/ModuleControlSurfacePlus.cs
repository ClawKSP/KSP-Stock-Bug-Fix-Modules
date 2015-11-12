/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleControlSurfacePlus - Written for KSP v1.0
 * 
 * - (Plus) Adds tweakable authority
 * - (Plus) Disables flight controls in space
 * 
 * Change Log:
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

        // Plus option
        [KSPField(guiName = "Authority", isPersistant = true)]
        [UI_FloatRange(minValue = 0.1f, maxValue = 1.9f, stepIncrement = 0.1f, affectSymCounterparts = UI_Scene.All)]
        public float Authority = 1.0f;

        private ModuleControlSurface ControlSurfaceModule;

        // Plus option
        private float ctrlSurfaceRange;
        private float vacuumRange = 1.0f; // disables flight controls when in vacuum

        private void SetupStockPlus()
        {
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.controlSurfacePlus == false)
            {
                plusEnabled = false;
                Fields["Authority"].guiActive = false;
                Fields["Authority"].guiActiveEditor = false;
                return;
            }

            Debug.Log(moduleName + " StockPlus Enabled");

            Fields["Authority"].guiActive = true;
            Fields["Authority"].guiActiveEditor = true;

            if (FlightGlobals.getStaticPressure(part.transform.position) < 0.001f)
            {
                vacuumRange = 0.01f;
            }
            ctrlSurfaceRange = ControlSurfaceModule.ctrlSurfaceRange;
            ControlSurfaceModule.ctrlSurfaceRange = ctrlSurfaceRange * Authority * vacuumRange;
        }

        public override void OnStart(StartState state)
        {
            Debug.Log(moduleName + ".Start(): v01.04");

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

            if (true == plusEnabled)
            {
                if (FlightGlobals.getStaticPressure(part.transform.position) < 0.001f)
                {
                    if (vacuumRange > 0.01f)
                    {
                        vacuumRange -= 0.05f;
                    }
                    else
                    {
                        vacuumRange = 0.01f;
                    }
                }
                else
                {
                    if (vacuumRange < 1.0f)
                    {
                        vacuumRange += 0.05f;
                    }
                    else
                    {
                        vacuumRange = 1.0f;
                    }
                }
                ControlSurfaceModule.ctrlSurfaceRange = ctrlSurfaceRange * Authority * vacuumRange * Mathf.Sign(ControlSurfaceModule.ctrlSurfaceRange);
            }
        }
    }
}