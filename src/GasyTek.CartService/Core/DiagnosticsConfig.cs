using System.Diagnostics;

namespace GasyTek.CartService.Core
{
    public static class DiagnosticsConfig
    {
        public const string ServiceName = "CartService";

        public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
    }
}
