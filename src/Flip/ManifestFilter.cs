using Umbraco.Cms.Core.Manifest;

namespace Flip;

internal sealed class ManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            PackageName = Constants.Name,
            Scripts =
            [
                "/App_Plugins/Flip/Backoffice/flip.min.js",
            ],
            Stylesheets =
            [
                "/App_Plugins/Flip/Backoffice/flip.min.css",
            ],
            BundleOptions = BundleOptions.None,
            Version = "11.0.0",
        });
    }
}
