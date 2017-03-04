using Sitecore.Mvc.Presentation;
using System.Collections;
using System.Linq;

namespace Sitecore.Feature.Weather.Models
{
    public class CarouselRenderingModel : RenderingModel
    {
        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            CarouselSlides =
                Sitecore.Data.ID.ParseArray(Item["SelectedItems"])
                    .Select(id => Item.Database.GetItem(id)).ToList();
        }

        public IList CarouselSlides { get; private set; }
    }
}