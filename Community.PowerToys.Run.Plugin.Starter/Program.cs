using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Security.Policy;
using System.Windows.Controls;
using Wox.Infrastructure;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.Starter
{
    public class Program : IPlugin, ISettingProvider
    {
        public static string PluginID => "6f34e080c4664cbda43a3bd2f9572344";
        public string Name => "Starter";
        public string Description => "Starter Project for PowerToys Run PlugIn";

        private PluginInitContext? _context;
        private PluginOption _pluginOption = new();

        public Program() { }

        #region Init Plugin and execute Query action

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += OnThemeChanged;
            //UpdateIconsPath(_context.API.GetCurrentTheme());

            void OnThemeChanged(Theme currentTheme, Theme newTheme)
            {
                //UpdateIconsPath(newTheme);
            }
        }

        public List<Result> Query(Query query)
        {
            var url = "https://www.bing.com/";
            var results = new List<Result>
            {
                new Result
                {
                    Title = "Title1",
                    SubTitle = "SubTitle1",
                    IcoPath = "Images/Star.dark.png",
                    QueryTextDisplay = "QueryTextDisplay",
                    ContextData = this,
                },
                new Result
                {
                    Title = "Title2",
                    SubTitle = "SubTitle2",
                    IcoPath = "Images/Star.dark.png",
                    QueryTextDisplay = "QueryTextDisplay",
                    Action = _ =>
                    {
                        Helper.OpenInShell($"microsoft-edge:{url}");
                        return true;
                    },
                    ToolTipData = new ToolTipData("Title", "Tip Text"),
                    ContextData = this,
                }
            };

            Log.Info("Logging...", typeof(Program));

            return results.OrderBy(r => r.Title).ToList();
        }

        #endregion

        #region Setting

        public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>
        {
            new PluginAdditionalOption
            {
                Key = "Option1",
                Value = false,
                DisplayLabel = "Plugin Option 1",
                DisplayDescription = "Plugin Option 1",
            },
        };

        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            if (settings != null && settings.AdditionalOptions != null)
            {
                _pluginOption.Option1 = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "Option1")?.Value ?? default;
            }
            else
            {
                _pluginOption.Option1 = default;
            }
        }

        #endregion

        #region Load Context Menu

        //public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        //{
        //    if (selectedResult.ContextData is not FavoriteItem favorite)
        //    {
        //        return new List<ContextMenuResult>();
        //    }

        //    return favorite.CreateContextMenuResult();
        //}

        #endregion
    }

    internal class PluginOption
    {
        public bool Option1 { get; set; }
    }
}
