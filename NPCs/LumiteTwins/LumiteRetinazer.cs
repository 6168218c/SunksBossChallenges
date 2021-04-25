using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.Projectiles.LumiteTwins;
using Terraria.Graphics.Effects;
using Terraria.Localization;

namespace SunksBossChallenges.NPCs.LumiteTwins
{
	public class LumiteRetinazer : LumTwins
	{
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
			npc.lifeMax = 105000;
			animationType = NPCID.Retinazer;
			music = MusicID.Boss2;
        }
        public override void AI()
        {
			/**/

			string aiDump = $"ai:{string.Join(",", npc.ai.Select(fl => $"{fl}"))}";
			//Main.NewText($"{aiDump}");

			if (!CheckBrother<LumiteSpazmatism>())
			{
				UpdateBrother<LumiteSpazmatism>();
			}

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest();
			}
			var player = Main.player[npc.target];
			bool targetDead = player.dead;

			var vecToPlayer = player.Center - npc.Center;

			float distRotation = vecToPlayer.ToRotation() - 1.57f;
            if (Main.rand.Next(5) == 0)
			{
				int num383 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), DustID.Blood, npc.velocity.X, 2f);
				Main.dust[num383].velocity.X *= 0.5f;
				Main.dust[num383].velocity.Y *= 0.1f;
			}
            #region Despawn
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
				handleRotation(distRotation);
				return;
			}
            #endregion
            #region Phase 1
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
				else if (npc.ai[1] == 3f)
				{
					npc.dontTakeDamage = true;
					if (CheckBrother<LumiteSpazmatism>())
					{
						SlowDown(0.98f);
						distRotation = (Main.npc[brotherId].Center - npc.Center).ToRotation() - 1.57f;
						//npc.rotation = distRotation;
						if (Main.npc[brotherId].ai[1] == 3f)
						{
							if (++npc.ai[2] >= 60)
                            {
								Main.npc[brotherId].ai[2] = 60;//should not be more than a tick
								this.Phase = 1f;
								npc.ai[1] = 0f;
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								NewOrBoardcastText($"Connection established", Color.Red);
								npc.netUpdate = true;
							}
						}
                        else
                        {
							Vector2 pivot = Main.npc[brotherId].Center;
							if(npc.Distance(pivot)> 900f)
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
					handleRotation(distRotation);
					return;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.75)
				{
					//this.Phase = 1f;
					npc.ai[1] = 3f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					if(CheckBrother<LumiteSpazmatism>())
                    {
                        if (Main.npc[brotherId].ai[1] != 3f)
                        {
							NewOrBoardcastText($"Battle mode on.Sync server started at port 8{npc.whoAmI.ToString("D3")}", Color.Red);
						}
                    }
					npc.netUpdate = true;
				}
				handleRotation(distRotation);
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
						for (int num400 = 0; num400 < 2; num400++)
						{
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 143);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
							Gore.NewGore(npc.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6);
						}
						for (int num401 = 0; num401 < 20; num401++)
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
				handleRotation(distRotation);
				return;
			}
            #endregion
            #region Phase 2
            npc.damage = (int)((double)npc.defDamage * 1.5);
			npc.defense = npc.defDefense + 10;
			npc.HitSound = SoundID.NPCHit4;
			if (!CheckBrother<LumiteSpazmatism>() && !(npc.ai[1] == EnragedState) && !(npc.ai[1] == EnragedStateTrans))//this means brother has died
			{
				npc.dontTakeDamage = true;
				npc.ai[1] = EnragedStateTrans;
				npc.ai[2] = 0;
			}
			if (npc.ai[1] == 0f)
			{
				float num402 = 8f;
				float accle = 0.15f;
				if (Main.expertMode)
				{
					num402 = 9.5f;
					accle = 0.175f;
				}
				//Vector2 vector39 = npc.Center;//new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				/*float num404 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector39.X;
				float num405 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 300f - vector39.Y;
				float num406 = (float)Math.Sqrt(num404 * num404 + num405 * num405);
				num406 = num402 / num406;
				num404 *= num406;
				num405 *= num406;*/
				Vector2 distHoverSpeed = player.Center + new Vector2(0, -300f);
				HoverMovement(distHoverSpeed, num402, accle);
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 300f)
				{
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.TargetClosest();
					npc.netUpdate = true;
				}
				Vector2 vector39 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num404 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector39.X;
				float num405 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector39.Y;
				npc.rotation = (float)Math.Atan2(num405, num404) - 1.57f;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					#region AttackRate
					if (npc.life < npc.lifeMax * 0.75)
					{
						npc.localAI[1] += 1f;
					}
					if (npc.life < npc.lifeMax * 0.5)
					{
						npc.localAI[1] += 1f;
					}
					if (npc.life < npc.lifeMax * 0.25)
					{
						npc.localAI[1] += 1f;
					}
					if (npc.life < npc.lifeMax * 0.1)
					{
						npc.localAI[1] += 2f;
					}
					#endregion
					if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						npc.localAI[1] = 0f;
						float speed = 8.5f;
						int damage = 25;
						if (Main.expertMode)
						{
							speed = 10f;
							damage = 23;
						}
						/*num406 = (float)Math.Sqrt(num404 * num404 + num405 * num405);
						num406 = num407 / num406;
						num404 *= num406;
						num405 *= num406;
						vector39.X += num404 * 15f;
						vector39.Y += num405 * 15f;*/
						Vector2 velocity = vecToPlayer;
						velocity *= speed / velocity.Length();
						int num410 = Projectile.NewProjectile(npc.Center + velocity * 15f, velocity, ProjectileID.DeathLaser, damage, 0f, Main.myPlayer);
					}
				}
				return;
			}
			else if (npc.ai[1] == 1f)
            {
				int direction = Math.Sign(npc.Center.X - player.Center.X);
				float num412 = 8f;
				float accle = 0.2f;
				if (Main.expertMode)
				{
					num412 = 9.5f;
					accle = 0.25f;
				}
				/*Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num414 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num411 * 340) - vector40.X;
				float num415 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector40.Y;
				float num416 = (float)Math.Sqrt(num414 * num414 + num415 * num415);
				num416 = num412 / num416;
				num414 *= num416;
				num415 *= num416;*/
				Vector2 distHoverSpeed = player.Center + new Vector2(direction * 340, 0);
				HoverMovement(distHoverSpeed, num412, accle);
				Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num414 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector40.X;
				float num415 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector40.Y;
				npc.rotation = (float)Math.Atan2(num415, num414) - 1.57f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					#region AttackRate
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
                    #endregion
                    if (npc.localAI[1] > 60f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						npc.localAI[1] = 0f;
						float num417 = 9f;
						int damage = 18;
						if (Main.expertMode)
						{
							damage = 17;
						}
						/*num416 = (float)Math.Sqrt(num414 * num414 + num415 * num415);
						num416 = num417 / num416;
						num414 *= num416;
						num415 *= num416;
						vector40.X += num414 * 15f;
						vector40.Y += num415 * 15f;*/
						Vector2 velocity = vecToPlayer;
						velocity *= num417 / velocity.Length();
						int num420 = Projectile.NewProjectile(npc.Center+velocity*15f, velocity, ProjectileID.DeathLaser, damage, 0f, Main.myPlayer);
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
				return;
			}
            #region CoAttack Pattern 1
            else if (npc.ai[1] == 3f)
            {
				Vector2 dist = new Vector2(npc.localAI[1] == 0 ? 450f : 600f, 0);
				HoverMovement(player.Center + dist.RotatedBy(npc.localAI[0]), 30f + player.velocity.Length(), 0.8f);
				npc.ai[2]++;

				if (npc.localAI[1] == 1)
				{
					setRotation(new Vector2(vecToPlayer.X, 0));
				}

				if (npc.ai[2] >= 120)
				{
					npc.ai[1] = 4f;
					npc.ai[2] = 0;
					npc.netUpdate = true;
					if (npc.localAI[1] == 0)
						npc.velocity = Vector2.Normalize(vecToPlayer + player.velocity * 10) * (20f + player.velocity.Length() / 2);
					else if (npc.localAI[1] == 1)
						npc.velocity = new Vector2(Math.Sign(vecToPlayer.X) * (15f + player.velocity.Length() / 2), 0);
				}
			}
			else if (npc.ai[1] == 4f)
			{
				npc.ai[2]++;

				if (npc.localAI[1] == 1 && npc.ai[2] % 5 == 0 && npc.ai[2] <= 55 && Main.netMode != NetmodeID.MultiplayerClient)  //horizonal dash,while releasing projectiles
				{
					float speed = 5.5f;
					int damage = 30;
					if (Main.expertMode)
					{
						speed = 7.5f;
						damage = 27;
					}
					Vector2 velocity = vecToPlayer;
					velocity *= speed / velocity.Length();
					int num410 = Projectile.NewProjectile(npc.Center + velocity * 15f, velocity, ProjectileID.RocketSkeleton, damage, 0f, Main.myPlayer);
				}

				if (npc.ai[2] >= 30)
				{
					SlowDown(0.98f);
				}

				if (npc.localAI[1] == 1 && npc.ai[2] <= 60)
				{
					setRotation(npc.velocity);
					return;
				}

				if (npc.ai[2] >= 75)
                {
					npc.ai[1] = 3f;
					npc.ai[2] = 0;
					npc.netUpdate = true;
				}
			}
			#endregion
			#region CoAttack Pattern 2
			else if (npc.ai[1] == 5f)
            {
				npc.ai[2]++;
				Vector2 dist = player.Center + Vector2.Normalize(npc.Center - player.Center) * 600f;
				HoverMovement(dist, 30f + player.velocity.Length(), 0.8f);
                if (npc.ai[2] % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
					float speed = 7.5f;
					int damage = 40;
					if (Main.expertMode)
					{
						speed = 10.5f;
						damage = 35;
					}
					Vector2 velocity = vecToPlayer;
					velocity *= speed / vecToPlayer.Length();
					Projectile.NewProjectile(npc.Center + velocity.RotatedBy(Math.PI / 3) * 15f, velocity.RotatedBy(Math.PI / 3),
						ModContent.ProjectileType<HomingLaser>(), damage, 0f, Main.myPlayer, npc.target, 0.1f);
					Projectile.NewProjectile(npc.Center + velocity.RotatedBy(-Math.PI / 3) * 15f, velocity.RotatedBy(-Math.PI / 3),
						ModContent.ProjectileType<HomingLaser>(), damage, 0f, Main.myPlayer, npc.target, 0.1f);
				}
            }
			else if (npc.ai[1] == 6)
            {
				npc.ai[2]++;
				if (npc.ai[2] <= 45)
                {
					HoverMovement(new Vector2(Main.npc[brotherId].localAI[2], Main.npc[brotherId].localAI[3]), 30f, 0.8f);
                }
				else if (npc.ai[2] < 60)
                {
					npc.Center = new Vector2(Main.npc[brotherId].localAI[2], Main.npc[brotherId].localAI[3]);
				}
                else
                {
					if (npc.ai[2] == 60)
                    {
						Vector2 speed = Vector2.UnitX.RotatedBy(npc.rotation);
						Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<RetinDeathRay>(), npc.damage / 2, 0f, Main.myPlayer, 480f, npc.whoAmI);
					}
					npc.Center = new Vector2(Main.npc[brotherId].localAI[2], Main.npc[brotherId].localAI[3]);
					handleRotation(npc.rotation - 0.025f);
					return;
                }
            }
			#endregion
			#region CoAttack Pattern 3
			#endregion
			else if (npc.ai[1] == EnragedStateTrans)
			{
				npc.ai[2]++;
				SlowDown(0.98f);
				if (npc.ai[2] >= 80)
				{
					npc.ai[1] = EnragedState;
					npc.ai[2] = 0;
					npc.ai[3] = 0;
					NewOrBoardcastText("Overloaded mode on.", Color.Red);
				}
			}
			else if (npc.ai[1] == EnragedState)
            {

            }
			#endregion
			handleRotation(distRotation);
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

        public override void DrawEffects(ref Color drawColor)
        {
			if (CheckBrother<LumiteSpazmatism>())
			{
				if (brotherId < npc.whoAmI)
				{
					for (int i = 0; i < 200; i++)
					{
						if (!Main.npc[i].active || npc.whoAmI == i || (Main.npc[i].type != ModContent.NPCType<LumiteSpazmatism>()))
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }
    }
}
