using System.Text.Json;
using Microsoft.Playwright;
using PersonalFinanceManager.Application.Data.DTOs;

namespace PersonalFinanceManager.PlaywrightTests.Helpers;

internal class TransactionCategoryHelper
{
    public static async Task<IReadOnlyList<TransactionCategoryDto>> GetTransactionCategoriesAsync(
        IPage page,
        string baseApiUrl,
        JsonSerializerOptions jsonOptions
    )
    {
        var response = await page.APIRequest.GetAsync(
            $"{baseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}"
        );
        Assert.Equal(200, response.Status);
        var categoriesJson = await response.TextAsync();
        var categories = JsonSerializer.Deserialize<List<TransactionCategoryDto>>(
            categoriesJson,
            jsonOptions
        );
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
        return categories;
    }
}
