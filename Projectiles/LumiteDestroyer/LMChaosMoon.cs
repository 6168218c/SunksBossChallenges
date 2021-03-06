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
    public class LMChaosMoon:ModProjectile
    {
        float planetDistance = 24;
        readonly float GFactor = 3200;
        float maxSpeed = 20f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Moon");
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
            /// AI Contents:
            ///     ai[0]:should die
            ///     ai[1]:weight

            if (projectile.ai[0] == 1)
            {
                projectile.localAI[0]++;
                projectile.SlowDown(0.8f);
                if (projectile.localAI[0] >= 2)
                {
                    projectile.Kill();
                }
                return;
            }

            projectile.Loomup(3);
			if (projectile.alpha != 0)
			{
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Vortex, 0f, 0f, 100, default, 2f);
					dust.noGravity = true;
					dust.noLight = true;
					dust.color = Color.Pink;
				}
			}
            bool othersLeft = false;
            projectile.rotation += 0.015f * projectile.velocity.Length();

            int index = NPC.FindFirstNPC(ModContent.NPCType<LumiteDestroyerHead>());
            if (index != -1) 
            {
                NPC head = Main.npc[index];
                Vector2 center = (head.modNPC as LumiteDestroyerHead).spinCenter;
                if (Vector2.Distance(projectile.Center, center) > LumiteDestroyerArguments.R)
                {
                    projectile.ai[0] = 1;
                    return;
                }
                foreach(int item in (head.modNPC as LumiteDestroyerHead).chaosPlanets)
                {
                    if (item != -1 && Main.projectile[item].whoAmI != projectile.whoAmI)
                    {
                        if((Main.projectile[item].type==projectile.type)&& (Main.projectile[item].ai[0] != 1 || Main.projectile[item].active))
                        {
                            if (Vector2.Distance(projectile.Center, Main.projectile[item].Center) < planetDistance)
                            {
                                projectile.ai[0] = Main.projectile[item].ai[0] = 1;
                                return;
                            }
                            if (Main.projectile[item].ai[0] != 1)
                            {
                                othersLeft = true;
                                var force= Vector2.Normalize(Main.projectile[item].Center - projectile.Center) * GFactor * projectile.ai[1] * Main.projectile[item].ai[1]
                                        / ((Main.projectile[item].Center - projectile.Center).LengthSquared());
#if DEBUG
                                if (force.HasNaNs())
                                    System.Diagnostics.Debugger.Break();
#endif
                                projectile.velocity += (force / projectile.ai[1]);
                            }
                            else
                            {
                                projectile.ai[0] = 1;//weird code to make 3 explode at once
                            }
                        }
                    }
                }
            }
            if (projectile.velocity.Compare(1.5f) < 0) projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * 1.5f;
            if (!othersLeft)
                projectile.velocity = Vector2.Normalize(projectile.velocity) * maxSpeed;

            if (projectile.Center.HasNaNs() || projectile.velocity.HasNaNs())
                System.Diagnostics.Debugger.Break();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color glow = new Color(Main.DiscoR + 210, Main.DiscoG + 210, Main.DiscoB + 210);
            Color glow2 = new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50) * projectile.Opacity;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item89, projectile.position);

            Vector2 size = new Vector2(40, 40);
            Vector2 spawnPos = projectile.Center;
            spawnPos.X -= size.X / 2;
            spawnPos.Y -= size.Y / 2;

            for (int num615 = 0; num615 < 45; num615++)
            {
                int num616 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num616].velocity *= 1.4f;
            }

            for (int num617 = 0; num617 < 60; num617++)
            {
                int num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[num618].noGravity = true;
                Main.dust[num618].velocity *= 7f;
                num618 = Dust.NewDust(spawnPos, (int)size.X, (int)size.Y, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num618].velocity *= 3f;
            }

            for (int num619 = 0; num619 < 3; num619++)
            {
                float scaleFactor9 = 0.4f;
                if (num619 == 1) scaleFactor9 = 0.8f;
                int num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore97 = Main.gore[num620];
                gore97.velocity.X++;
                Gore gore98 = Main.gore[num620];
                gore98.velocity.Y++;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore99 = Main.gore[num620];
                gore99.velocity.X--;
                Gore gore100 = Main.gore[num620];
                gore100.velocity.Y++;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore101 = Main.gore[num620];
                gore101.velocity.X++;
                Gore gore102 = Main.gore[num620];
                gore102.velocity.Y--;
                num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore103 = Main.gore[num620];
                gore103.velocity.X--;
                Gore gore104 = Main.gore[num620];
                gore104.velocity.Y--;
            }


            for (int k = 0; k < 40; k++) //make visual dust
            {
                Vector2 dustPos = spawnPos;
                dustPos.X += Main.rand.Next((int)size.X);
                dustPos.Y += Main.rand.Next((int)size.Y);

                for (int i = 0; i < 30; i++)
                {
                    int dust = Dust.NewDust(dustPos, 32, 32, DustID.Smoke, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                float scaleFactor9 = 0.5f;
                for (int j = 0; j < 4; j++)
                {
                    int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[gore].velocity *= scaleFactor9;
                    Main.gore[gore].velocity.X += 1f;
                    Main.gore[gore].velocity.Y += 1f;
                }
            }


            const int num226 = 80;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 40f;
                vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
            base.Kill(timeLeft);


            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                const int max = 12;
                const float rotationInterval = 2f * (float)Math.PI / max;
                Vector2 speed = new Vector2(0f, 2f).RotatedBy(projectile.rotation);
                for (int i = 0; i < max; i++)
                    Projectile.NewProjectile(projectile.Center, speed.RotatedBy(rotationInterval * i),
                        ModContent.ProjectileType<DecimatorOfPlanets.DarkStar>(), projectile.damage * 2 / 3, 0f, Main.myPlayer, 1, 36f);
            }
        }
    }
}
