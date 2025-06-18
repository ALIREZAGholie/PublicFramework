using System.Text;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Webgostar.Framework.Application.ApplicationExceptions;

namespace Webgostar.Framework.Application.QueryCommandTools
{
    public class CommandValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            IEnumerable<ValidationFailure>? errors = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(r => r != null);

            if (errors is not null && errors.Any())
            {
                StringBuilder stringBuilder = new();

                foreach (var error in errors)
                {
                    stringBuilder.AppendLine(error.ErrorMessage);
                }

                throw new InvalidCommandException(stringBuilder.ToString());
            }

            var response = await next(cancellationToken);

            return response;
        }
    }
}
