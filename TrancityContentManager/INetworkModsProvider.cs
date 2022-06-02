using System;
using System.Threading.Tasks;

namespace TrancityContentManager
{
    public interface INetworkModsProvider
    {
        Task<ModInfo[]> GetModsFromServer(IProgress<int> progress);

        string DownloadMod(ModInfo modInfo);
    }
}