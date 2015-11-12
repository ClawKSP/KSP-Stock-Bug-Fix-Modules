/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleWheelPlus - Written for KSP v1.0
 * 
 * - Adds a traction tweak.
 * 
 * Change Log:
 * - v01.06  (10 Nov 15)   Updated for KSP v1.0.5. Renamed from ModuleWheelFix. Integrated into StockBugFixPlus
 * - v01.05  (1 Jul 15)    Recompiled and tested for KSP v1.0.4
 * - v01.04  (18 May 15)   Fixed some minor StockPlus UI bugs
 * - v01.03  (11 May 15)   New UI fix, reworked loading procedures (again again) and fixed a bug
 * - v01.02  (9 May 15)    Reworked loading procedures (again) and updated for StockPlus
 * - v01.01  (1 May 15)    Reworked loading procedure and tweakables, recompiled for KSP 1.0.2
 * - v01.00  (27 Apr 15)   Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MWPlusHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleWheel", "MWPlus");
        }
    }

    public class MWPlus : PartModule
    {
        [KSPField(guiName = "Forward Grip", isPersistant = true, guiActive = false, guiActiveEditor = false)]
        [UI_FloatRange(minValue = 0.1f, maxValue = 3f, stepIncrement = 0.1f)]
        public float forwardStiffnessMult = 1f;

        [KSPField(guiName = "Sideways Grip", isPersistant = true, guiActive = false, guiActiveEditor = false)]
        [UI_FloatRange(minValue = 0.1f, maxValue = 3f, stepIncrement = 0.1f)]
        public float sidewaysStiffnessMult = 1f;

        private float lastForwardStiffnessMult = -1f;
        private float lastSidewaysStiffnessMult = -1f;

        ModuleWheel WheelModule;

        public bool plusEnabled = false;

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


        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            Debug.Log(moduleName + ".Start(): v01.06");

            WheelModule = (ModuleWheel)GetModule("ModuleWheel");

            if (null == WheelModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find Wheel Module.");
                return;
            }

            // get the wheel grip
            if (forwardStiffnessMult < 0 || sidewaysStiffnessMult < 0)
            {
                forwardStiffnessMult = WheelModule.forwardStiffnessMult;
                sidewaysStiffnessMult = WheelModule.sidewaysStiffnessMult;
            }

            SetupStockPlus();
        }

        private void SetupStockPlus()
        {
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.wheelPlus == false)
            {
                Fields["forwardStiffnessMult"].guiActive = false;
                Fields["forwardStiffnessMult"].guiActiveEditor = false;
                Fields["sidewaysStiffnessMult"].guiActive = false;
                Fields["sidewaysStiffnessMult"].guiActiveEditor = false;
                plusEnabled = false;
                return;
            }

            Fields["forwardStiffnessMult"].guiActive = true;
            Fields["forwardStiffnessMult"].guiActiveEditor = true;
            Fields["sidewaysStiffnessMult"].guiActive = true;
            Fields["sidewaysStiffnessMult"].guiActiveEditor = true;
        }

        public void FixedUpdate()
        {
            if (null == WheelModule) { return; }

            if (lastForwardStiffnessMult != forwardStiffnessMult || lastSidewaysStiffnessMult != sidewaysStiffnessMult)
            {
                lastForwardStiffnessMult = forwardStiffnessMult;
                lastSidewaysStiffnessMult = sidewaysStiffnessMult;

                UpdateStiffness();
            }
        }

        private void UpdateStiffness()
        {
            if (part.partInfo != null && part.partInfo.partPrefab != null && part.partInfo.partPrefab != part)
            {
                Part tempPart = part.partInfo.partPrefab;
                ModuleWheel MW;

                try
                {
                    MW = (ModuleWheel)(tempPart.Modules["ModuleWheel"]);
                }
                catch
                {
                    Debug.LogError("MWPlus.UpdateFriction() failed to update.");
                    return;
                }

                for (int indexWheels = 0; indexWheels < MW.wheels.Count; indexWheels++)
                {
                    WheelFrictionCurve wheelFrictionCurve = MW.wheels[indexWheels].whCollider.forwardFriction;
                    wheelFrictionCurve.stiffness *= forwardStiffnessMult;
                    WheelModule.wheels[indexWheels].whCollider.forwardFriction = wheelFrictionCurve;

                    wheelFrictionCurve = MW.wheels[indexWheels].whCollider.sidewaysFriction;
                    wheelFrictionCurve.stiffness *= sidewaysStiffnessMult;
                    WheelModule.wheels[indexWheels].whCollider.sidewaysFriction = wheelFrictionCurve;
                }

                //WheelModule.forwardStiffnessMult = forwardStiffnessMult;
                //WheelModule.sidewaysStiffnessMult = sidewaysStiffnessMult;
            }
        }
    }
}
