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
        protected float Relaxed => 98f;
        protected float EnragedStateTrans => 99f;
        protected float EnragedState => 100f;
        protected int brotherId = -1;

        protected float fastHoverAccle => 0.75f;
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

        protected void NewOrBoardcastText(string quote, Color color, bool combatText = true)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(quote, color);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(quote), color);
            CombatText.NewText(npc.Hitbox, color, quote);
        }

        protected void SetRotation(Vector2 direction)
        {
            npc.rotation = direction.ToRotation() - 1.57f;
        }

        protected void HandleRotation(Vector2 direct, float rotateAccle = 0.1f)
        {
            HandleRotation(direct.ToRotation() - 1.57f, rotateAccle);
        }
        protected void HandleRotation(float direction, float rotateAccle = 0.1f)
        {
            rotateAccle = Math.Abs(rotateAccle);
            if (direction < 0f)
            {
                direction += 6.283f;
            }
            else if ((double)direction > 6.283f)
            {
                direction -= 6.283f;
            }
            if (npc.rotation < direction)
            {
                if ((double)(direction - npc.rotation) > 3.1415)
                {
                    npc.rotation -= rotateAccle;
                }
                else
                {
                    npc.rotation += rotateAccle;
                }
            }
            else if (npc.rotation > direction)
            {
                if ((double)(npc.rotation - direction) > 3.1415)
                {
                    npc.rotation += rotateAccle;
                }
                else
                {
                    npc.rotation -= rotateAccle;
                }
            }
            /*if ((double)Math.Abs(npc.rotation - direction) > 3.1415)
            {
                npc.rotation += rotateAccle * Math.Sign(npc.rotation - direction);
            }
            else
            {
                npc.rotation -= rotateAccle * Math.Sign(npc.rotation - direction);
            }*/
            if (npc.rotation > direction - rotateAccle && npc.rotation < direction + rotateAccle)
            {
                npc.rotation = direction;
            }
            if (npc.rotation < 0f)
            {
                npc.rotation += 6.283f;
            }
            else if ((double)npc.rotation > 6.283f)
            {
                npc.rotation -= 6.283f;
            }
            if (npc.rotation > direction - rotateAccle && npc.rotation < direction + rotateAccle)
            {
                npc.rotation = direction;
            }
        }
        protected int GetRotationDirection(Vector2 direct)
        {
            return GetRotationDirection(direct.ToRotation() - 1.57f);
        }
        protected int GetRotationDirection(float direction)
        {
            if (npc.rotation < 0f)
            {
                npc.rotation += 6.283f;
            }
            else if ((double)npc.rotation > 6.283f)
            {
                npc.rotation -= 6.283f;
            }
            if (direction < 0f)
            {
                direction += 6.283f;
            }
            else if ((double)direction > 6.283f)
            {
                direction -= 6.283f;
            }
            if (npc.rotation < direction)
            {
                if ((double)(direction - npc.rotation) > 3.1415)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (npc.rotation > direction)
            {
                if ((double)(npc.rotation - direction) > 3.1415)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return 0;
        }
        protected void SlowDown(float accle)
        {
            npc.velocity.X *= accle;
            npc.velocity.Y *= accle;
            if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
            {
                npc.velocity.X = 0f;
            }
            if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
            {
                npc.velocity.Y = 0f;
            }
        }
        protected void HoverMovement(Vector2 dist,float maxSpeed,float accle)
        {
            Vector2 velo = dist - npc.Center;
            velo *= maxSpeed / velo.Length();
            if (npc.velocity.X < velo.X)
            {
                npc.velocity.X += accle;
                if (npc.velocity.X < 0f && velo.X > 0f)
                {
                    npc.velocity.X += accle;
                }
            }
            else if (npc.velocity.X > velo.X)
            {
                npc.velocity.X -= accle;
                if (npc.velocity.X > 0f && velo.X < 0f)
                {
                    npc.velocity.X -= accle;
                }
            }
            if (npc.velocity.Y < velo.Y)
            {
                npc.velocity.Y += accle;
                if (npc.velocity.Y < 0f && velo.Y > 0f)
                {
                    npc.velocity.Y += accle;
                }
            }
            else if (npc.velocity.Y > velo.Y)
            {
                npc.velocity.Y -= accle;
                if (npc.velocity.Y > 0f && velo.Y < 0f)
                {
                    npc.velocity.Y -= accle;
                }
            }
        }

        protected void DrawAim(SpriteBatch spriteBatch, Vector2 endpoint,Color color)
        {
            Texture2D aimTexture = mod.GetTexture("NPCs/LumiteTwins/RayAim");
            Vector2 unit = endpoint - npc.Center;
            float length = unit.Length();
            unit.Normalize();
            for (int k = 0; k <= length; k += 4)
            {
                Vector2 drawPos = npc.Center + unit * k - Main.screenPosition;
                Color alphaCenter = color * 0.8f;
                spriteBatch.Draw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(brotherId);
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            brotherId = reader.ReadInt32();
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
        }
    }
}
