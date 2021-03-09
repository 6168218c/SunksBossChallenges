using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SunksBossChallenges.Projectiles.DecimatorOfPlanets;

namespace SunksBossChallenges.NPCs.DecimatorOfPlanets
{
    [AutoloadBossHead]
    public class DecimatorOfPlanetsHead:ModNPC
    {
        int Length => 80;
        private float spinTimer = 0;
        private int[] chaosPlanets = new int[3];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Decimator of Planets");
            NPCID.Sets.TrailingMode[npc.type] = 3;
            NPCID.Sets.TrailCacheLength[npc.type] = 16;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.boss = true;
            npc.npcSlots = 1f;
            npc.width = npc.height = 44;
            npc.defense = 0;
            npc.damage = 120;
            npc.lifeMax = 200000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.netAlways = true;
            npc.alpha = 255;
            npc.scale = DecimatorOfPlanetsArguments.Scale * 1.2f;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            //music = mod.GetSoundSlot(SoundType.Music, "");
            music = MusicID.Boss3;
            musicPriority = MusicPriority.BossHigh;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax / 2 * bossLifeScale);
        }

        public override void AI()//used:all ai[],localAI
        {
            ///shared contents:
            ///  ai[0]:rear segment
            ///  ai[1]:previous segment
            ///  ai[2]:mode/state
            ///     0 - normal,11 - spin setup, 12 - spinning
            ///  ai[3]:head,but for head it self,it is a counter
            ///  localAI:for spinning attacks
            ///    [0]:timer for spinning
            ///    [1]:posx
            ///    [2]:posy
            ///    [3]:direction
            ///
            Vector2 scaleLength(Vector2 source,float desiredLength)
            {
                return Vector2.Normalize(source) * desiredLength;
            }

            /*if (npc.ai[3] > 0f)//life is more than 0
                npc.realLife = (int)npc.ai[3];

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
                npc.TargetClosest();

            var player = Main.player[npc.target];
            if (player.immuneTime > 50) player.immuneTime = 50;

            if(npc.alpha==255)//just spawned
            {
                int previous = npc.whoAmI;
                for(int i=1;i<=Length;i++)
                {
                    int npcType = ModContent.NPCType<DecimatorOfPlanetsBody>();
                    if (i == Length)
                        npcType = ModContent.NPCType<DecimatorOfPlanetsTail>();

                    int current = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, npcType, npc.whoAmI);
                    Main.npc[current].ai[3] = npc.whoAmI;
                    Main.npc[current].realLife = npc.whoAmI;
                    Main.npc[current].ai[1] = previous;
                    Main.npc[previous].ai[0] = current;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, current);

                    previous = current;
                }

                if(!NPC.AnyNPCs(ModContent.NPCType<DecimatorOfPlanetsTail>()))
                {
                    npc.active = false;
                    return;
                }
            }

            npc.alpha -= 42;
            if (npc.alpha < 0) npc.alpha = 0;*/

            if (NPC.FindFirstNPC(ModContent.NPCType<DecimatorOfPlanetsHead>()) != npc.whoAmI)
            {
                npc.active = false;
                return;
            }
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            Player player = Main.player[npc.target];
            if (player.immuneTime > 50)
            {
                player.immuneTime = 50;
            }
            if (npc.alpha != 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int num = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 229, 0f, 0f, 100, default, 2f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].noLight = true;
                    Main.dust[num].color = Color.LightBlue;
                }
            }
            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }

            if (npc.ai[0] == 0f)
            {
                int previous = npc.whoAmI;
                for (int j = 1; j <= Length; j++)
                {
                    int npcType = ModContent.NPCType<DecimatorOfPlanetsBody>();
                    if (j == Length)
                    {
                        npcType = ModContent.NPCType<DecimatorOfPlanetsTail>();
                    }
                    int current = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, npcType, npc.whoAmI);
                    Main.npc[current].ai[3] = npc.whoAmI;
                    Main.npc[current].realLife = npc.whoAmI;
                    Main.npc[current].ai[1] = previous;
                    Main.npc[previous].ai[0] = current;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, current);

                    previous = current;
                }

                if (!NPC.AnyNPCs(ModContent.NPCType<DecimatorOfPlanetsTail>()))
                {
                    npc.active = false;
                    return;
                }
            }         


            var accleration = 0.25f;
            var rotationSpeed = Math.PI * 2f / 60 * 0.8f;
            var minSpeed = 5f;
            var maxSpeed = 24f;
            if (Main.expertMode)
                maxSpeed *= 1.35f;
            //if (Main.getGoodWorld)
            //    maxSpeed *= 1.25f;
            bool allowSpin = npc.life < npc.lifeMax * 0.75;

            if (allowSpin)
                spinTimer++;

            if (npc.ai[2] == 12&&spinTimer==1800)
            {//spawn chaos system
                var center = new Vector2(npc.localAI[1], npc.localAI[2]);
                Vector2 offset = new Vector2(600, 0);
                for(int i = 0; i < 3; i++)
                {
                    chaosPlanets[i] = Projectile.NewProjectile(center + offset, Vector2.Normalize(offset.RotatedBy(MathHelper.TwoPi / 3.6f)) * 3f,
                        ModContent.ProjectileType<ChaosPlanet>(), npc.damage / 2, 0f, Main.myPlayer);
                    offset = offset.RotatedBy(MathHelper.TwoPi / 3);
                    Main.projectile[chaosPlanets[i]].localAI[1] = Main.rand.NextFloat(1, 12);
                }
                Main.projectile[chaosPlanets[0]].ai[0] = chaosPlanets[1];
                Main.projectile[chaosPlanets[0]].ai[1] = chaosPlanets[2];
                Main.projectile[chaosPlanets[1]].ai[0] = chaosPlanets[0];
                Main.projectile[chaosPlanets[1]].ai[1] = chaosPlanets[2];
                Main.projectile[chaosPlanets[2]].ai[0] = chaosPlanets[0];
                Main.projectile[chaosPlanets[2]].ai[1] = chaosPlanets[1];//...
            }


            //movement control
            if (spinTimer <= 1200)
            {//ground movement code
                WormMovement(player.Center, maxSpeed * 0.8f, 0.15f);
            }
            else if (spinTimer > 1200 && spinTimer < 1600) //preparing to spin,this ai tends to wrap around the player
            {
                var distToPlayer = player.Center - npc.Center;
                if (Math.Cos(npc.velocity.ToRotation() - distToPlayer.ToRotation()) < Math.Cos(Math.PI / 3) && npc.Distance(player.Center) <= 800f)
                {
                    var normVelocity = (Vector2.Normalize(npc.velocity).HasNaNs() ? Vector2.Zero : Vector2.Normalize(npc.velocity))
                        + new Vector2(0.025f * Math.Sign(distToPlayer.X), 0.025f * Math.Sign(distToPlayer.Y));
                    var speedNow = npc.velocity.Length() - accleration;
                    if (speedNow < minSpeed) speedNow = minSpeed;
                    npc.velocity = normVelocity * speedNow;
                }
                if (distToPlayer.Length() > 900)
                {
                    npc.ai[2] = 1;
                }
                else if(npc.ai[2]==0)
                {
                    var directionVect = player.Center - npc.Center;
                    if (npc.velocity.HasNaNs() || npc.velocity == Vector2.Zero)
                        npc.velocity = scaleLength(player.Center - npc.Center, Math.Min(npc.velocity.Length()+accleration, maxSpeed));
                    else
                        npc.velocity = scaleLength(npc.velocity, Math.Min(npc.velocity.Length()+accleration, maxSpeed));
                    if (Math.Cos(npc.velocity.ToRotation() - directionVect.ToRotation()) > Math.Cos(Math.PI / 36))
                    {
                        npc.velocity = npc.velocity.RotatedBy(rotationSpeed * -Math.Sign(npc.Center.X - player.Center.X) / 3);
                        var speedNow = npc.velocity.Length() + accleration;
                        if (speedNow > maxSpeed) speedNow = maxSpeed;
                        npc.velocity = scaleLength(npc.velocity, speedNow);
                    }

                }

                if (npc.ai[2] == 1)
                {
                    var directionVect = player.Center - npc.Center;
                    var npcSpeed = npc.velocity.Length();
                    if (npcSpeed != 0)
                        directionVect += directionVect.Length() / npc.velocity.Length() * player.velocity / 2;
                    directionVect.Normalize();
                    if (Math.Cos(npc.velocity.ToRotation() - directionVect.ToRotation()) > Math.Cos(Math.PI / 12))
                    {
                        npc.velocity = npc.velocity.RotatedBy(rotationSpeed * -Math.Sign(npc.Center.X - player.Center.X));
                        var speedNow = npc.velocity.Length() + accleration;
                        if (speedNow > maxSpeed) speedNow = maxSpeed;
                        npc.velocity = scaleLength(npc.velocity, speedNow);
                    }
                    else npc.ai[2] = 0;
                    npc.velocity += directionVect * accleration;
                    if (npc.velocity.Length() > maxSpeed)
                        npc.velocity = scaleLength(npc.velocity, maxSpeed);
                }
            }
            else //do spin attack
            {
                float r = (float)(DecimatorOfPlanetsArguments.SpinSpeed / DecimatorOfPlanetsArguments.SpinRadiusSpeed);
                if (npc.ai[2]<11)//not even have set up
                {
                    npc.ai[2] = 11;
                    npc.localAI[1] = player.Center.X;
                    npc.localAI[2] = player.Center.Y;
                    var center = new Vector2(npc.localAI[1], npc.localAI[2]);
                    Vector2 destination = Vector2.Normalize(npc.Center - center) * r;
                    if (destination == Vector2.Zero || destination.HasNaNs())
                        destination = center + new Vector2(0, r);
                    npc.velocity = (destination-npc.Center) / 2;//arrive in two ticks,leaving time for other segments to react
                }
                if (npc.ai[2] == 11)
                {
                    var center = new Vector2(npc.localAI[1], npc.localAI[2]);
                    if(Vector2.Distance(npc.Center,center)>=r)//has moved to the desired position
                    {
                        if(npc.Distance(center)>r)//modify it to retain accuracy
                            npc.position = center + Vector2.Normalize(npc.Center - center) * r;
                        int direction = Main.rand.NextBool() ? 1 : -1;
                        npc.velocity = Vector2.Normalize(npc.Center - center)
                            .RotatedBy(-Math.PI / 2 * direction) * DecimatorOfPlanetsArguments.SpinSpeed;
                        npc.rotation = npc.velocity.ToRotation() + (float)(Math.PI / 2) * npc.direction;
                        npc.localAI[3] = direction;
                        chaosPlanets[0] = chaosPlanets[1] = chaosPlanets[2] = -1;
                        npc.ai[2] = 12;
                    }
                }
                else if (npc.ai[2] == 12)
                {
                    int direction = (int)npc.localAI[3];
                    var center = new Vector2(npc.localAI[1], npc.localAI[2]);
                    if (npc.Distance(center) < DecimatorOfPlanetsArguments.R)
                    {
                        npc.Center = center + npc.DirectionFrom(center) * DecimatorOfPlanetsArguments.R;
                    }
                    //player.wingTime = 100;
                    npc.velocity = npc.velocity.RotatedBy(-DecimatorOfPlanetsArguments.SpinRadiusSpeed * direction);
                    npc.rotation -= (float)(DecimatorOfPlanetsArguments.SpinRadiusSpeed * direction);
                    if (chaosPlanets.All(item => item != -1 && (Main.projectile[item].type!=ModContent.ProjectileType<ChaosPlanet>() || (Main.projectile[item].localAI[0] == 1 || Main.projectile[item].active == false))))
                    {
                        spinTimer = 0;
                        npc.ai[2] = 0;//reset to normal
                    }
                }
                var pivot = new Vector2(npc.localAI[1], npc.localAI[2]);
                for(int i = 0; i < 20; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2 * Math.PI;
                    offset.X += (float)(Math.Cos(angle) * r);
                    offset.Y += (float)(Math.Sin(angle) * r);
                    Dust dust = Main.dust[Dust.NewDust(pivot + offset,0,0,112,0,0,100,Color.White)];
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.Next(3) == 0)
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                }
            }
            npc.rotation = npc.velocity.ToRotation() + (float)(Math.PI / 2) * npc.direction;
        }

        protected void WormMovement(Vector2 position,float maxSpeed,float turnAccle=0.1f,float ramAccle=0.15f)
        {
            Vector2 targetVector = position - npc.Center;
            targetVector = Vector2.Normalize(targetVector) * maxSpeed;
            if ((targetVector.X * npc.velocity.X > 0f) && (targetVector.Y * npc.velocity.Y > 0f)) //acclerate
            {
                npc.velocity.X += Math.Sign(targetVector.X - npc.velocity.X) * ramAccle;
                npc.velocity.Y += Math.Sign(targetVector.Y - npc.velocity.Y) * ramAccle;
            }
            if ((targetVector.X * npc.velocity.X > 0f) || (targetVector.Y * npc.velocity.Y > 0f)) //turn
            {
                npc.velocity.X += Math.Sign(targetVector.X - npc.velocity.X) * turnAccle;
                npc.velocity.Y += Math.Sign(targetVector.Y - npc.velocity.Y) * turnAccle;

                if (Math.Abs(targetVector.Y) < maxSpeed * 0.2 && targetVector.X * npc.velocity.X < 0)
                {
                    npc.velocity.Y += Math.Sign(npc.velocity.Y) * turnAccle * 2f;
                }

                if (Math.Abs(targetVector.X) < maxSpeed * 0.2 && targetVector.Y * npc.velocity.Y < 0)
                {
                    npc.velocity.X += Math.Sign(npc.velocity.X) * turnAccle * 2f;
                }
            }
            else if (Math.Abs(targetVector.X) > Math.Abs(targetVector.Y)) 
            {
                npc.velocity.X += Math.Sign(targetVector.X - npc.velocity.X) * turnAccle * 1.1f;
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5)
                {
                    npc.velocity.Y += Math.Sign(npc.velocity.Y) * turnAccle;
                }
            }
            else
            {
                npc.velocity.Y += Math.Sign(targetVector.Y - npc.velocity.Y) * turnAccle * 1.1f;
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed * 0.5)
                {
                    npc.velocity.X += Math.Sign(npc.velocity.X) * turnAccle;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
            writer.Write(spinTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
            spinTimer = reader.ReadSingle();
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.alpha > 0)
                damage *= (1 - 0.99);
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = 4;
        }
    }
}
