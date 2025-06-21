using System.Text.Json;
using AppHost;
using PersonalFinanceManager.Application.Data.DTOs;

namespace PersonalFinanceManager.PlaywrightTests.Endpoints;

public class TransactionTypeTests(AspireManager aspireManager) : BasePlaywrightTests(aspireManager)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiGetTransactionTypes()
    {
        await SetupAsync();

        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_TYPES_ENDPOINT}"
                );
                var transactionTypeJson = await response.TextAsync();
                var transactionTypes = JsonSerializer.Deserialize<List<TransactionTypeDto>>(
                    transactionTypeJson,
                    _jsonOptions
                );
                Assert.Equal(200, response.Status);
                Assert.NotNull(transactionTypes);
                Assert.True(transactionTypes.Count > 0);
            }
        );
    }

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiGetTransactionTypeById()
    {
        await SetupAsync();

        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_TYPES_ENDPOINT}"
                );
                var transactionTypeJson = await response.TextAsync();
                var transactionTypes = JsonSerializer.Deserialize<List<TransactionTypeDto>>(
                    transactionTypeJson,
                    _jsonOptions
                );
                Assert.Equal(200, response.Status);
                Assert.NotNull(transactionTypes);
                Assert.True(transactionTypes.Count > 0);
                var firstTransactionType = transactionTypes.First();
                response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_TYPES_ENDPOINT}/{firstTransactionType.Id}"
                );
                var transactionTypeByIdJson = await response.TextAsync();
                var transactionTypeById = JsonSerializer.Deserialize<TransactionTypeDto>(
                    transactionTypeByIdJson,
                    _jsonOptions
                );
                Assert.Equal(200, response.Status);
                Assert.NotNull(transactionTypeById);
                Assert.Equal(firstTransactionType.Id, transactionTypeById.Id);
                Assert.Equal(firstTransactionType.Name, transactionTypeById.Name);
                Assert.Equal(firstTransactionType.Description, transactionTypeById.Description);
            }
        );
    }
}
