/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleGrappleNodeFix - Written for KSP v1.0
 * - Fixes bug when clawing onto the active vessel
 * - Fixes bug which prevents activation of time warp
 * - Fail Safe prevents kraken eating ship during time warp failure
 * 
 * Change Log:
 * - v01.00  (1 Aug 15)  Initial Release
 * - v00.01  (21 Jul 15) Initial Experimental Release
 * 
 */

using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MGNFailSafe : MonoBehaviour
    {
        private bool lastKinematic = true;

        public void Start()
        {
            Debug.Log("MGNFailSafe.Start(): v01.00");
        }

        public void FixedUpdate()
        {
            if (null == FlightGlobals.ActiveVessel)
            {
                //Debug.LogError("FixedUpdate: Null Active Vessel");
                return;
            }

            if (null != FlightGlobals.ActiveVessel.parts && FlightGlobals.ActiveVessel.parts.Count > 0)
            {
                if (null == FlightGlobals.ActiveVessel.parts[0].rigidbody)
                {
                    return;
                }

                bool isKinematicRoot = FlightGlobals.ActiveVessel.parts[0].rigidbody.isKinematic;

                if (lastKinematic != isKinematicRoot)
                {
                    lastKinematic = isKinematicRoot;

                    for (int IndexParts = 0; IndexParts < FlightGlobals.ActiveVessel.parts.Count; IndexParts++)
                    {
                        Part TempPart = FlightGlobals.ActiveVessel.parts[IndexParts];

                        if (null != TempPart.rigidbody)
                        {
                            if (TempPart.rigidbody.isKinematic != isKinematicRoot)
                            {
                                Debug.LogError("Enacting Failsafe: Kinematic Mismatch: #" + IndexParts);
                                TempPart.rigidbody.isKinematic = isKinematicRoot;

                                ScreenMessages.PostScreenMessage("ERROR: Time Warp Claw Bug", 20.0f, ScreenMessageStyle.UPPER_CENTER);
                                ScreenMessages.PostScreenMessage("Quicksave and Restart NOW", 20.0f, ScreenMessageStyle.LOWER_CENTER);
                            }
                        }
                    }
                }
            }
        }

        public void OnDestroy()
        {
        }
    }

    public class MGNFix : PartModule
    {

        ModuleGrappleNode GrappleNodeModule;
        static ActiveJointPivot AJP;

        public void Start()
        {
            Debug.Log(moduleName + ".Start(): v01.00");

            GrappleNodeModule = (ModuleGrappleNode)GetModule("ModuleGrappleNode");

            if (null == GrappleNodeModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find GrappleNode Module.");
                return;
            }

        }

        public void OnDestroy()
        {
            if (GrappleNodeModule == null) { return; }

            FieldInfo[] MGNField = GrappleNodeModule.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            if (MGNField == null) { return; }

            for (int i = 0; i < MGNField.Length; i++)
            {
                if (MGNField[i].FieldType == typeof(ActiveJointPivot))
                {
                    AJP = (ActiveJointPivot)MGNField[i].GetValue(GrappleNodeModule);

                    if (AJP != null)
                    {
                        if (AJP.joint != null)
                        {
                            Debug.LogWarning("MGNFix: Joint Found.");

                            PropertyInfo TempProp = AJP.GetType().GetProperty("driveMode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                            if (TempProp != null)
                            {
                                TempProp.SetValue(AJP, ActiveJoint.DriveMode.NoJoint, null);

                                try
                                {
                                    AJP.Terminate();
                                    Debug.LogWarning("MGNFix: Joint terminated.");
                                    return;
                                }
                                catch
                                {
                                    Debug.LogError("MGNFix: Termination Failed.");
                                }
                            }

                            try
                            {
                                Debug.LogWarning("MGNFix: Attempting secondary fix.");
                                AJP.SetDriveMode(ActiveJoint.DriveMode.NoJoint);
                            }
                            catch { }

                        }
                        return;
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            if (GrappleNodeModule == null)
            {
                GrappleNodeModule = (ModuleGrappleNode)GetModule("ModuleGrappleNode");
                return;
            }
            if (FlightGlobals.ActiveVessel == null)
            {
                FieldInfo[] MGNField = GrappleNodeModule.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                Part TempPart;

                Debug.LogError("MGNFix: Null Active Vessel");

                for (int i = 0; i < MGNField.Length; i++)
                {
                    if (MGNField[i].FieldType == typeof(Part))
                    {
                        TempPart = (Part)MGNField[i].GetValue(GrappleNodeModule);

                        if (part.vessel == null)
                        {
                            return;
                        }

                        if (part.vessel == TempPart.vessel)
                        {
                            Debug.LogWarning("MGNFix: Found Target Vessel");
                            FlightGlobals.ForceSetActiveVessel(part.vessel);
                            FlightInputHandler.SetNeutralControls();
                            break;
                        }
                    }
                }
            }
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
    }
}
