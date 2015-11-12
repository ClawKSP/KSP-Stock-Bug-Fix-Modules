/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleAeroSurfacePlus - Written for KSP v1.0
 * 
 * - (Plus) When stowed, the airbrake doesn't generate drag.
 * 
 * Change Log:
 * - v01.06  (10 Nov 15)   Updated for v1.0.5. Integrated into StockPlus. Renamed from ModuleAeroSurfaceFix to ModuleAeroSurfacePlus
 * - v01.05  (1 Nov 15)    Fixed some log spam in the editor
 * - v01.04  (1 Sep 15)    Added plus function to disable movement in space
 * - v01.03  (1 Jul 15)    Recompiled and tested for KSP v1.0.4
 * - v01.02  (1 Jun 15)    Changed brake default action to toggle
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
            StockBugFixPlusController.HookModule("ModuleAeroSurface", "MASPlus");
        }
    }

    public class MASPlus : PartModule
    {
        //[KSPField(isPersistant = true)]
        //public bool deploy = false;

        [KSPField(isPersistant = false)]
        public bool plusEnabled = false;

        [KSPField(isPersistant = false)]
        private float deflectionLiftCoeff;

        private ModuleAeroSurface AeroSurfaceModule;

        // plus options
        private float ctrlSurfaceRange;
        private float vacuumRange = 1.0f; // disables flight controls when in vacuum

        private void SetupStockPlus()
        {
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.aeroSurfacePlus == false)
            {
                plusEnabled = false;
                return;
            }

            Debug.Log(moduleName + " StockPlus Enabled");

            deflectionLiftCoeff = AeroSurfaceModule.deflectionLiftCoeff;

            if (FlightGlobals.getStaticPressure(part.transform.position) < 0.001f)
            {
                vacuumRange = 0.01f;
            }
            ctrlSurfaceRange = AeroSurfaceModule.ctrlSurfaceRange;
            AeroSurfaceModule.ctrlSurfaceRange = ctrlSurfaceRange * vacuumRange;
        }

        public override void OnStart(StartState state)
        {
            Debug.Log(moduleName + ".Start(): v01.06");

            base.OnStart(state);

            AeroSurfaceModule = part.FindModuleImplementing<ModuleAeroSurface>();
            if (null == AeroSurfaceModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find Aero Surface Module.");
                return;
            }

            SetupStockPlus();
        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT) { return; }

            AeroSurfaceModule = part.FindModuleImplementing<ModuleAeroSurface>();
            if (null == AeroSurfaceModule) { return; }

            if (true == plusEnabled)
            {
                if (false == AeroSurfaceModule.deploy)
                {
                    if (part.vessel.ctrlState.pitch > 0.01 || part.vessel.ctrlState.pitch < -0.01)
                    {
                        if (part.vessel.ctrlState.yaw > 0.01 || part.vessel.ctrlState.yaw < -0.01)
                        {
                            AeroSurfaceModule.deflectionLiftCoeff = deflectionLiftCoeff;
                        }
                    }
                    else
                    {
                        AeroSurfaceModule.deflectionLiftCoeff = 0f;
                    }
                }
                else
                {
                    AeroSurfaceModule.deflectionLiftCoeff = deflectionLiftCoeff;
                }

                if (FlightGlobals.getStaticPressure(part.transform.position) < 0.0001f)
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
                AeroSurfaceModule.ctrlSurfaceRange = ctrlSurfaceRange * vacuumRange;
            }
        }

    }
}
