using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Webgostar.Framework.Presentation.Web.Middlewares;

namespace Webgostar.Framework.Presentation.Web
{
    public static class FrameworkPresentationWebUseApp
    {
        public static IApplicationBuilder UseFrameworkPresentationWeb(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(option =>
                {
                    option.EnablePersistAuthorization();

                    using var scope = app.Services.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                    foreach (var item in service.ApiVersionDescriptions)
                    {
                        option.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json", item.GroupName);
                    }
                });
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseSwagger();
                //app.UseSwaggerUI(option =>
                //{
                //    option.EnablePersistAuthorization();

                //    using var scope = app.Services.CreateScope();
                //    var service = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                //    foreach (var item in service.ApiVersionDescriptions)
                //    {
                //        option.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json", item.GroupName.ToUpper());
                //    }
                //});
                app.UseHsts();
            }

            app.UseCors("TarazWebGostar");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseDefaultFiles();
            //app.UseStaticFiles();
            app.UseApiExceptionHandler();
            app.MapControllers();
            return app;
        }
    }
}
