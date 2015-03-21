using LeagueSharp;
using LeagueSharp.Common;
using SADES.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Controller
{
    class ControllerHandler
    {
        /*
         Inicializa o SADES
         */
        public static void GameStart()
        {
            Game.PrintChat(MiscControl.stringColor("SADES , " + GameControl.version, MiscControl.TableColor.Blue));

            var CreditsMenu = new Menu("Credits", "Credits");
            {
                CreditsMenu.AddItem(new MenuItem("c1", "Mr Articuno"));
            }

            GameControl.Config.AddSubMenu(CreditsMenu);
        }
                
    }
}
