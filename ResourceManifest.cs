using Orchard.UI.Resources;

namespace Szmyd.Orchard.Modules.Sharing {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
        }
    }
}
