using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SunksBossChallenges.NPCs.LumiteTwins;

namespace SunksBossChallenges.Projectiles.LumiteTwins
{
    public class LSFlameBomb:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.CursedFlameHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Bomb");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.hostile = true;
        }
        public override void AI()
        {
            projectile.Loomup();
            float maxTime = projectile.ai[0] == 0 ? 45 : projectile.ai[0];
            if (projectile.localAI[0] % 5 == 0)
            {
                int num97 = Dust.NewDust(new Vector2(projectile.position.X + projectile.velocity.X, projectile.position.Y + projectile.velocity.Y),
                projectile.width, projectile.height, DustID.CursedTorch, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 3f * projectile.scale);
                Main.dust[num97].noGravity = true;
            }

            projectile.localAI[0]++;

            if (projectile.localAI[0] >= maxTime)
            {
                projectile.SlowDown(0.95f);
            }

            if (projectile.velocity.Compare(6f) < 0)
            {
                projectile.Kill();
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 baseVelocity = Vector2.UnitX * 3f;
                if (Main.rand.NextBool()) baseVelocity = baseVelocity.RotatedBy(MathHelper.PiOver4);
                for(int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectile(projectile.Center, baseVelocity, ModContent.ProjectileType<LSAccleFlame>(),
                        projectile.damage, 0f, projectile.owner, 60f);
                    baseVelocity = baseVelocity.RotatedBy(MathHelper.PiOver2);
                }
            }
            base.Kill(timeLeft);
        }
    }

    public class LSAccleFlame : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.CursedFlameHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Flame");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.hostile = true;

            projectile.timeLeft = 180;
        }
        public override void AI()
        {
            projectile.Loomup();
            /*if (projectile.localAI[0] % 5 == 0)
            {
                int num97 = Dust.NewDust(new Vector2(projectile.position.X + projectile.velocity.X, projectile.position.Y + projectile.velocity.Y),
                projectile.width, projectile.height, DustID.CursedTorch, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 3f * projectile.scale);
                Main.dust[num97].noGravity = true;
            }*/
            
            projectile.localAI[0]++;

            if (projectile.velocity.Compare(projectile.ai[0]) < 0)
                projectile.velocity *= 1.1f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color alpha = Color.Green * projectile.Opacity;
            if (projectile.localAI[0] <= 10)
            {
                alpha *= projectile.localAI[0] / 10f;
            }
            else if (projectile.localAI[0] >= 30 && projectile.localAI[0] <= 45) 
            {
                alpha *= (45 - projectile.localAI[0]) / 15f;
            }
            projectile.DrawAim(spriteBatch, projectile.Center + projectile.velocity.SafeNormalize(Vector2.Zero) * 1500, alpha);
            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}
