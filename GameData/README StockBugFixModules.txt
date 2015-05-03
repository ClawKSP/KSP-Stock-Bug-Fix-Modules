KSP-Stock-Bug-Fix-Modules
=========================

Stand alone fixes for common stock KSP bugs. These modules are meant to be fully stock compatible. My aim is to be able to remove them from the game at any time without causing a problem for the stock saves.



Installation
============
Please delete any old installations of KSP-Stock-Bug-Fix-Modules before installing.

Unzip the zip file to the KSP/GameData directory.

The fixes are all stand alone. So if there are any that you don't want, just delete the directory inside the StockBugFixModules folder.


Bug Fixes Included
==================

+ BandwidthFix
Description: Transmitter bandwidth listed in the SPH/VAB is incorrect.
  - An incredibly minor fix, but fixes incorrect calculation/display of transmitter bandwidth in the editors Info Box.

--

+ KerbalDebrisFix
Description: Kerbals sometimes turn into debris when crashing in External Command Seats.
  - Also recovers kerbals who have been previous frozen by this bug.

--

+ Mk3StrengthFix
Description: Mk3 parts are easy to break relative to their size.
  - Mk3 parts are rebalanced to have joint strength on par with other size 3 (SLS) parts (which might be a bit excessive).

--

+ ModuleProceduralFairingFix
Description: Fairings removed from craft sometimes break when reattaching in VAB/SPH.
  - Fixes some bugs with removing/replacing interstage fairings which can cause fairings to lock up
  - Fixes fairing decoupler force (small fairings are too high, and large fairings too low)
  - (removed) Adds tweakable that allows adjusting the number of shell splits on the fairing
  - (removed) Adds tweakable decoupler force range.
  - (removed) Adds tweakable option to enable/disable fairings inside a procedural fairing

--

+ ModuleWheelFix
Description: Rover wheel brakes are rendered ineffective and traction is low.
  - Fixes bug with brake torque not working on rover wheels (changes tweakable range to 0 to 200)
  - Improves wheel grip for all rover wheels
    -- (removed) Also adds tweakable "grip" multiplier (from 1 to 3x)

----

+ StickyLaunchPadFix.dll
Description: Certain rocket configurations "stick" to the career tier 2 launch pad.
  - Fixes bug where engines (mostly BACC and LV-T30/45) will stick to tier 2 launch pad.

--

