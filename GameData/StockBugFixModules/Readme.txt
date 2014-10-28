KSP-Stock-Bug-Fix-Modules

=========================



Stand alone fixes for common stock KSP bugs. These modules are meant to be fully stock compatible. My aim is to be able to remove them from the game at any time without causing a problem for the stock saves.





Bug fixes included so far

=========================



+ ChuteQuickloadFixer.dll - Fixes "Tiny Chutes" or disappearing chutes on quickload


+ AnchoredDecouplerFix.dll - Fixes radial decouplers not wanting to decouple correctly at high speed (leading to boosters striking the core stack)

+ EVAEjectionFix.dll - Fixes the bug that causes a kerbal to be ejected away from a capsule upon EVA.





Installation

============



Just download the desired fix's .dll and place in the KSP/GameData directory. No other downloads, file changes, or add-ons are required.



Alternately, you can download the release and place the "StockBugFixModules" directory in the KSP/GameData directory.








License

=======



Covered under the CC-BY-NC-SA license. See the license.txt for more details.

(https://creativecommons.org/licenses/by-nc-sa/4.0/)





Change Log

==========
v0.1.2a (27 Oct 14) - Removed "Reset" message from ChuteQuickloadFixer.dll and recompiled for .NET 3.5, updated readme to accomodate releases
v0.1.2 (21 Oct 14) - Reworked AnchoredDecouplerFix to better handle struts and prevent decouplers from ripping off. (Should work like pre KSP v0.24.2)
v0.1.1a (21 Oct 14) - Updated error handling in EVAEjectionFix to prevent log spam and kerbal lockup with incompatible mod
v0.1.1 (19 Oct 14) - Release of EVAEjectionFix.dll and reduced some log spam with ChuteQuickloadFixer and AnchoredDecouplerFix



v0.1 (17 Oct 14) - Initial release, includes ChuteQuickloadFixer and AnchoredDecouplerFix
