using MaxSys.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaxSys.Controllers.Components
{
    public class ComponentTableCardViewComponent : ViewComponent
    {
        public virtual async Task<IViewComponentResult> InvokeAsync(COM_TABLE_CARD componentTableCard)
        {
            if (componentTableCard == null) { componentTableCard = new COM_TABLE_CARD(); }

            return await Task.FromResult((IViewComponentResult)View("ComponentTableCard", componentTableCard));
        }
    }
}
