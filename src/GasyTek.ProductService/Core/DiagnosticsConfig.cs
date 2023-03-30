using System.Diagnostics;

namespace GasyTek.ProductService.Core
{
    public static class DiagnosticsConfig
    {
        public const string ServiceName = "ProductService";

        public const string DbServiceName = "ProductServiceDatabase";

        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    }
}
