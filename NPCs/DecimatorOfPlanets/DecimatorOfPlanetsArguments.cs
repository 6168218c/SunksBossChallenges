using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SunksBossChallenges.NPCs.DecimatorOfPlanets
{
    static class DecimatorOfPlanetsArguments
    {
        public static float SpinSpeed
        {
            get{
                float Speed = 30f;
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
                double Speed = Math.PI * 2f / 150;
                return Speed;
            }
        }

        public static float R => (float)(SpinSpeed / SpinRadiusSpeed);

        public static float Scale => 1.2f;

        public static Microsoft.Xna.Framework.Color TextColor => Microsoft.Xna.Framework.Color.Cyan;
    }
}
