using LeagueSharp;
using LeagueSharp.Common;
using SADES.Model;
using SADES.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Controller
{
    internal static class ItemHandler
    {
        private static readonly List<Item> Items = new List<Item>();

        static ItemHandler()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        private static Menu ItemsSubMenu
        {
            get { return GameControl.Config.SubMenu("Items"); }
        }

        internal static void Load()
        {
            const string @namespace = "SADES.Model.Items";

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Name != "Item" && t.Namespace == @namespace
                    select t;

            q.ToList().ForEach(t => LoadItem((Item)Activator.CreateInstance(t)));

            /*const string @deffensive = "SADES.Model.Items.Offensive";

            var e = from s in Assembly.GetExecutingAssembly().GetTypes()
                    where s.IsClass && s.Name != "Item" && s.Namespace == @deffensive
                    select s;

            e.ToList().ForEach(s => LoadItem((Item)Activator.CreateInstance(s)));*/
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (MiscControl.OrbwalkerMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            Items.Where(item => item.IsActive).ToList().ForEach(item => item.Use());
        }

        private static void LoadItem(Item item)
        {
            Items.Add(item.CreateMenuItem(ItemsSubMenu));
        }
    }
}
