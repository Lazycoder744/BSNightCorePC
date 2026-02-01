using System.Reflection;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace BSNightcore
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static Harmony HarmonyInstance { get; private set; }

        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config)
        {
            Instance = this;
            Log = logger;
            PluginConfig.Instance = config.Generated<PluginConfig>();
            HarmonyInstance = new Harmony("com.github.bsnightcore");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Info("BSNightcore starting...");
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            Log.Info("BSNightcore started!");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Info("BSNightcore stopping...");
            HarmonyInstance.UnpatchSelf();
        }
    }
}
