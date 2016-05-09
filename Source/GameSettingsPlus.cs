/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * GameSettingsPlus - Written for KSP v1.1.2
 * 
 * - Provides GUI Access to some of the stock settings in settings.cfg
 * 
 * Change Log:
 * - v01.01  ( 8 May 16)    Updated for KSP v1.1.2
 * - v01.00  (21 Apr 16)    Initial release for KSP v1.1.0
 * 
 */

using UnityEngine;
using KSP;
using KSP.UI;

namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class GameSettingsPlus : MonoBehaviour
    {
        //private float inFlightUIScale = 0f; // tracks the current scale of the StockPlus flight UI. Set to zero to force an update in the first frame
        //private float overrideUIScale = 0f; // tracks the current scale of the StockPlus global UI. Set to zero to force an update in the first frame
        private int saveBackups = 10;
        private float saveInterval = 300f;
        private int conicPatchLimit = 0;
        private PatchRendering.RelativityMode conicPatchMode;
        private float wheelClipOffset = 0f;
        private float wheelClipRange = 1f;
        private float wheelClipMultiplier = 1.05f;

        private bool changesDetected = false;
        private static Rect settingsWindowPos = new Rect(370, 20, 300, 300);
        private static Vector2 settingsScrollPos = Vector2.zero;

        public void Start()
        {
            Debug.Log("GameSettingsPlus.Start(): v1.01");

            //if (StockBugFixPlusController.plusActive && StockBugFixPlusController.gameSettingsPlus)
            //{
            //    if (StockBugFixPlusController.overrideUIScalePlus)
            //    {
            //        GameSettings.UI_SCALE = StockBugFixPlusController.overrideUIScale;
            //    }
            //    else
            //    {
            //        StockBugFixPlusController.overrideUIScale = GameSettings.UI_SCALE;
            //    }

            //    inFlightUIScale = StockBugFixPlusController.inFlightUIScale;
            //    overrideUIScale = StockBugFixPlusController.overrideUIScale;
            //    UpdateUIScale();
            //}
            //else
            //{
            //    inFlightUIScale = GameSettings.UI_SCALE;
            //    overrideUIScale = GameSettings.UI_SCALE;
            //    StockBugFixPlusController.inFlightUIScale = GameSettings.UI_SCALE;
            //    StockBugFixPlusController.overrideUIScale = GameSettings.UI_SCALE;
            //}

            saveInterval = GameSettings.AUTOSAVE_INTERVAL;
            saveBackups = GameSettings.SAVE_BACKUPS;

            conicPatchLimit = GameSettings.CONIC_PATCH_LIMIT;
            conicPatchMode = (PatchRendering.RelativityMode) GameSettings.CONIC_PATCH_DRAW_MODE;

            wheelClipMultiplier = GameSettings.WHEEL_CLIP_MULTIPLIER;
            wheelClipOffset = GameSettings.WHEEL_CLIP_OFFSET;
            wheelClipRange = GameSettings.WHEEL_CLIP_RANGE;

            //setUIScale = UIMasterController.Instance.uiScale;
        }

        public void Update()
        {
            //Debug.LogWarning("GameSettingsPlus.Update()");
        }

        private void UpdateUIScale()
        {
            UIMasterController.Instance.uiScale = 0;

            if (StockBugFixPlusController.overrideUIScalePlus)
                UIMasterController.Instance.SetScale(StockBugFixPlusController.overrideUIScale);
            else
                UIMasterController.Instance.SetScale(GameSettings.UI_SCALE);

            if (HighLogic.LoadedScene == GameScenes.FLIGHT && StockBugFixPlusController.inFlightUIScalePlus)
            {
                UIMasterController.Instance.mainCanvas.scaleFactor = StockBugFixPlusController.inFlightUIScale;
                UIMasterController.Instance.appCanvas.scaleFactor = StockBugFixPlusController.inFlightUIScale;
            }
            Canvas.ForceUpdateCanvases();
        }

        public void OnGUI()
        {
            if (StockBugFixPlusController.SettingsIsOpen())
            {
                GUI.skin = null;

                StockBugFixPlusController.SetInfoWindowPos(GUILayout.Window(1000002, StockBugFixPlusController.InfoWindowPos(),
                    DrawSettingsPlus, "Game Settings Plus Configuration", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)));
            }
        }


        public void DrawSettingsPlus(int id)
        {
            string buttonText;
            Color currentColor = GUI.color;

            #region Admin Buttons
            if (GUILayout.Button("Save & Close"))
            {
                SaveGameSettings();
                StockBugFixPlusController.CloseSettings();
            }

            if (GUILayout.Button("Cancel"))
            {
                StockBugFixPlusController.CloseSettings();
            }

            if (changesDetected)
            {
                GUI.color = Color.red;
                GUILayout.Label("   *****   Save Required   *****");
                GUI.color = currentColor;
            }
            else
                GUILayout.Label("");
            #endregion

            settingsScrollPos = GUILayout.BeginScrollView(settingsScrollPos);

            #region In Flight UI Scale
            //// Begin Toggle Line
            //GUILayout.BeginVertical();
            //GUILayout.BeginHorizontal();
            //buttonText = "In Flight UI Scale: ";
            //if (StockBugFixPlusController.inFlightUIScalePlus)
            //{
            //    buttonText += "Active";
            //    GUI.color = Color.green;
            //}
            //else
            //{
            //    buttonText += "Disabled";
            //    GUI.color = Color.yellow;
            //}
            //if (GUILayout.Button(buttonText))
            //{
            //    StockBugFixPlusController.inFlightUIScalePlus = !StockBugFixPlusController.inFlightUIScalePlus;
            //    UpdateUIScale();
            //    changesDetected = true;
            //}
            //GUI.color = currentColor;
            //GUILayout.EndHorizontal();
            //GUILayout.EndVertical();
            //// End Toggle Line

            //GUILayout.BeginVertical();
            //GUILayout.Label("In Flight UI Scale " + inFlightUIScale.ToString("0.00"));
            //inFlightUIScale = GUILayout.HorizontalSlider(inFlightUIScale, 0.5f, 1.5f);
            //inFlightUIScale = Mathf.Round(inFlightUIScale * 100f) / 100f;
            //if (inFlightUIScale != StockBugFixPlusController.inFlightUIScale)
            //{
            //    StockBugFixPlusController.inFlightUIScale = inFlightUIScale;
            //    UpdateUIScale();
            //    changesDetected = true;
            //}
            //GUILayout.EndVertical();
            #endregion
            //GUILayout.Label("");

            #region Override UI Scale
            //// Begin Toggle Line
            //GUILayout.BeginVertical();
            //GUILayout.BeginHorizontal();
            //buttonText = "Override UI Scale: ";
            //if (StockBugFixPlusController.overrideUIScalePlus)
            //{
            //    buttonText += "Active";
            //    GUI.color = Color.green;
            //}
            //else
            //{
            //    buttonText += "Disabled";
            //    GUI.color = Color.yellow;
            //}
            //if (GUILayout.Button(buttonText))
            //{
            //    StockBugFixPlusController.overrideUIScalePlus = !StockBugFixPlusController.overrideUIScalePlus;
            //    UpdateUIScale();
            //    changesDetected = true;
            //}
            //GUI.color = currentColor;
            //GUILayout.EndHorizontal();
            //GUILayout.EndVertical();
            //// End Toggle Line

            //GUILayout.BeginVertical();

            //GUILayout.Label("Override UI Scale " + overrideUIScale.ToString("0.00"));
            //overrideUIScale = GUILayout.HorizontalSlider(overrideUIScale, 0.5f, 2.0f);
            //overrideUIScale = Mathf.Round(overrideUIScale * 100f) / 100f;
            //if (overrideUIScale != StockBugFixPlusController.overrideUIScale)
            //{
            //    StockBugFixPlusController.overrideUIScale = overrideUIScale;
            //    UpdateUIScale();
            //    changesDetected = true;
            //}
            //GUILayout.EndVertical();
            #endregion
            //GUILayout.Label("");

            #region In Flight Highlight
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            buttonText = "In Flight Highlight: ";
            if (GameSettings.INFLIGHT_HIGHLIGHT)
            {
                buttonText += "Active";
                GUI.color = Color.green;
            }
            else
            {
                buttonText += "Disabled";
                GUI.color = Color.yellow;
            }
            if (GUILayout.Button(buttonText))
            {
                GameSettings.INFLIGHT_HIGHLIGHT = !GameSettings.INFLIGHT_HIGHLIGHT;
                changesDetected = true;
            }
            GUI.color = currentColor;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            // End Toggle Line 
            #endregion
            GUILayout.Label("");

            #region Save Backup Count
            //GUILayout.BeginVertical();
            //GUILayout.Label("Save Backup Count: " + saveBackups);
            //saveBackups = (int)Mathf.Round(GUILayout.HorizontalSlider(saveBackups, 0f, 10f));
            //if (saveBackups != GameSettings.SAVE_BACKUPS)
            //{
            //    changesDetected = true;
            //}
            //GUILayout.EndVertical();
            //// End Toggle Line 
            #endregion
            //GUILayout.Label("");

            #region Save Interval
            GUILayout.BeginVertical();
            GUILayout.Label("Autosave Interval (minutes): " + (saveInterval / 60f).ToString("0.0"));
            saveInterval = (int)Mathf.Round(GUILayout.HorizontalSlider(saveInterval, 0f, 10 * 60f));
            if (saveInterval != GameSettings.AUTOSAVE_INTERVAL)
            {
                changesDetected = true;
            }
            GUILayout.EndVertical();
            // End Toggle Line 
            #endregion
            GUILayout.Label("");

            #region Save Backups
            GUILayout.BeginVertical();
            GUILayout.Label("Save Backup Count: " + saveBackups.ToString("0"));
            saveBackups = (int)Mathf.Round(GUILayout.HorizontalSlider(saveBackups, 0f, 30f));
            if (saveBackups != GameSettings.SAVE_BACKUPS)
            {
                changesDetected = true;
            }
            GUILayout.EndVertical();
            // End Toggle Line 
            #endregion
            GUILayout.Label("");

            #region Conic Patch Limit
            GUILayout.BeginVertical();
            GUILayout.Label("Conic Patch Limit: " + conicPatchLimit);
            conicPatchLimit = (int)Mathf.Round(GUILayout.HorizontalSlider(conicPatchLimit, 1f, 6f));
            if (conicPatchLimit != GameSettings.CONIC_PATCH_LIMIT)
            {
                changesDetected = true;
                StockBugFixPlusController.SettingsChanged(false, true);
            }
            GUILayout.EndVertical();
            // End Toggle Line 
            #endregion
            GUILayout.Label("");

            #region Conic Patch Mode
            GUILayout.BeginVertical();
            if (GUILayout.Button("Conic Patch Mode:\n" + conicPatchMode))
            {
                if (conicPatchMode == PatchRendering.RelativityMode.RELATIVE) conicPatchMode = PatchRendering.RelativityMode.LOCAL_TO_BODIES;
                else if (conicPatchMode == PatchRendering.RelativityMode.LOCAL_TO_BODIES) conicPatchMode = PatchRendering.RelativityMode.LOCAL_AT_SOI_EXIT_UT;
                else if (conicPatchMode == PatchRendering.RelativityMode.LOCAL_AT_SOI_EXIT_UT) conicPatchMode = PatchRendering.RelativityMode.LOCAL_AT_SOI_ENTRY_UT;
                else if (conicPatchMode == PatchRendering.RelativityMode.LOCAL_AT_SOI_ENTRY_UT) conicPatchMode = PatchRendering.RelativityMode.DYNAMIC;
                else conicPatchMode = PatchRendering.RelativityMode.RELATIVE;
                changesDetected = true;
                StockBugFixPlusController.SettingsChanged(false, true);
            }
            GUILayout.EndVertical();
            // End Toggle Line 
            #endregion
            GUILayout.Label("");

            #region Wheel Clipping
            GUILayout.BeginVertical();

            GUILayout.Label("Wheel Clip Offset: " + wheelClipOffset.ToString("0.00"));
            wheelClipOffset = GUILayout.HorizontalSlider(wheelClipOffset, 0f, 2f);
            if (wheelClipOffset != GameSettings.WHEEL_CLIP_OFFSET)
            {
                changesDetected = true;
                StockBugFixPlusController.SettingsChanged(false, true);
            }
            GUILayout.Label("Wheel Clip Range: " + wheelClipRange.ToString("0.00"));
            wheelClipRange = GUILayout.HorizontalSlider(wheelClipRange, 0.5f, 2f);
            if (wheelClipRange != GameSettings.WHEEL_CLIP_RANGE)
            {
                changesDetected = true;
                StockBugFixPlusController.SettingsChanged(false, true);
            }
            GUILayout.Label("Wheel Clip Multiplier: " + wheelClipMultiplier.ToString("0.00"));
            wheelClipMultiplier = GUILayout.HorizontalSlider(wheelClipMultiplier, 0.5f, 2f);
            if (wheelClipMultiplier != GameSettings.WHEEL_CLIP_MULTIPLIER)
            {
                changesDetected = true;
                StockBugFixPlusController.SettingsChanged(false, true);
            }

            GUILayout.EndVertical();
            // End Toggle Line 
            #endregion
            GUILayout.Label("");

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void SaveGameSettings()
        {
            //GameSettings.UI_SCALE = overrideUIScale;
            GameSettings.AUTOSAVE_INTERVAL = saveInterval;
            GameSettings.SAVE_BACKUPS = saveBackups;

            GameSettings.CONIC_PATCH_LIMIT = conicPatchLimit;
            GameSettings.CONIC_PATCH_DRAW_MODE = (int)conicPatchMode;

            GameSettings.WHEEL_CLIP_OFFSET = wheelClipOffset;
            GameSettings.WHEEL_CLIP_RANGE = wheelClipRange;
            GameSettings.WHEEL_CLIP_MULTIPLIER = wheelClipMultiplier;

            GameSettings.SaveSettings();

            StockBugFixPlusController.RemoteSave();
        }

    }
}