﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SunksBossChallenges.NPCs.DecimatorOfPlanets
{
    [AutoloadBossHead]
    public class DecimatorOfPlanetsTail:DecimatorOfPlanetsBody
    {
        public override void SetDefaults()
        {
			npc.CloneDefaults(NPCID.TheDestroyerTail);
            npc.boss = true;
            npc.aiStyle = -1;
            npc.npcSlots = 1f;
            npc.width = npc.height = 38;
            npc.defense = 0;
            npc.damage = 60;
            npc.lifeMax = 320000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.behindTiles = true;
            npc.value = 10000f;
            npc.netAlways = true;
            npc.alpha = 255;
            npc.scale = DecimatorOfPlanetsArguments.Scale;
            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
			npc.timeLeft = NPC.activeTime * 30;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/LastBattleBallosMix");
            musicPriority = MusicPriority.BossHigh;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (npc.alpha > 0 || Main.npc[npc.realLife].ai[2] == 12)
                damage *= (1 - 0.99);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);
        }
    }
}
