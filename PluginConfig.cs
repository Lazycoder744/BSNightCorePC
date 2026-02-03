namespace BSNightcore
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public virtual bool ModEnabled { get; set; } = true;
        public virtual bool EnableInPractice { get; set; } = true;
        public virtual bool EnableInSlowerMode { get; set; } = true;
        public virtual bool EnableInMenu { get; set; } = true;
        public virtual bool EnableUnlimitedPreviews { get; set; } = true;
    }
}