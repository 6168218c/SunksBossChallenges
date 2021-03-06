using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SunksBossChallenges.NPCs.DecimatorOfPlanets
{
    [AutoloadBossHead]
    public class DecimatorOfPlanetsHead:ModNPC
    {
        int Length => 60;
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
            npc.width = npc.height = 38;
            npc.defense = 0;
            npc.damage = 120;
            npc.lifeMax = 120000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.scale = 1f;
            npc.netAlways = true;
            npc.alpha = 255;
            npc.scale = 1.2f;
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
            ///  ai[3]:head
            ///  localAI:for spinning attacks
            ///    [0]:
            ///    [1]:posx
            ///    [2]:posy
            ///    [3]:direction(0,1)
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
                int PreviousSeg = npc.whoAmI;
                for (int j = 1; j <= Length; j++)
                {
                    int type = ModContent.NPCType<DecimatorOfPlanetsBody>();
                    if (j == Length)
                    {
                        type = ModContent.NPCType<DecimatorOfPlanetsTail>();
                    }
                    int CurrentSeg = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, type, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    Main.npc[CurrentSeg].ai[3] = npc.whoAmI;
                    Main.npc[CurrentSeg].realLife = npc.whoAmI;
                    Main.npc[CurrentSeg].ai[1] = PreviousSeg;
                    Main.npc[PreviousSeg].ai[0] = CurrentSeg;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, CurrentSeg);

                    PreviousSeg = CurrentSeg;
                }

                if (!NPC.AnyNPCs(ModContent.NPCType<DecimatorOfPlanetsTail>()))
                {
                    npc.active = false;
                    return;
                }
            }         //生成身体


            var accleration = 1f;
            var rotationSpeed = Math.PI * 2f / 60 * 0.8f;
            var minSpeed = 5f;
            var maxSpeed = 15f;
            if (Main.expertMode)
                maxSpeed *= 1.35f;
            //if (Main.getGoodWorld)
            //    maxSpeed *= 1.25f;

            //for testing use,the ai now only charges at the player
            var distToPlayer = player.Center - npc.Center;
            if (Vector2.Dot(distToPlayer, npc.velocity) < 0 && npc.Distance(player.Center) <= 200f) 
            {
                var normVelocity = Vector2.Normalize(npc.velocity) + new Vector2(0, -0.1f);
                var speedNow = npc.velocity.Length() - accleration;
                if (speedNow < minSpeed) speedNow = minSpeed;
                npc.velocity = normVelocity * speedNow;
            }
            if (distToPlayer.Length() > 600f)
            {
                var directionVect = player.Center - npc.Center;
                var npcSpeed = npc.velocity.Length();
                if (npcSpeed != 0) 
                    directionVect += directionVect.Length() / npc.velocity.Length() * player.velocity;
                directionVect.Normalize();
                if (Math.Cos(npc.velocity.ToRotation() - directionVect.ToRotation()) > Math.Cos(Math.PI / 12))
                {
                    npc.velocity.RotatedBy(rotationSpeed);
                    var speedNow = npc.velocity.Length() + accleration;
                    if (speedNow > maxSpeed) speedNow = maxSpeed;
                    npc.velocity = scaleLength(npc.velocity, speedNow);
                }
                npc.velocity += directionVect * accleration;
                if (npc.velocity.Length() > maxSpeed)
                    npc.velocity = scaleLength(npc.velocity, maxSpeed);
            }
            else
            {
                if (npc.velocity.HasNaNs() || npc.velocity == Vector2.Zero)
                    npc.velocity = scaleLength(player.Center - npc.Center, maxSpeed);
                else
                    npc.velocity = scaleLength(npc.velocity, maxSpeed);
            }
            npc.rotation = npc.velocity.ToRotation() + (float)(Math.PI / 2);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }
    }
}
