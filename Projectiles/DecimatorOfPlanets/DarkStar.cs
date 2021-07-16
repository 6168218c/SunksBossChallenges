using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SunksBossChallenges.Projectiles.DecimatorOfPlanets
{
    //Original Author:FargoWilta(https://github.com/FargoWilta)
    public class DarkStar:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FallingStar;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Star");

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            //projectile.light = 1f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 900;
            projectile.hostile = true;
        }

        public override void AI()
        {
            if (projectile.soundDelay == 0 && projectile.ai[0] != 2f)
            {
                projectile.soundDelay = 60 + Main.rand.Next(60);
                Main.PlaySound(SoundID.Item9, projectile.position);
            }

            if (projectile.ai[0] == 1f)
            {
                if (projectile.localAI[1] == 0)
                {
                    projectile.timeLeft = 180;
                }
                projectile.localAI[1]++;
                if (projectile.velocity.Compare(projectile.ai[1]) < 0)
                    projectile.velocity *= 1.05f;
            }
            else if (projectile.ai[0] == 3f)
            {
                if (projectile.localAI[1] == 0)
                {
                    projectile.timeLeft = 180;
                }
                projectile.localAI[1]++;
                if (projectile.localAI[1] % 60 <= 20)
                {
                    projectile.velocity *= 0.95f;
                }
                else
                {
                    projectile.velocity *= 1.035f;
                }
            }

            if (projectile.localAI[0] == 0)
                projectile.localAI[0] = 1f;
            projectile.alpha += (int)(25.0 * projectile.localAI[0]);
            if (projectile.alpha > 200)
            {
                projectile.alpha = 200;
                projectile.localAI[0] = -1f;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
                projectile.localAI[0] = 1f;
            }

            projectile.rotation = projectile.rotation + (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;

            if (Main.rand.Next(30) == 0)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Enchanted_Pink, (float)(projectile.velocity.X * 0.5), (float)(projectile.velocity.Y * 0.5), 150, default, 1.2f);

            Lighting.AddLight(projectile.Center, 0.9f, 0.8f, 0.1f);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.ai[0] == 2f)
                return;
            Main.PlaySound(SoundID.Item10, projectile.position);
            int num1 = 10;
            int num2 = 3;

            for (int index = 0; index < num1; ++index)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Enchanted_Pink, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
            for (int index = 0; index < num2; ++index)
            {
                int Type = Main.rand.Next(16, 18);
                if (projectile.type == 503)
                    Type = 16;
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Type, 1f);
            }

            for (int index = 0; index < 10; ++index)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Enchanted_Gold, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 150, new Color(), 1.2f);
            for (int index = 0; index < 3; ++index)
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D glow = mod.GetTexture("Projectiles/DecimatorOfPlanets/DarkStar_Glow");
            int rect1 = glow.Height / Main.projFrames[projectile.type];
            int rect2 = rect1 * projectile.frame;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(180, 100, 180, 150), Color.Transparent, 0.8f);
            Vector2 drawCenter = projectile.Center - (projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);

            Main.spriteBatch.Draw(glow, drawCenter - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),//create small, non transparent trail texture
                   projectile.GetAlpha(lightColor), projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, projectile.scale / 2, SpriteEffects.None, 0f);

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the projectile
            {
                Vector2 drawCenter2 = drawCenter + (projectile.velocity.SafeNormalize(Vector2.UnitX) * 12).RotatedBy(MathHelper.Pi / 5 - (i * MathHelper.Pi / 5)); //use a normalized version of the projectile's velocity to offset it at different angles
                drawCenter2 -= (projectile.velocity.SafeNormalize(Vector2.UnitX) * 12); //then move it backwards
                Main.spriteBatch.Draw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, projectile.scale, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++) //reused betsy fireball scaling trail thing
            {
                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                float scale = projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i] - (projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);
                Main.spriteBatch.Draw(glow, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale, SpriteEffects.None, 0f);
            }

            if (projectile.ai[0] == 1)
            {
                int timer = (int)projectile.localAI[1];
                if (timer <= 45)
                {
                    Color alpha = Color.BlueViolet;
                    if (timer <= 10)
                    {
                        alpha *= timer / 10f;
                    }
                    else if (timer >= 30 && timer <= 45)
                    {
                        alpha *= (45 - timer) / 15f;
                    }
                    projectile.DrawAim(spriteBatch, projectile.Center + projectile.velocity.SafeNormalize(Vector2.Zero) * 3600, alpha);
                }
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
        }
    }
}
