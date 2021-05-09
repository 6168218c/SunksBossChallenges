using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace SunksBossChallenges.Sky
{
    public abstract class LinearGradientSky:CustomSky
    {
		public abstract string BackgroundTexture { get; }
		public abstract int NPCType { get; }
		public virtual double yOffset { get; } = 2400.0;

		private Texture2D _bgTexture;

		private bool _isActive;

		private float _fadeOpacity;

		private int _npcIndex;

		public override void OnLoad()
		{
			this._bgTexture = SunksBossChallenges.Instance.GetTexture(BackgroundTexture);
		}

		private bool CheckAlive()
		{
			if (this._npcIndex >= 0 && Main.npc[this._npcIndex].active &&
				Main.npc[this._npcIndex].type == NPCType)
			{
				return true;
			}
			int num = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == NPCType)
				{
					num = i;
					break;
				}
			}
			this._npcIndex = num;
			return num != -1;
		}

		public override void Update(GameTime gameTime)
		{
			if (Main.gamePaused || !Main.hasFocus)
			{
				return;
			}
            if (!CheckAlive())
            {
				this._isActive = false;
            }
			if (this._isActive)
			{
				this._fadeOpacity = Math.Min(1f, 0.01f + this._fadeOpacity);
			}
			else
			{
				this._fadeOpacity = Math.Max(0f, this._fadeOpacity - 0.01f);
			}
		}

		public override Color OnTileColor(Color inColor)
		{
			return new Color(Vector4.Lerp(inColor.ToVector4(), Vector4.One, this._fadeOpacity * 0.5f));
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
			{
				spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * this._fadeOpacity);
				spriteBatch.Draw(this._bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - (double)Main.screenPosition.Y - yOffset/*modify this?*/) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * this._fadeOpacity));
			}
		}

		public override float GetCloudAlpha()
		{
			return (1f - this._fadeOpacity) * 0.3f + 0.7f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this._fadeOpacity = 0.002f;
			this._isActive = true;
		}

		public override void Deactivate(params object[] args)
		{
			this._isActive = false;
		}

		public override void Reset()
		{
			this._isActive = false;
		}

		public override bool IsActive()
		{
			if (!this._isActive)
			{
				return this._fadeOpacity > 0.001f;
			}
			return true;
		}
	}
}
