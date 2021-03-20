using Terraria.ModLoader;
using Terraria;
using Terraria.Graphics.Effects;

namespace SunksBossChallenges
{
	public class SunksBossChallenges : Mod
	{
        public override void Load()
        {
            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsAggressive"] = new Sky.DecimatorOfPlanets.AggressiveSky();
            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsPassive"] = new Sky.DecimatorOfPlanets.PassiveSky();
            SkyManager.Instance["SunksBossChallenges:DecimatorOfPlanetsLastPhase"] = new Sky.DecimatorOfPlanets.LastPhaseSky();
            base.Load();
        }
    }
}