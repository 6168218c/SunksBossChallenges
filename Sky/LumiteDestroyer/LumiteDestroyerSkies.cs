using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.NPCs.LumiteDestroyer;

namespace SunksBossChallenges.Sky.LumiteDestroyer
{
    public class LumiteDestroyerSky:LinearGradientSky
    {
        public override string BackgroundTexture => "Sky/LumiteDestroyer/Background";
        public override int NPCType => ModContent.NPCType<LumiteDestroyerHead>();
        public override double yOffset => 3600;
    }
}
