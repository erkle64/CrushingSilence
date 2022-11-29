using BepInEx;
using HarmonyLib;
using System.Reflection;
using System;

namespace CrushingSilence
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Plugin : BepInEx.IL2CPP.BasePlugin
    {
        public const string
            MODNAME = "CrushingSilence",
            AUTHOR = "erkle64",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "1.1.0";

        public static BepInEx.Logging.ManualLogSource log;
        public Plugin()
        {
            log = Log;
        }

        public override void Load()
        {
            log.LogMessage((string)$"Loading {MODNAME}");

            try
            {
                var harmony = new Harmony(GUID);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
        }

        [HarmonyPatch]
        public static class Patch
        {
            [HarmonyPatch(typeof(BuildableObjectTemplate), nameof(BuildableObjectTemplate.onLoad))]
            [HarmonyPrefix]
            private static void BuildableObjectTemplate_onLoad(BuildableObjectTemplate __instance)
            {
                if (__instance.name.StartsWith("Crusher"))
                {
                    log.LogInfo((string)$"Silencing '{__instance.name}'");
                    __instance.producer_audioClip_active = null;
                }
            }
        }
    }
}
