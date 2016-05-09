/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleRSASFix - Written for KSP v1.1.2
 * 
 * - Fixes overreaction by the SAS for small vessels with excess torque/RCS control.
 * - Improved reduction of wobbly craft
 * - Added integral component back in and adjusted the min response
 * - (Plus) Gives tweakable RSAS adjustment parameters
 * 
 * Change Log:
 * - v00.06  ( 8 May 16)  Updated for KSP v1.1.2
 * - v00.05  (21 Apr 16)  Updated for KSP v1.1.0
 * - v00.04  (27 Dec 15)  Fixed a bug causing the Plus features to be disabled
 * - v00.03  (10 Nov 15)  Updated for KSP v1.0.5, integrated into new StockBugFixPlusController
 * - v00.02  (1 Jul 15)   Recompiled and tested for KSP v1.0.4, adjusted response
 * - v00.01  (7 Jun 15)   Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class PilotRSASFixHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleCommand", "PilotRSASFix");
        }
    }

    public class PilotRSASFix : PartModule
    {
        [KSPField(guiName = "Min Response", isPersistant = true, guiActive = true, guiActiveEditor = true)]
        [UI_FloatRange(minValue = 0.05f, maxValue = 1.0f, stepIncrement = 0.05f)]
        public float minResponseLimit = 0.5f;

        [KSPField(isPersistant = true)]
        public float versionNumber = 0.01f;

        [KSPField(guiName = "Min Clamp", isPersistant = true, guiActive = true, guiActiveEditor = true)]
        [UI_FloatRange(minValue = 0.1f, maxValue = 0.3f, stepIncrement = 0.01f)]
        public float minClamp = 0.2f;

        [KSPField(guiName = "Threshold", isPersistant = true, guiActive = true, guiActiveEditor = true)]
        [UI_FloatRange(minValue = 0.1f, maxValue = 0.9f, stepIncrement = 0.1f)]
        public float threshold = 0.3f;

        //[KSPField(guiName = "Response Limit", isPersistant = false, guiActive = true, guiActiveEditor = true)]
        public float responseLimit = 1f;

        //[KSPField(guiName = "Clamp", isPersistant = false, guiActive = true, guiActiveEditor = true)]
        public float Clamp = 1f;


        [KSPField(isPersistant = false)]
        public bool plusEnabled = false;

        private bool isActiveGUI = false;

        private static Vessel setVessel;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            Debug.Log(moduleName + ".Start(): v00.06");

            GameEvents.onVesselChange.Add(DisableGUI);
            DisableGUI(null);
            SetupStockPlus();

            // fix the minResponseLimit from previous versions
            if (versionNumber < 0.02f)
            {
                if (minResponseLimit == 0.3f)
                {
                    minResponseLimit = 0.5f;
                }
                versionNumber = 0.02f;
            }
        }

        public void LateUpdate()
        {
            setVessel = null;
        }

        public void DisableGUI(Vessel v)
        {
            Fields["minResponseLimit"].guiActive = false;
            Fields["minResponseLimit"].guiActiveEditor = false;
            Fields["minClamp"].guiActive = false;
            Fields["minClamp"].guiActiveEditor = false;
            Fields["threshold"].guiActive = false;
            Fields["threshold"].guiActiveEditor = false;
            isActiveGUI = false;
        }

        private void SetupStockPlus()
        {
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.pilotRSASPlus == false)
            {
                plusEnabled = false;
                return;
            }

            plusEnabled = true;
            Debug.Log(moduleName + " StockPlus Enabled");
        }

        public void FixedUpdate()
        {
            if (setVessel == part.vessel)
            {
                return;
            }

            setVessel = part.vessel;

            if (plusEnabled && !isActiveGUI)
            {
                Fields["minResponseLimit"].guiActive = true;
                Fields["minResponseLimit"].guiActiveEditor = true;
                Fields["minClamp"].guiActive = true;
                Fields["minClamp"].guiActiveEditor = true;
                Fields["threshold"].guiActive = true;
                Fields["threshold"].guiActiveEditor = true;
                isActiveGUI = true;
            }

            if (null != setVessel)
            {
                if (null != setVessel.Autopilot.RSAS.pidPitch)
                {
                    Vector3 vT = setVessel.ReferenceTransform.InverseTransformDirection(setVessel.Autopilot.RSAS.targetOrientation);
                    float dP = 90f - Mathf.Atan2(vT.y, vT.z) * (180f / 3.14159265359f);
                    if (dP > 180f) { dP -= 360f; }
                    float dY = Mathf.Atan2(vT.x, vT.y) * (180f / 3.14159265359f);


                    float dA = Mathf.Sqrt((dP * dP) + (dY * dY));
                    Clamp = 1.0f;
                    if (dA < 5)
                    {
                        if (setVessel.angularVelocity.magnitude < threshold)
                        {
                            Clamp = dA / 5f;
                            if (Clamp < minClamp) { Clamp = minClamp; }
                        }
                    }

                    if (Clamp < 1.0f)
                    {
                        responseLimit -= 0.01f;
                        if (responseLimit < minResponseLimit)
                        {
                            responseLimit = minResponseLimit;
                        }
                    }
                    else
                    {
                        responseLimit += 0.01f;
                        if (responseLimit > 1.0f)
                        {
                            responseLimit = 1.0f;
                        }
                    }

                    // Original RSAS Values
                    //   pitch (18.3f, 1.3f, 0.5f, 1f);
                    //   roll (6f, 0.25f, 0.025f, 1f);
                    //   yaw (18.3f, 1.3f, 0.5f, 1f);
                    FlightGlobals.ActiveVessel.Autopilot.RSAS.pidPitch.ReinitializePIDsOnly(18.3f * responseLimit, 1.3f * responseLimit, 0.5f * responseLimit);
                    FlightGlobals.ActiveVessel.Autopilot.RSAS.pidRoll.ReinitializePIDsOnly(6f * responseLimit, 0.25f * responseLimit, 0.025f * responseLimit);
                    FlightGlobals.ActiveVessel.Autopilot.RSAS.pidYaw.ReinitializePIDsOnly(18.3f * responseLimit, 1.3f * responseLimit, 0.5f * responseLimit);
                    setVessel.Autopilot.RSAS.pidPitch.Clamp(Clamp);
                    setVessel.Autopilot.RSAS.pidRoll.Clamp(Clamp);
                    setVessel.Autopilot.RSAS.pidYaw.Clamp(Clamp);
                }
            }
        }
    }
}
