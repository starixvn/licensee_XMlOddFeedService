namespace Fanex.OddsFeed.WebService
{
    using System.Web.Optimization;
    using BundleTransformer.Core.Orderers;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                    "~/content/help/css",
                    "~/content/shCore.css",
                    "~/content/shThemeDefault.css",
                    "~/stylesheets/Help/menu.css",
                    "~/stylesheets/Help/content.css");

            bundles.Add(new ScriptBundle("~/bundles/help/js").Include(
                    "~/scripts/jquery-latest.js",
                    "~/scripts/shCore.js",
                    "~/scripts/shBrushXml.js",
                    "~/scripts/Help/menu.js"));
        }

        public static void AddAndInclude(this BundleCollection bundles, string bundleVirtualPath, params string[] virtualPaths)
        {
            bundles?.Add(new Bundle(bundleVirtualPath)
            {
                Transforms = { new CssMinify() },
                Orderer = new NullOrderer()
            }.Include(virtualPaths));
        }
    }
}