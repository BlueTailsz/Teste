using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using SADES.Util;
using SADES.Controller;

namespace SADES
{
    static class Program
    {
        static void Main(string[] args)
        {
            GameControl.Load();
            ControllerHandler.GameStart();
        }
    }
}
