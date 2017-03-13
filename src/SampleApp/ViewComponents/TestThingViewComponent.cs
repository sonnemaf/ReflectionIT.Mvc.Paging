using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.ViewComponents
{
    public class TestThingViewComponent : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(string textData)
        {
            await Task.Delay(1);

            return View(model: textData);
        }

    }
}
