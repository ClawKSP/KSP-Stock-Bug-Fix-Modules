/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleGimbalPlus - Written for KSP v1.1.2
 * 
 * - (Plus) Adds tweakable gimbal rate for engines with gimbal
 * 
 * Change Log:
 * - v01.03  ( 8 May 16)   Updated for KSP v1.1.2
 * - v01.02  (27 Dec 15)   Added support for user configurable gimbal rates
 * - v01.01  (14 Nov 15)   Made moduleGimbal
 * - v01.00  (11 Nov 15)   Updated for KSP 1.0.5. Renamed from ModuleGimbalFix. Integrated into StockBugFixPlus controller.
 * - v00.03  (28 Jul 15)   Attempted fixes for mod compatibility
 * - v00.02  ( 1 Jul 15)    Recompiled for KSP v1.0.4, added toggle to activate Gimbal Rate usage
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
        public float gimbalResponseSpeed = -1f;

        //[KSPField(guiName = "Gimbal Yaw", isPersistant = true)]
        //[UI_Toggle(disabledText = "Disabled", enabledText = "Active", affectSymCounterparts = UI_Scene.All)]
        //public bool gimbalAngleYaw = true;

        //[KSPField(guiName = "Gimbal Pitch", isPersistant = true)]
        //[UI_Toggle(disabledText = "Disabled", enabledText = "Active", affectSymCounterparts = UI_Scene.All)]
        //public bool gimbalAnglePitch = true;

        //[KSPField(guiName = "Gimbal Roll", isPersistant = true)]
        //[UI_Toggle(disabledText = "Disabled", enabledText = "Active", affectSymCounterparts = UI_Scene.All)]
        //public bool gimbalAngleRoll = true;

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
                //Fields["gimbalAngleYaw"].guiActive = false;
                //Fields["gimbalAngleYaw"].guiActiveEditor = false;
                //Fields["gimbalAnglePitch"].guiActive = false;
                //Fields["gimbalAnglePitch"].guiActiveEditor = false;
                //Fields["gimbalAngleRoll"].guiActive = false;
                //Fields["gimbalAngleRoll"].guiActiveEditor = false;
                return;
            }

            Debug.Log(moduleName + " StockPlus Enabled");

            plusEnabled = true;
            Fields["gimbalResponseSpeed"].guiActive = true;
            Fields["gimbalResponseSpeed"].guiActiveEditor = true;
            Fields["gimbalRateIsActive"].guiActive = true;
            Fields["gimbalRateIsActive"].guiActiveEditor = true;
            //Fields["gimbalAngleYaw"].guiActive = true;
            //Fields["gimbalAngleYaw"].guiActiveEditor = true;
            //Fields["gimbalAnglePitch"].guiActive = true;
            //Fields["gimbalAnglePitch"].guiActiveEditor = true;
            //Fields["gimbalAngleRoll"].guiActive = true;
            //Fields["gimbalAngleRoll"].guiActiveEditor = true;

            if (gimbalResponseSpeed == -1)
            {
                gimbalResponseSpeed = StockBugFixPlusController.gimbalResponseSpeed;
                gimbalRateIsActive = StockBugFixPlusController.gimbalRateIsActive;
            }

            GimbalModule.useGimbalResponseSpeed = gimbalRateIsActive;
            GimbalModule.gimbalResponseSpeed = gimbalResponseSpeed;
        }

        public override void OnStart(StartState state)
        {
            Debug.Log(moduleName + ".Start(): v01.03");

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

            //if (HighLogic.LoadedScene == GameScenes.FLIGHT && part.State == PartStates.IDLE)
            //{
            //    GimbalModule.OnFixedUpdate();
            //}

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
                //if (!gimbalAngleYaw)
                //{
                //    GimbalModule.gimbalAngleYaw = 0f;
                //}
                //if (!gimbalAnglePitch)
                //{
                //    GimbalModule.gimbalAnglePitch = 0f;
                //}
                //if (!gimbalAngleRoll)
                //{
                //    GimbalModule.gimbalAngleRoll = 0f;
                //}
            }
        }
    }
}
