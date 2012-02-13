using System;
using System.Web;

using Orchard;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Title.Models;
using Orchard.Mvc;
using Orchard.Utility.Extensions;
using Szmyd.Orchard.Modules.Sharing.Models;
using Szmyd.Orchard.Modules.Sharing.Settings;
using Szmyd.Orchard.Modules.Sharing.ViewModels;

namespace Szmyd.Orchard.Modules.Sharing.Drivers
{

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
            var shareSettings = _services.WorkContext.CurrentSite.As<ShareBarSettingsPart>();

            // Prevent share bar from showing if account is not set
            if (shareSettings == null || string.IsNullOrWhiteSpace(shareSettings.AddThisAccount))
            {
                return null;
            }

            var routePart = part.As<AutoroutePart>();

            // Prevent share bar from showing when current item is not Routable
            if (routePart != null)
            {
                var typeSettings = part.Settings.GetModel<ShareBarTypePartSettings>();
                HttpRequestBase request = _httpContextAccessor.Current().Request;
                var model = new ShareBarViewModel
                {
                    Link = String.Format("{0}/{1}", request.ToApplicationRootUrlString(), routePart.Path),
                    Title = routePart.As<TitlePart>().Title,
                    Account = shareSettings.AddThisAccount,
                    Mode = typeSettings.Mode
                };

                return ContentShape("Parts_Share_ShareBar", () => shapeHelper.Parts_Share_ShareBar(ViewModel: model));

            }

            return null;
        }
    }
}