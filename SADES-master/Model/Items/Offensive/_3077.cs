﻿using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SADES.Model.Items
{
    internal class _3077 : Item
    {
        internal override int Id
        {
            get { return 3077; }
        }

        internal override string Name
        {
            get { return "Tiamat"; }
        }

        public override void Use()
        {
            Orbwalking.AfterAttack += AfterAttack;
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || !(target is Obj_AI_Hero))
                return;

            var t = (Obj_AI_Hero)target;

            if (!t.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (t.IsValidTarget())
            {
                LeagueSharp.Common.Items.UseItem(3077, ObjectManager.Player);
            }
            
        }
    }
}
