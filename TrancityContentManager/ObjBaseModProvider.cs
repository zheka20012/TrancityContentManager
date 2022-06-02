using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;

namespace TrancityContentManager
{
    /// <summary>
    /// Provider for https://obj-base.ucoz.org/
    /// </summary>
    public class ObjBaseModProvider : INetworkModsProvider
    {
        private const string LoadUri = "https://obj-base.ucoz.org/";
        private IProgress<int> taskProgress;

        public async Task<ModInfo[]> GetModsFromServer(IProgress<int> progress)
        {
            taskProgress = progress;
            return await FindModsOnServer();
        }

        public string DownloadMod(ModInfo modInfo)
        {
            throw new System.NotImplementedException();
        }

        private async Task<IDocument> OpenPage(string address)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader() ;
            IBrowsingContext context = BrowsingContext.New(config);
            return await context.OpenAsync(address);
        }

        private async Task<ModInfo[]> FindModsOnServer()
        {
            IDocument document = await OpenPage(LoadUri + "load/");

            int filesCount = int.Parse(document.QuerySelector("div.items-stat b").Text());
            int gotFiles = 0;
            int currentPage = 1;
            int pagesCount = (int)Math.Ceiling(filesCount / 10f);
            string fileSelector = "eTitle";

            List<ModInfo> mods = new List<ModInfo>();

            while (currentPage <= pagesCount)
            {
                document = await OpenPage($"{LoadUri}load/?page{currentPage}");

                var titles = document.GetElementsByClassName(fileSelector);

                foreach (var title in titles)
                {
                    mods.Add(await GetModInfo(((IHtmlAnchorElement) title.ChildNodes[0]).Href));
                    gotFiles++;
                    taskProgress.Report((int)((gotFiles / (float)filesCount) * 100f));
                }

                currentPage++;
            }


            return mods.ToArray();
        }

        private async Task<ModInfo> GetModInfo(string address)
        {
            IDocument document = await OpenPage(address);

            ModInfo info = new ModInfo();

            info.Name = document.QuerySelector("td.content div.eTitle").Text();
            info.Uploader = document.QuerySelector("span.e-author span.ed-value").Text();



            var cells = document.QuerySelectorAll("td.content td");

            info.DownloadLink = ((IHtmlAnchorElement) cells[0].QuerySelector("a")).Href;

            string pattern = @"\((.+)\)";
            RegexOptions options = RegexOptions.Multiline;
            string input = cells[0].Text();

            info.FileSize = Regex.Match(input, pattern, options).Groups[1].Value;

            info.DateUploaded = cells[1].Text();

            string imageAddress = cells[2].QuerySelector("img").GetAttribute("src");
            Uri result;
            if (!Uri.TryCreate(imageAddress, UriKind.Absolute, out result))
            {
                imageAddress = LoadUri + imageAddress;
            }

            try
            {

                var request = WebRequest.Create(imageAddress);
                var response = await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                info.Image = new Bitmap(responseStream);
            }
            catch (Exception e)
            {
                MessageBox.Show(imageAddress, e.ToString(), MessageBoxButtons.OKCancel);
            }
            

            info.Type = ModType.Object;

            return info;
        }
    }
}