namespace ocelot_common
{
    public static class Constants
    {
        // URLs
        public static string BaseUrl = "http://localhost:4000";
        public static string IdentityServerUrl = "http://localhost:5000";

        // APIs endpoints
        public static string UnsecuredApiGetRequest = $"{BaseUrl}/WeatherForecast";
        public static string SecuredApiGetRequest = $"{BaseUrl}/secured";
        public static string SecuredApiPostRequest = $"{BaseUrl}/secured";
    }
}
