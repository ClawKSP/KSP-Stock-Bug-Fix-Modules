/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleGimbalPlus - Written for KSP v1.0.5
 * 
 * - (Plus) Adds tweakable gimbal rate for engines with gimbal
 * 
 * Change Log:
 * - v01.01  (14 Nov 15)   Made moduleGimbal
 * - v01.00  (11 Nov 15)   Updated for KSP 1.0.5. Renamed from ModuleGimbalFix. Integrated into StockBugFixPlus controller.
 * - v00.03  (28 Jul 15)   Attempted fixes for mod compatibility
 * - v00.02  (1 Jul 15)    Recompiled for KSP v1.0.4, added toggle to activate Gimbal Rate usage
 * - v00.01  (15 May 15)   Initial Experimental Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MGPlusHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleGimbal", "MGPlus");
        }
    }

    public class MGPlus : PartModule
    {

        public bool plusEnabled = false;

        [KSPField(guiName = "Gimbal Rate", isPersistant = true)]
        [UI_Toggle(disabledText = "Disabled", enabledText = "Active", affectSymCounterparts = UI_Scene.All)]
        public bool gimbalRateIsActive = true;

        [KSPField(guiName = "Gimbal Rate", isPersistant = true)]
        [UI_FloatRange(minValue = 1f, maxValue = 30f, stepIncrement = 1f, affectSymCounterparts = UI_Scene.All)]
        public float gimbalResponseSpeed = 10f;

        private ModuleGimbal GimbalModule;

        private void SetupStockPlus()
        {
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.gimbalPlus == false)
            {
                plusEnabled = false;
                Fields["gimbalResponseSpeed"].guiActive = false;
                Fields["gimbalResponseSpeed"].guiActiveEditor = false;
                Fields["gimbalRateIsActive"].guiActive = false;
                Fields["gimbalRateIsActive"].guiActiveEditor = false;
                return;
            }

            Debug.Log(moduleName + " StockPlus Enabled");

            Fields["gimbalResponseSpeed"].guiActive = true;
            Fields["gimbalResponseSpeed"].guiActiveEditor = true;
            Fields["gimbalRateIsActive"].guiActive = true;
            Fields["gimbalRateIsActive"].guiActiveEditor = true;

            GimbalModule.useGimbalResponseSpeed = gimbalRateIsActive;
            GimbalModule.gimbalResponseSpeed = gimbalResponseSpeed;
        }

        public override void OnStart(StartState state)
        {
            Debug.Log(moduleName + ".Start(): v01.01");

            //base.OnStart(state);

            GimbalModule = (ModuleGimbal)GetModule("ModuleGimbal");
            if (null == GimbalModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find Gimbal Module.");
                return;
            }

            SetupStockPlus();
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

        public void FixedUpdate()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT) { return; }

            if (null == GimbalModule)
            {
                GimbalModule = (ModuleGimbal)GetModule("ModuleGimbal");
                return;
            }

            if (HighLogic.LoadedScene == GameScenes.FLIGHT && part.State == PartStates.IDLE)
            {
                GimbalModule.OnFixedUpdate();
            }

            if (plusEnabled == true)
            {
                if (gimbalRateIsActive == true)
                {
                    GimbalModule.useGimbalResponseSpeed = true;
                    GimbalModule.gimbalResponseSpeed = gimbalResponseSpeed;
                }
                else
                {
                    GimbalModule.useGimbalResponseSpeed = false;
                }
            }
        }
    }
}
