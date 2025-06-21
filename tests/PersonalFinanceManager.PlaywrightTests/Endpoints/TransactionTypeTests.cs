using System.Text.Json;
using AppHost;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalFinanceManager.Application.Data.DTOs;
using PersonalFinanceManager.Application.Data.Models;
using PersonalFinanceManager.PlaywrightTests.Helpers;

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
                await TransactionTypeHelper.GetTransactionTypesAsync(
                    page,
                    BaseApiUrl!,
                    _jsonOptions
                );
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

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiCreateTransactionType()
    {
        await SetupAsync();
        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var transactionCategories =
                    await TransactionCategoryHelper.GetTransactionCategoriesAsync(
                        page,
                        BaseApiUrl!,
                        _jsonOptions
                    );

                var token = await AuthHelper.AuthenticateAsync(page, BaseApiUrl!, _jsonOptions);

                var data = new Dictionary<string, object>()
                {
                    { "Name", "Test Transaction Type" },
                    { "Description", "This is a test transaction type" },
                    { "Status", 1 },
                    { "TransactionCategoryId", transactionCategories[0].Id },
                };
                var headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" },
                };
                var response = await page.APIRequest.PostAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_TYPES_ENDPOINT}",
                    new() { DataObject = data, Headers = headers }
                );

                var transactionTypeJson = await response.TextAsync();
                var createdTransactionType = JsonSerializer.Deserialize<TransactionTypeDto>(
                    transactionTypeJson,
                    _jsonOptions
                );
                Assert.Equal(201, response.Status);
                Assert.NotNull(createdTransactionType);
                Assert.Equal("Test Transaction Type", createdTransactionType.Name);
                Assert.Equal("This is a test transaction type", createdTransactionType.Description);
                Assert.Equal(EntityEnum.Status.Enabled, createdTransactionType.Status);
            }
        );
    }
}
