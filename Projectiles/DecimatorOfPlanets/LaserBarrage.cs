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
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LaserBarrageAim>(),
                        0, 0f, Main.myPlayer, 0, projectile.whoAmI);
                }
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

        public override bool CanHitPlayer(Player target)
        {
            if (projectile.localAI[0] < 75) 
            {
                return false;
            }
            return true;
        }
    }

    public class LaserBarrageAim : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.timeLeft = 90;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 15;
        }

        public override void AI()
        {
            ///ai:
            ///  [0]:timer
            ///  [1]:parent
            projectile.ai[0]++;
            projectile.Name = "Laser Barrage Aim";

            if (projectile.ai[0] < 20)
            {
                projectile.alpha = (int)(255 * (20 - projectile.ai[0]) / 20);
                if (projectile.alpha < 0) projectile.alpha = 0;
            }
            else
            {
                projectile.alpha = (int)(255 * (projectile.ai[0] - 20) / 25);
                if (projectile.alpha >255) projectile.alpha = 255;
            }

            if (projectile.ai[0] >= 60)
            {
                projectile.Kill();
            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Projectile parent = Main.projectile[(int)projectile.ai[1]];
            Vector2 endpoint = new Vector2(parent.ai[0], parent.ai[1]);
            endpoint += endpoint - projectile.Center;
            Vector2 unit = endpoint - projectile.Center;
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 4f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                Color alphaCenter = new Color(0, 153, 230) * ((255 - projectile.alpha) / 255f);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                Vector2 surroundPos = Vector2.Normalize(drawPos) * 4;
                surroundPos = surroundPos.RotatedBy(MathHelper.PiOver2);
                Color colorSur = new Color(0, 230, 230) * ((255 - projectile.alpha) / 255f);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], surroundPos + drawPos, null, colorSur, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
                surroundPos = surroundPos.RotatedBy(Math.PI);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], surroundPos + drawPos, null, colorSur, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
