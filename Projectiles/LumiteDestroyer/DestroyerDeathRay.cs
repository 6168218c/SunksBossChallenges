using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Graphics.Effects;
using Terraria.Localization;
using SunksBossChallenges.NPCs.LumiteDestroyer;
using Terraria.Enums;

namespace SunksBossChallenges.Projectiles.LumiteDestroyer
{
    public class DestroyerDeathRay:ModProjectile
    {
        float rotateFactor = 0;
        int Timer = 0;
        float length = 0;
        //float rotation;
        int maxTime { get; set; } = 480;
        float transparency => 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Deathray");
        }
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;

            cooldownSlot = 1; //not in warning line, test?
            projectile.hide = true; //fixes weird issues on spawn with scaling
        }
        public override void AI()
        {
            maxTime = (int)projectile.ai[0];
            Vector2? vector78 = null;
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            if (Timer == 0)
            {
                rotateFactor = projectile.velocity.Length();
                projectile.velocity.Normalize();
            }
            if (Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].type == ModContent.NPCType<LumiteDestroyerHead>())
            {
                //Vector2 value21 = new Vector2(27f, 59f);
                //Vector2 fireFrom = new Vector2(Main.npc[(int)projectile.ai[1]].Center.X, Main.npc[(int)projectile.ai[1]].Center.Y);
                //Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)projectile.ai[1]].localAI[2].ToRotationVector2(), value21 * Main.npc[(int)projectile.ai[1]].localAI[3]);
                //projectile.position = fireFrom + value22 - new Vector2(projectile.width, projectile.height) / 2f;
                Vector2 offset = new Vector2(Main.npc[(int)projectile.ai[1]].width, 0).RotatedBy(Main.npc[(int)projectile.ai[1]].rotation);
                if (projectile.localAI[1] != 1f && projectile.localAI[1] != 2f) 
                    projectile.Center = Main.npc[(int)projectile.ai[1]].Center + offset;
            }
            else if(Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].type == ModContent.NPCType<LumiteDestroyerBody>())
            {
                //Vector2 value21 = new Vector2(27f, 59f);
                //Vector2 fireFrom = new Vector2(Main.npc[(int)projectile.ai[1]].Center.X, Main.npc[(int)projectile.ai[1]].Center.Y);
                //Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)projectile.ai[1]].localAI[2].ToRotationVector2(), value21 * Main.npc[(int)projectile.ai[1]].localAI[3]);
                //projectile.position = fireFrom + value22 - new Vector2(projectile.width, projectile.height) / 2f;
                Vector2 offset = new Vector2(Main.npc[(int)projectile.ai[1]].width, 0).RotatedBy(Main.npc[(int)projectile.ai[1]].rotation);
                if (projectile.localAI[1] != 1f && projectile.localAI[1] != 2f)
                    projectile.Center = Main.npc[(int)projectile.ai[1]].Center + offset;
            }
            else
            {
                projectile.Kill();
                return;
            }
            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
            {
                projectile.velocity = -Vector2.UnitY;
            }
            float num801 = 1f;
            if (projectile.localAI[1] == 1)
            {
                if (this.Timer == 60f)
                {
                    Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
                }
                if (this.Timer <= 60f)
                {
                    num801 = 0.1f;
                }
                else
                {
                    num801 = 1f;
                }
            }
            else
            {
                if (this.Timer == 0f && projectile.localAI[1] != 2)
                {
                    Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
                }
            }
            if (projectile.localAI[0] == 0f)
                this.Timer += 1;
            if (this.Timer >= maxTime)
            {
                projectile.Kill();
                return;
            }
            projectile.scale = (float)Math.Sin(this.Timer * 3.14159274f / maxTime) * 10f * num801;
            if (projectile.scale > num801)
            {
                projectile.scale = num801;
            }
            if (projectile.localAI[1] == 1f)
            {
                projectile.velocity = projectile.velocity.RotatedBy(0.01 * rotateFactor);
                float num804 = projectile.velocity.ToRotation();
                projectile.rotation = num804 - 1.57079637f;
                projectile.velocity = num804.ToRotationVector2();
            }
            else
            {
                /*float num804 = Main.npc[(int)projectile.ai[1]].rotation + 1.57079637f;
                projectile.rotation = num804;
                num804 -= MathHelper.Pi;
                projectile.velocity = num804.ToRotationVector2();*/
                float num804 = projectile.velocity.ToRotation();
                projectile.rotation = num804 - 1.57079637f;
                projectile.velocity = num804.ToRotationVector2();
            }
            float num805 = 3f;
            float num806 = (float)projectile.width;
            Vector2 samplingPoint = projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 2400f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            this.length = MathHelper.Lerp(this.length, num807, amount);
            Vector2 vector79 = projectile.Center + projectile.velocity * (this.length - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.CopperCoin, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.Next(5) == 0)
            {
                Vector2 value29 = projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
        }

        public override void PostAI()
        {
            projectile.hide = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = Main.projectileTexture[projectile.type];
            Texture2D texture2D20 = mod.GetTexture("Projectiles/LumiteDestroyer/DestroyerDeathRay2");
            Texture2D texture2D21 = mod.GetTexture("Projectiles/LumiteDestroyer/DestroyerDeathRay3");
            float num223 = this.length;
            Microsoft.Xna.Framework.Color color44 = new Microsoft.Xna.Framework.Color(255, 255, 255, 0) * 0.8f;
            color44 = Color.Lerp(color44, Color.Transparent, transparency);
            SpriteBatch arg_ABD8_0 = Main.spriteBatch;
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = projectile.Center - Main.screenPosition;
            Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = null;
            arg_ABD8_0.Draw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, color44, projectile.rotation, texture2D19.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            num223 -= (float)(texture2D19.Height / 2 + texture2D21.Height) * projectile.scale;
            Vector2 value20 = projectile.Center;
            value20 += projectile.velocity * projectile.scale * (float)texture2D19.Height / 2f;
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
                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, projectile.rotation, new Vector2((float)(rectangle7.Width / 2), 0f), projectile.scale, SpriteEffects.None, 0f);
                    num224 += (float)rectangle7.Height * projectile.scale;
                    value20 += projectile.velocity * (float)rectangle7.Height * projectile.scale;
                    rectangle7.Y += 16;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            SpriteBatch arg_AE2D_0 = Main.spriteBatch;
            Texture2D arg_AE2D_1 = texture2D21;
            Vector2 arg_AE2D_2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            arg_AE2D_0.Draw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, color44, projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * this.length, (float)projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.localAI[1] == 1 && Timer <= 60)
            {
                return false;
            }
            else
            {
                if (projHitbox.Intersects(targetHitbox))
                {
                    return true;
                }
                float num6 = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * this.length, 22f * projectile.scale, ref num6))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(target, damage, knockback, crit);
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

        public override bool ShouldUpdatePosition() => false;
    }
}
