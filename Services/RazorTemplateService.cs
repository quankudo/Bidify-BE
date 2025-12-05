using RazorLight;

namespace bidify_be.Services
{

    public class RazorTemplateService
    {
        private readonly RazorLightEngine _engine;

        public RazorTemplateService(IWebHostEnvironment env)
        {
            var templateRoot = Path.Combine(env.ContentRootPath, "Resources", "EmailTemplates");

            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templateRoot)
                .Build();
        }

        public async Task<string> RenderAsync<T>(string templateName, T model)
        {
            return await _engine.CompileRenderAsync(templateName, model);
        }
    }

}
