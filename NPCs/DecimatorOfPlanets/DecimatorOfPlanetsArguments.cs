using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SunksBossChallenges.NPCs.DecimatorOfPlanets
{
    static class DecimatorOfPlanetsArguments
    {
        public static float SpinSpeed
        {
            get{
                float Speed = 20f;
                if (Main.expertMode)
                    Speed *= 1.125f;
                //if (Main.getGoodWorld)
                //    Speed *= 1.25f;
                return Speed;
            }
        }
        public static double SpinRadiusSpeed
        {
            get
            {
                double Speed = Math.PI * 2f / 225;
                return Speed;
            }
        }

        public static float R => (float)(SpinSpeed / SpinRadiusSpeed);

        public static float Scale => 1.2f;

        public static Color TextColor => Color.Cyan;
    }
}
