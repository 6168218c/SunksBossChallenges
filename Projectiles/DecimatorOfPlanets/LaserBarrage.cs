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
    public class LaserBarrage:ModProjectile
    {
        const float Speed = 24f;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FallingStar;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Barrage");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 12;
            projectile.timeLeft = 135;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            ///ai:
            ///  [0]:posx
            ///  [1]:posy
            ///localAI:
            ///  [0]:timer
            ///  [1]:reserved
            if (projectile.localAI[0]++ == 0)
            {
                projectile.alpha = 255;
            }
            if (projectile.localAI[0] == 75)
            {
                projectile.alpha = 0;
                Vector2 target = new Vector2(projectile.ai[0], projectile.ai[1]) - projectile.Center;
                projectile.velocity = Vector2.Normalize(target) * Speed * 1.6F;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
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
            Texture2D aimTexture = mod.GetTexture("Projectiles/DecimatorOfPlanets/LaserBarrageAim");
            if (projectile.localAI[0] >= 15 && projectile.localAI[0] <= 60)
            {
                Vector2 endpoint = new Vector2(projectile.ai[0], projectile.ai[1]);
                endpoint += endpoint - projectile.Center;
                Vector2 unit = endpoint - projectile.Center;
                float length = unit.Length();
                unit.Normalize();
                for (int k = 0; k <= length; k += 4)
                {
                    Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                    Color alphaCenter = (((int)projectile.localAI[0] / 5) % 2 == 0) ? Color.CornflowerBlue : Color.BlueViolet;
                    spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                }
            }
            return base.PreDraw(spriteBatch, lightColor);
        }

        public override bool CanHitPlayer(Player target)
        {
            if (projectile.localAI[0] < 75) 
            {
                return false;
            }
            return true;
        }
    }
}
