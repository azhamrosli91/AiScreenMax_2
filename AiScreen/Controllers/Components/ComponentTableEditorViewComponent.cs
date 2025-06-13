using BaseModel.Models.Component;
using Microsoft.AspNetCore.Mvc;

namespace MaxSys.Controllers.Components
{
    public class ComponentTableEditorViewComponent : ViewComponent
    {
        public virtual async Task<IViewComponentResult> InvokeAsync(COM_TABLE_EDITOR com_TABLE_EDITOR) 
        {
            if (com_TABLE_EDITOR == null) { com_TABLE_EDITOR = new COM_TABLE_EDITOR(); }

            return await Task.FromResult((IViewComponentResult)View("ComponentTableEditor", com_TABLE_EDITOR));
        }

    }
}
