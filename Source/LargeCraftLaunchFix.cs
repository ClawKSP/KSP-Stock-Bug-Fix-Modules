using KSP;
using UnityEngine;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class LargeCraftLaunchFix : UnityEngine.MonoBehaviour
    {
        private bool UserSelection;
        private int CountdownTimer = 5;
        private bool isActive = false;

        public void Start()
        {
            //Debug.LogWarning("LargeCraftLaunchFix.Start()");
            GameEvents.onVesselGoOffRails.Add(OffRails);
        }

        public void OffRails (Vessel VesselToFix)
        {
            if (Vessel.Situations.PRELAUNCH == VesselToFix.situation)
            {
                isActive = true;
                UserSelection = HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities;
                //Debug.LogWarning("LargeCraftLaunchFix: UserSetting = " + UserSelection);
                HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = true;
            }
            else
            {
                this.isActive = false;
                isActive = false;
            }
        }

        public void FixedUpdate ()
        {
            //Debug.LogWarning("LargeCraftLaunchFix.FixedUpdate()");
            if (false == isActive)
            {
                return;
            }
            CountdownTimer--;
            //Debug.LogWarning("Countdown : " + CountdownTimer);

            if (CountdownTimer <= 0)
            {
                isActive = false;
                HighLogic.CurrentGame.Parameters.Difficulty.IndestructibleFacilities = UserSelection;
                Debug.Log("LargeCraftLaunchFix Deactivating");
            }
        }

        public void OnDestroy()
        {
            //Debug.LogWarning("LargeCraftLaunchFix.OnDestroy()");
            GameEvents.onVesselGoOffRails.Remove(OffRails);
        }
    }
}
