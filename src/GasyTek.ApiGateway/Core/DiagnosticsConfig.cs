using System.Diagnostics;

namespace GasyTek.ApiGateway.Core
{
    public static class DiagnosticsConfig
    {
        public const string ServiceName = "ApiGateway";

        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    }
}
