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
    public class FinalPhaseSummoner:ModNPC
    {
        public override string Texture => "Terraria/NPC_" + NPCID.StardustWormHead;
#if DEBUG
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("You felt it's not over");
        }
#endif
        public override void SetDefaults()
        {
            npc.dontTakeDamage = true;
            npc.npcSlots = 1;
            npc.life = 1;
            npc.noGravity = npc.noTileCollide = true;
            npc.netAlways = true;
            npc.alpha = 255;
            npc.boss = false;
        }

        public override void AI()
        {
            npc.ai[0]++;
            npc.active = true;
            npc.life = 1;
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
                npc.TargetClosest();

            if (npc.ai[0] == 120)
                Main.NewText("Don't you think I'm so easy a boss to defeat.", 122, 122, 255);
            if (npc.ai[0] == 180)
                Main.NewText("Well,good news for you.", 122, 122, 255);
            if (npc.ai[0] == 300)
            {
                Main.NewText("IT'S NOT OVER YET!", 122, 122, 255);
                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<DecimatorOfPlanetsHead>());
                //NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DecimatorOfPlanetsHead>());
                //Main.NewText(Terraria.Localization.Language.GetTextValue("Announcement.HasAwoken", "The Decimator Of Planets"), 175, 75);
                npc.life = 0;
                npc.checkDead();
            }
        }

        public override bool CheckActive()
        {
            return false;
        }
    }
}
