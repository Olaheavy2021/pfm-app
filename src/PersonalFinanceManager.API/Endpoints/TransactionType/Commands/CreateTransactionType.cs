using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManager.API.Endpoints.TransactionType.Queries;
using PersonalFinanceManager.Application.Data.Models;

namespace PersonalFinanceManager.API.Endpoints.TransactionType.Commands;

[UsedImplicitly]
public class CreateTransactionType : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/transactionTypes",
                ([FromBody] CreateTransactionTypeCommand command, IMediator mediator) =>
                    mediator.Send(command)
            )
            .HasApiVersion(new ApiVersion(1))
            .WithName(nameof(CreateTransactionType))
            .WithTags(nameof(TransactionType))
            .WithDescription("Create a new transaction type")
            .WithSummary("Create a transaction type")
            .Produces<TransactionTypeDto>()
            .ProducesProblem(500)
            .RequireAuthorization();
    }

    [UsedImplicitly]
    public record CreateTransactionTypeCommand(
        string Name,
        string Description,
        Guid TransactionCategoryId,
        EntityEnum.Status Status = EntityEnum.Status.Enabled
    ) : IRequest<IResult>;

    public class CreateTransactionTypeCommandValidator
        : AbstractValidator<CreateTransactionTypeCommand>
    {
        public CreateTransactionTypeCommandValidator()
        {
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

    public class CreateTransactionTypeCommandHandler(ITransactionTypeService transactionTypeService)
        : IRequestHandler<CreateTransactionTypeCommand, IResult>
    {
        public async Task<IResult> Handle(
            CreateTransactionTypeCommand command,
            CancellationToken cancellationToken
        )
        {
            var result = await transactionTypeService.CreateAsync(
                new UpsertTransactionTypeDto(
                    command.Name,
                    command.Description,
                    command.TransactionCategoryId,
                    command.Status
                ),
                cancellationToken
            );

            return result.IsSuccess
                ? Results.CreatedAtRoute(
                    nameof(GetTransactionType),
                    new { id = result.Value.Id },
                    result.Value
                )
                : Results.Problem(
                    result.Errors.FirstOrDefault()?.Message
                        ?? "An error occurred while creating the transaction type.",
                    statusCode: 500
                );
        }
    }
}
