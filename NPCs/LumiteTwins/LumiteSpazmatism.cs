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
    public class LumiteSpazmatism:LumTwins
    {
		public override string Texture => "Terraria/NPC_" + NPCID.Spazmatism;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(@"LS-005 ""Zeus""");
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.Spazmatism];
			NPCID.Sets.TrailingMode[npc.type] = NPCID.Sets.TrailingMode[NPCID.Spazmatism];
			NPCID.Sets.TrailCacheLength[npc.type] = NPCID.Sets.TrailCacheLength[NPCID.Spazmatism];
			NPCID.Sets.BossHeadTextures[npc.type] = NPCID.Sets.BossHeadTextures[NPCID.Spazmatism];
		}

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.Spazmatism);
            npc.aiStyle = -1;
            animationType = NPCID.Spazmatism;
            music = MusicID.Boss2;
        }

		public override void AI()
		{
			string aiDump = $"ai:{string.Join(",", npc.ai.Select(fl => $"{fl}"))}";
			//Main.NewText($"{aiDump}");

			if (!CheckBrother<LumiteRetinazer>())
			{
				UpdateBrother<LumiteRetinazer>();
			}

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest();
			}
			var player = Main.player[npc.target];
			bool targetDead = Main.player[npc.target].dead;
            #region RotationHandler
            float num379 = npc.position.X + (float)(npc.width / 2) - Main.player[npc.target].position.X - (float)(Main.player[npc.target].width / 2);
			float num380 = npc.position.Y + (float)npc.height - 59f - Main.player[npc.target].position.Y - (float)(Main.player[npc.target].height / 2);
			if (CheckBrother<LumiteRetinazer>() && this.Phase == 0 && npc.ai[1] == 3)
			{
				num379 = npc.position.X + (float)(npc.width / 2) - Main.npc[brotherId].Center.X;
				num380 = npc.position.Y + (float)npc.height - 59f - Main.npc[brotherId].Center.Y;
			}
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
            #endregion
            if (Main.rand.Next(5) == 0)
			{
				int num425 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f);
				Main.dust[num425].velocity.X *= 0.5f;
				Main.dust[num425].velocity.Y *= 0.1f;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient && !Main.dayTime && !targetDead && npc.timeLeft < 10)
			{
				for (int num426 = 0; num426 < 200; num426++)
				{
					if (num426 != npc.whoAmI && Main.npc[num426].active && (Main.npc[num426].type == ModContent.NPCType<LumiteSpazmatism>() || Main.npc[num426].type == ModContent.NPCType<LumiteRetinazer>()) && Main.npc[num426].timeLeft - 1 > npc.timeLeft)
					{
						npc.timeLeft = Main.npc[num426].timeLeft - 1;
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
			if (this.Phase == 0f)
			{
                #region Phase 1
                if (npc.ai[1] == 0f)
				{
					npc.TargetClosest();
					float num427 = 12f;
					float num428 = 0.4f;
					int num429 = 1;
					if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
					{
						num429 = -1;
					}
					Vector2 vector41 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num430 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num429 * 400) - vector41.X;
					float num431 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector41.Y;
					float num432 = (float)Math.Sqrt(num430 * num430 + num431 * num431);
					float num433 = num432;
					num432 = num427 / num432;
					num430 *= num432;
					num431 *= num432;
					if (npc.velocity.X < num430)
					{
						npc.velocity.X += num428;
						if (npc.velocity.X < 0f && num430 > 0f)
						{
							npc.velocity.X += num428;
						}
					}
					else if (npc.velocity.X > num430)
					{
						npc.velocity.X -= num428;
						if (npc.velocity.X > 0f && num430 < 0f)
						{
							npc.velocity.X -= num428;
						}
					}
					if (npc.velocity.Y < num431)
					{
						npc.velocity.Y += num428;
						if (npc.velocity.Y < 0f && num431 > 0f)
						{
							npc.velocity.Y += num428;
						}
					}
					else if (npc.velocity.Y > num431)
					{
						npc.velocity.Y -= num428;
						if (npc.velocity.Y > 0f && num431 < 0f)
						{
							npc.velocity.Y -= num428;
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
					else
					{
						if (!Main.player[npc.target].dead)
						{
							npc.ai[3] += 1f;
							if (Main.expertMode && (double)npc.life < (double)npc.lifeMax * 0.8)
							{
								npc.ai[3] += 0.6f;
							}
						}
						if (npc.ai[3] >= 60f)
						{
							npc.ai[3] = 0f;
							vector41 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num430 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector41.X;
							num431 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector41.Y;
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								float num434 = 12f;
								int num435 = 25;
								int num436 = 96;
								if (Main.expertMode)
								{
									num434 = 14f;
									num435 = 22;
								}
								num432 = (float)Math.Sqrt(num430 * num430 + num431 * num431);
								num432 = num434 / num432;
								num430 *= num432;
								num431 *= num432;
								num430 += (float)Main.rand.Next(-40, 41) * 0.05f;
								num431 += (float)Main.rand.Next(-40, 41) * 0.05f;
								vector41.X += num430 * 4f;
								vector41.Y += num431 * 4f;
								int num437 = Projectile.NewProjectile(vector41.X, vector41.Y, num430, num431, num436, num435, 0f, Main.myPlayer);
							}
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.rotation = distRotation;
					float num438 = 13f;
					if (Main.expertMode)
					{
						if ((double)npc.life < (double)npc.lifeMax * 0.9)
						{
							num438 += 0.5f;
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.8)
						{
							num438 += 0.5f;
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.7)
						{
							num438 += 0.55f;
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.6)
						{
							num438 += 0.6f;
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.5)
						{
							num438 += 0.65f;
						}
					}
					Vector2 vector42 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num439 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector42.X;
					float num440 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector42.Y;
					float num441 = (float)Math.Sqrt(num439 * num439 + num440 * num440);
					num441 = num438 / num441;
					npc.velocity.X = num439 * num441;
					npc.velocity.Y = num440 * num441;
					npc.ai[1] = 2f;
				}
				else if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 8f)
					{
						npc.velocity.X *= 0.9f;
						npc.velocity.Y *= 0.9f;
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
						npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - 1.57f;
					}
					if (npc.ai[2] >= 42f)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = distRotation;
						if (npc.ai[3] >= 10f)
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
				else if (npc.ai[1] == 3f)
                {
					npc.dontTakeDamage = true;
                    if (CheckBrother<LumiteRetinazer>())
                    {
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
						//npc.rotation = distRotation;
						if (Main.npc[brotherId].ai[1] == 3f)
                        {
                            if (npc.ai[2] >= 60)
                            {
								this.Phase = 1f;
								npc.ai[1] = 0f;
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								NewOrBoardcastText($"Connection established", Color.Green);
								npc.netUpdate = true;
							}
						}
						else
						{
							Vector2 pivot = Main.npc[brotherId].Center;
							if (npc.Distance(pivot) > 900f)
								npc.Center = pivot + npc.DirectionFrom(pivot) * 900f;
						}
					}
                    else
                    {
						this.Phase = 1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
                }
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					//this.Phase = 1f;
					npc.ai[1] = 3f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					if (CheckBrother<LumiteRetinazer>())
					{
						if (Main.npc[brotherId].ai[1] != 3f)
						{
							NewOrBoardcastText($"Battle mode on.Sync server started at port 8{npc.whoAmI}", Color.Green);
						}
					}
					npc.netUpdate = true;
				}
				return;
                #endregion
            }
            if (this.Phase == 1f || this.Phase == 2f)
			{
				if (music != mod.GetSoundSlot(SoundType.Music, "Sounds/Music/HellMarch3"))
				{
					music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/HellMarch3");
				}
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
					npc.dontTakeDamage = false;
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
						for (int num442 = 0; num442 < 2; num442++)
						{
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 144);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6);
						}
						for (int num443 = 0; num443 < 20; num443++)
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
			npc.HitSound = SoundID.NPCHit4;
			npc.damage = (int)((double)npc.defDamage * 1.5);
			npc.defense = npc.defDefense + 18;
			if (npc.ai[1] == 0f)
			{
				float num444 = 4f;
				float num445 = 0.1f;
				int num446 = 1;
				if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
				{
					num446 = -1;
				}
				Vector2 vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num447 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num446 * 180) - vector43.X;
				float num448 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector43.Y;
				float num449 = (float)Math.Sqrt(num447 * num447 + num448 * num448);
				if (Main.expertMode)
				{
					if (num449 > 300f)
					{
						num444 += 0.5f;
					}
					if (num449 > 400f)
					{
						num444 += 0.5f;
					}
					if (num449 > 500f)
					{
						num444 += 0.55f;
					}
					if (num449 > 600f)
					{
						num444 += 0.55f;
					}
					if (num449 > 700f)
					{
						num444 += 0.6f;
					}
					if (num449 > 800f)
					{
						num444 += 0.6f;
					}
				}
				num449 = num444 / num449;
				num447 *= num449;
				num448 *= num449;
				if (npc.velocity.X < num447)
				{
					npc.velocity.X += num445;
					if (npc.velocity.X < 0f && num447 > 0f)
					{
						npc.velocity.X += num445;
					}
				}
				else if (npc.velocity.X > num447)
				{
					npc.velocity.X -= num445;
					if (npc.velocity.X > 0f && num447 < 0f)
					{
						npc.velocity.X -= num445;
					}
				}
				if (npc.velocity.Y < num448)
				{
					npc.velocity.Y += num445;
					if (npc.velocity.Y < 0f && num448 > 0f)
					{
						npc.velocity.Y += num445;
					}
				}
				else if (npc.velocity.Y > num448)
				{
					npc.velocity.Y -= num445;
					if (npc.velocity.Y > 0f && num448 < 0f)
					{
						npc.velocity.Y -= num445;
					}
				}
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 400f)
				{
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.target = 255;
					npc.netUpdate = true;
				}
				if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					return;
				}
				npc.localAI[2] += 1f;
				if (npc.localAI[2] > 22f)
				{
					npc.localAI[2] = 0f;
					Main.PlaySound(SoundID.Item34, npc.position);
				}
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
				if (npc.localAI[1] > 8f)
				{
					npc.localAI[1] = 0f;
					float num450 = 6f;
					int num451 = 30;
					if (Main.expertMode)
					{
						num451 = 27;
					}
					vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num447 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector43.X;
					num448 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector43.Y;
					num449 = (float)Math.Sqrt(num447 * num447 + num448 * num448);
					num449 = num450 / num449;
					num447 *= num449;
					num448 *= num449;
					num448 += (float)Main.rand.Next(-40, 41) * 0.01f;
					num447 += (float)Main.rand.Next(-40, 41) * 0.01f;
					num448 += npc.velocity.Y * 0.5f;
					num447 += npc.velocity.X * 0.5f;
					vector43.X -= num447 * 1f;
					vector43.Y -= num448 * 1f;
					int num453 = Projectile.NewProjectile(vector43.X, vector43.Y, num447, num448, ProjectileID.EyeFire, num451, 0f, Main.myPlayer);
				}
			}
			else if (npc.ai[1] == 1f)
			{
				Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);
				npc.rotation = distRotation;
				float num454 = 14f;
				if (Main.expertMode)
				{
					num454 += 2.5f;
				}
				Vector2 vector44 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num455 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector44.X;
				float num456 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector44.Y;
				float num457 = (float)Math.Sqrt(num455 * num455 + num456 * num456);
				num457 = num454 / num457;
				npc.velocity.X = num455 * num457;
				npc.velocity.Y = num456 * num457;
				npc.ai[1] = 2f;
			}
			else
			{
				if (npc.ai[1] != 2f)
				{
					return;
				}
				npc.ai[2] += 1f;
				if (Main.expertMode)
				{
					npc.ai[2] += 0.5f;
				}
				if (npc.ai[2] >= 50f)
				{
					npc.velocity.X *= 0.93f;
					npc.velocity.Y *= 0.93f;
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
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - 1.57f;
				}
				if (npc.ai[2] >= 80f)
				{
					npc.ai[3] += 1f;
					npc.ai[2] = 0f;
					npc.target = 255;
					npc.rotation = distRotation;
					if (npc.ai[3] >= 6f)
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
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
			for (int i = 0; i < 200; i++)
			{
				if (!Main.npc[i].active || npc.whoAmI == i || (Main.npc[i].type != ModContent.NPCType<LumiteRetinazer>()))
				{
					continue;
				}
				float num2 = Main.npc[i].position.X + (float)Main.npc[i].width * 0.5f;
				float num3 = Main.npc[i].position.Y + (float)Main.npc[i].height * 0.5f;
				Vector2 vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
				float num4 = num2 - vector.X;
				float num5 = num3 - vector.Y;
				float rotation = (float)Math.Atan2(num5, num4) - 1.57f;
				bool flag2 = true;
				float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
				if (num6 > 2000f)
				{
					flag2 = false;
				}
				while (flag2)
				{
					num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
					if (num6 < 40f)
					{
						flag2 = false;
						continue;
					}
					num6 = (float)Main.chain12Texture.Height / num6;
					num4 *= num6;
					num5 *= num6;
					vector.X += num4;
					vector.Y += num5;
					num4 = num2 - vector.X;
					num5 = num3 - vector.Y;
					Microsoft.Xna.Framework.Color color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f));
					Main.spriteBatch.Draw(Main.chain12Texture, new Vector2(vector.X - Main.screenPosition.X, vector.Y - Main.screenPosition.Y), new Microsoft.Xna.Framework.Rectangle(0, 0, Main.chain12Texture.Width, Main.chain12Texture.Height), color, rotation, new Vector2((float)Main.chain12Texture.Width * 0.5f, (float)Main.chain12Texture.Height * 0.5f), 1f, SpriteEffects.None, 0f);
				}
			}

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
