using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace SunksBossChallenges.Sky
{
	public abstract class BasicColoredSky : CustomSky
	{
		protected abstract int NPCType { get; }

		protected abstract Color SkyColor { get; }

		private UnifiedRandom _random = new UnifiedRandom();

		private bool _isActive;
		private bool _isLeaving;
		private float _opacity;

		private int _npcIndex = -1;

		public override void OnLoad()
		{
		}

		public override void Update(GameTime gameTime)
		{
			if (Main.gamePaused || !Main.hasFocus)
			{
				return;
			}
			if (this._isLeaving)
			{
				this._opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (this._opacity < 0f)
				{
					this._isActive = false;
					this._opacity = 0f;
				}
			}
			else
			{
				this._opacity += (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (this._opacity > 1f)
				{
					this._opacity = 1f;
				}
			}
		}

		private float GetIntensity()
		{
			if (this.UpdateNPCIndex())
			{
				float x = 0f;
				if (this._npcIndex != -1)
				{
					x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[this._npcIndex].Center);
				}
				return 1f - Utils.SmoothStep(3000f, 6000f, x);
			}
			return 0f;
		}

		public override Color OnTileColor(Color inColor)
		{
			float intensity = this.GetIntensity();
			return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
		}

		private bool UpdateNPCIndex()
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

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 0f && minDepth < 0f)
			{
				float intensity = this.GetIntensity();
				intensity = MathHelper.Min(intensity, 0.375f);
				spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), SkyColor * intensity * _opacity);
			}
		}

		public override float GetCloudAlpha()
		{
			return 0f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this._isActive = true;
			this._isLeaving = false;
		}

		public override void Deactivate(params object[] args)
		{
			//this._isActive = false;
			this._isLeaving = true;
		}

		public override void Reset()
		{
			this._isActive = false;
		}

		public override bool IsActive()
		{
			return this._isActive;
		}
	}
}
