using BepInEx;
using HarmonyLib;
using System.Reflection;
using System;

namespace CrushingSilence
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency(Unfoundry.Plugin.GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BepInEx.IL2CPP.BasePlugin
    {
        public const string
            MODNAME = "CrushingSilence",
            AUTHOR = "erkle64",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "1.0.0";

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
            private static bool silenceCurrentMachine = false;

            [HarmonyPatch(typeof(CrusherGO), nameof(CrusherGO.machineUpdates))]
            [HarmonyPrefix]
            private static void CrusherGO_machineUpdates(CrusherGO __instance)
            {
                silenceCurrentMachine = __instance.template.name.StartsWith("Crusher");
            }

            [HarmonyPatch(typeof(CrusherGO), nameof(CrusherGO.machineUpdates))]
            [HarmonyPostfix]
            private static void CrusherGO_machineUpdatesPostfix(CrusherGO __instance)
            {
                silenceCurrentMachine = false;
            }

            [HarmonyPatch(typeof(AudioSourceFader), nameof(AudioSourceFader.renderUpdate), new Type[] { typeof(bool), typeof(bool) })]
            [HarmonyPrefix]
            private static void AudioSourceFader_renderUpdate(AudioSourceFader __instance, ref bool state, ref bool enableSound)
            {
                if (silenceCurrentMachine) enableSound = false;
            }
        }
    }
}
