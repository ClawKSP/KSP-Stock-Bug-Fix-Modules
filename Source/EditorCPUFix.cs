/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * EditorCPUFix - Written for KSP v1.1.2
 * 
 * Deactivates VAB and SPH lighting, ground crew, and vehicles.
 * 
 * Change Log:
 * - v00.01  (30 May 16)   Initial Release
 * 
 */

using UnityEngine;
using KSP;
using System.Collections;
using KSP.UI.Screens;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class EditorCPUFix : MonoBehaviour
    {
        public void Start ()
        {
            Debug.Log("EditorCPUFix.Start(): v00.02");
            StartCoroutine(DisableDoodads());
        }


        IEnumerator DisableDoodads()
        {
            GameObject editorObject = null;
            //string crewName = "VABCrew";

            while (EditorDriver.fetch == null)
                yield return null;

            //if (EditorDriver.editorFacility == EditorFacility.SPH)
            //    crewName = "SPHCrew";

            //if (GameSettings.SHOW_SPACE_CENTER_CREW)
            //{
            //    while (editorObject == null)
            //    {
            //        yield return null;
            //        editorObject = GameObject.Find(crewName);
            //    }
            //    editorObject = GameObject.Find(crewName);
            //    if (editorObject) editorObject.SetActive(false);
            //}

            editorObject = null;
            while (editorObject == null)
            {
                yield return null;
                editorObject = GameObject.Find("model_props");
            }
            editorObject.SetActive(false);

            if (EditorDriver.editorFacility == EditorFacility.SPH)
            {
                editorObject = GameObject.Find("model_sph_interior_lights_v16");
                if (editorObject != null) editorObject.SetActive(GameSettings.SHOW_SPACE_CENTER_CREW);
                //editorObject = GameObject.Find("Lighting_Baked");
                //if (editorObject != null) editorObject.SetActive(GameSettings.SHOW_SPACE_CENTER_CREW);
            }
            else // VAB
            {
                editorObject = GameObject.Find("model_vab_prop_truck_01");
                if (editorObject != null) editorObject.SetActive(GameSettings.SHOW_SPACE_CENTER_CREW);
                editorObject = GameObject.Find("model_vab_elevators");
                if (editorObject != null) editorObject.SetActive(GameSettings.SHOW_SPACE_CENTER_CREW);
                //editorObject = GameObject.Find("VAB_Interior_BakeLights");
                //if (editorObject != null) editorObject.SetActive(GameSettings.SHOW_SPACE_CENTER_CREW);
                editorObject = GameObject.Find("model_vab_interior_lights_flood_v16");
                if (editorObject != null) editorObject.SetActive(GameSettings.SHOW_SPACE_CENTER_CREW);
            }

        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class EditorCrashFix : PartModule
    {
        private StageIcon stageIcon;

        public void Start()
        {
            if (!(HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)) return;

            //Debug.Log("EditorCrashFix.Start(): v00.01");

            if (part != null && part.stackIcon != null && part.stackIcon.StageIcon != null && part.stackIcon.StageIcon.gameObject != null)
                stageIcon = part.stackIcon.StageIcon;
        }

        public void OnDestroy()
        {
            if (!(HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)) return;

            if (stageIcon != null && stageIcon.gameObject != null)
                stageIcon.gameObject.SetActive(false);
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class EditorCrashHook : MonoBehaviour
    {
        private static bool modulesHooked = false;

        public void Start()
        {
            if (modulesHooked) return;
            modulesHooked = true;

            Debug.LogWarning("EditorCrashHook.Start(): v00.01");

            AvailablePart currentAP;

            for (int indexParts = 0; indexParts < PartLoader.LoadedPartsList.Count; indexParts++)
            {
                currentAP = PartLoader.LoadedPartsList[indexParts];
                if (currentAP != null && currentAP.partPrefab != null && currentAP.partPrefab.Modules != null && currentAP.partPrefab.Modules.Count > 0)
                {
                    currentAP.partPrefab.AddModule("EditorCrashFix");
                }
            }
        }
    }
}
