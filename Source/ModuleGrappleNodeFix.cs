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
 * - Prevents kraken eating ship during time warp failure
 * 
 * Change Log:
 * - v00.01  (21 Jul 15) Initial Experimental Release
 * 
 */

using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class MGNFixAddon : MonoBehaviour
    {

        public void Start()
        {
            Debug.LogWarning("MGNFixAddon.Start(): v00.01");
        }


        public void FixedUpdate()
        {
            if (null == FlightGlobals.ActiveVessel)
            {
                //Debug.LogError("FixedUpdate: Null Active Vessel");
                return;
            }

            if (null != FlightGlobals.ActiveVessel.parts)
            {
                if (null == FlightGlobals.ActiveVessel.parts[0].rigidbody)
                {
                    return;
                }

                bool isKinematicRoot = FlightGlobals.ActiveVessel.parts[0].rigidbody.isKinematic;

                for (int IndexParts = 0; IndexParts < FlightGlobals.ActiveVessel.parts.Count; IndexParts++)
                {
                    Part TempPart = FlightGlobals.ActiveVessel.parts[IndexParts];

                    if (null != TempPart.rigidbody)
                    {
                        if (TempPart.rigidbody.isKinematic != isKinematicRoot)
                        {
                            Debug.LogError("Kinematic Mismatch: #" + IndexParts);
                            TempPart.rigidbody.isKinematic = isKinematicRoot;

                            ScreenMessages.PostScreenMessage("ERROR: Time Warp Claw Bug", 20.0f, ScreenMessageStyle.UPPER_CENTER);
                            ScreenMessages.PostScreenMessage("Quicksave and Restart NOW", 20.0f, ScreenMessageStyle.LOWER_CENTER);
                            // TempPart.Pack();
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
            Debug.Log(moduleName + ".Start(): v00.01");

            GrappleNodeModule = (ModuleGrappleNode)GetModule("ModuleGrappleNode");

            if (null == GrappleNodeModule)
            {
                Debug.LogWarning(moduleName + ".Start(): Did not find GrappleNode Module.");
                return;
            }

        }

        public void OnDestroy()
        {
            FieldInfo[] MGNField = GrappleNodeModule.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < MGNField.Length; i++)
            {
                if (MGNField[i].FieldType == typeof(ActiveJointPivot))
                {
                    AJP = (ActiveJointPivot)MGNField[i].GetValue(GrappleNodeModule);
                    AJP.SetDriveMode(ActiveJoint.DriveMode.NoJoint);

                    FieldInfo TempField = AJP.GetType().GetField("driveMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    ActiveJoint.DriveMode DM = (ActiveJoint.DriveMode)TempField.GetValue(AJP);
                    DM = ActiveJoint.DriveMode.NoJoint;

                    AJP.Terminate();
                }
            }
        }

        public void FixedUpdate()
        {
            if (FlightGlobals.ActiveVessel == null)
            {
                FieldInfo[] MGNField = GrappleNodeModule.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                Part TempPart;

                Debug.LogError("Null Active Vessel");

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
                            Debug.LogWarning("Found Target Vessel");
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
