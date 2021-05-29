using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.LumiteDestroyer;

namespace SunksBossChallenges.Projectiles.LumiteDestroyer
{
    public class LMDoublePlanet:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Double Planet");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 4;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;

            projectile.timeLeft = 600;
        }
        public override void AI()
        {
            if (projectile.ai[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LMPlanetMoon>(),
                    projectile.damage, 0f, projectile.owner, -1, projectile.whoAmI);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LMPlanetMoon>(),
                    projectile.damage, 0f, projectile.owner, 1, projectile.whoAmI);
            }
            projectile.ai[0]++;
            if (projectile.ai[0] >= 150)
            {
                projectile.SlowDown(0.98f);
                if (projectile.velocity == Vector2.Zero)
                {
                    projectile.Kill();
                }
            }
        }
    }
    public class LMPlanetMoon : LMChaosMoon
    {
        public override string Texture => "SunksBossChallenges/Projectiles/LumiteDestroyer/LMChaosMoon";
        public float Omega => 0.1f;
        public override void AI()
        {
            Projectile parent = Main.projectile[(int)projectile.ai[1]];
            if (parent.active && parent.type == ModContent.ProjectileType<LMDoublePlanet>())
            {
                projectile.localAI[0]++;
                projectile.Center = parent.Center
                    + parent.velocity.RotatedBy(MathHelper.PiOver2 * projectile.ai[0]) * (float)Math.Sin(projectile.localAI[0] * Omega) * 10;
            }
            else
            {
                projectile.Kill();
            }
        }
    }
}
