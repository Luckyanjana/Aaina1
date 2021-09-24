
namespace Aaina.Web
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;

    public class ViewRender : IViewRender
    {

        private readonly IRazorViewEngine viewEngine;


        private readonly ITempDataProvider tempDataProvider;


        private readonly IServiceProvider serviceProvider;


        public ViewRender(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            this.viewEngine = viewEngine;
            this.tempDataProvider = tempDataProvider;
            this.serviceProvider = serviceProvider;
        }


        public string Render(string name)
        {
            var actionContext = this.GetActionContext();

            var viewEngineResult = this.viewEngine.FindView(actionContext, name, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary()),
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        this.tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                view.RenderAsync(viewContext).GetAwaiter().GetResult();

                return output.ToString();
            }
        }

        public string Render<TModel>(TModel model)
        {
            var actionContext = this.GetActionContext();
            string actionName = actionContext.RouteData.Values["action"].ToString();
            var viewEngineResult = this.viewEngine.FindView(actionContext, actionName, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", actionName));
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary()),
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        this.tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                view.RenderAsync(viewContext).GetAwaiter().GetResult();

                return output.ToString();
            }
        }


        public string Render<TModel>(string name, TModel model)
        {
            var actionContext = this.GetActionContext();

            var viewEngineResult = this.viewEngine.FindView(actionContext, name, true);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model,
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        this.tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                view.RenderAsync(viewContext).GetAwaiter().GetResult();

                return output.ToString();
            }
        }


        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = this.serviceProvider;
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
