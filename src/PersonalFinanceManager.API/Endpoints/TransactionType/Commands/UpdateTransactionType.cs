using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManager.API.Endpoints.TransactionCategory.Commands;
using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.API.Endpoints.TransactionType.Commands;

[UsedImplicitly]
public class UpdateTransactionType : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/transactionTypes/{id}",
                (
                    [FromRoute] Guid id,
                    [FromBody] UpdateTransactionTypeCommand command,
                    IMediator mediator
                ) => mediator.Send(command with { Id = id })
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(UpdateTransactionType))
            .WithTags(nameof(TransactionType))
            .WithDescription("Update a transaction type")
            .WithSummary("Update a transaction type")
            .Produces<TransactionTypeDto>()
            .ProducesProblem(500)
            .RequireAuthorization();
    }

    [UsedImplicitly]
    public record UpdateTransactionTypeCommand(
        Guid Id,
        string Name,
        string Description,
        Guid TransactionCategoryId,
        EntityEnum.Status Status = EntityEnum.Status.Enabled
    ) : IRequest<IResult>;

    [UsedImplicitly]
    public class UpdateTransactionTypeCommandValidator
        : AbstractValidator<UpdateTransactionTypeCommand>
    {
        public UpdateTransactionTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("The transaction type ID must not be empty.");
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("The transaction type name must not be empty.")
                .MaximumLength(30)
                .WithMessage("The transaction type name must not exceed 30 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("The transaction type description must not be empty.")
                .MaximumLength(200)
                .WithMessage("The transaction type description must not exceed 500 characters.");
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("The transaction type status must be a valid enum value.");
            RuleFor(x => x.TransactionCategoryId)
                .NotEmpty()
                .WithMessage("The transaction category ID must not be empty.");
        }
    }

    [UsedImplicitly]
    public class UpdateTransactionTypeCommandHandler(ITransactionTypeService transactionTypeService)
        : IRequestHandler<UpdateTransactionTypeCommand, IResult>
    {
        public async Task<IResult> Handle(
            UpdateTransactionTypeCommand command,
            CancellationToken cancellationToken
        )
        {
            var result = await transactionTypeService.UpdateAsync(
                command.Id,
                new UpsertTransactionTypeDto(
                    command.Name,
                    command.Description,
                    command.TransactionCategoryId,
                    command.Status
                ),
                cancellationToken
            );

            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(
                    result.Errors.FirstOrDefault()?.Message
                        ?? "An error occurred while creating the transaction type.",
                    statusCode: 500
                );
        }
    }
}
