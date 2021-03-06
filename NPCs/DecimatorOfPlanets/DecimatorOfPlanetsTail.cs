using Terraria;
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
            npc.boss = true;
            npc.aiStyle = -1;
            npc.npcSlots = 1f;
            npc.width = npc.height = 38;
            npc.defense = 0;
            npc.damage = 60;
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
        }
    }
}
