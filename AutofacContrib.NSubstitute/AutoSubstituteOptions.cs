﻿using Autofac;
using Autofac.Builder;
using System;
using System.Collections.Generic;

namespace AutofacContrib.NSubstitute
{
    public class AutoSubstituteOptions
    {
        private readonly static Func<ContainerBuilder, IContainer> _defaultContainerBuilder = b => b.Build();

        internal bool AutoInjectProperties { get; set; }

        /// <summary>
        /// Gets a collection of handlers that can be used to modify mocks after they are created.
        /// </summary>
        public ICollection<MockHandler> MockHandlers { get; } = new List<MockHandler>();

        /// <summary>
        /// Gets a collection of delegates that can augment the registrations of objects created but not registered.
        /// </summary>
        public ICollection<Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>>> ConfigureAnyConcreteTypeRegistration { get; } = new List<Action<IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>>>();

        /// <summary>
        /// Gets a collection of types that will be skipped during generation of NSubstitute mocks.
        /// </summary>
        public ICollection<Type> TypesToSkipForMocking { get; } = new HashSet<Type>();

        /// <summary>
        /// Gets or sets a flag indicating whether mocks should be excluded for provided values. This will automatically add values given to Provide methods to <see cref="TypesToSkipForMocking"/>.
        /// </summary>
        public bool AutomaticallySkipMocksForProvidedValues { get; set; }

        /// <summary>
        /// Gets or sets a factory to create an <see cref="IContainer"/> given a <see cref="ContainerBuilder"/>. This defaults to simply calling <see cref="ContainerBuilder.Build()"/>.
        /// </summary>
        public Func<ContainerBuilder, IContainer> BuildContainerFactory { get; set; } = _defaultContainerBuilder;
    }
}