using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.Projectiles.DecimatorOfPlanets;
using Terraria.Graphics.Effects;
using Terraria.Localization;

namespace SunksBossChallenges.NPCs.LumiteTwins
{
	public class LumiteRetinazer : ModNPC
	{
		protected float Phase { get => npc.ai[0]; set => npc.ai[0] = value; }
        public override string Texture => "Terraria/NPC_" + NPCID.Retinazer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(@"LR-003 ""Apollo""");
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.Retinazer];
			NPCID.Sets.TrailingMode[npc.type] = NPCID.Sets.TrailingMode[NPCID.Retinazer];
			NPCID.Sets.TrailCacheLength[npc.type] = NPCID.Sets.TrailCacheLength[NPCID.Retinazer];
			NPCID.Sets.BossHeadTextures[npc.type] = NPCID.Sets.BossHeadTextures[NPCID.Retinazer];
		}

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.Retinazer);
            npc.aiStyle = -1;
            animationType = NPCID.Retinazer;
			music = MusicID.Boss2;
        }
        public override void AI()
        {
			/**/

			string aiDump = $"ai:{string.Join(",", npc.ai.Select(fl => $"{fl}"))}";
			Main.NewText($"{aiDump}");
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest();
			}
			var player = Main.player[npc.target];
			bool targetDead = player.dead;

			var vecToPlayer = player.Center - npc.Center;

			float num379 = npc.position.X + (float)(npc.width / 2) - Main.player[npc.target].position.X - (float)(Main.player[npc.target].width / 2);
			float num380 = npc.position.Y + (float)npc.height - 59f - Main.player[npc.target].position.Y - (float)(Main.player[npc.target].height / 2);
			float distRotation = (float)Math.Atan2(num380, num379) + 1.57f;
			if (distRotation < 0f)
			{
				distRotation += 6.283f;
			}
			else if ((double)distRotation > 6.283)
			{
				distRotation -= 6.283f;
			}
			float num382 = 0.1f;
			if (npc.rotation < distRotation)
			{
				if ((double)(distRotation - npc.rotation) > 3.1415)
				{
					npc.rotation -= num382;
				}
				else
				{
					npc.rotation += num382;
				}
			}
			else if (npc.rotation > distRotation)
			{
				if ((double)(npc.rotation - distRotation) > 3.1415)
				{
					npc.rotation += num382;
				}
				else
				{
					npc.rotation -= num382;
				}
			}
			if (npc.rotation > distRotation - num382 && npc.rotation < distRotation + num382)
			{
				npc.rotation = distRotation;
			}
			if (npc.rotation < 0f)
			{
				npc.rotation += 6.283f;
			}
			else if ((double)npc.rotation > 6.283)
			{
				npc.rotation -= 6.283f;
			}
			if (npc.rotation > distRotation - num382 && npc.rotation < distRotation + num382)
			{
				npc.rotation = distRotation;
			}
			if (Main.rand.Next(5) == 0)
			{
				int num383 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f);
				Main.dust[num383].velocity.X *= 0.5f;
				Main.dust[num383].velocity.Y *= 0.1f;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime && !targetDead && npc.timeLeft < 10)
			{
				for (int num384 = 0; num384 < 200; num384++)
				{
					if (num384 != npc.whoAmI && Main.npc[num384].active && (Main.npc[num384].type == ModContent.NPCType<LumiteRetinazer>() || Main.npc[num384].type == ModContent.NPCType<LumiteSpazmatism>()) && Main.npc[num384].timeLeft - 1 > npc.timeLeft)
					{
						npc.timeLeft = Main.npc[num384].timeLeft - 1;
					}
				}
			}
			if (Main.dayTime || targetDead)
			{
				npc.velocity.Y -= 0.04f;
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
				return;
			}
			if (this.Phase == 0f)//phase 1
			{
				if (npc.ai[1] == 0f)
				{
					float num385 = 7f;
					float num386 = 0.1f;
					if (Main.expertMode)
					{
						num385 = 8.25f;
						num386 = 0.115f;
					}
					int num387 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
					{
						num387 = -1;
					}
					Vector2 vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num388 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num387 * 300) - vector37.X;
					float num389 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector37.Y;
					float num390 = (float)Math.Sqrt(num388 * num388 + num389 * num389);
					float num391 = num390;
					num390 = num385 / num390;
					num388 *= num390;
					num389 *= num390;
					if (npc.velocity.X < num388)
					{
						npc.velocity.X += num386;
						if (npc.velocity.X < 0f && num388 > 0f)
						{
							npc.velocity.X += num386;
						}
					}
					else if (npc.velocity.X > num388)
					{
						npc.velocity.X -= num386;
						if (npc.velocity.X > 0f && num388 < 0f)
						{
							npc.velocity.X -= num386;
						}
					}
					if (npc.velocity.Y < num389)
					{
						npc.velocity.Y += num386;
						if (npc.velocity.Y < 0f && num389 > 0f)
						{
							npc.velocity.Y += num386;
						}
					}
					else if (npc.velocity.Y > num389)
					{
						npc.velocity.Y -= num386;
						if (npc.velocity.Y > 0f && num389 < 0f)
						{
							npc.velocity.Y -= num386;
						}
					}
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 600f)
					{
						npc.ai[1] = 1f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.target = 255;
						npc.netUpdate = true;
					}
					else if (npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && num391 < 400f)
					{
						if (!Main.player[npc.target].dead)
						{
							npc.ai[3] += 1f;
							if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.9)
							{
								npc.ai[3] += 0.3f;
							}
							if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.8)
							{
								npc.ai[3] += 0.3f;
							}
							if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.7)
							{
								npc.ai[3] += 0.3f;
							}
							if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.6)
							{
								npc.ai[3] += 0.3f;
							}
						}
						if (npc.ai[3] >= 60f)
						{
							npc.ai[3] = 0f;
							vector37 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num388 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector37.X;
							num389 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector37.Y;
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								float num392 = 9f;
								int num393 = 20;
								int num394 = 83;
								if (Main.expertMode)
								{
									num392 = 10.5f;
									num393 = 19;
								}
								num390 = (float)Math.Sqrt(num388 * num388 + num389 * num389);
								num390 = num392 / num390;
								num388 *= num390;
								num389 *= num390;
								num388 += (float)Main.rand.Next(-40, 41) * 0.08f;
								num389 += (float)Main.rand.Next(-40, 41) * 0.08f;
								vector37.X += num388 * 15f;
								vector37.Y += num389 * 15f;
								int num395 = Projectile.NewProjectile(vector37.X, vector37.Y, num388, num389, num394, num393, 0f, Main.myPlayer);
							}
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.rotation = distRotation;
					float num396 = 12f;
					if (Main.expertMode)
					{
						num396 = 15f;
					}
					Vector2 vector38 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num397 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector38.X;
					float num398 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector38.Y;
					float num399 = (float)Math.Sqrt(num397 * num397 + num398 * num398);
					num399 = num396 / num399;
					npc.velocity.X = num397 * num399;
					npc.velocity.Y = num398 * num399;
					npc.ai[1] = 2f;
				}
				else if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 25f)
					{
						npc.velocity.X *= 0.96f;
						npc.velocity.Y *= 0.96f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
						{
							npc.velocity.X = 0f;
						}
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
						{
							npc.velocity.Y = 0f;
						}
					}
					else
					{
						npc.rotation = distRotation;
					}
					if (npc.ai[2] >= 70f)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = distRotation;
						if (npc.ai[3] >= 4f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
						}
						else
						{
							npc.ai[1] = 1f;
						}
					}
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.4)
				{
					this.Phase = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
				return;
			}
			if (this.Phase == 1f || this.Phase == 2f)
			{
				if (this.Phase == 1f)
				{
					npc.ai[2] += 0.005f;
					if ((double)npc.ai[2] > 0.5)
					{
						npc.ai[2] = 0.5f;
					}
				}
				else
				{
					npc.ai[2] -= 0.005f;
					if (npc.ai[2] < 0f)
					{
						npc.ai[2] = 0f;
					}
				}
				npc.rotation += npc.ai[2];
				npc.ai[1] += 1f;
				if (npc.ai[1] == 100f)
				{
					this.Phase += 1f;
					npc.ai[1] = 0f;
					if (this.Phase == 3f)
					{
						npc.ai[2] = 0f;
					}
					else
					{
						Main.PlaySound(SoundID.NPCHit, (int)npc.position.X, (int)npc.position.Y);
						for (int num400 = 0; num400 < 2; num400++)
						{
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 143);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6);
						}
						for (int num401 = 0; num401 < 20; num401++)
						{
							Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f);
						}
						Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);
					}
				}
				Dust.NewDust(npc.position, npc.width, npc.height, 5, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f);
				npc.velocity.X *= 0.98f;
				npc.velocity.Y *= 0.98f;
				if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
				{
					npc.velocity.X = 0f;
				}
				if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
				{
					npc.velocity.Y = 0f;
				}
				return;
			}
			npc.damage = (int)((double)npc.defDamage * 1.5);
			npc.defense = npc.defDefense + 10;
			npc.HitSound = SoundID.NPCHit4;
			if (npc.ai[1] == 0f)
			{
				float num402 = 8f;
				float num403 = 0.15f;
				if (Main.expertMode)
				{
					num402 = 9.5f;
					num403 = 0.175f;
				}
				Vector2 vector39 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num404 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector39.X;
				float num405 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector39.Y;
				float num406 = (float)Math.Sqrt(num404 * num404 + num405 * num405);
				num406 = num402 / num406;
				num404 *= num406;
				num405 *= num406;
				if (npc.velocity.X < num404)
				{
					npc.velocity.X += num403;
					if (npc.velocity.X < 0f && num404 > 0f)
					{
						npc.velocity.X += num403;
					}
				}
				else if (npc.velocity.X > num404)
				{
					npc.velocity.X -= num403;
					if (npc.velocity.X > 0f && num404 < 0f)
					{
						npc.velocity.X -= num403;
					}
				}
				if (npc.velocity.Y < num405)
				{
					npc.velocity.Y += num403;
					if (npc.velocity.Y < 0f && num405 > 0f)
					{
						npc.velocity.Y += num403;
					}
				}
				else if (npc.velocity.Y > num405)
				{
					npc.velocity.Y -= num403;
					if (npc.velocity.Y > 0f && num405 < 0f)
					{
						npc.velocity.Y -= num403;
					}
				}
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 300f)
				{
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
				vector39 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				num404 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector39.X;
				num405 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector39.Y;
				npc.rotation = (float)Math.Atan2(num405, num404) - 1.57f;
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					return;
				}
				npc.localAI[1] += 1f;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					npc.localAI[1] += 1f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					npc.localAI[1] += 1f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
				{
					npc.localAI[1] += 1f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.1)
				{
					npc.localAI[1] += 2f;
				}
				if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					npc.localAI[1] = 0f;
					float num407 = 8.5f;
					int num408 = 25;
					int num409 = 100;
					if (Main.expertMode)
					{
						num407 = 10f;
						num408 = 23;
					}
					num406 = (float)Math.Sqrt(num404 * num404 + num405 * num405);
					num406 = num407 / num406;
					num404 *= num406;
					num405 *= num406;
					vector39.X += num404 * 15f;
					vector39.Y += num405 * 15f;
					int num410 = Projectile.NewProjectile(vector39.X, vector39.Y, num404, num405, num409, num408, 0f, Main.myPlayer);
				}
				return;
			}
			int num411 = 1;
			if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
			{
				num411 = -1;
			}
			float num412 = 8f;
			float num413 = 0.2f;
			if (Main.expertMode)
			{
				num412 = 9.5f;
				num413 = 0.25f;
			}
			Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num414 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num411 * 340) - vector40.X;
			float num415 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector40.Y;
			float num416 = (float)Math.Sqrt(num414 * num414 + num415 * num415);
			num416 = num412 / num416;
			num414 *= num416;
			num415 *= num416;
			if (npc.velocity.X < num414)
			{
				npc.velocity.X += num413;
				if (npc.velocity.X < 0f && num414 > 0f)
				{
					npc.velocity.X += num413;
				}
			}
			else if (npc.velocity.X > num414)
			{
				npc.velocity.X -= num413;
				if (npc.velocity.X > 0f && num414 < 0f)
				{
					npc.velocity.X -= num413;
				}
			}
			if (npc.velocity.Y < num415)
			{
				npc.velocity.Y += num413;
				if (npc.velocity.Y < 0f && num415 > 0f)
				{
					npc.velocity.Y += num413;
				}
			}
			else if (npc.velocity.Y > num415)
			{
				npc.velocity.Y -= num413;
				if (npc.velocity.Y > 0f && num415 < 0f)
				{
					npc.velocity.Y -= num413;
				}
			}
			vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			num414 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector40.X;
			num415 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector40.Y;
			npc.rotation = (float)Math.Atan2(num415, num414) - 1.57f;
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.localAI[1] += 1f;
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					npc.localAI[1] += 0.5f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					npc.localAI[1] += 0.75f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.25)
				{
					npc.localAI[1] += 1f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.1)
				{
					npc.localAI[1] += 1.5f;
				}
				if (Main.expertMode)
				{
					npc.localAI[1] += 1.5f;
				}
				if (npc.localAI[1] > 60f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					npc.localAI[1] = 0f;
					float num417 = 9f;
					int num418 = 18;
					int num419 = 100;
					if (Main.expertMode)
					{
						num418 = 17;
					}
					num416 = (float)Math.Sqrt(num414 * num414 + num415 * num415);
					num416 = num417 / num416;
					num414 *= num416;
					num415 *= num416;
					vector40.X += num414 * 15f;
					vector40.Y += num415 * 15f;
					int num420 = Projectile.NewProjectile(vector40.X, vector40.Y, num414, num415, num419, num418, 0f, Main.myPlayer);
				}
			}
			npc.ai[2] += 1f;
			if (npc.ai[2] >= 180f)
			{
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
				npc.TargetClosest();
				npc.netUpdate = true;
			}
		}

		protected void setRotation(Vector2 direction)
		{
			npc.rotation = direction.ToRotation() - MathHelper.PiOver2;
		}
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.ScaleExpertStats(numPlayers, bossLifeScale);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Microsoft.Xna.Framework.Rectangle frame4 = npc.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            float num64 = 0f;
            float num65 = Main.NPCAddHeight(NPCID.Retinazer);
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
			var vector10 = new Vector2(55f, 107f);
			for (int num90 = 9; num90 >= 0; num90 -= 2)
            {
                _ = ref npc.oldPos[num90];
                Microsoft.Xna.Framework.Color alpha9 = npc.GetAlpha(drawColor);
                alpha9.R = (byte)(alpha9.R * (10 - num90) / 20);
                alpha9.G = (byte)(alpha9.G * (10 - num90) / 20);
                alpha9.B = (byte)(alpha9.B * (10 - num90) / 20);
                alpha9.A = (byte)(alpha9.A * (10 - num90) / 20);
				Vector2 position21 = npc.oldPos[num90] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
				position21 -= new Vector2(Main.npcTexture[npc.type].Width, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
				position21 += vector10 * npc.scale + new Vector2(0f, num64 - 8f + num65 + npc.gfxOffY);
				Main.spriteBatch.Draw(Main.npcTexture[npc.type], position21, npc.frame, alpha9, npc.rotation, vector10, npc.scale, spriteEffects, 0f);
            }

			Vector2 position23 = npc.Center - Main.screenPosition;
			position23 -= new Vector2(Main.npcTexture[npc.type].Width, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			position23 += vector10 * npc.scale + new Vector2(0f, num64 - 8f + num65 + npc.gfxOffY);
			Main.spriteBatch.Draw(Main.npcTexture[npc.type], position23, frame4, npc.GetAlpha(drawColor), npc.rotation, vector10, npc.scale, spriteEffects, 0f);
            if (npc.color != default(Microsoft.Xna.Framework.Color))
            {
                Main.spriteBatch.Draw(Main.npcTexture[npc.type], position23, frame4, npc.GetColor(drawColor), npc.rotation, vector10, npc.scale, spriteEffects, 0f);
            }
            return false;
        }
    }
}
