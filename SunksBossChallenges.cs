using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;

namespace SunksBossChallenges
{
	public class SunksBossChallenges : Mod
	{
        public static SunksBossChallenges Instance { get; private set; }
        public override void Load()
        {
            Instance = this;

            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsAggressive"] = new Sky.DecimatorOfPlanets.AggressiveSky();
            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsPassive"] = new Sky.DecimatorOfPlanets.PassiveSky();
            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsLastPhase"] = new Sky.DecimatorOfPlanets.LastPhaseSky();
            SkyManager.Instance["SunksBossChallenges:LumiteDestroyer"] = new Sky.LumiteDestroyer.LumiteDestroyerSky();

            //Boss heads
            AddBossHeadTexture("SunksBossChallenges/NPCs/LumiteTwins/LumiteRetinazer_Head_Boss2");
            AddBossHeadTexture("SunksBossChallenges/NPCs/LumiteTwins/LumiteSpazmatism_Head_Boss2");

            base.Load();
        }

        public override void Unload()
        {
            Instance = null;
            SkyManager.Instance.Deactivate("SunksBossChallenges:DecimatorOfPlanetsAggressive");
            SkyManager.Instance.Deactivate("SunksBossChallenges:DecimatorOfPlanetsPassive");
            SkyManager.Instance.Deactivate("SunksBossChallenges:DecimatorOfPlanetsLastPhase");
            base.Unload();
        }
    }
}