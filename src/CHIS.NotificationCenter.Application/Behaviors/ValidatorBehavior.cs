﻿//using CHIS.NotificationCenter.Domain.Exceptions;
//using FluentValidation;
//using MediatR;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace CHIS.NotificationCenter.Application.Behaviors
//{
//    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    {
//        private readonly IValidator<TRequest>[] _validators;
//        public ValidatorBehavior(IValidator<TRequest>[] validators) => _validators = validators;

//        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//        {
//            var failures = _validators
//                .Select(v => v.Validate(request))
//                .SelectMany(result => result.Errors)
//                .Where(error => error != null)
//                .ToList();

//            if (failures.Any())
//            {
//                throw new NotificationCenterDomainException(
//                    string.IsNullOrEmpty(failures[0].ErrorMessage) ? $"Command Validation Errors for type {typeof(TRequest).Name}" : failures[0].ErrorMessage
//                    , new ValidationException("Validation exception", failures));
//            }

//            var response = await next().ConfigureAwait(false);
//            return response;
//        }

//        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
//        {
//            var response = await next().ConfigureAwait(false);
//            return response;
//        }
//    }
//}
