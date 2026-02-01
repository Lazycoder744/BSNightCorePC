using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace BSNightcore.HarmonyPatches
{
    [HarmonyPatch(typeof(LevelCollectionViewController), "SongPlayerCrossfadeToLevel")]
    public class SongPlayerCrossFadePatch
    {
        private static Dictionary<string, float> oldPreviewDurations = new Dictionary<string, float>();
        private static FieldInfo _previewDurationField;

        static SongPlayerCrossFadePatch()
        {
            _previewDurationField = typeof(BeatmapLevel).GetField("previewDuration", BindingFlags.Instance | BindingFlags.Public);
        }

        static void Prefix(BeatmapLevel level)
        {
            if (PluginConfig.Instance.EnableUnlimitedPreviews && _previewDurationField != null)
            {
                float oldDuration = level.previewDuration;
                oldPreviewDurations[level.levelID] = oldDuration;
                _previewDurationField.SetValue(level, -1f);
            }
        }

        public static void ClearCache()
        {
            oldPreviewDurations.Clear();
        }

        public static Dictionary<string, float> GetCache()
        {
            return oldPreviewDurations;
        }
    }

    [HarmonyPatch(typeof(LevelCollectionViewController), "UnloadPreviewAudioClip")]
    public class UnloadPreviewPatch
    {
        private static FieldInfo _previewDurationField;

        static UnloadPreviewPatch()
        {
            _previewDurationField = typeof(BeatmapLevel).GetField("previewDuration", BindingFlags.Instance | BindingFlags.Public);
        }

        static void Prefix(BeatmapLevel level)
        {
            var cache = SongPlayerCrossFadePatch.GetCache();
            if (cache.ContainsKey(level.levelID) && _previewDurationField != null)
            {
                _previewDurationField.SetValue(level, cache[level.levelID]);
            }
        }
    }
}
