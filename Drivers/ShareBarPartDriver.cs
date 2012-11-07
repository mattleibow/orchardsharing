using System;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Drivers;
using Orchard.Mvc;
using Orchard.Utility.Extensions;
using Szmyd.Orchard.Modules.Sharing.Models;
using Szmyd.Orchard.Modules.Sharing.Settings;
using Szmyd.Orchard.Modules.Sharing.ViewModels;

namespace Szmyd.Orchard.Modules.Sharing.Drivers
{
    [UsedImplicitly]
    public class ShareBarPartDriver : ContentPartDriver<ShareBarPart>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrchardServices _services;

        public ShareBarPartDriver(IHttpContextAccessor httpContextAccessor, IOrchardServices services)
        {
            _httpContextAccessor = httpContextAccessor;
            _services = services;
        }

        protected override DriverResult Display(ShareBarPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Share_ShareBar", () => {
                                                                var shareSettings = _services.WorkContext.CurrentSite.As<ShareBarSettingsPart>();
                                                                var httpContext = _httpContextAccessor.Current();

                                                                // Prevent share bar from showing if account is not set
                                                                if (shareSettings == null || string.IsNullOrWhiteSpace(shareSettings.AddThisAccount))
                                                                {
                                                                    return null;
                                                                }

                                                                // Prevent share bar from showing when current item is not Routable
                                                                if (!part.Is<IAliasAspect>()) return null;

                                                                var path = part.As<IAliasAspect>().Path;
                                                                var typeSettings = part.Settings.GetModel<ShareBarTypePartSettings>();
                                                                var baseUrl = httpContext.Request.ToApplicationRootUrlString();

                                                                // remove any application path from the base url
                                                                var applicationPath = httpContext.Request.ApplicationPath ?? String.Empty;

                                                                if (path.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    path = path.Substring(applicationPath.Length);
                                                                }

                                                                baseUrl = baseUrl.TrimEnd('/');
                                                                path = path.TrimStart('/');

                                                                path = baseUrl + "/" + path;
                                                                var model = new ShareBarViewModel
                                                                {
                                                                    Link = path,
                                                                    Title = _services.ContentManager.GetItemMetadata(part).DisplayText,
                                                                    Account = shareSettings.AddThisAccount,
                                                                    Mode = typeSettings.Mode
                                                                };
                                                                return shapeHelper.Parts_Share_ShareBar(ViewModel: model);
                                                            });

        }
    }
}