using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.API.Endpoints.TransactionCategory.Commands;

[UsedImplicitly]
public class UpdateTransactionCategory : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/transactionCategories/{id}",
                (
                    [FromRoute] Guid id,
                    [FromBody] UpdateTransactionCategoryCommand command,
                    IMediator mediator
                ) => mediator.Send(command with { Id = id })
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(UpdateTransactionCategory))
            .WithTags(nameof(TransactionCategory))
            .WithDescription("Update a transaction category")
            .WithSummary("Update a transaction category")
            .Produces<TransactionCategoryDto>()
            .ProducesProblem(500)
            .RequireAuthorization();
    }

    [UsedImplicitly]
    public record UpdateTransactionCategoryCommand(
        Guid Id,
        string Name,
        string Description,
        EntityEnum.Status Status = EntityEnum.Status.Enabled
    ) : IRequest<IResult>;

    [UsedImplicitly]
    public class UpdateTransactionCategoryCommandValidator
        : AbstractValidator<UpdateTransactionCategoryCommand>
    {
        public UpdateTransactionCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("The transaction category ID must not be empty.");
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("The transaction category name must not be empty.")
                .MaximumLength(30)
                .WithMessage("The transaction category name must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("The transaction description must not be empty.")
                .MaximumLength(200)
                .WithMessage(
                    "The transaction category description must not exceed 500 characters."
                );
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("The transaction category status must be a valid enum value.");
        }
    }

    [UsedImplicitly]
    public class UpdateTransactionCategoryCommandHandler(
        ITransactionCategoryService transactionCategoryService
    ) : IRequestHandler<UpdateTransactionCategoryCommand, IResult>
    {
        public async Task<IResult> Handle(
            UpdateTransactionCategoryCommand command,
            CancellationToken cancellationToken
        )
        {
            var result = await transactionCategoryService.UpdateAsync(
                command.Id,
                new UpsertTransactionCategoryDto(command.Name, command.Description, command.Status),
                cancellationToken
            );

            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(
                    result.Errors.FirstOrDefault()?.Message
                        ?? "An error occurred while creating the transaction category.",
                    statusCode: 500
                );
        }
    }
}
