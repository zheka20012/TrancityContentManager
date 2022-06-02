using System;
using System.Threading.Tasks;

namespace TrancityContentManager
{
    public interface INetworkModsProvider
    {
        public Task<ModInfo[]> GetModsFromServer(IProgress<int> progress);

        public string DownloadMod(ModInfo modInfo);
    }
}