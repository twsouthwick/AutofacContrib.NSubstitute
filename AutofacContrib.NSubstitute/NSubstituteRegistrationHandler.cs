﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Decorators;
using NSubstitute;
using NSubstitute.Core;

namespace AutofacContrib.NSubstitute
{
    /// <summary> Resolves unknown interfaces and Mocks using the <see cref="Substitute" />. </summary>
    internal class NSubstituteRegistrationHandler : IRegistrationSource
    {
        private static readonly IReadOnlyCollection<Type> GenericCollectionTypes = new List<Type>
        {
            typeof(IEnumerable<>),
            typeof(IList<>),
            typeof(IReadOnlyCollection<>),
            typeof(ICollection<>),
            typeof(IReadOnlyList<>)
        };

        private readonly AutoSubstituteOptions _options;

        public NSubstituteRegistrationHandler(AutoSubstituteOptions options)
        {
            _options = options;
        }

        /// <summary>
        ///     Retrieve a registration for an unregistered service, to be used
        ///     by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor"></param>
        ///  <remarks>
        ///     Since Autofac v4.9.0 the DecoratorService also passed though this
        ///     registration source, make sure this is not mocked out by a proxy.
        /// </remarks>
        /// <returns>
        ///     Registrations for the service.
        /// </returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor
            (Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var typedService = service as IServiceWithType;

            if (typedService == null ||
                !typedService.ServiceType.IsInterface ||
                IsGenericListOrCollectionInterface(typedService.ServiceType) ||
                typedService.ServiceType.IsArray ||
                typeof(IStartable).IsAssignableFrom(typedService.ServiceType) ||
                service is DecoratorService)
                return Enumerable.Empty<IComponentRegistration>();

            if (_options.TypesToSkipForMocking.Contains(typedService.ServiceType))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var rb = RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    var instance = Substitute.For(new[] { typedService.ServiceType }, null);
                    var ctx = c.Resolve<IComponentContext>();

                    foreach (var handler in _options.MockHandlers)
                    {
                        handler.OnMockCreated(instance, typedService.ServiceType, ctx, SubstitutionContext.Current);
                    }

                    return instance;
                })
                .As(service)
                .InstancePerLifetimeScope();

            return new[] { rb.CreateRegistration() };
        }

        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }

        private static bool IsGenericListOrCollectionInterface(Type serviceType)
        {
            return serviceType.IsGenericType && GenericCollectionTypes.Contains(serviceType.GetGenericTypeDefinition());
        }
    }
}