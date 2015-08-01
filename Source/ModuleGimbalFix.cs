/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleGimbalFix - Written for KSP v1.0
 * 
 * - (Plus) Adds tweakable gimbal rate for engines with gimbal
 * 
 * Change Log:
 * - v00.03  (28 Jul 15)   Attempted fixes for mod compatibility
 * - v00.02  (1 Jul 15)    Recompiled for KSP v1.0.4, added toggle to activate Gimbal Rate usage
 * - v00.01  (15 May 15)   Initial Experimental Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    public class MGFix : PartModule
    {

        [KSPField(isPersistant = false)]
        public bool plusEnabled = false;

        [KSPField(guiName = "Gimbal Rate", isPersistant = true)]
        [UI_Toggle(disabledText = "Disabled", enabledText = "Active", affectSymCounterparts = UI_Scene.All)]
        public bool gimbalRateIsActive = false;

        [KSPField(guiName = "Gimbal Rate", isPersistant = true)]
        [UI_FloatRange(minValue = 1f, maxValue = 30f, stepIncrement = 1f, affectSymCounterparts = UI_Scene.All)]
        public float gimbalResponseSpeed = 10f;

        private ModuleGimbal GimbalModule;

        private void SetupStockPlus()
        {
            if (StockPlusController.plusActive == false || plusEnabled == false)
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
            Debug.Log(moduleName + ".Start(): v00.03");

            //base.OnStart(state);

            //GimbalModule = part.FindModuleImplementing<ModuleGimbal>();
            GimbalModule = (ModuleGimbal) GetModule("ModuleGimbal");
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
            //GimbalModule = part.FindModuleImplementing<ModuleGimbal>();
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
