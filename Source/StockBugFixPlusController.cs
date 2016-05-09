/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * StockPlusController - Written for KSP v1.1.0
 * 
 * - Enables StockPlus features
 * 
 * Change Log:
 * - v00.14  ( 8 May 16)    Updated for KSP v1.1.2
 * - v00.13  (21 Apr 16)    Updated for KSP v1.1.0
 * - v00.12  (27 Dec 15)    Added support for user configurable gimbal defaults
 * - v00.11  (14 Nov 15)    Fixed a bug loading fixes multiple times, due to Part.AddModule not adding the moduleName correctly
 * - v00.10  (10 Nov 15)    Rewritten to support in-game configuration (includes GUI), Renamed from StockPlusController to StockBugFixPlusController.
 *                          Supports persistent configuration across version updates.
 * - v00.03  ( 1 Jul 15)    Recompiled for KSP v1.0.4
 * - v00.02  ( 1 Jun 15)    Minor bug fix
 * - v00.01  ( 8 May 15)    Initial Release
 * 
 */

using System;
using System.Collections;
using UnityEngine;
using KSP;
using System.IO;
using System.Reflection;
using System.Globalization;


namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class StockBugFixPlusController : MonoBehaviour
    {
        public string configFilePath;

        public static bool plusActive = false;

        private static bool GUIActive = false;
        private static bool infoActive = false;
        private static bool changesDetected = false;
        private static bool sceneChangeRequired = false;
        private static bool remoteSave = false;
        private static string infoText;
        private static Vector2 infoScrollPos = Vector2.zero;

        public static bool userConfigured = false;
        public static int windowPosX = 20;
        public static int windowPosY = 20;
        private static Vector2 scrollPos = Vector2.zero;

        public static bool gameSettingsPlus = false;
        public static bool inFlightUIScalePlus = false;
        public static float inFlightUIScale = 1.0f;
        public static bool overrideUIScalePlus = false;
        public static float overrideUIScale = 1.0f;
        private static bool settingsOpen = false;

        //public static bool inFlightHighlightOff = false;
        public static bool editorSymmetryHighlight = false;
        //public static bool aeroSurfacePlus = false;
        public static bool controlSurfacePlus = false;
        public static bool gimbalPlus = false;
        public static bool gimbalRateIsActive = false;
        public static float gimbalResponseSpeed = 10f;
        public static bool parachutePlus = false;
        public static bool pilotRSASPlus = false;
        //public static bool proceduralFairingPlus = false;
        public static bool wheelPlus = false;
        
        private static Rect plusWindowPos = new Rect(20, 20, 350, 320);
        private static Rect infoWindowPos = new Rect(400, 20, 300, 300);

        public static Rect InfoWindowPos ()
        {
            return infoWindowPos;
        }

        public static void SetInfoWindowPos (Rect setInfo)
        {
            infoWindowPos = setInfo;
        }

        public static void SettingsChanged(bool changed = true, bool sceneChangeIsRequired = false)
        {
            changesDetected |= changed;
            sceneChangeRequired |= sceneChangeIsRequired;
        }

        public void Awake()
        {
        }

        public void Start()
        {
            if (HighLogic.LoadedScene == GameScenes.LOADING && !GUIActive)
            {
                LoadSettings();
            }

            plusWindowPos.x = windowPosX;
            plusWindowPos.y = windowPosY;
            sceneChangeRequired = false;
            remoteSave = false;

            if (!userConfigured)
            {
                GUIActive = true;
            }

            Debug.Log("StockBugFixPlusController.Start(): v00.14");
        }

        public void Update()
        {
            if (GameSettings.MODIFIER_KEY.GetKey() && Input.GetKeyDown(KeyCode.F8))
            {
                GUIActive = !GUIActive;
                CloseSettings();
            }
        }

        public static void CloseSettings()
        {
            settingsOpen = false;
        }

        public static bool SettingsIsOpen()
        {
            return settingsOpen;
        }

        public void OnGUI()
        {
            if (GUIActive)
            {
                GUI.skin = null;
                plusWindowPos = GUILayout.Window(1000001, plusWindowPos, DrawGUI, "Stock Plus Configuration", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                windowPosX = (int)plusWindowPos.x;
                windowPosY = (int)plusWindowPos.y;

                if (infoActive)
                {
                    infoWindowPos = GUILayout.Window(1000002, infoWindowPos, DrawInfo, "Stock Plus Info", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                }

                if (remoteSave)
                {
                    remoteSave = false;
                    userConfigured = true;
                    changesDetected = false;
                    SaveSettings();
                }
            }
        }

        public void DrawGUI(int id)
        {
            string buttonText;
            Color currentColor = GUI.color;

            #region Admin Buttons
            
            GUILayout.BeginVertical();
            if (!userConfigured)
            {
                GUILayout.Label("Initial Setup Detected: Please Configure and Save Settings. MOD+F8 will activate this GUI.");
            }
            else if (changesDetected)
            {
                GUILayout.Label("Settings have changed. Please click Save to keep them, or Load to restore.");
            }
            else
            {
                GUILayout.Label("Changes to settings may require a scene change to properly activate.");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            if (changesDetected)
            {
                if (GUILayout.Button("Load Settings"))
                {
                    LoadSettings();
                }
                if (GUILayout.Button("Save Settings"))
                {
                    userConfigured = true;
                    changesDetected = false;
                    SaveSettings();
                }
            }
            if (GUILayout.Button("Close"))
            {
                settingsOpen = false;
                GUIActive = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion

            #region plusActive
            // Begin Toggle Line
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            buttonText = "Stock Plus Master: ";
            if (plusActive)
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
                plusActive = !plusActive;
                changesDetected = true;
                sceneChangeRequired = true;
            }
            GUI.color = currentColor;
            GUI.color = currentColor;
            if (GUILayout.Button("Info", GUILayout.Width(35)))
            {
                InfoToggle(true, "Stock Plus Master \n\nDescription: Allows disabling all StockPlus components. The intent of StockPlus is to be a series of modlets that add small features to Stock without changing the overall feel of the game. StockPlus can be disabled or uninstalled at any time, and the save will still be stock compatible. Some of the selections below can toggled real-time, while others require a scene change to reflect any setting changes. Please see the Info button for specifics on each StockPlus module. \n\nActive: Unlocks StockPlus modules. \nDisabled: Disables all StockPlus modules, but does not disable StockBugFixes. \n\nRequires a scene change to activate. \n\nTo disable a StockBugFix, delete the corresponding .dll file in the GameData\\StockBugFixPlus directory.");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            // End Toggle Line 
            #endregion

            #region Change Required
            GUILayout.BeginHorizontal();
            if (sceneChangeRequired)
            {
                GUI.color = Color.red;
                GUILayout.Label("   *****   Scene Change Required   *****");
                GUI.color = currentColor;
            }
            else
            {
                GUILayout.Label(" ");
            }
            GUILayout.EndHorizontal(); 
            #endregion

            if (plusActive)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos);

                #region GameSettingsPlus
                // Begin Toggle Line
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                buttonText = "Game Settings Plus: ";
                if (gameSettingsPlus)
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
                    gameSettingsPlus = !gameSettingsPlus;
                    changesDetected = true;
                }
                GUI.color = currentColor;
                if (GUILayout.Button((gameSettingsPlus ? "Set." : "Info"), GUILayout.Width(35)))
                {
                    if (!gameSettingsPlus)
                    {
                        InfoToggle(true, "Description: Provides an interface to several of the game's settings that are unavailable in the Main Settings Menu. /n/nYou must save the settings from the settings menu for them to persist. /n/nSome settings require a scene change to activate.");
                        settingsOpen = false;
                    }
                    else
                    {
                        InfoToggle(false, "");
                        settingsOpen = true;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                // End Toggle Line 
                #endregion

                #region editorSymmetryHighlight
                //// Begin Toggle Line
                //GUILayout.BeginVertical();
                //GUILayout.BeginHorizontal();
                //buttonText = "Editor Symmetry Highlight: ";
                //if (editorSymmetryHighlight)
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
                //    editorSymmetryHighlight = !editorSymmetryHighlight;
                //    changesDetected = true;
                //}
                //GUI.color = currentColor;
                //if (GUILayout.Button("Info", GUILayout.Width(35)))
                //{
                //    InfoToggle(true, "Editor Symmetry Highlight \n\nDescription: Activates additional highlighting in the editors, which highlights symmetric partners in blue, and primary part branches as green (when hovering over with the mouse). \n\nWhen Active: Highlighting is turned on. \nWhen Disabled: Highlighting is stock standard. \n\nThis can be toggled this while in the editor.");
                //}
                //GUILayout.EndHorizontal();
                //GUILayout.EndVertical();
                //// End Toggle Line 
                #endregion

                #region controlSurfacePlus
                // Begin Toggle Line
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                buttonText = "Control Surface Plus: ";
                if (controlSurfacePlus)
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
                    controlSurfacePlus = !controlSurfacePlus;
                    changesDetected = true;
                    sceneChangeRequired = true;
                }
                GUI.color = currentColor;
                if (GUILayout.Button("Info", GUILayout.Width(35)))
                {
                    InfoToggle(true, "ControlSurface Plus \n\nDescription: Adds a tweakable to flight controls that allows deployment by individual control surface.");
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                // End Toggle Line 
                #endregion

                #region gimbalPlus
                // Begin Toggle Line
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                buttonText = "Gimbal Plus: ";
                if (gimbalPlus)
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
                    gimbalPlus = !gimbalPlus;
                    changesDetected = true;
                    sceneChangeRequired = true;
                }
                GUI.color = currentColor;
                if (GUILayout.Button("Info", GUILayout.Width(35)))
                {
                    InfoToggle(true, "Gimbal Plus \n\nDescription: Adds tweakable engine gimbal response speed. \n\nRequires a scene change to activate. \n\nDefaults are configurable by editing StockBugFixPlusController.cfg");
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                // End Toggle Line 
                #endregion

                #region parachutePlus
                // Begin Toggle Line
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                buttonText = "Parachute Plus: ";
                if (parachutePlus)
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
                    parachutePlus = !parachutePlus;
                    changesDetected = true;
                    sceneChangeRequired = true;
                }
                GUI.color = currentColor;
                if (GUILayout.Button("Info", GUILayout.Width(35)))
                {
                    InfoToggle(true, "Parachute Plus \n\nDescription: Adds tweakable semi-deploy and deploy speeds, and tweakable parachute spread. \n\nRequires a scene change to activate.");
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                // End Toggle Line 
                #endregion

                #region pilotRSASPlus
                // Begin Toggle Line
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                buttonText = "PilotRSAS Plus: ";
                if (pilotRSASPlus)
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
                    pilotRSASPlus = !pilotRSASPlus;
                    changesDetected = true;
                    sceneChangeRequired = true;
                }
                GUI.color = currentColor;
                if (GUILayout.Button("Info", GUILayout.Width(35)))
                {
                    InfoToggle(true, "PilotRSAS Plus \n\nDescription: Activates tweakable SAS values (available on pods and probe cores) for the advanced auto-pilot functions. \n\nWhen Active: PilotRSAS tweakables are available. \nWhen Disabled: PilotRSASFix default values are used. \n\nRequires a scene change to activate. \n\nRequires PilotRSASFix installed to work.");
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                // End Toggle Line 
                #endregion

                #region wheelPlus
                //// Begin Toggle Line
                //GUILayout.BeginVertical();
                //GUILayout.BeginHorizontal();
                //buttonText = "Wheel Plus: ";
                //if (wheelPlus)
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
                //    wheelPlus = !wheelPlus;
                //    changesDetected = true;
                //    sceneChangeRequired = true;
                //}
                //GUI.color = currentColor;
                //if (GUILayout.Button("Info", GUILayout.Width(35)))
                //{
                //    InfoToggle(true, "Wheel Plus \n\nDescription: Adds tweakable grip multiplier, which allows for differential traction. \n\nRequires a scene change to activate.");
                //}
                //GUILayout.EndHorizontal();
                //GUILayout.EndVertical();
                //// End Toggle Line 
                #endregion 

                #region Legacy
                //#region inFlightHighlightOff
                //// Begin Toggle Line
                //GUILayout.BeginVertical();
                //GUILayout.BeginHorizontal();
                //buttonText = "In-Flight Highlight Disabler: ";
                //if (inFlightHighlightOff)
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
                //    inFlightHighlightOff = !inFlightHighlightOff;
                //    changesDetected = true;
                //}
                //GUI.color = currentColor;
                //if (GUILayout.Button("Info", GUILayout.Width(35)))
                //{
                //    InfoToggle(true, "In-Flight Highlight Disabler \n\nDescription: Turns off the green shader while in the flight scene. \n\nWhen Active: Disables the green shader. \nWhen Disabled: In-flight highlighting is stock standard.");
                //}
                //GUILayout.EndHorizontal();
                //GUILayout.EndVertical();
                //// End Toggle Line 
                //#endregion

                //#region aeroSurfacePlus
                //// Begin Toggle Line
                //GUILayout.BeginVertical();
                //GUILayout.BeginHorizontal();
                //buttonText = "AeroSurface Plus: ";
                //if (aeroSurfacePlus)
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
                //    aeroSurfacePlus = !aeroSurfacePlus;
                //    changesDetected = true;
                //    sceneChangeRequired = true;
                //}
                //GUI.color = currentColor;
                //if (GUILayout.Button("Info", GUILayout.Width(35)))
                //{
                //    InfoToggle(true, "AeroSurface Plus \n\nDescription: Disables off-axis drag when airbrake is stowed. Locks airbrakes in space. \n\nRequires a scene change to activate.");
                //}
                //GUILayout.EndHorizontal();
                //GUILayout.EndVertical();
                //// End Toggle Line 
                //#endregion

                //#region proceduralFairingPlus
                //// Begin Toggle Line
                //GUILayout.BeginVertical();
                //GUILayout.BeginHorizontal();
                //buttonText = "Procedural Fairing Plus: ";
                //if (proceduralFairingPlus)
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
                //    proceduralFairingPlus = !proceduralFairingPlus;
                //    changesDetected = true;
                //    sceneChangeRequired = true;
                //}
                //GUI.color = currentColor;
                //if (GUILayout.Button("Info", GUILayout.Width(35)))
                //{
                //    InfoToggle(true, "Procedural Fairing Plus \n\nDescription: Activates tweakable panel counts and decoupling force. \n\nRequires a scene change to activate.");
                //}
                //GUILayout.EndHorizontal();
                //GUILayout.EndVertical();
                //// End Toggle Line 
                //#endregion
                #endregion

                GUILayout.EndScrollView();
                GUI.DragWindow();

            }
        }

        private void InfoToggle (bool isInfoActive, string infoTextInput)
        {
            infoText = infoTextInput;
            infoScrollPos = Vector2.zero;
            infoActive = isInfoActive;
            if (settingsOpen) settingsOpen = false;
        }

        public void DrawInfo(int id)
        {
            infoScrollPos = GUILayout.BeginScrollView(infoScrollPos);

            if (GUILayout.Button("Close"))
            {
                InfoToggle(false, "");
            }
            GUILayout.Label(infoText);

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        public static void RemoteSave()
        {
            remoteSave = true;
        }

        private void SaveSettings()
        {

            ConfigNode CN = new ConfigNode();
            FieldInfo[] FI = this.GetType().GetFields();

            for (int indexFields = 0; indexFields < FI.Length; indexFields++)
            {
                FieldInfo currentFI = FI[indexFields];

                if (currentFI.IsPublic && currentFI.IsStatic)
                {
                    IConfigNode tempCN = currentFI.GetValue(this) as IConfigNode;

                    if (null != tempCN)
                    {
                        tempCN.Save(CN.AddNode(currentFI.Name));
                    }
                    else
                    {
                        CN.AddValue(currentFI.Name, Convert.ToString(currentFI.GetValue(this)));
                    }
                }
            }

            string filePath = Path.ChangeExtension(typeof(StockBugFixPlusController).Assembly.Location, ".cfg");
            Debug.Log("Saving StockBugFix and StockPlus Settings: " + filePath);
            CN.Save(filePath, "StockBugFix and StockPlus Settings");
        }

        private void LoadSettings()
        {
            string filePath = Path.ChangeExtension(typeof(StockBugFixPlusController).Assembly.Location, ".cfg");
            ConfigNode CN = ConfigNode.Load(filePath);

            if (null == CN)
            {
                Debug.Log("Generating Default Configuration for StockBugFix / StockPlus.");
                SaveSettings();
                return;
            }
            foreach (ConfigNode.Value value in CN.values)
            {
                FieldInfo FI = this.GetType().GetField(value.name);
                if (FI != null)
                {
                    if (FI.FieldType == typeof(string))
                    {
                        FI.SetValue(this, value.value);
                    }
                    else if (FI.FieldType == typeof(int))
                    {
                        FI.SetValue(this, int.Parse(value.value, CultureInfo.InvariantCulture));
                    }
                    else if (FI.FieldType == typeof(bool))
                    {
                        FI.SetValue(this, bool.Parse(value.value));
                    }
                    else if (FI.FieldType == typeof(float))
                    {
                        FI.SetValue(this, float.Parse(value.value, CultureInfo.InvariantCulture));
                    }
                }
            }

            infoWindowPos.x = windowPosX + 360;
            if ((infoWindowPos.x + 320) > Screen.width) { infoWindowPos.x = windowPosX - 360; }
            infoWindowPos.y = windowPosY;
        }

        public static void HookModule(string targetModule, string attachingModule)
        {
            for (int indexParts = 0; indexParts < PartLoader.LoadedPartsList.Count; indexParts++)
            {
                AvailablePart currentAP = PartLoader.LoadedPartsList[indexParts];
                Part currentPart = currentAP.partPrefab;

                for (int indexModules = 0; indexModules < currentPart.Modules.Count; indexModules++)
                {
                    // did we find the target module?
                    if (targetModule == currentPart.Modules[indexModules].moduleName)
                    {
                        // does this part already contain a copy of the attaching module?
                        //if (!currentPart.Modules.Contains(attachingModule))
                        if (!ModuleAttached(currentPart, attachingModule))
                        {
                            Debug.Log(targetModule + " found. Attaching " + attachingModule + ".");
                            PartModule newModule = currentPart.AddModule(attachingModule);
                            if (null == newModule)
                            {
                                Debug.LogError(attachingModule + " attachment failed.");
                            }
                            newModule.moduleName = attachingModule;
                        }
                        break; // If found, move to the next part.
                    }
                }
            }
        }

        private static bool ModuleAttached (Part part, string moduleName)
        {
            for (int indexModules = 0; indexModules < part.Modules.Count; indexModules++)
            {
                //Debug.LogWarning("Name: " + part.Modules[indexModules].moduleName);
                // did we find the module?
                if (moduleName == part.Modules[indexModules].moduleName)
                {
                    return (true);
                }
            }
            return (false);
        }
    }
}
