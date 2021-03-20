using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;
using Terraria;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.DecimatorOfPlanets;

namespace SunksBossChallenges.Sky.DecimatorOfPlanets
{
    public class AggressiveSky:BasicColoredSky
    {
        protected override Color SkyColor => Color.Purple;
        protected override int NPCType => ModContent.NPCType<DecimatorOfPlanetsHead>();
    }

    public class PassiveSky : BasicColoredSky
    {
        protected override Color SkyColor => Color.Cyan;
        protected override int NPCType => ModContent.NPCType<DecimatorOfPlanetsHead>();
    }
    public class LastPhaseSky : BasicColoredSky
    {
        protected override Color SkyColor => Color.Black;
        protected override float MinIntensity => 0.96f;
        protected override int NPCType => ModContent.NPCType<DecimatorOfPlanetsHead>();
    }
}