+ SymmetryActionFix.dll
Description: Various fixes for symmetry errors in the VAB/SPH
*** I continue to advise caution with this, although it seems pretty stable now. This unlocks more potential within the editor, but also causes more of the stock bugs to surface (which I've been trying to squish).***

  - Editors now properly default to Radial Symmetry (VAB) and Mirror Symmetry (SPH)
  - Fixes some recursive symmetry problems
  - Prevents symmetric partners from becoming disassociated
  - Retains action groups for symmetric parts when they are removed and replaced in the editor.
  - Includes a toggle for my debugging highlighter tool (toggle on/off with MOD+H).
    -- Parent parts are highlighted brighter, symmetric partners are highlighted blue.
    -- If you want to change the keybinding or force it to default to ON, edit the included SymmetryActionFix.cfg.

So, symmetry within symmetry is still a bit buggy in stock especially when using the gizmos (I'm slowly fixing it, one step at a time). This fix can handle copying Action groups buried in symmetry within symmetry, but might still fail when encountering certain stock bugs.



DEPRICATED
=======

Release v0.1.7d is the last compatible version for KSP v0.90
Release v0.1.5c is the last compatible version for KSP v0.25

--

+ [FIXED IN KSP v1.0.2] ModuleCargoBayFix
Description: Parts in cargo bays are inoperative when removed from the bay.
  - When parts are place in a cargo bay, they remain marked as "shielded" even after they leave the bay, when the bay doors are re-closed. This causes some parts to be unresponsive.

--

+ [FIXED IN KSP v1.0.2] HeatBalanceFix
Description: Service bays block all heat flow (acting as infinte heat shields).
  - Right now this is only a minor change to the service bays (which are blocking all heat)

--

+ [FIXED IN KSP v1.0.2] HeatShieldFix
Description: Heat shields cause pods to reenter at an offset angle.
  - Resets the pod to be physics based. Fixes mass and stability issues.

--

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
  - DEPRICATED: This bug is now fixed in stock KSP.



License
=======

Covered under the CC-BY-NC-SA license. See the license.txt for more details.
(https://creativecommons.org/licenses/by-nc-sa/4.0/)


Change Log
==========
v1.0.2a (2 May 15) - Updated for KSP v1.0.2
v1.0.0  (27 Apr 15) - Depricated and Added several new fixes for KSP v1.0
v0.1.7e (28 Feb 15) - SymmetryActionFix fixes some default behaviors in the editors, prevents staging icons from separating, and includes part highlighting.
v0.1.7d (6 Jan 15) - Rebuild of SymmetryActionFix.dll. More robust at handling nested symmetry and collecting symmetric parts / action groups.
v0.1.7c (4 Jan 15) - Regressed SymmetryActionFix.dll again. Found further bugs that break new KSP symmetry features.
v0.1.7b (29 Dec 14) - Reinstated SymmetryActionFix.dll for KSP v0.90.0.705.
v0.1.7a (29 Dec 14) - Updated CrewRosterFreezeFix.dll to be a bit more robust. Reduced StickyLaunchPadFix.dll log spam.
v0.1.7 (23 Dec 14) - Initial release StickyLaunchPadFix.dll.
v0.1.6 (20 Dec 14) - Initial release CrewRosterFreezeFix.dll, regressed SymmetryActionFix.dll (broken in KSP v0.90), depricated LargeCraftLaunchFix.dll
v0.1.5c (10 Dec 14) - AnchoredDecouplerFix.dll updated for compatilibity with Kerbquake.
v0.1.5b (23 Nov 14) - EVAEjectionFix now nullifies ladder slide bug for initial EVA, and added minor error checking for KerbalDebrsFix.
v0.1.5a (22 Nov 14) - KerbalDebrisFix now properly recovers names for already frozen kerbals.
v0.1.5 (22 Nov 14) - Initial release of KerbalDebrisFix.dll
v0.1.4d (22 Nov 14) - LargeCraftLaunchFix.dll now functions correctly with Kerbal Joint Reinforcement installed.
v0.1.4c (6 Nov 14) - SymmetryActionFix.dll now handles action groups buried in symmetry within symmetry.
v0.1.4b (5 Nov 14) - Added some error checking to SymmetryActionFix.dll
v0.1.4a (3 Nov 14) - Updated LargeCraftLaunchFix.dll to be a bit more robust. Now works through quickloads, scene transitions, and coming within physics range.
v0.1.4 (2 Nov 14) - Initial release of LargeCraftLaunchFix.dll
v0.1.3 & v0.1.3a (1 Nov 14) - Initial release of SymmetryActionFix.dll
v0.1.2a (27 Oct 14) - Removed "Reset" message from ChuteQuickloadFixer.dll and recompiled for .NET 3.5, updated readme to accomodate releases
v0.1.2 (21 Oct 14) - Reworked AnchoredDecouplerFix to better handle struts and prevent decouplers from ripping off. (Should work like pre KSP v0.24.2)
v0.1.1a (21 Oct 14) - Updated error handling in EVAEjectionFix to prevent log spam and kerbal lockup with incompatible mod
v0.1.1 (19 Oct 14) - Release of EVAEjectionFix.dll and reduced some log spam with ChuteQuickloadFixer and AnchoredDecouplerFix
v0.1 (17 Oct 14) - Initial release, includes ChuteQuickloadFixer and AnchoredDecouplerFix
