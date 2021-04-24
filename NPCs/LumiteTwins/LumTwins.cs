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
    public abstract class LumTwins:ModNPC
    {
        protected int brotherId = -1;
        protected float Phase { get => npc.ai[0]; set => npc.ai[0] = value; }

        public bool CheckBrother<T>() where T : LumTwins
        {
            return !(brotherId == -1 || !Main.npc[brotherId].active || Main.npc[brotherId].type != ModContent.NPCType<T>());
        }

        public void UpdateBrother<T>() where T : LumTwins
        {
            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && npc.whoAmI != i && Main.npc[i].type == ModContent.NPCType<T>())
                {
                    brotherId = i;
                    break;
                }
            }
        }

        protected void NewOrBoardcastText(string quote,Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(quote, color);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(quote), color);
        }

        protected void setRotation(Vector2 direction)
        {
            npc.rotation = direction.ToRotation() - MathHelper.PiOver2;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(brotherId);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            brotherId = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
    }
}
