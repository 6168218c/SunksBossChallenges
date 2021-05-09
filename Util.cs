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


namespace SunksBossChallenges
{
    public static class Util
    {
        public static void HandleRotation(this NPC npc, Vector2 direct, float rotateAccle = 0.1f)
        {
            HandleRotation(npc, direct.ToRotation() - 1.57f, rotateAccle);
        }
        public static void HandleRotation(this NPC npc, float direction, float rotateAccle = 0.1f)
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
        public static int GetRotationDirection(this NPC npc, Vector2 direct)
        {
            return GetRotationDirection(npc, direct.ToRotation() - 1.57f);
        }
        public static int GetRotationDirection(this NPC npc, float direction)
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
        public static void SlowDown(this NPC npc, float accle)
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
        public static void HoverMovement(this NPC npc, Vector2 dist, float maxSpeed, float accle)
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
        public static void WormMovement(this Entity entity, Vector2 position, float maxSpeed, float turnAccle = 0.1f, float ramAccle = 0.15f)
        {
            Vector2 targetVector = position - entity.Center;
            targetVector = Vector2.Normalize(targetVector) * maxSpeed;
            if ((targetVector.X * entity.velocity.X > 0f) && (targetVector.Y * entity.velocity.Y > 0f)) //acclerate
            {
                entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * ramAccle;
                entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * ramAccle;
            }
            if ((targetVector.X * entity.velocity.X > 0f) || (targetVector.Y * entity.velocity.Y > 0f)) //turn
            {
                entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * turnAccle;
                entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * turnAccle;

                if (Math.Abs(targetVector.Y) < maxSpeed * 0.2 && targetVector.X * entity.velocity.X < 0)
                {
                    entity.velocity.Y += Math.Sign(entity.velocity.Y) * turnAccle * 2f;
                }

                if (Math.Abs(targetVector.X) < maxSpeed * 0.2 && targetVector.Y * entity.velocity.Y < 0)
                {
                    entity.velocity.X += Math.Sign(entity.velocity.X) * turnAccle * 2f;
                }
            }
            else if (Math.Abs(targetVector.X) > Math.Abs(targetVector.Y))
            {
                entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * turnAccle * 1.1f;
                if (Math.Abs(entity.velocity.X) + Math.Abs(entity.velocity.Y) < maxSpeed * 0.5)
                {
                    entity.velocity.Y += Math.Sign(entity.velocity.Y) * turnAccle;
                }
            }
            else
            {
                entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * turnAccle * 1.1f;
                if (Math.Abs(entity.velocity.X) + Math.Abs(entity.velocity.Y) < maxSpeed * 0.5)
                {
                    entity.velocity.X += Math.Sign(entity.velocity.X) * turnAccle;
                }
            }
        }
        public static float GetLerpValue(float from, float to, float t, bool clamped = false)
        {
            if (clamped)
            {
                if (from < to)
                {
                    if (t < from)
                        return 0f;

                    if (t > to)
                        return 1f;
                }
                else
                {
                    if (t < to)
                        return 1f;

                    if (t > from)
                        return 0f;
                }
            }

            return (t - from) / (to - from);
        }
    }
}
