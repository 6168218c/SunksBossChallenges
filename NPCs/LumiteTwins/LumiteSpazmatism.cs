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
using Terraria.Utilities;
using SunksBossChallenges.Projectiles.LumiteTwins;
using SunksBossChallenges.Projectiles;

namespace SunksBossChallenges.NPCs.LumiteTwins
{
	[AutoloadBossHead]
    public class LumiteSpazmatism:LumTwins
    {
		public int CoAttackTimer = 0;
		public int CoAttackPatternAI = 3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(@"Gemini-S");
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.Spazmatism];
			NPCID.Sets.TrailingMode[npc.type] = NPCID.Sets.TrailingMode[NPCID.Spazmatism];
			NPCID.Sets.TrailCacheLength[npc.type] = NPCID.Sets.TrailCacheLength[NPCID.Spazmatism];
		}

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
			npc.damage /= 2;
			npc.lifeMax = npc.lifeMax * 2 / 3;
            base.ScaleExpertStats(numPlayers, bossLifeScale);
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.Spazmatism);
            npc.aiStyle = -1;
			npc.lifeMax = 120000;
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
			Vector2 vecToPlayer = player.Center - npc.Center;

			float distRotation = vecToPlayer.ToRotation() - 1.57f;
            if (Main.rand.Next(5) == 0)
			{
				int num425 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), DustID.Blood, npc.velocity.X, 2f);
				Main.dust[num425].velocity.X *= 0.5f;
				Main.dust[num425].velocity.Y *= 0.1f;
			}
			if (npc.ai[1] >= EnragedState && npc.ai[1] < EnragedState + 2)
            {
				int num425 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), DustID.Electric, npc.velocity.X, 2f);
				Main.dust[num425].velocity.X *= 0.5f;
				Main.dust[num425].velocity.Y *= 0.1f;
				Main.dust[num425].noGravity = true;
			}
            #region Despawn
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
			/*if (Main.dayTime || targetDead)
			{
				npc.velocity.Y -= 0.04f;
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
				HandleRotation(distRotation);
				return;
			}*/
            #endregion
            #region Phase 1
            if (this.Phase == 0f)
			{
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
						SlowDown(0.98f);
						distRotation = (Main.npc[brotherId].Center - npc.Center).ToRotation() - MathHelper.PiOver2;
						if (Main.npc[brotherId].ai[1] == 3f)
                        {
                            if (npc.ai[2] >= 60)
                            {
								this.Phase = 1f;
								npc.ai[1] = 0f;
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								NewOrBoardcastText($"CONNECTION ESTABLISHED", Color.Green);
								if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
									int index = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -11);
								}
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
					HandleRotation(distRotation);
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
							NewOrBoardcastText($"Battle mode on.Sync server started at port 8{npc.whoAmI.ToString("D3")}", Color.Green, false);
						}
					}
					npc.netUpdate = true;
				}
				HandleRotation(distRotation);
				return;
            }
            #endregion
            #region Transition
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
				}
				npc.rotation += npc.ai[2];
				npc.ai[1] += 1f;
				if (npc.ai[1] == 100f)
				{
					this.Phase += 1f;
					npc.ai[1] = 0f;
					if (this.Phase == 3f)
					{
						for (int i = 0; i < npc.buffImmune.Length; i++)
							npc.buffImmune[i] = true;
						npc.dontTakeDamage = false;
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
							Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f);
						}
						Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);
					}
				}
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, (float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f);
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
				HandleRotation(distRotation);
				return;
			}
			#endregion
			#region Phase 2
			void resetCoAttack()
            {
				npc.ai[1] = Main.rand.NextBool() ? 0 : 1;
				npc.ai[2] = 0;
				npc.ai[3] = 0;
				npc.localAI[1] = 0;
				CoAttackTimer = 0;
				Main.npc[brotherId].ai[1] = Main.rand.NextBool() ? 0 : 1;
				Main.npc[brotherId].ai[2] = 0;
				Main.npc[brotherId].ai[3] = 0;
				Main.npc[brotherId].localAI[1] = 0;
				npc.netUpdate = true;
				Main.npc[brotherId].netUpdate = true;
			}
			void setRelax()
            {
				npc.ai[1] = Relaxed;
				Main.npc[brotherId].ai[1] = Relaxed;
				npc.ai[2] = Main.npc[brotherId].ai[2] = 0;
				npc.netUpdate = Main.npc[brotherId].netUpdate = true;
            }
			/// this automatically increases localAI
			void CoAttackInnerSwitch(float ai1, bool resetlocalAI = false)
            {
				npc.ai[1] = ai1;
				npc.ai[2] = 0;
				npc.ai[3] = 0;
				if (!resetlocalAI)
					npc.localAI[1]++;
				else npc.localAI[1] = 0;
				CoAttackTimer = 0;
				Main.npc[brotherId].ai[1] = ai1;
				Main.npc[brotherId].ai[2] = 0;
				Main.npc[brotherId].ai[3] = 0;
				if (!resetlocalAI)
					Main.npc[brotherId].localAI[1]++;
				else Main.npc[brotherId].localAI[1] = 0;
				npc.netUpdate = true;
				Main.npc[brotherId].netUpdate = true;
			}
			npc.HitSound = SoundID.NPCHit4;
			npc.damage = (int)((double)npc.defDamage * 1.5);
			npc.defense = npc.defDefense + 18;
			/*if (!CheckBrother<LumiteRetinazer>() && (npc.ai[1] < EnragedStateTrans))//this means brother has died
			{
				npc.dontTakeDamage = true;
				CoAttackTimer = 0;
				npc.ai[1] = EnragedStateTrans;
				npc.ai[2] = 0;
            }*/
            if (CheckBrother<LumiteRetinazer>())//check and setup enrage
            {
                if (npc.life <= 1)
                {
                    if (Main.npc[brotherId].life > 1)
                    {
						npc.dontTakeDamage = true;
                        if (npc.ai[1] < EnragedStateTrans)
                        {
							npc.ai[1] = EnragedStateTrans;
							npc.ai[2] = 0;
							npc.localAI[0] = EnragedState;
							Main.npc[brotherId].ai[1] = EnragedStateTrans;
							Main.npc[brotherId].ai[2] = 0;
							Main.npc[brotherId].localAI[0] = EnragedState;
							npc.netUpdate = true;
							Main.npc[brotherId].netUpdate = true;
						}
                    }
                    else
                    {
						npc.dontTakeDamage = false;
						npc.life = 0;
						Main.npc[brotherId].dontTakeDamage = false;
						Main.npc[brotherId].life = 0;
						npc.checkDead();
						Main.npc[brotherId].checkDead();
						return;
					}
                }
            }
            else
            {
				npc.life = 0;
				npc.checkDead();
			}
			if (npc.ai[1] == 0f)
			{
				float num444 = 4f;
				float accle = 0.1f;
				int direction = Math.Sign(npc.Center.X - player.Center.X);
				/*Vector2 vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num447 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(direction * 180) - vector43.X;
				float num448 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector43.Y;*/
				Vector2 distHoverSpeed = player.Center + new Vector2(direction * 180, 0) - npc.Center;
				float scaleFactor = distHoverSpeed.Length();

				if (Main.expertMode)
				{
					if (scaleFactor > 300f)
					{
						num444 += 0.5f;
					}
					if (scaleFactor > 400f)
					{
						num444 += 0.5f;
					}
					if (scaleFactor > 500f)
					{
						num444 += 0.55f;
					}
					if (scaleFactor > 600f)
					{
						num444 += 0.55f;
					}
					if (scaleFactor > 700f)
					{
						num444 += 0.6f;
					}
					if (scaleFactor > 800f)
					{
						num444 += 0.6f;
					}
				}
				HoverMovement(distHoverSpeed + npc.Center, num444, accle);
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 400f)
				{
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.target = 255;//reset target
					npc.netUpdate = true;
				}
				if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
				{
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > 22f)
					{
						npc.localAI[2] = 0f;
						Main.PlaySound(SoundID.Item34, npc.position);
					}
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[1] += 1f;
						#region Attack Rate
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
						#endregion
						if (npc.localAI[1] > 8f)
						{
							npc.localAI[1] = 0f;
							float num450 = 6f;
							int num451 = 30;
							if (Main.expertMode)
							{
								num451 = 27;
							}
							/*vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num447 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector43.X;
							num448 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector43.Y;*/
							Vector2 velocity = vecToPlayer;
							scaleFactor = vecToPlayer.Length();
							scaleFactor = num450 / scaleFactor;
							/*num447 *= scaleFactor;
							num448 *= scaleFactor;
							num448 += (float)Main.rand.Next(-40, 41) * 0.01f;
							num447 += (float)Main.rand.Next(-40, 41) * 0.01f;
							num448 += npc.velocity.Y * 0.5f;
							num447 += npc.velocity.X * 0.5f;
							vector43.X -= num447 * 1f;
							vector43.Y -= num448 * 1f;*/
							velocity *= scaleFactor;
							velocity += new Vector2(Main.rand.Next(-40, 41) * 0.01f, Main.rand.Next(-40, 41) * 0.01f);
							velocity += npc.velocity * 0.6f;
							int num453 = Projectile.NewProjectile(npc.Center - velocity * 1f, velocity, ProjectileID.EyeFire, num451, 0f, Main.myPlayer);
						}
					}
				}
			}
			else if (npc.ai[1] == 1f)
			{
				Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0);
				npc.rotation = distRotation;
				float speed = 16f;
				if (Main.expertMode)
				{
					speed += 4.5f;
				}
				Vector2 vector44 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num455 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector44.X;
				float num456 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector44.Y;
				float num457 = (float)Math.Sqrt(num455 * num455 + num456 * num456);
				num457 = speed / num457;
				/*npc.velocity.X = num455 * num457;
				npc.velocity.Y = num456 * num457;*/
				npc.velocity = Vector2.Normalize(new Vector2(num455, num456) + player.velocity * 5) * speed;
				npc.ai[1] = 2f;
			}
			else if (npc.ai[1] == 2f)
			{
				npc.ai[2] += 1f;
				if (Main.expertMode)
				{
					npc.ai[2] += 0.5f;
				}
				if (npc.ai[2] >= 50f)
				{
					SlowDown(0.93f);
				}
				else
				{
					npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) - 1.57f;
				}
				if (npc.ai[2] >= 80f)//stop this charge,and we need to insert our coattack here
				{
					npc.ai[3] += 1f;
					npc.ai[2] = 0f;
					npc.target = 255;
					npc.rotation = distRotation;
					if (CoAttackTimer >= 600)
					{
						if (CheckBrother<LumiteRetinazer>())
						{
							npc.target = Main.npc[brotherId].target;
							if (npc.target != 255)//255 means to reset target,we need to wait another tick to let retinazer choose its target
							{
								CoAttackTimer = 0;
								npc.ai[1] = CoAttackPatternAI;
								npc.ai[3] = Main.npc[brotherId].ai[3] = 0f;
								npc.localAI[0] = npc.localAI[1]
									= npc.localAI[2] = npc.localAI[3] = 0;
								Main.npc[brotherId].localAI[0] = Main.npc[brotherId].localAI[1]
									= Main.npc[brotherId].localAI[2] = Main.npc[brotherId].localAI[3] = 0;
								Main.npc[brotherId].ai[1] = CoAttackPatternAI;
							}
						}
					}
					else if (npc.ai[3] >= 6f)
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
			#region CoAttack Pattern 1
			else if (npc.ai[1] == 3f)
			{
				//some setup stuff
				if (npc.ai[2] == 0)
				{
					if (npc.localAI[1] == 0)
					{
						int baseRot = Main.rand.Next(0, 3);
						int offset = Main.rand.NextBool() ? -1 : 1;
						Main.npc[brotherId].ai[1] = 3f;
						Main.npc[brotherId].ai[2] = 0;
						npc.localAI[0] = MathHelper.Pi / 4 * (baseRot * 2 + offset);
						Main.npc[brotherId].localAI[0] = MathHelper.Pi / 4 * (baseRot * 2 - offset);
						npc.netUpdate = true;
						Main.npc[brotherId].netUpdate = true;
					}
					else if (npc.localAI[1] == 1)
					{
						float baseRot = MathHelper.Pi / 6 * (Main.rand.NextBool() ? 1 : 5) + MathHelper.PiOver2;
						int offset = Main.rand.NextBool() ? -1 : 1;
						Main.npc[brotherId].ai[1] = 3f;
						Main.npc[brotherId].ai[2] = 0;
						npc.localAI[0] = baseRot + offset * MathHelper.PiOver2;
						Main.npc[brotherId].localAI[0] = baseRot - offset * MathHelper.PiOver2;
						npc.netUpdate = true;
						Main.npc[brotherId].netUpdate = true;
					}
				}
				Vector2 dist = new Vector2(npc.localAI[1] == 0 ? 450f : 600f, 0);
				HoverMovement(player.Center + dist.RotatedBy(npc.localAI[0]), 30f + player.velocity.Length(), fastHoverAccle);
				npc.ai[2]++;

				if (npc.localAI[1] == 1)
				{
					SetRotation(new Vector2(vecToPlayer.X, 0));
				}

				if (npc.ai[2] >= 120)
				{
					npc.ai[1] = 4f;
					npc.ai[2] = 0;
					npc.netUpdate = true;
					Main.PlaySound(SoundID.Roar, npc.Center);
					if (npc.localAI[1] == 0)
						npc.velocity = Vector2.Normalize(vecToPlayer + player.velocity * 10) * (20f + player.velocity.Length() / 2);
					else if (npc.localAI[1] == 1)
						npc.velocity = new Vector2(Math.Sign(vecToPlayer.X) * (15f + player.velocity.Length() / 2), 0);
				}
			}
			else if (npc.ai[1] == 4f)
			{
				npc.ai[2]++;

				if (npc.localAI[1] == 1 && npc.ai[2] % 5 == 0 && npc.ai[2] <= 55 && Main.netMode != NetmodeID.MultiplayerClient)//horizonal dash,while releasing projectiles
				{
					float speed = 6.5f;
					int damage = 30;
					if (Main.expertMode)
					{
						speed = 9.5f;
						damage = 27;
					}
					Vector2 velocity = vecToPlayer;
					velocity *= speed / velocity.Length();
					int num410 = Projectile.NewProjectile(npc.Center + velocity * 15f, velocity, ProjectileID.CursedFlameHostile, damage, 0f, Main.myPlayer);
				}

				if (npc.ai[2] >= 30)
				{
					SlowDown(0.98f);
				}

				if (npc.localAI[1] == 1 && npc.ai[2] <= 60)
				{
					SetRotation(npc.velocity);
					return;
				}

				if (npc.ai[2] >= 75 && npc.localAI[1] == 0)
				{
					npc.ai[1] = 3f;
					npc.ai[2] = 0;
					npc.localAI[1]++;
					CoAttackTimer = 0;
					Main.npc[brotherId].ai[1] = 3f;
					Main.npc[brotherId].ai[2] = 0;
					Main.npc[brotherId].localAI[1]++;
					npc.netUpdate = true;
					Main.npc[brotherId].netUpdate = true;
				}
				else if (npc.ai[2] >= 75 && npc.localAI[1] == 1)
				{
					setRelax();
					CoAttackPatternAI = 5;
				}
			}
			#endregion
			#region CoAttack Pattern 2
			else if (npc.ai[1] == 5f)
			{
				if (npc.localAI[1] >= 7)
				{
					if (npc.ai[2] == 0)//setup rotation attack for brother
					{
						Main.npc[brotherId].ai[1] = 6f;
						Main.npc[brotherId].ai[2] = 0;
						Main.npc[brotherId].netUpdate = true;
						var target = player.Center + player.velocity * 10f;
						npc.localAI[2] = target.X;
						npc.localAI[3] = target.Y;
					}
					var center = new Vector2(npc.localAI[2], npc.localAI[3]);
					if (npc.ai[2] < 45)
					{
						Vector2 dist = center + Vector2.Normalize(npc.Center - center) * 750f;
						HoverMovement(dist, 30f + player.velocity.Length(), fastHoverAccle);
					}
					else if (npc.ai[2] < 60)
					{
						Vector2 dist = center + Vector2.Normalize(npc.Center - center) * 750f;
						npc.Center = dist;
					}
					else
					{
						float speed = 30f;
						float r = 750f;
						if (npc.ai[2] == 60)
						{
							Vector2 dist = center + Vector2.Normalize(npc.Center - center) * 750f;
							npc.Center = dist;
							npc.velocity = Vector2.Normalize((npc.Center - center).RotatedBy(Math.PI / 2)) * speed;
							SetRotation(npc.velocity);
							npc.ai[2]++;
							return;
						}
						else if (npc.ai[2] <= 540)
						{
							npc.velocity = npc.velocity.RotatedBy(speed / r);
							SetRotation(npc.velocity);

							if (npc.ai[2] % 30 == 0)
							{
								int damage = 30;
								Vector2 velocity = vecToPlayer;
								velocity = Main.rand.NextVector2Unit(velocity.ToRotation(), 0.06f) * velocity.Length();
								velocity *= 10f / velocity.Length();
								int num410 = Projectile.NewProjectile(npc.Center + velocity * 15f, velocity, ProjectileID.CursedFlameHostile, damage, 0f, Main.myPlayer);
							}
							npc.ai[2]++;
							return;
						}
						else//ended co-attack
						{
							setRelax();
							CoAttackPatternAI = 7;
						}
					}
				}
				else if ((npc.ai[2] >= 30 && npc.localAI[1] == 0) || (npc.localAI[1] > 0 && npc.ai[2] >= 15))//localAI[1] is always the counter
				{
					float speed = 27f;
					Vector2 vector8 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num48 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector8.X;
					float num49 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector8.Y;
					float num50 = Math.Abs(Main.player[npc.target].velocity.X) / 4f + Math.Abs(Main.player[npc.target].velocity.Y) / 4f;
					num50 += 10f - num50;
					if (num50 < 5f)
					{
						num50 = 5f;
					}
					if (num50 > 15f)
					{
						num50 = 15f;
					}
					/*if (npc.ai[2] == -1f)
					{
						num50 *= 4f;
						num47 *= 1.3f;
					}*/
					num48 -= Main.player[npc.target].velocity.X * num50 / 4f;
					num49 -= Main.player[npc.target].velocity.Y * num50 / 4f;
					num48 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
					num49 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
					float num51 = (float)Math.Sqrt(num48 * num48 + num49 * num49);
					float num52 = num51;
					num51 = speed / num51;
					npc.velocity.X = num48 * num51;
					npc.velocity.Y = num49 * num51;
					npc.velocity.X += (float)Main.rand.Next(-20, 21) * 0.1f;
					npc.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.1f;
					if (num52 < 100f)
					{
						if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
						{
							float num55 = Math.Abs(npc.velocity.X);
							float num56 = Math.Abs(npc.velocity.Y);
							if (npc.Center.X > Main.player[npc.target].Center.X)
							{
								num56 *= -1f;
							}
							if (npc.Center.Y > Main.player[npc.target].Center.Y)
							{
								num55 *= -1f;
							}
							npc.velocity.X = num56;
							npc.velocity.Y = num55;
						}
					}
					else if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
					{
						float num57 = (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) / 2f;
						float num58 = num57;
						if (npc.Center.X > Main.player[npc.target].Center.X)
						{
							num58 *= -1f;
						}
						if (npc.Center.Y > Main.player[npc.target].Center.Y)
						{
							num57 *= -1f;
						}
						npc.velocity.X = num58;
						npc.velocity.Y = num57;
					}
					npc.ai[1] = 6f;
					npc.ai[2] = 0;
					npc.netUpdate = true;
					if (npc.netSpam > 10)
					{
						npc.netSpam = 10;
					}
					SetRotation(npc.velocity);
					npc.ai[2]++;
					return;//dont handle rotation to player
				}
				npc.ai[2]++;
			}
			else if (npc.ai[1] == 6f)
			{
				if (npc.ai[2] == 0)
				{
					Main.PlaySound(SoundID.ForceRoar, (int)npc.position.X, (int)npc.position.Y, -1);
				}
				npc.ai[2]++;

				//if(npc.localAI[1] < 7)
				//{
				if (npc.ai[2] >= 20)
				{
					npc.ai[1] = 5f;
					npc.ai[2] = 0;
					npc.localAI[1]++;
					npc.netUpdate = true;
				}
				//}
				SetRotation(npc.velocity);
				return;
			}
			#endregion
			#region CoAttack Pattern 3
			else if (npc.ai[1] == 7f)
			{
				if (npc.localAI[1] == 0)
				{
					if (npc.ai[2] == 0)
					{
						int baseRot = Main.rand.NextBool() ? 1 : 3;
						int offset = Main.rand.NextBool() ? -1 : 1;
						Main.npc[brotherId].ai[1] = 7f;
						Main.npc[brotherId].ai[2] = 0;
						npc.localAI[0] = MathHelper.PiOver2 * baseRot + offset * MathHelper.PiOver2;
						Main.npc[brotherId].localAI[0] = MathHelper.PiOver2 * baseRot - offset * MathHelper.PiOver2;
						npc.netUpdate = true;
						Main.npc[brotherId].netUpdate = true;
					}
					var dist = player.Center + Vector2.UnitX.RotatedBy(npc.localAI[0]) * 750f;
					HoverMovement(dist, 30f + player.velocity.Length(), fastHoverAccle);
					if (npc.ai[2] >= 120)
					{
						npc.ai[1] = 8f;
						npc.ai[2] = 0;
						npc.netUpdate = true;
						Main.PlaySound(SoundID.Roar, npc.Center);
						npc.velocity = Vector2.Normalize(vecToPlayer + player.velocity * 10) * (30f + player.velocity.Length() / 2);
					}
				}
				npc.ai[2]++;
				if (npc.localAI[1] >= 3)//weird number isn't it
				{
					//now it should use waypoint dash
					CoAttackInnerSwitch(9, true);
				}
				if (npc.localAI[1] > 0)
                {
					if (npc.localAI[1] % 2 == 1)
					{
						if (npc.ai[2] >= 30)
						{
							npc.ai[1] = 8f;
							npc.ai[2] = 0;
							npc.netUpdate = true;
							Main.PlaySound(SoundID.Roar, npc.Center);
							npc.velocity = Vector2.Normalize(vecToPlayer + player.velocity * 10) * (30f + player.velocity.Length() / 2);
						}
					}
					else if (npc.localAI[1] % 2 == 0)
					{
						Vector2 dist = vecToPlayer + player.velocity * 100f;
						SlowDown(0.98f);
						if (npc.ai[2] < 60)
						{
							HandleRotation(dist.ToRotation() - 1.57f, 0.05f);
							return;
						}
						else
						{
							if (npc.ai[2] >= 60 && npc.ai[2] <= 105)
							{
								int damage = 80;
								if (npc.ai[2] == 60)
								{
									//SetRotation(dist);
								}
								if (npc.ai[2] == 105)
								{
									npc.ai[3] = GetRotationDirection(dist);
								}
								Vector2 velocity = Vector2.Normalize((npc.rotation + 1.57f).ToRotationVector2()) * Math.Min(vecToPlayer.Length() / 60f, 60f);
								Projectile.NewProjectile(npc.Center + Vector2.Normalize((npc.rotation + 1.57f).ToRotationVector2()) * npc.width, velocity, ProjectileID.EyeFire, damage, 0f, Main.myPlayer);
							}
							else if (npc.ai[2] < 150)
							{
								int damage = 80;
								Vector2 velocity = Vector2.Normalize((npc.rotation + 1.57f).ToRotationVector2()) * Math.Min(vecToPlayer.Length() / 60f, 60f);
								HandleRotation(npc.rotation + 0.01f * npc.ai[3]);
								Projectile.NewProjectile(npc.Center + Vector2.Normalize((npc.rotation + 1.57f).ToRotationVector2()) * npc.width, velocity, ProjectileID.EyeFire, damage, 0f, Main.myPlayer);
							}
							else if (npc.ai[2] < 180)
                            {
								HandleRotation(vecToPlayer);
                            }
							else
                            {
								npc.ai[1] = 8f;
								npc.ai[2] = 0;
								npc.ai[3] = 0;
								npc.netUpdate = true;
								Main.PlaySound(SoundID.Roar, npc.Center);
								npc.velocity = Vector2.Normalize(vecToPlayer + player.velocity * 10) * (30f + player.velocity.Length() / 2);
							}
						}

						return;
					}
				}
			}
			else if (npc.ai[1] == 8f)
			{
				npc.ai[2]++;
                if (npc.ai[2] < 30)
                {
					SetRotation(npc.velocity);
                }
				if (npc.ai[2] >= 30)
				{
					SlowDown(0.98f);
				}

				if (npc.ai[2] >= 75)
				{
					CoAttackInnerSwitch(7f);
				}
			}
			else if (npc.ai[1] == 9f)//waypoint dash
            {
				if (npc.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
					npc.localAI[0] = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LSWaypointLauncher>(),
						0, 0f, Main.myPlayer, npc.target);
					npc.localAI[1] = -1;
					npc.localAI[2] = 0;
                }
				npc.ai[2]++;
				if (npc.ai[2] < 290)
                {
					npc.SlowDown(0.98f);
					HandleRotation(vecToPlayer);
                }
				else if (npc.ai[2] < 300)
                {
					if (Util.CheckProjAlive<LSWaypointLauncher>((int)npc.localAI[0]))
                    {
						npc.FastMovement(Main.projectile[(int)npc.localAI[0]].Center);
					}
				}
                else if (npc.ai[2] >= 300)
                {
					if (Util.CheckProjAlive<LSWaypointLauncher>((int)npc.localAI[0]))
                    {
						if (npc.localAI[1] == -1 && npc.localAI[2] == 0)
							npc.localAI[1] = Main.projectile[(int)npc.localAI[0]].ai[1];
						if (npc.localAI[2] < 6 && Util.CheckProjAlive<LSWaypoint>((int)npc.localAI[1]))
                        {
                            if (npc.DistanceSQ(Main.projectile[(int)npc.localAI[1]].Center) <= 10000)
                            {
								Projectile proj = Main.projectile[(int)npc.localAI[1]];
								npc.Center = proj.Center;
								npc.velocity = Vector2.Zero;
								npc.localAI[1] = proj.ai[1];
								npc.localAI[2]++;
								proj.Kill();
                            }
                            else
                            {
								npc.velocity = (Main.projectile[(int)npc.localAI[1]].Center - npc.Center).SafeNormalize(Vector2.Zero) * 30f;
								SetRotation(npc.velocity);
							}
						}
                        else
                        {
							if (npc.localAI[2] == 6)
							{
								npc.ai[2] = 0;
								npc.ai[3]++;
								Main.projectile[(int)npc.localAI[0]].Kill();
							}
						}
					}
                }
                if (npc.ai[3] >= 2)
                {
					setRelax();
					CoAttackPatternAI = 3;
                }
				return;
            }
			#endregion
			else if (npc.ai[1] == Relaxed)
			{
				SlowDown(0.9f);
				npc.ai[2]++;
				if (npc.ai[2] == 60)
				{
					resetCoAttack();
				}
			}
			else if (npc.ai[1] == EnragedStateTrans)
			{
				npc.ai[2]++;
				SlowDown(0.98f);
				npc.dontTakeDamage = true;
				if (npc.ai[2] == 100)
				{
					NewOrBoardcastText("OVERLOADED MODE ON", Color.Green);
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int index = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, npc.whoAmI, -11);
					}
				}
				if (npc.ai[2] >= 120)
				{
					if (npc.life > 1)
					{
						npc.dontTakeDamage = false;
						npc.ai[1] = EnragedState;
					}
					else
						npc.ai[1] = EnragedState + 2;
					npc.ai[2] = 0;
					npc.ai[3] = 0;
				}
			}
			else if (npc.ai[1] == EnragedState)
            {
				Vector2 dist = player.Center + Vector2.Normalize(npc.Center - player.Center) * 100f;
				npc.HoverMovement(dist, 6f, 0.1f);
				npc.ai[2]++;
				if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] % 3 == 0)
                {
					float num450 = 6f;
					int num451 = 30;
					if (Main.expertMode)
					{
						num451 = 27;
					}
					/*vector43 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num447 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector43.X;
					num448 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector43.Y;*/
					Vector2 velocity = vecToPlayer;
					float scaleFactor = vecToPlayer.Length();
					scaleFactor = num450 / scaleFactor;
					/*num447 *= scaleFactor;
					num448 *= scaleFactor;
					num448 += (float)Main.rand.Next(-40, 41) * 0.01f;
					num447 += (float)Main.rand.Next(-40, 41) * 0.01f;
					num448 += npc.velocity.Y * 0.5f;
					num447 += npc.velocity.X * 0.5f;
					vector43.X -= num447 * 1f;
					vector43.Y -= num448 * 1f;*/
					velocity *= scaleFactor;
					velocity += new Vector2(Main.rand.Next(-40, 41) * 0.01f, Main.rand.Next(-40, 41) * 0.01f);
					velocity += npc.velocity * 0.45f;
					int num453 = Projectile.NewProjectile(npc.Center - velocity * 1f, velocity, ProjectileID.EyeFire, num451, 0f, Main.myPlayer);

                    if (npc.ai[2] % 60 == 0)
                    {
						Vector2 offset = Main.rand.NextVector2CircularEdge(1200, 900);
						Vector2 velo = -offset.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 12, MathHelper.Pi / 12))
							.SafeNormalize(Vector2.Zero) * 20f;

						Projectile.NewProjectile(player.Center + offset, velo, ModContent.ProjectileType<LSFlameBomb>(),
							npc.damage * 2 / 5, 0f, Main.myPlayer);
                    }
				}

                if (npc.ai[2] >= 720)
                {
					npc.ai[1] = EnragedState + 2;
					npc.ai[2] = 0;
					npc.ai[3] = 0;
					npc.localAI[0] = EnragedState + 1;
					npc.netUpdate = true;
					Main.npc[brotherId].ai[1] = Main.npc[brotherId].localAI[0];
					Main.npc[brotherId].netUpdate = true;
                }
			}
			else if (npc.ai[1] == EnragedState + 1f)
            {
                if (npc.ai[2] % 45 == 0)
                {
					float speed = 27f;
					Vector2 vector8 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num48 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector8.X;
					float num49 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector8.Y;
					float num50 = Math.Abs(Main.player[npc.target].velocity.X) / 4f + Math.Abs(Main.player[npc.target].velocity.Y) / 4f;
					num50 += 10f - num50;
					if (num50 < 5f)
					{
						num50 = 5f;
					}
					if (num50 > 15f)
					{
						num50 = 15f;
					}
					/*if (npc.ai[2] == -1f)
					{
						num50 *= 4f;
						num47 *= 1.3f;
					}*/
					num48 -= Main.player[npc.target].velocity.X * num50 / 4f;
					num49 -= Main.player[npc.target].velocity.Y * num50 / 4f;
					num48 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
					num49 *= 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
					float num51 = (float)Math.Sqrt(num48 * num48 + num49 * num49);
					float num52 = num51;
					num51 = speed / num51;
					npc.velocity.X = num48 * num51;
					npc.velocity.Y = num49 * num51;
					npc.velocity.X += (float)Main.rand.Next(-20, 21) * 0.1f;
					npc.velocity.Y += (float)Main.rand.Next(-20, 21) * 0.1f;
					if (num52 < 100f)
					{
						if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
						{
							float num55 = Math.Abs(npc.velocity.X);
							float num56 = Math.Abs(npc.velocity.Y);
							if (npc.Center.X > Main.player[npc.target].Center.X)
							{
								num56 *= -1f;
							}
							if (npc.Center.Y > Main.player[npc.target].Center.Y)
							{
								num55 *= -1f;
							}
							npc.velocity.X = num56;
							npc.velocity.Y = num55;
						}
					}
					else if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y))
					{
						float num57 = (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) / 2f;
						float num58 = num57;
						if (npc.Center.X > Main.player[npc.target].Center.X)
						{
							num58 *= -1f;
						}
						if (npc.Center.Y > Main.player[npc.target].Center.Y)
						{
							num57 *= -1f;
						}
						npc.velocity.X = num58;
						npc.velocity.Y = num57;
					}
					SetRotation(npc.velocity);
				}
				npc.ai[2]++;

                if (npc.ai[2] % 25 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
					Projectile.NewProjectile(npc.Center, Math.Sign(vecToPlayer.X) * Vector2.UnitX, ModContent.ProjectileType<LSAccleFlame>(),
						npc.damage * 2 / 3, 0f, Main.myPlayer, 60);
                }

                if (npc.ai[2] >= 450)
                {
					npc.ai[1] = EnragedState + 2;
					npc.ai[2] = 0;
					npc.ai[3] = 0;
					npc.localAI[0] = EnragedState;
					npc.netUpdate = true;
					Main.npc[brotherId].ai[1] = Main.npc[brotherId].localAI[0];
					Main.npc[brotherId].netUpdate = true;
				}
				return;//dont handle rotation to player
			}
			else if (npc.ai[1] == EnragedState + 2)
			{
				//idle
				Vector2 dist = player.Center + Vector2.Normalize(npc.Center - player.Center) * 600f;
				npc.FastMovement(dist);
			}
			if (CheckBrother<LumiteRetinazer>() && npc.ai[1] <= 2)
            {
				CoAttackTimer++;
            }
			#endregion
			HandleRotation(distRotation);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
			if (npc.target != 255 && npc.target >= 0)
            {
                if (npc.ai[1] == 7f)
                {
					if (npc.localAI[1] > 0)
					{
						if (npc.localAI[1] % 2 == 0)
						{
							if(npc.ai[2] <= 60)
								DrawAim(spriteBatch, (npc.rotation + 1.57f).ToRotationVector2() * (Main.player[npc.target].Center - npc.Center).Length() * 5 + npc.Center, Color.Green);
							if (npc.ai[2] >= 180)
								DrawAim(spriteBatch, (Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 10) * 5 + npc.Center, Color.Green);
						}
                        else
                        {
							DrawAim(spriteBatch, (Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 10) * 5 + npc.Center, Color.Green);
						}
					}
                    else
                    {
                        if (npc.ai[2] >= 90)
                        {
							DrawAim(spriteBatch, (Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 10) * 5 + npc.Center, Color.Green);
						}
                    }
				}
                if (npc.ai[1] == 3f)
                {
					if (npc.ai[2] >= 90 && npc.localAI[1] == 0)
					{
						DrawAim(spriteBatch, (Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 10) * 5 + npc.Center, Color.Green);
					}
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
        public override Color? GetAlpha(Color drawColor)
        {
			if (npc.ai[1] == EnragedState + 2) return Color.Lerp(drawColor, Color.Black, 0.3f);
            return base.GetAlpha(drawColor);
        }
        public override void DrawEffects(ref Color drawColor)
        {
            if (CheckBrother<LumiteRetinazer>())
            {
                if (brotherId < npc.whoAmI)
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
				}
            }
			base.DrawEffects(ref drawColor);
        }
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return npc.ai[1] != 3f;
		}
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.ai[0] == 3)
            {
				damage *= (1 - 0.375);
				if(npc.ai[1] == 7)
				{
					damage *= (1 - 0.80);
				}
            }
			return true;
        }
        public override bool CheckDead()
        {
			UpdateBrother<LumiteRetinazer>();
            if (CheckBrother<LumiteRetinazer>())
            {
                if (Main.npc[brotherId].life > 1)
                {
					npc.dontTakeDamage = true;
					npc.life = 1;
					return false;
                }
            }
			return true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(CoAttackTimer);
			writer.Write(CoAttackPatternAI);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
			CoAttackTimer = reader.ReadInt32();
			CoAttackPatternAI = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
    }
}
