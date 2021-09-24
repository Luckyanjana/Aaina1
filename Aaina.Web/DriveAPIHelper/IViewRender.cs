using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aaina.Web
{
    public interface IViewRender
    {
        string Render(string name);


        string Render<TModel>(string name, TModel model);


        string Render<TModel>(TModel model);
    }
}
