using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SunksBossChallenges.Projectiles
{
    public class WarningMark:ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warning Mark");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.alpha = 0;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.scale = 0.125f;
        }
        public override void AI()
        {
            if (projectile.localAI[1] == 1)
            {
                projectile.alpha += 25;
                if (projectile.alpha > 255)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                    return;
                }
            }
            projectile.localAI[0]++;

            if (projectile.localAI[0] % 6 == 0)
            {
                projectile.alpha = 255 - projectile.alpha;
            }

            if (projectile.localAI[0] >= projectile.ai[0])
            {
                projectile.localAI[1] = 1;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Yellow * projectile.Opacity;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
