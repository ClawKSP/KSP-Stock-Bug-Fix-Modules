KSP-Stock-Bug-Fix-Modules
=========================

Stand alone fixes for common stock KSP bugs. These modules are meant to be fully stock compatible. As always, my aim is to be able to remove them from the game at any time without causing a problem for the stock saves.

StockPlus: The StockPlus additions are all minor polish fixes or add-ons that unlock stock features. The stock bug fix modules now incorporate StockPlus additions. These additions are disabled by default, and can be unlocked by following the StockPlus Instructions below.


Installation
============
Please delete any old installations of KSP-Stock-Bug-Fix-Modules before installing.

Unzip the zip file to the KSP/GameData directory. ModuleManager.X.X.X.dll and StockPlusController.dll should be in the root GameData directory next to a StockBugFixModules folder.

The fixes are all stand alone. So if there are any that you don't want, just delete the directory inside the StockBugFixModules folder. For example: If you don't want to use "ModuleParachuteFix," delete that directory.

IMPORTANT: Do NOT delete StockPlusController.dll. You can delete any of the sub-folders, but must have StockPlusController.dll installed so that the modules can talk to it correctly. StockPlus is disabled by default, even with StockPlusController.dll installed (read below for activating StockPlus).


StockPlus Instructions
======================

StockPlus features are disabled by default. If you want to enable the StockPlus features...

1) Download the "StockPlusController.cfg" or edit your own with a plain text editor (like notepad)
  -- The file should have a "plusActive = True" (or copy and paste the text below)

STOCK_PLUS
{
	plusActive = True
}

2) Place the StockPlusController.cfg file in your GameData director (next to ModuleManager).

Additionally, StockPlus for individual fixes can be selectively disabled by editing the appropriate .cfg file (inside the individual fix directory) and changing the "plusEnabled = True" line to "plusEnabled = False"

Fix Modules that include StockPlus features are marked (Plus) below.


Bug Fixes Included
==================

+ AnchoredDecouplerCrossfeedFix
Description: Fuel feeds across radial decouplers that have parts (other than fuel tanks) attached.
  - Disables crossfeed on all radial decouplers (decouplers that use ModuleAnchoredDecoupler).

----

+ BandwidthFix
Description: Transmitter bandwidth listed in the SPH/VAB is incorrect.
  - An incredibly minor fix, but fixes incorrect calculation/display of transmitter bandwidth in the editors Info Box.

----

+ HighestSpeedFix
Description: The "Highest Speed Achieved" reading in the F3 Flight Log maxes out at 750 m/s.
  - The highest speed achieved is now uncapped, and the Flight Log will display the highest speed achieved.
    -- Note: Because of when KSP calculates and displays values in the background, the speed on the NAV ball won't always coincide with the speed in the Flight Log, but both will register the highest speed achieved.

----

+ KerbalDebrisFix
Description: Kerbals sometimes turn into debris when crashing in External Command Seats.
  - Also recovers kerbals who have been previous frozen by this bug.

----

+ LaunchClampFix (Experimental)
Description: Sometimes launch clamps follow a craft around, causing collisions and craft explosions.
  - Launch clamps should still auto-cleanup when flying far enough away
  - There may be some strange side effects still from the clamp cleanup if leaving physics range and flying back too close.

----

+ ModuleAeroSurfaceFix
Description: Aero Surface (Airbrakes) action groups do not work properly.
  - Action groups fixed (no longer sticks to the "brake" action group)
  - Added some error checking to make the fix compatible with other mods
  - Fixed default brakes action to "toggle" vice "extend"
  - (Plus) When stowed (not deployed) air brakes do not contribute drag (flush with the fuselage)

----

+ ModuleControlSurfaceFix
Description: Control surfaces do not deploy when launched or loaded in the editor
  - Fixes deployment of flight control surfaces on launch and in the editor (loading, cloning, etc)
  - Added some error checking to make the fix compatible with other mods
  - Fixed bug that caused reverse roll inputs with surfaces forward of CoM
  - (Plus) Adds tweakable authority range
  - (Plus) Disables flight controls in space, so they aren't moving around when maneuvering

----

+ ModuleGimbalFix (still a Work In Progress)
Description: Gimbals do not work on engines activated via right click, and gimbaling engines reach their gimbal limits instantaneously.
  - Gimbals now work on engines before activation, or when activated via Right-Click->Activate
  - (Plus) Activates gimbaling speed
  - (Plus) Adds gimbal speed tweakable
  - (Plus) Adds toggle to enable gimbal response speed

----

+ ModuleGrappleNodeFix
Description: The claw part causes a variety of issues, the worst of which results in shredding of ships in orbit.
  - Fixes bug when clawing onto the active vessel.
  - Fixes bug which prevents activation of time warp
  - Fail Safe prevents kraken eating ship during time warp failure

