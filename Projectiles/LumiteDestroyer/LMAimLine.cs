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
            projectile.timeLeft = 180;
        }
        public override void AI()
        {
            projectile.localAI[0]++;
            if (projectile.localAI[0] >= (projectile.localAI[1] > 15 ? projectile.localAI[1] - 15 : 135))
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
            Color color = Color.Lerp(Color.CornflowerBlue, Color.BlueViolet, (float)Math.Sin(MathHelper.Pi / 15 * projectile.timeLeft)) * projectile.Opacity * 0.5f;
            Texture2D texture2D20 = mod.GetTexture("Projectiles/LumiteDestroyer/DestroyerDeathRay2");
            float num223 = (endpoint - projectile.Center).Length();
            Microsoft.Xna.Framework.Color color44 = color;
            SpriteBatch arg_ABD8_0 = Main.spriteBatch;
            Vector2 arg_ABD8_2 = projectile.Center - Main.screenPosition;            
            Vector2 value20 = projectile.Center;
            var unit = (endpoint - projectile.Center).SafeNormalize(Vector2.UnitY);
            var rotation = unit.ToRotation() - MathHelper.PiOver2;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Microsoft.Xna.Framework.Rectangle rectangle7 = new Microsoft.Xna.Framework.Rectangle(0, 16 * (projectile.timeLeft / 3 % 5), texture2D20.Width, 16);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < (float)rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }
                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, rotation, new Vector2((float)(rectangle7.Width / 2), 0f), projectile.scale, SpriteEffects.None, 0f);
                    num224 += (float)rectangle7.Height * projectile.scale;
                    value20 += unit * (float)rectangle7.Height * projectile.scale;
                    rectangle7.Y += 16;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
        }
    }
}
