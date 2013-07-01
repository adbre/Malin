using System;
using System.ServiceProcess;

namespace Malin.Host
{
    public partial class MalinServiceHost : ServiceBase
    {
        private readonly Func<IDisposable> _factory;
        private IDisposable _host;

        public MalinServiceHost(Func<IDisposable> factory)
        {
            _factory = factory;
        }

        protected override void OnStart(string[] args)
        {
            _host = _factory();
        }

        protected override void OnStop()
        {
            if (_host != null) _host.Dispose();
        }
    }
}