----

+ ModuleParachuteFix (Plus)
Description: Minor fixes for chutes mounted 90 degrees to airflow
  - Fixes log spam and NREs from stock chutes
  - Adds a couple minor visual bug fixes.
  - (Plus) Adds ability to reset chutes that have been activated (staged) but haven't yet deployed.
    -- Must move the chute icon to a new stage in order to "restage" it, or deploy with right-click (same as repack)
  - (Plus) Adds semi deployment and deployment time tweakable
  - (Plus) Adds a couple visual effects (such as symmetric chute spread and asynchronous chute movement)
  - (Plus) Adds a safe/risky/unsafe indicator to staging icons

----

+ ModuleProceduralFairingFix (Plus)
Description: Fairings removed from craft sometimes break when reattaching in VAB/SPH.
  - Fixes some bugs with removing/replacing interstage fairings which can cause fairings to lock up
  - Fixes fairing decoupler force (small fairings are too high, and large fairings too low)
  - Fairings that are children parts now properly rebuild when added with symmetry (is actually in SymmetryActionFix)
  - Fixes some "stuck fairing" issues when jettisoning
  - Fairings stuck on vessel no longer cause the vessel to register as "landed" (and won't save)
  - (Plus) Adds tweakable that allows adjusting the number of shell splits on the fairing
  - (Plus) Adds tweakable decoupler force range.
  - (Not Yet Enabled) Adds tweakable option to enable/disable fairings inside a procedural fairing

----

+ ModuleWheelFix (Plus)
Description: Rover wheel brakes are rendered ineffective and traction is low.
  - Fixes bug with brake torque not working on rover wheels (changes tweakable range to 0 to Max Torque for that part)
  - Improves wheel grip for all rover wheels
    -- (Plus) Also adds tweakable "grip" multiplier (from 1 to 3x)

----

+ OverheatFix (Experimental)
Description: Some parts (especially cubic and octagonal struts) overheat and explode, often associated with time warp.
  - Prevents feedback looping overheat for smaller parts.

----

+ PilotRSASFix
Description: The new(ish) pilot abilities cause smaller vessels to jitter and rapidly deplete electrical charge or monopropellant.
   - Currently does NOT try to prevent overcontrol by RSAS when moving between targets. Only the long term response.
   - Dynamically self adjusts to reduce jitter and resource consumption.
   - Somewhat reduces wobble for bendy craft during launch, when using pilot RSAS abilities.
   - (Plus) Gives access to the tweakable options that control the dynamic tweaks.
     -- The "Plus" tweakable options are available in the right-click menu of the first pod/probe core.
     -- "Min Response" sets the lowest reaction response from the RSAS ("how far" is the initial move)
     -- "Min Clamp" sets the lowest clamp value (how much reaction) as the RSAS nears it's target.
     -- "Threshold" sets the angular speed where clamp and response values start to adjust.
   - Still a bit of a work in progress, and needs testing with more types of craft.

----

+ StickyLaunchPadFix.dll
Description: Certain rocket configurations "stick" to the career tier 2 launch pad.
  - Fixes bug where engines (mostly BACC and LV-T30/45) will stick to tier 2 launch pad.

----

+ SymmetryActionFix.dll
Description: Various fixes for symmetry errors in the VAB/SPH
*** I continue to advise caution with this, although it seems pretty stable now. This unlocks more potential within the editor, but also causes more of the stock bugs to surface (which I've been trying to squish).***

  - Editors now properly default to Radial Symmetry (VAB) and Mirror Symmetry (SPH)
  - Fixes some recursive symmetry problems
  - Prevents symmetric partners from becoming disassociated
  - Retains action groups for symmetric parts when they are removed and replaced in the editor.
  - Fairings that are children parts now properly rebuild when added with symmetry (is actually in SymmetryActionFix)
  - Includes a toggle for my debugging highlighter tool (toggle on/off with MOD+H).
    -- Parent parts are highlighted brighter, symmetric partners are highlighted blue.
    -- If you want to change the keybinding or force it to default to ON, edit the included SymmetryActionFix.cfg.

So, symmetry within symmetry is still a bit buggy in stock especially when using the gizmos (I'm slowly fixing it, one step at a time). This fix can handle copying Action groups buried in symmetry within symmetry, but might still fail when encountering certain stock bugs.

----


DEPRECATED
=======

Release v0.1.7d is the last compatible version for KSP v0.90
Release v0.1.5c is the last compatible version for KSP v0.25
Further releases are labeled by compatible version. Example: Release v1.0.2e.3 is compatible with KSP v1.0.2

----

+ [FIXED IN KSP v1.0.4] Mk3StrengthFix
Description: Mk3 parts are easy to break relative to their size.
  - Mk3 parts are rebalanced to have joint strength on par with other size 3 (SLS) parts (which might be a bit excessive).
  - Now includes Mk3 adapter parts. Their strength is midway between Mk3 and Mk2 parts.

----

+ [FIXED IN KSP v1.0.4] PartDragFix
Description: Drag for the new landing gear and Mk3 cargo bays are a bit off.
  - Landing gear drag is backwards. Gear up gives more drag than gear down.
  - Mk3 cargo bays (should) now properly occlude parts behind them.

----

+ [FIXED IN KSP v1.0.4] PhysicsFix
Description: Convective heating has an error in one of the constants.
  - Fixes one of the global convective heating numbers

----

+ [FIXED IN KSP v1.0.4] ToroidalAerospikeFix
Description: The curves for the toroidal aerospike are a bit off due to missing tangents
  - Atmosphere curve has been updated to include tangents

----

+ [FIXED IN KSP v1.0.2] ModuleCargoBayFix
Description: Parts in cargo bays are inoperative when removed from the bay.
  - When parts are place in a cargo bay, they remain marked as "shielded" even after they leave the bay, when the bay doors are re-closed. This causes some parts to be unresponsive.

----

+ [FIXED IN KSP v1.0.2] HeatBalanceFix
Description: Service bays block all heat flow (acting as infinite heat shields).
  - Right now this is only a minor change to the service bays (which are blocking all heat)

----

+ [FIXED IN KSP v1.0.2] HeatShieldFix
Description: Heat shields cause pods to reenter at an offset angle.
  - Resets the pod to be physics based. Fixes mass and stability issues.

----

+ [FIXED IN KSP v1.0.2] ParachuteFix
Description: Various fixes for chutes
  - Fixes drag values displayed in Editor to show values useful for KSP 1.0 aero (still WIP)
  - Fixes deployment altitude tweakable to allow more low-end altitude selection (instead of it jumping from 50 to 300/350).
  - Lowers semi-deployed drag to be more inline with previous performance, and (importantly) to reduce opening shock
  - Adjusts fully deployed drag slightly to match previous landing performance 
  - Adjusts drogue chute drag to be more drogue like
  - Unlocks tweakables for deployment time and higher altitudes (typically for drogues)
  - Adds a couple visual effects (such as symmetric chute spread and asymmetric chute movement) 

Note: Drag values for chutes are a bit high. This fix modifies drag values.

  - If you do not want to use the chute numbers in this fix, delete the ParachuteDragFix.cfg file (not required, but safest way to be sure).
  - If you want to use the chute numbers in this fix, you MUST rename or delete the KSP/PartDatabase.cfg file.
  - If you uninstall this fix, you must also go back and delete the KSP/PartDatabase.cfg file.



+ [FIXED IN KSP v1.00] AnchoredDecouplerFix.dll - Fixes radial decouplers not wanting to decouple correctly at high speed (leading to boosters striking the core stack)
  - Updated for compatibility with Kerbquake

--

+ [FIXED IN KSP v1.0] ChuteQuickloadFixer.dll - Fixes "Tiny Chutes" or disappearing chutes on quickload

--

+ [FIXED IN KSP v1.00] CrewRosterFreezeFix.dll - Fixes a bug where firing a kerbal who had logged Achiements caused the game to lock up and corrupt the save.
  - Now handles firing kerbals who are MIA and works when managing kerbals from the Editors.

--

+ [FIXED IN KSP v1.00] EVAEjectionFix.dll - Fixes the bug that causes a kerbal to be ejected away from a capsule upon EVA.
  - Nullifies ladder slide bug for initial EVA.

--

+ [FIXED IN KSP v0.90] LargeCraftLaunchFix.dll - Fixes launch pad and runway explosions when launching large vessels.
  - Now works through quickloads, scene transitions, and when approaching a craft on the pad/runway from outside physics range.
  - Functions correctly with Kerbal Joint Reinforcement installed.
  - DEPRECATED: This bug is now fixed in stock KSP.



License
=======

Covered under the CC-BY-NC-SA license. See the license.txt for more details.
(https://creativecommons.org/licenses/by-nc-sa/4.0/)

ModuleManager by Sarbian (bundled) is covered under a CC share-alike license.


Change Log
==========
v1.0.4c.1 (22 Aug 15) - Added OverheatFix and LaunchClampFix. Some minor updates to other modules.
v1.0.4b.2 (1 Aug 15)  - Major update to ModuleGrappleNodeFix. Minor updates to ModuleParachuteFix and ModuleGimbalFix
v1.0.4b.1 (24 Jul 15) - Initial release of HighestSpeedFix, ModuleGrappleNodeFix. Minor updates to ModuleParachuteFix and SymmetryActionFix.
v1.0.4a.1 (1 Jul 15)  - Updated for KSP v1.0.4, Fixed parachute log spam and linux crashing KSP bug, adjusted PilotRSASFix
v1.0.2e.3 (10 Jun 15) - Removed some WIP code that snuck into ModuleProceduralFairingFix.
v1.0.2e.2 (9 Jun 15) - Additional fixes added to ModuleProceduralFairingFix.
v1.0.2e.1 (7 Jun 15) - Initial release of PilotRSASFix.
v1.0.2d.5 (5 Jun 15) - Deactivated ModuleGimbalFix Plus features by default.
v1.0.2d.4 (1 Jun 15) - Fixed default aero surface action group and a bug in StockPlusController (when the .cfg is missing)
v1.0.2d.3 (18 May 15) - Fixed reversed roll control in ModuleControlSurfaceFix, Mk3-Mk2 slanted adapter, & minor StockPlus UI bugs
v1.0.2d.2 (14 May 15) - Improved mod compatibility, converted ModuleGimbalFix to StockPlus
v1.0.2d.1 (13 May 15) - Added ModuleAeroSurfaceFix and ModuleControlSurfaceFix, plus some updates to disable StockPlus
v1.0.2c.2 (9 May 15) - Fixed the StockPlus config.
v1.0.2c.1 (9 May 15) - Added decoupler fuel feed fixes. Incorporates StockPlus additions.
v1.0.2b (3 May 15) - Additions include fixes to landing gear drag, Mk3 cargo bays, and a couple minor physics fixes.
v1.0.2a (2 May 15) - Updated for KSP v1.0.2
v1.0.0  (27 Apr 15) - Deprecated and Added several new fixes for KSP v1.0
v0.1.7e (28 Feb 15) - SymmetryActionFix fixes some default behaviors in the editors, prevents staging icons from separating, and includes part highlighting.
v0.1.7d (6 Jan 15) - Rebuild of SymmetryActionFix.dll. More robust at handling nested symmetry and collecting symmetric parts / action groups.
v0.1.7c (4 Jan 15) - Regressed SymmetryActionFix.dll again. Found further bugs that break new KSP symmetry features.
v0.1.7b (29 Dec 14) - Reinstated SymmetryActionFix.dll for KSP v0.90.0.705.
v0.1.7a (29 Dec 14) - Updated CrewRosterFreezeFix.dll to be a bit more robust. Reduced StickyLaunchPadFix.dll log spam.
v0.1.7 (23 Dec 14) - Initial release StickyLaunchPadFix.dll.
v0.1.6 (20 Dec 14) - Initial release CrewRosterFreezeFix.dll, regressed SymmetryActionFix.dll (broken in KSP v0.90), deprecated LargeCraftLaunchFix.dll
v0.1.5c (10 Dec 14) - AnchoredDecouplerFix.dll updated for compatibility with Kerbquake.
v0.1.5b (23 Nov 14) - EVAEjectionFix now nullifies ladder slide bug for initial EVA, and added minor error checking for KerbalDebrsFix.
v0.1.5a (22 Nov 14) - KerbalDebrisFix now properly recovers names for already frozen kerbals.
v0.1.5 (22 Nov 14) - Initial release of KerbalDebrisFix.dll
v0.1.4d (22 Nov 14) - LargeCraftLaunchFix.dll now functions correctly with Kerbal Joint Reinforcement installed.
v0.1.4c (6 Nov 14) - SymmetryActionFix.dll now handles action groups buried in symmetry within symmetry.
v0.1.4b (5 Nov 14) - Added some error checking to SymmetryActionFix.dll
v0.1.4a (3 Nov 14) - Updated LargeCraftLaunchFix.dll to be a bit more robust. Now works through quickloads, scene transitions, and coming within physics range.
v0.1.4 (2 Nov 14) - Initial release of LargeCraftLaunchFix.dll
v0.1.3 & v0.1.3a (1 Nov 14) - Initial release of SymmetryActionFix.dll
v0.1.2a (27 Oct 14) - Removed "Reset" message from ChuteQuickloadFixer.dll and recompiled for .NET 3.5, updated readme to accommodate releases
v0.1.2 (21 Oct 14) - Reworked AnchoredDecouplerFix to better handle struts and prevent decouplers from ripping off. (Should work like pre KSP v0.24.2)
v0.1.1a (21 Oct 14) - Updated error handling in EVAEjectionFix to prevent log spam and kerbal lockup with incompatible mod
v0.1.1 (19 Oct 14) - Release of EVAEjectionFix.dll and reduced some log spam with ChuteQuickloadFixer and AnchoredDecouplerFix
v0.1 (17 Oct 14) - Initial release, includes ChuteQuickloadFixer and AnchoredDecouplerFix
