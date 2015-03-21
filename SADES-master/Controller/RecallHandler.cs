﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using SADES.Util;

namespace SADES.Controller
{
    internal static class RecallHandler
    {

        static List<Obj_AI_Hero> Heroes;
        static List<Obj_AI_Hero> Enemies;
        static List<Obj_AI_Hero> Allies;

        static public List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();

        static Font Text;

        static float BarX = Drawing.Width * 0.425f;
        static float BarY = Drawing.Height * 0.80f;
        static int BarWidth = (int)(Drawing.Width - 2 * BarX);
        static int BarHeight = 6;
        static int SeperatorHeight = 5;
        static float Scale = (float)BarWidth / 8000;

        public static void Load()
        {
            Heroes = ObjectManager.Get<Obj_AI_Hero>().ToList();
            Enemies = Heroes.Where(x => x.IsEnemy).ToList();

            EnemyInfo = Enemies.Select(x => new EnemyInfo(x)).ToList();

            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnDraw += Drawing_OnDraw;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_DomainUnload;

            var RecallMenu = new Menu("Recall Tracker", "recallTracker");
            {

                RecallMenu.AddItem(new MenuItem("showRecalls", "Show Recall Tracker").SetValue(true));
                RecallMenu.AddItem(new MenuItem("credits", "All credits to Beaving"));

            }

            GameControl.Config.AddSubMenu(RecallMenu);

        }

        static void Obj_AI_Base_OnTeleport(GameObject sender, GameObjectTeleportEventArgs args)
        {
            var unit = sender as Obj_AI_Hero;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            var recall = Packet.S2C.Teleport.Decoded(unit, args);
            EnemyInfo.Find(x => x.Player.NetworkId == recall.UnitNetworkId).RecallInfo.UpdateRecall(recall);
        }

        static void Drawing_OnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
        }

