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
    public class LMAimLine:ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aim line");
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
            projectile.timeLeft = 90;
        }
        public override void AI()
        {
            if (projectile.timeLeft <= 20)
            {
                projectile.alpha += 25;
                if (projectile.alpha > 255) projectile.alpha = 255;
            }
            else
            {
                projectile.alpha -= 25;
                if (projectile.alpha < 0) projectile.alpha = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 endpoint = new Vector2(projectile.ai[0], projectile.ai[1]);
            projectile.DrawAim(spriteBatch, endpoint, Color.Lerp(Color.CornflowerBlue, Color.BlueViolet, (float)Math.Sin(MathHelper.Pi / 15 * projectile.timeLeft)) * projectile.Opacity);
            return false;
        }
    }
}
