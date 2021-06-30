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
    public class LMEllipseMoonAim : ModProjectile
    {
        Vector2 playerVec;
        float T;
        public Vector2 Focus => Vector2.Lerp(projectile.Center, playerVec, 0.3f);
        public static float GM => 0.1f;
        public static float m => 1f;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.StardustTowerMark;
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

            //projectile.timeLeft = 600;
        }
        public override void AI()
        {
            if (projectile.localAI[1] == 1)
            {
                projectile.alpha += 25;
                if(projectile.alpha>255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                }
                return;
            }
            if (projectile.localAI[0] == 0)
            {
                playerVec = Main.player[(int)projectile.ai[0]].Center + Main.player[(int)projectile.ai[0]].velocity * 30;
            }
            projectile.localAI[0]++;
            projectile.Loomup();
            if (projectile.localAI[0] == 30 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 a = (playerVec - projectile.Center);
                float aLen = a.Length();
                float speed = (float)Math.Sqrt((GM * m / (Focus - projectile.Center).Length() - GM * m / aLen) * 2 / m);
                Vector2 velocity = a.SafeNormalize(Vector2.Zero)
                    .RotatedBy(Main.rand.Next(new int[] { -1, 1 }) * MathHelper.PiOver2)
                    * speed;
                T = 2 * MathHelper.Pi * (float)Math.Sqrt(aLen * aLen * aLen / GM);
                Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<LMEllipseMoon>(),
                    projectile.damage, 0f, Main.myPlayer, projectile.whoAmI);
            }
            if (projectile.localAI[0] >= 30 + T)
            {
                projectile.localAI[1] = 1;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color = Color.BlueViolet * projectile.Opacity;
            projectile.DrawAim(spriteBatch, playerVec, color);
            return base.PreDraw(spriteBatch, lightColor);
        }
    }
    public class LMEllipseMoon:LMPlanetMoon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ellipse Moon");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 41;
            projectile.height = 41;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;

            projectile.timeLeft = 450;
            projectile.extraUpdates = 0;
            cooldownSlot = 1;
            projectile.penetrate = -1;

            projectile.scale = 1.5f;
        }

        public override void AI()
        {
            if (Util.CheckProjAlive<LMEllipseMoonAim>((int)projectile.ai[0]) && Main.projectile[(int)projectile.ai[0]].localAI[1] == 0) 
            {
                Projectile parent = Main.projectile[(int)projectile.ai[0]];
                LMEllipseMoonAim Aim = (parent.modProjectile as LMEllipseMoonAim);
                projectile.velocity += (projectile.Center - Aim.Focus).SafeNormalize(Vector2.Zero)
                    * LMEllipseMoonAim.GM / (projectile.Center - Aim.Focus).LengthSquared();
            }
            else
            {
                projectile.Kill();
            }
        }
    }
}
