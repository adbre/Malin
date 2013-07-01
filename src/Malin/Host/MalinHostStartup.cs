using Owin;

namespace Malin.Host
{
    public class MalinHostStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}