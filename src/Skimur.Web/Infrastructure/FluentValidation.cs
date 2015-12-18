using FluentValidation;
using FluentValidation.Attributes;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class FluentValidationModelValidatorProvider : IModelValidatorProvider
    {
        public IValidatorFactory ValidatorFactory { get; private set; }

        public FluentValidationModelValidatorProvider()
        {
            ValidatorFactory = new CustomValidatorFactory();
        }
        
        public void GetValidators(ModelValidatorProviderContext context)
        {
            var validator = CreateValidator(context);

            if (validator == null) return;

            if (!IsValidatingProperty(context))
            {
                context.Validators.Add(new FluentValidationModelValidator(validator));
            }
        }

        protected virtual IValidator CreateValidator(ModelValidatorProviderContext context)
        {
            if (IsValidatingProperty(context))
            {
                return ValidatorFactory.GetValidator(context.ModelMetadata.ContainerType);
            }
            return ValidatorFactory.GetValidator(context.ModelMetadata.ModelType);
        }

        protected virtual bool IsValidatingProperty(ModelValidatorProviderContext context)
        {
            return context.ModelMetadata.ContainerType != null && !string.IsNullOrEmpty(context.ModelMetadata.PropertyName);
        }

        class CustomValidatorFactory : IValidatorFactory
        {
            public IValidator GetValidator(Type type)
            {
                if (type == null)
                {
                    return null;
                }

                var validatorAttribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));

                if (validatorAttribute == null) return null;

                return (IValidator)ActivatorUtilities.GetServiceOrCreateInstance(SkimurContext.ServiceProvider, validatorAttribute.ValidatorType);
            }

            public IValidator<T> GetValidator<T>()
            {
                return (IValidator<T>)GetValidator(typeof(T));
            }
        }
    }

    public class FluentValidationModelValidator : IModelValidator
    {
        private IValidator _validator;

        public FluentValidationModelValidator(IValidator validator)
        {
            _validator = validator;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            var model = context.Container ?? context.Model;

            var result = _validator.Validate(model);

            return from error in result.Errors
                   select new ModelValidationResult(error.PropertyName, error.ErrorMessage);
        }

        public bool IsRequired
        {
            get { return false; }
        }
    }
}
