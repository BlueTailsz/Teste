using LeagueSharp;
using LeagueSharp.Common;
using SADES.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Model.Items
{
    internal class _3153 : Item
    {
        internal override int Id
        {
            get { return 3153; }
        }

        internal override string Name
        {
            get { return "Blade of the Ruined King"; }
        }

        internal override float Range
        {
            get { return 450; }
        }

        public override void Use()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (MiscControl.isValidTarget(target) && MiscControl.calcularPorcentagem(target) > MiscControl.calcularPorcentagem(ObjectManager.Player) 
                || target.Distance(ObjectManager.Player.Position) > ObjectManager.Player.AttackRange)
            {
                LeagueSharp.Common.Items.UseItem(Id, target);
            }
        }
    }
}
