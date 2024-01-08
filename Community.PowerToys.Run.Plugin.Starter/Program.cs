using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wox.Infrastructure;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.Starter
{
    public class Program : IPlugin, ISettingProvider, IContextMenu
    {
        public static string PluginID => "6f34e080c4664cbda43a3bd2f9572344";
        public string Name => "Starter";
        public string Description => "Starter Project for PowerToys Run PlugIn";

        private static string? _icoPath;
        private PluginInitContext? _context;
        private PluginOption _pluginOption = new();

        public Program() { }

        #region Init Plugin and execute Query action

        public void Init(PluginInitContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.API.ThemeChanged += (Theme currentTheme, Theme newTheme) => { UpdateIconsPath(newTheme); };
            UpdateIconsPath(_context.API.GetCurrentTheme());

            static void UpdateIconsPath(Theme theme)
            {
                var isSettingLightTheme = theme == Theme.Light || theme == Theme.HighContrastWhite;
                _icoPath = isSettingLightTheme ? "Images/Star.light.png" : "Images/Star.dark.png";
            }
        }

        public List<Result> Query(Query query)
        {
            // TODO: Create Query Result
            const string url = "https://www.bing.com/";
            var results = new List<Result>
            {
                new Result
                {
                    Title = "Title1",
                    SubTitle = "SubTitle1",
                    IcoPath = _icoPath,
                    QueryTextDisplay = "QueryTextDisplay1",
                    ContextData = new ResultContextData(),
                },
                new Result
                {
                    Title = "Title2",
                    SubTitle = "SubTitle2",
                    IcoPath = _icoPath,
                    QueryTextDisplay = "QueryTextDisplay2",
                    Action = _ =>
                    {
                        Helper.OpenInShell($"microsoft-edge:{url}");
                        return true;
                    },
                    ToolTipData = new ToolTipData("Title", "Tip Text"),
                    ContextData = new ResultContextData(),
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
                Key = "BooleanOption",
                Value = false,
                DisplayLabel = "Boolean Option",
                DisplayDescription = "Boolean Option",
            },
            new PluginAdditionalOption
            {
                Key = "NumberOption",
                NumberValue = 0,
                DisplayLabel = "Number Option",
                DisplayDescription = "Number Option",
            },
            new PluginAdditionalOption
            {
                Key ="TextOption",
                TextValue = "",
                DisplayLabel = "Text Option",
                DisplayDescription = "Text Option",
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
                _pluginOption.BooleanOption = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "BooleanOption")?.Value ?? default;
                _pluginOption.NumberOption = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "NumberOption")?.NumberValue ?? default;
                _pluginOption.TextOption = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "TextOption")?.TextValue ?? default;
            }
            else
            {
                _pluginOption.BooleanOption = default;
                _pluginOption.NumberOption = default;
                _pluginOption.TextOption = default;
            }
        }

        #endregion

        #region Load Context Menu

        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult.ContextData is not ResultContextData data)
            {
                return new List<ContextMenuResult>();
            }

            return data.CreateContextMenuResult();
        }

        #endregion
    }

    public class PluginOption
    {
        public bool BooleanOption { get; set; }
        public double NumberOption { get; set; }
        public string? TextOption { get; set; }
    }

    public class ResultContextData
    {
        private static readonly string _pluginName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
        public List<ContextMenuResult> CreateContextMenuResult()
        {
            // Character Map tool
            // To open the character map tool on Windows, press Win + R keys, then enter 'charmap'
            // Choose font as FontFamily then find the glyph you need
            const string url = "https://www.bing.com/";
            return new List<ContextMenuResult>
            {
                new ContextMenuResult
                {
                    Title = "Copy DateTime (Ctrl+C)",
                    Glyph = "\xE8C8",
                    FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                    AcceleratorKey = Key.C,
                    AcceleratorModifiers = ModifierKeys.Control,
                    PluginName = _pluginName,
                    Action = _ =>
                    {
                        try
                        {
                            Clipboard.SetText(DateTime.Now.ToString());
                        }
                        catch (Exception ex)
                        {
                            Log.Exception("Failed to copy URL to clipboard", ex, typeof(ResultContextData));
                        }
                        return true;
                    },
                },
                new ContextMenuResult
                {
                    Title = "Open Windows Terminal (Ctrl+T)",
                    Glyph = "\xEE40",
                    FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                    AcceleratorKey = Key.T,
                    AcceleratorModifiers = ModifierKeys.Control,
                    PluginName = _pluginName,
                    Action = _ =>
                    {
                        try
                        {
                            Helper.OpenInShell(@"shell:AppsFolder\Microsoft.WindowsTerminal_8wekyb3d8bbwe!App");
                        }
                        catch (Exception ex)
                        {
                            Log.Exception("Failed to launch Windows Terminal", ex, typeof(ResultContextData));
                        }
                        return true;
                    },
                },
                new ContextMenuResult
                {
                    Title = "Open MicrosoftEdge InPrivate (Ctrl+P)",
                    Glyph = "\xE727",
                    FontFamily = "Segoe Fluent Icons,Segoe MDL2 Assets",
                    AcceleratorKey = Key.P,
                    AcceleratorModifiers = ModifierKeys.Control,
                    PluginName = _pluginName,
                    Action = _ =>
                    {
                        try
                        {
                            Helper.OpenInShell(@"shell:AppsFolder\Microsoft.MicrosoftEdge.Stable_8wekyb3d8bbwe!App", $"-inprivate {url}");
                        }
                        catch (Exception ex)
                        {
                            Log.Exception("Failed to launch Microsoft Edge", ex, typeof(ResultContextData));
                        }
                        return true;
                    },
                },
            };
        }
    }
}
