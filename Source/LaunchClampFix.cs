/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * LaunchClampFix - Written for KSP v1.0
 * - Fixes bug where Launch Clamps follow the active craft around.
 * - Refunds clamp cost and allows the aero-cleanup to auto delete.
 * 
 * Change Log:
 * - v00.02  (1 Sep 15)   Fixed log spam in the editor
 * - v00.01  (22 Aug 15)  Initial Experimental Release.
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{
    public class LCFix : PartModule
    {

        public void Start()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT) { return; }

            Debug.Log(moduleName + ".Start(): v00.02 (Experimental)");
        }

        public void OnDestroy()
        {
        }

        public void Update()
        {
            //if (vessel.situation == Vessel.Situations.FLYING)
            //{
            //    vessel.situation = Vessel.Situations.LANDED;
            //}
        }

        public void LateUpdate()
        {
            //Update();
        }

        public void FixedUpdate()
        {
            //Debug.LogWarning("Situation: " + vessel.situation);
            if (HighLogic.LoadedScene != GameScenes.FLIGHT) { return; }
            
            // Vessels that are flying are automatically deleted when going on rails.
            if (vessel.situation == Vessel.Situations.FLYING)
            {
                if (Vector3.Distance(vessel.transform.position, FlightGlobals.ActiveVessel.transform.position) > 5500f)
                //if (Vector3.Distance(vessel.transform.position, FlightGlobals.ActiveVessel.transform.position) > vessel.vesselRanges.GetSituationRanges(Vessel.Situations.LANDED).pack)
                {
                    if (!vessel.packed)
                    {
                        if (null != Funding.Instance)
                        {
                            Debug.LogWarning("Refunding: " + CalculateFunds());
                            Funding.Instance.AddFunds(CalculateFunds(), TransactionReasons.VesselRecovery);
                        }
                        vessel.situation = Vessel.Situations.LANDED;
                        vessel.Landed = true;
                        vessel.GoOnRails();
                    }
                }
            }

            //vessel.vesselRanges.flying.pack = 350f;
        }

        public double CalculateFunds()
        {
            double reFunds = 0d;

            for (int IndexParts = 0; IndexParts < vessel.protoVessel.protoPartSnapshots.Count; IndexParts++)
            {
                ProtoPartSnapshot currentProtoPart = vessel.protoVessel.protoPartSnapshots[IndexParts];
                AvailablePart currentAvailable = PartLoader.getPartInfoByName(currentProtoPart.partInfo.name);

                if (null != currentAvailable)
                {

                }

                float dryCost;
                float fuelCost;

                ShipConstruction.GetPartCosts(currentProtoPart, currentAvailable, out dryCost, out fuelCost);

                reFunds += dryCost + fuelCost;

            }

            if (reFunds < 0)
            {
                reFunds = 0;
            }

            return (reFunds);
        }
    }
}
