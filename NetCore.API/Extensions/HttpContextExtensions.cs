using NetCore.DataAccess.DataObject.DTOs.Auth;

namespace NetCore.API.Extensions
{
    public static class HttpContextExtensions
    {
        public static DeviceInfo GetDeviceInfo(
            this HttpContext httpContext)
        {
            // ======================================================
            // STEP 1:
            // Get User-Agent from request header
            // ======================================================
            //
            // Example:
            // Mozilla/5.0 (Windows NT 10.0; Win64; x64)
            //
            string? userAgent =
                httpContext.Request.Headers["User-Agent"]
                    .FirstOrDefault();

            // ======================================================
            // STEP 2:
            // Get client IP address
            // ======================================================
            //
            // NOTE:
            // If using nginx/reverse proxy,
            // should use X-Forwarded-For
            //
            string? ipAddress =
                httpContext.Connection
                    .RemoteIpAddress
                    ?.ToString();

            // ======================================================
            // STEP 3:
            // Get device id from header
            // ======================================================
            //
            // Frontend should generate UUID
            // and store in localStorage
            //
            // Example:
            // x-device-id: 123-abc
            //
            string? deviceId =
                httpContext.Request.Headers["x-device-id"]
                    .FirstOrDefault();

            // ======================================================
            // STEP 4:
            // Generate readable device name
            // ======================================================
            //
            // Normally senior systems use:
            // UAParser / DeviceDetector.NET
            //
            string? deviceName =
                GetDeviceName(userAgent);

            return new DeviceInfo
            {
                DeviceID = deviceId,
                DeviceName = deviceName,
                IPAddress = ipAddress,
                UserAgent = userAgent
            };
        }

        private static string GetDeviceName(
            string? userAgent)
        {
            if (userAgent.Contains("Windows"))
            {
                return "Windows";
            }

            if (userAgent.Contains("Android"))
            {
                return "Android";
            }

            if (userAgent.Contains("iPhone"))
            {
                return "iPhone";
            }

            if (userAgent.Contains("Mac"))
            {
                return "MacOS";
            }

            return userAgent;
        }
    }
}
