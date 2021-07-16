using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SunksBossChallenges.NPCs.LumiteDestroyer
{
    public class LumiteDestroyerTail : LumiteDestroyerBody
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nova");
            NPCID.Sets.TrailingMode[npc.type] = 3;
            NPCID.Sets.TrailCacheLength[npc.type] = 16;
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.TheDestroyerTail];
        }
        public override void SetDefaults()
        {
            npc.boss = true;
            npc.aiStyle = -1;
            npc.npcSlots = 1f;
            npc.width = npc.height = 38;
            npc.defense = 0;
            npc.damage = 40;
            npc.lifeMax = 360000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 0f;
            npc.netAlways = true;
            npc.alpha = 255;
            npc.scale = LumiteDestroyerArguments.Scale;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            music = MusicID.Boss3;
            musicPriority = MusicPriority.BossMedium;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.alpha > 0 || npc.ai[2] >= 11)
                damage *= (1 - 0.99);
            if (npc.realLife != -1 && Main.npc[npc.realLife].type == ModContent.NPCType<LumiteDestroyerHead>())
            {
                NPC head = Main.npc[npc.realLife];
                damage *= (1 - (head.modNPC as LumiteDestroyerHead).DynDR);
            }
            return base.StrikeNPC(ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = 0;
        }
    }
}
