using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.NPCs.DecimatorOfPlanets;

namespace SunksBossChallenges.Projectiles.DecimatorOfPlanets
{
    public class FinalPhaseSummoner:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamFriendly;

        public override void SetDefaults()
        {
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
        }
        public override void AI()
        {
            projectile.ai[0]++;
            projectile.active = true;
            int target = (int)projectile.ai[1];
            if (target < 0 || target == 255 || Main.player[target].dead)
                projectile.Kill();

            if (projectile.ai[0] == 120)
                Main.NewText("Don't you think it so easy to defeat me?", DecimatorOfPlanetsArguments.TextColor);
            if (projectile.ai[0] == 300)
                Main.NewText("Well,good news for you.", DecimatorOfPlanetsArguments.TextColor);
            if (projectile.ai[0] == 600)
            {
                Main.NewText("IT'S NOT OVER YET!", DecimatorOfPlanetsArguments.TextColor);
                NPC.SpawnOnPlayer(target, ModContent.NPCType<DecimatorOfPlanetsHead>());
                //NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DecimatorOfPlanetsHead>());
                //Main.NewText(Terraria.Localization.Language.GetTextValue("Announcement.HasAwoken", "The Decimator Of Planets"), 175, 75);
                projectile.Kill();
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }
}
