/*
 * This module written by Claw.. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285-0-25-Stock-Bug-Fix-Modules
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the license.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 * Written for KSP v0.90.0
 *
 * CrewRosterFreezeFix v0.1.1
 * 
 * Change Log:
 * 
 * v0.1.1 (29 Dec 14) - Updated Release to handle firing MIA kerbals and contract kerbals
 * v0.1.0 (20 Dec 14) - Initial Release
 */

using UnityEngine;
using KSP;
using Contracts;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClawKSP
{
    // Needs to be in every scene because crew management can be invoked from editors or space centre.
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class CRFFix : MonoBehaviour
    {
        // Probably not needed anymore, but left in here because KSP was starting it multiple times at one point.
        private static bool Started = false;

        private static string KerbalList = String.Empty;

        public void Start ()
        {
            if (true == Started) return;

            Debug.Log("CRFFix.Start()");
            Started = true;

            // Clear out the Kerbal list if back at the main menu and clear it.
            if (GameScenes.MAINMENU == HighLogic.LoadedScene)
            {
                KerbalList = String.Empty;
                return;
            }

            if (GameScenes.SPACECENTER != HighLogic.LoadedScene
                && GameScenes.EDITOR != HighLogic.LoadedScene
                && GameScenes.FLIGHT != HighLogic.LoadedScene)
            {
                return;
            }

            KerbalListUpdate();

            GameEvents.onKerbalTypeChange.Add(TypeChange);
            GameEvents.onKerbalRemoved.Add(KerbalRemoved);
            GameEvents.onKerbalAdded.Add(KerbalAdded);

            GameEvents.onGameStateLoad.Add(GameLoaded);

            GameEvents.onGUIAstronautComplexSpawn.Add(new EventVoid.OnEvent(this.ComplexEntry));
            GameEvents.onGUIAstronautComplexDespawn.Add(new EventVoid.OnEvent(this.ComplexExit));
        } // Start()

        public void GameLoaded (ConfigNode CN)
        {
            KerbalList = String.Empty;
        } // GameLoaded()

        public void KerbalAdded (ProtoCrewMember AddedKerbal)
        {
            KerbalListUpdate();
        } // KerbalAdded()

        public void KerbalRemoved(ProtoCrewMember RemovedKerbal)
        {

            // Check if this kerbal is already in the list of known names. If not, then he was probably added
            // and deleted immediately by the contract system and we don't need to keep him.
            if (!KerbalList.Contains(RemovedKerbal.name))
            {
                // Debug.LogWarning("CRFFix.KerbalRemoved(): Short Lived Kerbal Found - " + RemovedKerbal.name);
                return;
            }

            ProtoCrewMember NewKerbal = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Unowned);

            if (null == NewKerbal)
            {
                Debug.LogError("CRFFix.KerbalRemoved(): Failed To Save Kerbal (" + RemovedKerbal.name + ")");
                return;
            }

            // Clones the kerbal to send into cryostasis before the existing one is deleted.
            NewKerbal.name = RemovedKerbal.name;
            NewKerbal.careerLog = RemovedKerbal.careerLog.CreateCopy();
            NewKerbal.courage = RemovedKerbal.courage;
            NewKerbal.experience = RemovedKerbal.experience;
            NewKerbal.experienceLevel = RemovedKerbal.experienceLevel;
            NewKerbal.experienceTrait = RemovedKerbal.experienceTrait;
            NewKerbal.flightLog = RemovedKerbal.flightLog.CreateCopy();
            NewKerbal.isBadass = RemovedKerbal.isBadass;
            NewKerbal.KerbalRef = RemovedKerbal.KerbalRef;
            NewKerbal.seat = RemovedKerbal.seat;
            NewKerbal.seatIdx = RemovedKerbal.seatIdx;
            NewKerbal.stupidity = RemovedKerbal.stupidity;

            // Set traits to move the kerbal to the cryochamber.
            NewKerbal.UTaR = double.PositiveInfinity;
            NewKerbal.type = ProtoCrewMember.KerbalType.Unowned;
            
            if (ProtoCrewMember.RosterStatus.Missing == RemovedKerbal.rosterStatus
                && ProtoCrewMember.KerbalType.Crew == RemovedKerbal.type)
            {
                NewKerbal.type = ProtoCrewMember.KerbalType.Crew;
                NewKerbal.rosterStatus = ProtoCrewMember.RosterStatus.Dead;
            }
            else
            {
                NewKerbal.type = ProtoCrewMember.KerbalType.Unowned;
                NewKerbal.rosterStatus = ProtoCrewMember.RosterStatus.Available;
            }
            

            Debug.LogWarning("CRFFix.KerbalRemoved(): Kerbal rescued and sent to CryoStasis - " + NewKerbal.name);

        } // KerbalRemoved()

        public void ComplexEntry ()
        {
            // Build a list of the current kerbal names, so that any deleted kerbals can be restored on ComplexExit()
            // Debug.LogWarning ("CRFFix.ComplexEntry()");

            KerbalListUpdate();

            // Debug.LogWarning("CRFFix.ComplexEntry(): Kerbal List - " + KerbalList);

        } // ComplexEntry()

        public void KerbalListUpdate ()
        {
            for (int IndexKerbals = 0; IndexKerbals < HighLogic.CurrentGame.CrewRoster.Count; IndexKerbals++)
            {
                if (!(KerbalList.Contains(HighLogic.CurrentGame.CrewRoster[IndexKerbals].name)))
                {
                    KerbalList = String.Concat(KerbalList, HighLogic.CurrentGame.CrewRoster[IndexKerbals].name, ", ");
                }
            }
        } // KerbalListUpdate()

        public void TypeChange(ProtoCrewMember KerbalCrew, ProtoCrewMember.KerbalType TypeFrom, ProtoCrewMember.KerbalType TypeTo)
        {
            // Captures when Kerbals experience a TypeChange. If the TypeChange is from Crew to Applicant, the kerbal is being fired.
            // Upon firing, the kerbal is set to "Missing" to indicate that the kerbal was modified during this visit to the complex.
            // ComplexExit() can then search through the flags. If the type is from Applicant to Crew, the flag is set to "Available"
            // to ensure it doesn't get saved incorrectly if a kerbal was fired, then rehired.
            
            // Debug.LogWarning("CRFFix.TypeChange(): From - " + TypeFrom + " To - " + TypeTo);

            if (ProtoCrewMember.KerbalType.Crew == TypeFrom
                && ProtoCrewMember.KerbalType.Applicant == TypeTo)
            {
                // Debug.Log("CRFFix: Setting Kerbal Status to Missing");
                KerbalCrew.rosterStatus = ProtoCrewMember.RosterStatus.Missing;
            }
            else if (ProtoCrewMember.KerbalType.Applicant == TypeFrom
                && ProtoCrewMember.KerbalType.Crew == TypeTo)
            {
                // Debug.Log("CRFFix: Setting Kerbal Status to Available");
                KerbalCrew.rosterStatus = ProtoCrewMember.RosterStatus.Available;
            }
        } // TypeChange()

        public void ComplexExit()
        {
            // Grab the list of Applicants who are marked as "missing." This checks the UTaR date to see if it is
            // in the past or future. If it's in the past, then the kerbal was hired previously. If it's in the future,
            // the kerbal was hired and fired during the last Astronaut Complex visit. Only the Applicants who have a
            // UTaR in the past (previously hired) need to be archived into "Unowned" type.

            // Debug.LogWarning("CRFFix.ComplexExit(): Kerbal List - " + KerbalList);

            // This block acts as a final catch-all for any kerbals that disappeared from the roster without being noticed.
            // This KerbalNamesSplit was adapted from godarklight's DMP code with permission.
            if (!String.IsNullOrEmpty(KerbalList))
            {
                string[] KerbalNamesSplit = KerbalList.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string KerbalName in KerbalNamesSplit)
                {
                    AddKerbalToRoster(KerbalName);
                }
            }

            // Check the entire roster and adjust any fired kerbals
            for (int IndexKerbals = 0; IndexKerbals < HighLogic.CurrentGame.CrewRoster.Count; IndexKerbals++)
            {
                ProtoCrewMember CrewToCheck = HighLogic.CurrentGame.CrewRoster[IndexKerbals];

                // Grab the list of Applicants who are listed as Missing. These have been fired and captured via CRFFix.TypeChange().
                if (ProtoCrewMember.KerbalType.Applicant == CrewToCheck.type
                    && ProtoCrewMember.RosterStatus.Missing == CrewToCheck.rosterStatus)
                {
                    CrewToCheck.rosterStatus = ProtoCrewMember.RosterStatus.Available;

                    if (CrewToCheck.UTaR <= HighLogic.CurrentGame.UniversalTime)
                    {
                        //Debug.LogWarning("Applicant Name = " + CrewToCheck.name
                        //    + " || UTaR = " + CrewToCheck.UTaR
                        //    + " || Time = " + HighLogic.CurrentGame.UniversalTime
                        //    + " || Status = " + CrewToCheck.rosterStatus);

                        Debug.Log("CRFFix.ComplexExit(): Fired Kerbal Detected and set to Unowned");
                        CrewToCheck.type = ProtoCrewMember.KerbalType.Unowned;
                        CrewToCheck.UTaR = double.PositiveInfinity;
                    }
                }
                    // Check all the available crew's UTaRs. For any new hires set their hire time.
                else if (ProtoCrewMember.KerbalType.Crew == CrewToCheck.type
                    && ProtoCrewMember.RosterStatus.Available == CrewToCheck.rosterStatus)
                {
                    if (CrewToCheck.UTaR > HighLogic.CurrentGame.UniversalTime)
                    {
                        Debug.Log("CRFFix.ComplexExit(): Newly Hired Kerbal Detected");
                        CrewToCheck.UTaR = HighLogic.CurrentGame.UniversalTime;
                    }
                }
                    // Check for any unowned kerbals. These could be previously fired kerbals or 
                    // contract kerbals awaiting rescue.
                else if (ProtoCrewMember.KerbalType.Unowned == CrewToCheck.type)
                {
                    CrewToCheck.UTaR = double.PositiveInfinity;
                }
            }
        } // ComplexExit()

        public void AddKerbalToRoster (string KerbalName)
        {
            if (String.IsNullOrEmpty(KerbalName))
            {
                return;
            }

            if (!HighLogic.CurrentGame.CrewRoster.Exists(KerbalName))
            {
                ProtoCrewMember NewKerbal = HighLogic.CurrentGame.CrewRoster.GetNewKerbal(ProtoCrewMember.KerbalType.Unowned);
                if (null == NewKerbal)
                {
                    Debug.LogError("CRFFix.AddKerbalToRoster(): Failed To Add Kerbal (" + KerbalName + ")");
                    return;
                }
                NewKerbal.name = KerbalName;
                NewKerbal.type = ProtoCrewMember.KerbalType.Unowned;
                NewKerbal.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                NewKerbal.UTaR = double.PositiveInfinity;
                Debug.LogWarning("CRFFix: Lost Kerbal found and restored.");
            }
        } // AddKerbalToRoster()

        public void OnDestroy()
        {
            Debug.Log("CRFFix.OnDestroy()");

            if (true == Started)
            {
                GameEvents.onGUIAstronautComplexSpawn.Remove(new EventVoid.OnEvent(this.ComplexEntry));
                GameEvents.onGUIAstronautComplexDespawn.Remove(new EventVoid.OnEvent(this.ComplexExit));

                GameEvents.onKerbalTypeChange.Remove(TypeChange);
                GameEvents.onKerbalRemoved.Remove(KerbalRemoved);
                GameEvents.onKerbalAdded.Remove(KerbalAdded);

                Started = false;
            }
        } // Destroy()

    } // CRFFix

} // ClawKSP
