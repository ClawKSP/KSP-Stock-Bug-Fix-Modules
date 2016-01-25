/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleProceduralFairingFix - Written for KSP v1.0
 * - Fixes some "stuck fairing" issues when jettisoning
 * - Fairings stuck on vessel no longer cause the vessel to register as "landed" (and won't save)
 * - Fixes some bugs with pulling and replacing fairings.
 * - (Plus) Activates a tweakable slider for the user to select the number of panels on the fairing.
 * - (Plus) Activates a tweakable slider to control ejection forces on the panels.
 * 
 * Change Log:
 * - v01.10  (25 Jan 16)  Fixed body lift when respawning meshes when StockPlus active.
 * - v01.09  (14 Nov 15)  Fixed body lift location for ProcFairings. Added overlay fix, but currently disabled.
 * - v01.08  (11 Nov 15)  Updated for KSP v1.0.5. Integrated into StockBugFixPlusController.
 * - v01.07  ( 1 Jul 15)  Recompiled for v1.0.4
 * - v01.06  (10 Jun 15)  Removed some WIP code that snuk in
 * - v01.05  ( 9 Jun 15)  Added MPFFixAddon
 * - v01.04  (18 May 15)  Fixed some minor StockPlus UI bugs
 * - v01.03  (13 May 15)  Updates and minor adjustments, incorporates StockPlus
 * - v01.02  ( 3 May 15)  Moved ejection force out to Module Manager
 * - v01.01  ( 2 May 15)  Updated and recompiled for KSP 1.0.2
 * - v01.00  (27 Apr 15)  Initial Release
 * 
 */

using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MPPlusHook : MonoBehaviour
    {
        public void Start()
        {
            StockBugFixPlusController.HookModule("ModuleProceduralFairing", "MPFFix");
        }
    }

    public class MPFFix : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Panels")]
        [UI_FloatRange(minValue = 1f, maxValue = 8f, stepIncrement = 1f)]
        public float nArcs = 0f;

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "Ejection Force")]
        [UI_FloatRange(minValue = 0f, maxValue = 200f, stepIncrement = 10f)]
        public float ejectionForce = -1f;

        ModuleProceduralFairing FairingModule;
        ModuleCargoBay CargoBayModule;

        public bool plusEnabled = false;
        private Part interstagePart = null;

        //private int cycleOverlay = 0;
        //private int xSectionCount = -1;

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

        private void SetupStockPlus()
        {
            if (StockBugFixPlusController.plusActive == false || StockBugFixPlusController.proceduralFairingPlus == false)
            {
                plusEnabled = false;
                Fields["nArcs"].guiActive = false;
                Fields["nArcs"].guiActiveEditor = false;
                Fields["ejectionForce"].guiActive = false;
                Fields["ejectionForce"].guiActiveEditor = false;
                return;
            }

            Debug.Log("MPFFix: StockPlus active.");
            plusEnabled = true;
            Fields["nArcs"].guiActiveEditor = true;
            Fields["ejectionForce"].guiActiveEditor = true;
        }
        
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Debug.Log("MPFFix.OnStart(): v01.10");

            FairingModule = (ModuleProceduralFairing) GetModule("ModuleProceduralFairing");

            if (null == FairingModule)
            {
                Debug.LogWarning("ModuleProceduralFairingFix.Start(): Did not find Fairing Module.");
                return;
            }

            //part.CoPOffset /= 2;
            //part.CoLOffset = part.CoPOffset;
            part.CoPOffset = Vector3.zero;
            part.CoLOffset = Vector3.zero;

            SetupStockPlus();

            if (plusEnabled == true)
            {
                if (ejectionForce < 0)
                {
                    ejectionForce = FairingModule.ejectionForce;
                }

                if (nArcs == 0)
                {
                    nArcs = FairingModule.nArcs;
                }

            }

            GameEvents.onPartRemove.Add(RemovePart);

            try { CargoBayModule = part.FindModuleImplementing<ModuleCargoBay>(); }
            catch { }
            interstagePart = GetInterstage();
            if (null != interstagePart)
            {
                Debug.LogWarning("interstagePart name = " + interstagePart.name + " partName = " + interstagePart.partName);
            }
        }

        private Part GetInterstage ()
        {
            FieldInfo MPFInfo = FairingModule.GetType().GetField("", BindingFlags.NonPublic | BindingFlags.Instance);

            if (null != MPFInfo)
            {
                //Debug.LogWarning("Found");
                return ((Part)MPFInfo.GetValue(FairingModule));
            }

            return (null);
        }

        public void RemovePart(GameEvents.HostTargetAction<Part, Part> RemovedPart)
        {
            if (null == FairingModule) { return; }

            if (RemovedPart.target == part)
            {
                if (FairingModule.xSections.Count > 0)
                {
                    Debug.LogWarning("Deleting Fairing");
                    FairingModule.DeleteFairing();
                }

                MethodInfo MPFMethod = FairingModule.GetType().GetMethod("DumpInterstage", BindingFlags.NonPublic | BindingFlags.Instance);

                if (MPFMethod != null)
                {
                    MPFMethod.Invoke(FairingModule, new object[] { });
                }
            }
        }

        public void FixedUpdate()
        {
            if (FairingModule == null) { return; }
            

            //if (xSectionCount != FairingModule.xSections.Count)
            //{
            //    if (PhysicsGlobals.AeroForceDisplay)
            //    {
            //        cycleOverlay = 2;
            //        PhysicsGlobals.AeroForceDisplay = false;
            //    }
            //    xSectionCount = FairingModule.xSections.Count;
            //}
            //else if (cycleOverlay == 1)
            //{
            //    cycleOverlay = 0;
            //    PhysicsGlobals.AeroForceDisplay = true;
            //}
            //else if (cycleOverlay > 0)
            //{
            //    cycleOverlay--;
            //}

            if (plusEnabled == true)
            {

                if (FairingModule.nArcs != nArcs)
                {
                    FairingModule.nArcs = (int)nArcs;

                    MethodInfo MPFMethod = FairingModule.GetType().GetMethod("WipeMesh", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (MPFMethod != null)
                    {
                        MPFMethod.Invoke(FairingModule, new object[] { });
                    }

                    MPFMethod = FairingModule.GetType().GetMethod("SpawnMeshes", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (MPFMethod != null)
                    {
                        MPFMethod.Invoke(FairingModule, new object[] { true });
                    }

                    part.CoPOffset = Vector3.zero;
                    part.CoLOffset = Vector3.zero;
                }

                FairingModule.ejectionForce = ejectionForce;
            }
        }

        public void OnDestroy()
        {
            GameEvents.onPartRemove.Remove(RemovePart);
        }
    }


    // MPFFixAddon shouldn't be needed anymore in KSP 1.0.5
    //[KSPAddon(KSPAddon.Startup.Flight, false)]
    //public class MPFFixAddon : MonoBehaviour
    //{

    //    private int gameObjectCount = 0;
    //    private int collisionCounter = 0;

    //    public void Start()
    //    {
    //        Debug.Log("MPFFixAddon.Start()");
    //    }

    //    private void SetLayer(int layer)
    //    {
    //        //for (int indexObjects = 0; indexObjects < FlightGlobals.physicalObjects.Count; indexObjects++)
    //        for (int indexObjects = FlightGlobals.physicalObjects.Count - 1; indexObjects >= 0; indexObjects--)
    //        {
    //            if (null == FlightGlobals.physicalObjects[indexObjects])
    //            {
    //                //Debug.LogWarning("Removing Null Object #" + indexObjects);
    //                FlightGlobals.physicalObjects.RemoveAt(indexObjects);
    //                //indexObjects--;
    //            }
    //            else
    //            {
    //                //Debug.LogWarning("Object #" + indexObjects + " | Name: " + FlightGlobals.physicalObjects[indexObjects].name + " | Layer: " + FlightGlobals.physicalObjects[indexObjects].layer);
    //                if (FlightGlobals.physicalObjects[indexObjects].name == "FairingPanel")
    //                {
    //                    //Debug.Log("Resetting Layer = " + layer);
    //                    FlightGlobals.physicalObjects[indexObjects].layer = layer;
    //                }
    //            }
    //        }
    //    }

    //    private void SetCollisions(bool detectCollisions)
    //    {
    //        //for (int indexObjects = 0; indexObjects < FlightGlobals.physicalObjects.Count; indexObjects++)
    //        for (int indexObjects = FlightGlobals.physicalObjects.Count - 1; indexObjects >= 0; indexObjects--)
    //        {
    //            if (null == FlightGlobals.physicalObjects[indexObjects])
    //            {
    //                FlightGlobals.physicalObjects.RemoveAt(indexObjects);
    //                //indexObjects--;
    //            }
    //            else
    //            {
    //                if (FlightGlobals.physicalObjects[indexObjects].name == "FairingPanel")
    //                {
    //                    FlightGlobals.physicalObjects[indexObjects].rigidbody.detectCollisions = detectCollisions;
    //                }
    //            }
    //        }
    //    }

    //    public void FixedUpdate()
    //    {

    //        if (FlightGlobals.ActiveVessel.isEVA)
    //        {
    //            SetLayer(15);
    //            gameObjectCount = -1;
    //            return;
    //        }

    //        // Fairing Name: gameObject.name = "FairingPanel"

    //        if (gameObjectCount != FlightGlobals.physicalObjects.Count)
    //        {
    //            SetLayer(0);
    //            SetCollisions(false);
    //            collisionCounter = 2;
    //            gameObjectCount = FlightGlobals.physicalObjects.Count;

    //            return;
    //        }

    //        if (collisionCounter > 0)
    //        {
    //            collisionCounter--;
    //        }
    //        else
    //        {
    //            collisionCounter = -1;
    //            SetCollisions(true);
    //        }
    //    }

    //    public void OnDestroy()
    //    {
    //        FlightGlobals.physicalObjects.Clear();
    //    }
    //}
}