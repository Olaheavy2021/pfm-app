namespace PersonalFinanceManager.PlaywrightTests;

internal class TestConstants
{
    internal const string API_VERSION = "api/v1/";
    internal const string API_WEATHER_FORECAST_ENDPOINT = $"{API_VERSION}weatherForecasts";
    internal const string API_TRANSACTION_CATEGORIES_ENDPOINT =
        $"{API_VERSION}transactionCategories";
    internal const string API_AUTH_LOGIN_ENDPOINT = $"{API_VERSION}auth/login";
    internal const string API_AUTH_REGISTER_ENDPOINT = $"{API_VERSION}auth/register";
}
