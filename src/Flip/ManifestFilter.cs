using Umbraco.Cms.Core.Manifest;

namespace Flip;

internal sealed class ManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            PackageName = Constants.Name,
            Scripts = new[]
            {
                "/App_Plugins/Flip/Backoffice/flip.min.js",
            },
            Stylesheets = new[]
            {
                "/App_Plugins/Flip/Backoffice/flip.min.css",
            },
            BundleOptions = BundleOptions.None,
            Version = "10.0.0",
        });
    }
}
