using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.ViewComponents
{
    public class TestViewComponent : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(string text)
        {
            await Task.Delay(1);

            return View(model: text);
        }

    }
}
