using System.Text.Json;
using AppHost;
using Microsoft.AspNetCore.Authentication.BearerToken;
using PersonalFinanceManager.Application.Data.DTOs;
using PersonalFinanceManager.Application.Data.Models;
using PersonalFinanceManager.PlaywrightTests.Helpers;

namespace PersonalFinanceManager.PlaywrightTests.Endpoints;

public class TransactionCategoryTests(AspireManager aspireManager)
    : BasePlaywrightTests(aspireManager)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiGetTransactionCategories()
    {
        await SetupAsync();

        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}"
                );
                var transactionCategoryJson = await response.TextAsync();
                var transactionCategories = JsonSerializer.Deserialize<
                    List<TransactionCategoryDto>
                >(transactionCategoryJson, _jsonOptions);
                Assert.Equal(200, response.Status);
                Assert.NotNull(transactionCategories);
                Assert.True(transactionCategories.Count > 0);
            }
        );
    }

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiGetTransactionCategoryById()
    {
        await SetupAsync();

        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}"
                );
                var transactionCategoryJson = await response.TextAsync();
                var transactionCategories = JsonSerializer.Deserialize<
                    List<TransactionCategoryDto>
                >(transactionCategoryJson, _jsonOptions);
                Assert.Equal(200, response.Status);
                Assert.NotNull(transactionCategories);
                Assert.True(transactionCategories.Count > 0);
                var firstCategory = transactionCategories.First();
                response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}/{firstCategory.Id}"
                );
                var transactionCategoryByIdJson = await response.TextAsync();
                var transactionCategoryById = JsonSerializer.Deserialize<TransactionCategoryDto>(
                    transactionCategoryByIdJson,
                    _jsonOptions
                );
                Assert.Equal(200, response.Status);
                Assert.NotNull(transactionCategoryById);
                Assert.Equal(firstCategory.Id, transactionCategoryById.Id);
                Assert.Equal(firstCategory.Name, transactionCategoryById.Name);
                Assert.Equal(firstCategory.Description, transactionCategoryById.Description);
            }
        );
    }

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiCreateTransactionCategory()
    {
        await SetupAsync();

        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var token = await AuthHelper.AuthenticateAsync(page, BaseApiUrl!, _jsonOptions);

                var data = new Dictionary<string, object>()
                {
                    { "Name", "Test Category" },
                    { "Description", "This is a test category" },
                    { "Status", 1 },
                };
                var headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" },
                };
                var response = await page.APIRequest.PostAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}",
                    new() { DataObject = data, Headers = headers }
                );

                var transactionCategoryJson = await response.TextAsync();
                var createdTransactionCategory = JsonSerializer.Deserialize<TransactionCategoryDto>(
                    transactionCategoryJson,
                    _jsonOptions
                );
                Assert.Equal(201, response.Status);
                Assert.NotNull(createdTransactionCategory);
                Assert.Equal("Test Category", createdTransactionCategory.Name);
                Assert.Equal("This is a test category", createdTransactionCategory.Description);
                Assert.Equal(EntityEnum.Status.Enabled, createdTransactionCategory.Status);
            }
        );
    }

    [IgnoreOnGitHubActionsFact]
    public async Task TestApiUpdateTransactionCategory()
    {
        await SetupAsync();

        await InteractWithPageAsync(
            AppHostConstants.ApiServiceProject,
            async page =>
            {
                var token = await AuthHelper.AuthenticateAsync(page, BaseApiUrl!, _jsonOptions);

                var headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" },
                };
                var response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}"
                );

                var transactionCategoryJson = await response.TextAsync();
                var transactionCategories = JsonSerializer.Deserialize<
                    List<TransactionCategoryDto>
                >(transactionCategoryJson, _jsonOptions);
                Assert.Equal(200, response.Status);
                Assert.NotNull(transactionCategories);
                Assert.True(transactionCategories.Count > 0);
                var firstCategory = transactionCategories.First();
                var data = new Dictionary<string, object>()
                {
                    { "Id", firstCategory.Id },
                    { "Name", "Updated Category" },
                    { "Description", "This is an updated test category" },
                    { "Status", 1 },
                };
                response = await page.APIRequest.PutAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}/{firstCategory.Id}",
                    new() { DataObject = data, Headers = headers }
                );

                Assert.NotNull(response);
                Assert.Equal(204, response.Status);

                // Verify the update by fetching the category again
                response = await page.APIRequest.GetAsync(
                    $"{BaseApiUrl}{TestConstants.API_TRANSACTION_CATEGORIES_ENDPOINT}/{firstCategory.Id}"
                );
                transactionCategoryJson = await response.TextAsync();
                var updatedTransactionCategory = JsonSerializer.Deserialize<TransactionCategoryDto>(
                    transactionCategoryJson,
                    _jsonOptions
                );
                Assert.Equal(200, response.Status);
                Assert.NotNull(updatedTransactionCategory);
                Assert.Equal(firstCategory.Id, updatedTransactionCategory.Id);
                Assert.Equal("Updated Category", updatedTransactionCategory.Name);
                Assert.Equal(
                    "This is an updated test category",
                    updatedTransactionCategory.Description
                );
                Assert.Equal(EntityEnum.Status.Enabled, updatedTransactionCategory.Status);
            }
        );
    }
}
