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

            /*SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsAggressive"] = new Sky.DecimatorOfPlanets.AggressiveSky();
            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsPassive"] = new Sky.DecimatorOfPlanets.PassiveSky();
            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsLastPhase"] = new Sky.DecimatorOfPlanets.LastPhaseSky();*/
            SkyManager.Instance["SunksBossChallenges:LumiteDestroyer"] = new Sky.LumiteDestroyer.LumiteDestroyerSky();

            //Boss heads
            AddBossHeadTexture("SunksBossChallenges/NPCs/LumiteTwins/LumiteRetinazer_Head_Boss2");
            AddBossHeadTexture("SunksBossChallenges/NPCs/LumiteTwins/LumiteSpazmatism_Head_Boss2");

            //Shaders
            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(GetEffect("Effects/Content/ShockwaveEffect")); // The path to the compiled shader file.
                Filters.Scene["SunksBossChallenges:Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["SunksBossChallenges:Shockwave"].Load();

                Ref<Effect> blackHoleRef = new Ref<Effect>(GetEffect("Effects/Content/BlackHoleDistort"));
                Filters.Scene["BlackHoleDistort"] = new Filter(new ScreenShaderData(blackHoleRef, "BlackHoleDistort"), EffectPriority.VeryHigh);
                Filters.Scene["BlackHoleDistort"].Load();

                Ref<Effect> colorizeRef = new Ref<Effect>(GetEffect("Effects/Content/Colorize"));
                Filters.Scene["SunksBossChallenges:Colorize"] = new Filter(new ScreenShaderData(colorizeRef, "Colorize"), EffectPriority.VeryHigh);
                Filters.Scene["SunksBossChallenges:Colorize"].Load();

                Trail = GetEffect("Effects/Trail");
            }

            

            base.Load();
        }

        public static Effect Trail { get; set; }
        public override void Unload()
        {
            Instance = null;
            /*SkyManager.Instance.Deactivate("SunksBossChallenges:DecimatorOfPlanetsAggressive");
            SkyManager.Instance.Deactivate("SunksBossChallenges:DecimatorOfPlanetsPassive");
            SkyManager.Instance.Deactivate("SunksBossChallenges:DecimatorOfPlanetsLastPhase");*/
            //SkyManager.Instance.Deactivate("SunksBossChallenges:LumiteDestroyer");
			//SkyManager.Instance["SunksBossChallenges:LumiteDestroyer"] = null;
            base.Unload();
        }
    }
}