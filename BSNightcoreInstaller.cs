using Zenject;

namespace BSNightcore
{
    internal class BSNightcoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BSNightcoreUI>().AsSingle();
        }
    }
}
