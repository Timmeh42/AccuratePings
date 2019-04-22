using System;
using System.Globalization;
using BepInEx;
using MonoMod.Cil;

namespace AccuratePings
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Timmeh42.AccuratePings", "Accurate Pings", "1.0.0")]
    public class AccuratePings : BaseUnityPlugin
    {
        public float ParseFloat(string strInput, float defaultVal = 1.0f, float min = float.MinValue, float max = float.MaxValue)
        {
            if (float.TryParse(strInput, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedFloat))
            {
                return parsedFloat <= min ? min : parsedFloat >= max ? max : parsedFloat;
            }
            return defaultVal;
        }

        float coneAngle => ParseFloat(Config.Wrap("AccuratePings", "Angle", "Angle in degrees of the cone in which objects are pinged. (default: 10 | suggested: 3 | min: 1)", "3.0").Value, 3.0f, 1f);
        public void Awake()
        {
            IL.RoR2.PingerController.AttemptPing += il =>
            {
                var c = new ILCursor(il);
                c.GotoNext(x => x.MatchLdcR4(10));
                c.Index++;
                c.EmitDelegate<Func<float, float>>((f) => { return coneAngle; });
            };
        }

    }
}