        static void Drawing_OnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Text.Dispose();
        }


        static void Drawing_OnDraw(EventArgs args)
        {
            if (!GameControl.Config.Item("showRecalls").GetValue<bool>() || Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;

            bool indicated = false;

            float fadeout = 1f;
            int count = 0;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x =>
                x.Player.IsValid<Obj_AI_Hero>() &&
                x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0).OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (!enemyInfo.RecallInfo.LockedTarget)
                {
                    fadeout = 1f;
                    Color color = System.Drawing.Color.White;

                    if (enemyInfo.RecallInfo.WasAborted())
                    {
                        fadeout = (float)enemyInfo.RecallInfo.GetDrawTime() / (float)enemyInfo.RecallInfo.FADEOUT_TIME;
                        color = System.Drawing.Color.Yellow;
                    }

                    DrawRect(BarX, BarY, (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1, System.Drawing.Color.FromArgb((int)(100f * fadeout), System.Drawing.Color.White));
                    DrawRect(BarX + Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - 1, BarY - SeperatorHeight, 0, SeperatorHeight + 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), color));

                    Text.DrawText(null, enemyInfo.Player.ChampionName, (int)BarX + (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - (float)(enemyInfo.Player.ChampionName.Length * Text.Description.Width) / 2), (int)BarY - SeperatorHeight - Text.Description.Height - 1, new ColorBGRA(color.R, color.G, color.B, (byte)((float)color.A * fadeout)));
                }
                else
                {
                    if (!indicated && enemyInfo.RecallInfo.EstimatedShootT != 0)
                    {
                        indicated = true;
                        DrawRect(BarX + Scale * enemyInfo.RecallInfo.EstimatedShootT, BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight * 2, 2, System.Drawing.Color.Orange);
                    }

                    DrawRect(BarX, BarY, (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.Red));
                    DrawRect(BarX + Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - 1, BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight + 1, 1, System.Drawing.Color.IndianRed);

                    Text.DrawText(null, enemyInfo.Player.ChampionName, (int)BarX + (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - (float)(enemyInfo.Player.ChampionName.Length * Text.Description.Width) / 2), (int)BarY + SeperatorHeight + Text.Description.Height / 2, new ColorBGRA(255, 92, 92, 255));
                }

                count++;
            }

            /*
             * Show in a red rectangle right next to the normal bar the names of champs which can be killed (when they are not recalling yet)
             * Requires calculating the damages (make more functions!)
             * 
             * var BaseUltableEnemies = EnemyInfo.Where(x =>
                x.Player.IsValid<Obj_AI_Hero>() &&
                !x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0 && x.RecallInfo.LockedTarget).OrderBy(x => x.RecallInfo.GetRecallCountdown());*/

            if (count > 0)
            {
                if (count != 1) //make the whole bar fadeout when its only 1
                    fadeout = 1f;

                DrawRect(BarX, BarY, BarWidth, BarHeight, 1, System.Drawing.Color.FromArgb((int)(40f * fadeout), System.Drawing.Color.White));

                DrawRect(BarX - 1, BarY + 1, 0, BarHeight, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY - 1, BarWidth + 2, 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY + BarHeight, BarWidth + 2, 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX + 1 + BarWidth, BarY + 1, 0, BarHeight, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
            }
        }

        public static void DrawRect(float x, float y, int width, int height, float thickness, System.Drawing.Color color)
        {
            for (int i = 0; i < height; i++)
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
        }
    }

    class EnemyInfo
    {
        public Obj_AI_Hero Player;
        public int LastSeen;

        public RecallInfo RecallInfo;

        public EnemyInfo(Obj_AI_Hero player)
        {
            Player = player;
            RecallInfo = new RecallInfo(this);
        }
    }

    class RecallInfo
    {
        public EnemyInfo EnemyInfo;
        public Dictionary<int, float> IncomingDamage; //from, damage
        public Packet.S2C.Teleport.Struct Recall;
        public Packet.S2C.Teleport.Struct AbortedRecall;
        public bool LockedTarget;
        public float EstimatedShootT;
        public int AbortedT;
        public int FADEOUT_TIME = 3000;

        public RecallInfo(EnemyInfo enemyInfo)
        {
            EnemyInfo = enemyInfo;
            Recall = new Packet.S2C.Teleport.Struct(EnemyInfo.Player.NetworkId, Packet.S2C.Teleport.Status.Unknown, Packet.S2C.Teleport.Type.Unknown, 0);
            IncomingDamage = new Dictionary<int, float>();
        }

        public bool ShouldDraw()
        {
            return IsPorting() || (WasAborted() && GetDrawTime() > 0);
        }

        public bool IsPorting()
        {
            return Recall.Type == Packet.S2C.Teleport.Type.Recall && Recall.Status == Packet.S2C.Teleport.Status.Start;
        }

        public bool WasAborted()
        {
            return Recall.Type == Packet.S2C.Teleport.Type.Recall && Recall.Status == Packet.S2C.Teleport.Status.Abort;
        }

        public EnemyInfo UpdateRecall(Packet.S2C.Teleport.Struct newRecall)
        {
            IncomingDamage.Clear();
            LockedTarget = false;
            EstimatedShootT = 0;

            if (newRecall.Type == Packet.S2C.Teleport.Type.Recall && newRecall.Status == Packet.S2C.Teleport.Status.Abort)
            {
                AbortedRecall = Recall;
                AbortedT = Environment.TickCount;
            }
            else
                AbortedT = 0;

            Recall = newRecall;
            return EnemyInfo;
        }

        public int GetDrawTime()
        {
            int drawtime = 0;

            if (WasAborted())
                drawtime = FADEOUT_TIME - (Environment.TickCount - AbortedT);
            else
                drawtime = GetRecallCountdown();

            return drawtime < 0 ? 0 : drawtime;
        }

        public int GetRecallCountdown()
        {
            int time = Environment.TickCount;
            int countdown = 0;

            if (time - AbortedT < FADEOUT_TIME)
                countdown = AbortedRecall.Duration - (AbortedT - AbortedRecall.Start);
            else if (AbortedT > 0)
                countdown = 0; //AbortedT = 0
            else
                countdown = Recall.Start + Recall.Duration - time;

            return countdown < 0 ? 0 : countdown;
        }

        public override string ToString()
        {
            String drawtext = EnemyInfo.Player.ChampionName + ": " + Recall.Status; //change to better string and colored

            float countdown = GetRecallCountdown() / 1000f;

            if (countdown > 0)
                drawtext += " (" + countdown.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "s)";

            return drawtext;
        }
    }

}
