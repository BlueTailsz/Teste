using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Model
{
    public class Item
    {
        internal virtual int Id { get; set; }
        internal virtual string Name { get; set; }
        internal virtual float Range { get; set; }

        public bool IsActive
        {
            get { return LeagueSharp.Common.Items.CanUseItem(Id) && MenuItem.GetValue<bool>(); }
        }

        public MenuItem MenuItem { get; private set; }

        public Item CreateMenuItem(Menu parent)
        {
            MenuItem = parent.AddItem(new MenuItem(Name, "Use " + Name).SetValue(true));
            Console.WriteLine(Name);
            return this;
        }

        public virtual void Use() { }
    }
}
