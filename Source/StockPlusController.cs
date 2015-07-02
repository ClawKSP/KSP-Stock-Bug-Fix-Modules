/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * StockPlusController - Written for KSP v1.0
 * 
 * - Enables StockPlus features
 * 
 * Change Log:
 * - v00.03  (1 Jul 15)    Recompiled for KSP v1.0.4
 * - v00.02  (1 Jun 15)    Minor bug fix
 * - v00.01  (8 May 15)    Initial Release
 * 
 */

using UnityEngine;
using KSP;


namespace ClawKSP
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class StockPlusController : MonoBehaviour
    {
        public static bool plusActive = false;

        public void Start ()
        {
            ConfigNode[] CNList = GameDatabase.Instance.GetConfigNodes("STOCK_PLUS");
            if (CNList != null && CNList.Length != 0)
            {
                ConfigNode CNBinding = new ConfigNode();
                CNBinding = GameDatabase.Instance.GetConfigNodes("STOCK_PLUS")[0];

                if (null != CNBinding)
                {
                    string BindingString = CNBinding.GetValue("plusActive");
                    if (!string.IsNullOrEmpty(BindingString))
                    {
                        if (false == System.Boolean.TryParse(BindingString, out plusActive))
                        {
                            plusActive = false;
                        }
                    }
                }
            }

            Debug.Log("StockPlusController.Start(): v00.03 (Active = " + plusActive + ")");
        }
    }
}
