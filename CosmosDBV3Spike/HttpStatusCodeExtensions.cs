using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CosmosDBV3Spike
{
    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            return ((int) statusCode >= 200) && ((int) statusCode <= 299);
        }
    }
}
