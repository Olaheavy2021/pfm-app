using System.Text.Json;
using Microsoft.Playwright;
using PersonalFinanceManager.Application.Data.DTOs;

namespace PersonalFinanceManager.PlaywrightTests.Helpers;

public class TransactionTypeHelper
{
    public static async Task<IReadOnlyList<TransactionTypeDto>> GetTransactionTypesAsync(
        IPage page,
        string baseApiUrl,
        JsonSerializerOptions jsonOptions
    )
    {
        var response = await page.APIRequest.GetAsync(
            $"{baseApiUrl}{TestConstants.API_TRANSACTION_TYPES_ENDPOINT}"
        );
        Assert.Equal(200, response.Status);
        var transactionTypesJson = await response.TextAsync();
        var transactionTypes = JsonSerializer.Deserialize<List<TransactionTypeDto>>(
            transactionTypesJson,
            jsonOptions
        );
        Assert.NotNull(transactionTypes);
        Assert.NotEmpty(transactionTypes);
        return transactionTypes;
    }
}
