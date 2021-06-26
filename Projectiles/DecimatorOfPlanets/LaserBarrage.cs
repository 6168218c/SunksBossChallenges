using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SunksBossChallenges.Projectiles.DecimatorOfPlanets
{
    public class LaserBarrage:ModProjectile
    {
        const float Speed = 24f;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FallingStar;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Barrage");

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 12;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            ///ai:
            ///  [0]:posx
            ///  [1]:posy
            ///localAI:
            ///  [0]:timer
            ///  [1]:reserved
			if (projectile.timeLeft <= 30)
			{
				projectile.alpha += 9;
				if (projectile.alpha > 255) projectile.alpha = 255;
			}
            if (projectile.localAI[0]++ == 0)
            {
                projectile.alpha = 255;
            }
            if (projectile.localAI[0] == 60)
            {
                Vector2 target = new Vector2(projectile.ai[0], projectile.ai[1]) - projectile.Center;
                projectile.velocity = Vector2.Normalize(target) * Speed * 1.6f;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            if (projectile.localAI[0] >= 60)//fade in
            {
                projectile.alpha -= 42;
                if (projectile.alpha < 0) projectile.alpha = 0;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D aimTexture = mod.GetTexture("Projectiles/AimLine");
            if (projectile.localAI[0] >= 0 && projectile.localAI[0] <= 45)
            {
                Vector2 endpoint = new Vector2(projectile.ai[0], projectile.ai[1]);
                endpoint += endpoint - projectile.Center;
                Vector2 unit = endpoint - projectile.Center;
                float length = unit.Length();
                unit.Normalize();
                for (int k = 0; k <= length; k += 4)
                {
                    Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                    Color alphaCenter = (((int)projectile.localAI[0] / 15) % 2 == 0) ? Color.CornflowerBlue : Color.BlueViolet;
                    if (projectile.localAI[0] <= 10)
                    {
                        alphaCenter *= projectile.localAI[0] / 10f;
                    }
                    if (projectile.localAI[0] >= 30)
                    {
                        alphaCenter *= (45 - projectile.localAI[0]) / 15f;
                    }
                    spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                }
            }
            if (projectile.localAI[0] >= 60)//fade in
            {
                Texture2D glow = mod.GetTexture("Projectiles/DecimatorOfPlanets/DarkStar_Glow");
                int rect1 = glow.Height / Main.projFrames[projectile.type];
                int rect2 = rect1 * projectile.frame;
                Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
                Vector2 gloworigin2 = glowrectangle.Size() / 2f;
                Color glowcolor = Color.Lerp(Color.Yellow, Color.Transparent, 0.8f);
                Vector2 drawCenter = projectile.Center - (projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);

                Main.spriteBatch.Draw(glow, drawCenter - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),//create small, non transparent trail texture
                       projectile.GetAlpha(lightColor), projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, projectile.scale, SpriteEffects.None, 0f);
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
            }
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (projectile.localAI[0] < 75 || projectile.timeLeft <= 30) 
            {
                return false;
            }
            return true;
        }
    }
}
