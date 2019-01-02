using System;
using Unity.Builder;
using Unity.Configuration;
using Unity.Injection;
using Unity.Interception.ContainerIntegration.ObjectBuilder;
using Unity.Interception.Interceptors;
using Unity.Interception.Interceptors.InstanceInterceptors;
using Unity.Interception.Interceptors.TypeInterceptors;
using Unity.Policy;

namespace Unity.Interception.Configuration
{
    /// <summary>
    /// A <see cref="InjectionMember"/> that can be passed to the
    /// <see cref="IUnityContainer.RegisterType"/> method to specify
    /// which interceptor to use. This member sets up the default
    /// interceptor for a type - this will be used regardless of which 
    /// name is used to resolve the type.
    /// </summary>
    internal class DefaultInterceptor : InjectionMember, IDefaultPolicy
    {
        private readonly IInterceptor _interceptor;
        private readonly NamedTypeBuildKey _interceptorKey;

        /// <summary>
        /// Construct a new <see cref="DefaultInterceptor"/> instance that,
        /// when applied to a container, will register the given
        /// interceptor as the default one.
        /// </summary>
        /// <param name="interceptor">Interceptor to use.</param>
        public DefaultInterceptor(IInterceptor interceptor)
        {
            _interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
        }

        /// <summary>
        /// Construct a new <see cref="DefaultInterceptor"/> that, when
        /// applied to a container, will register the given type as
        /// the default interceptor. 
        /// </summary>
        /// <param name="interceptorType">Type of interceptor.</param>
        /// <param name="name">Name to use to resolve the interceptor.</param>
        public DefaultInterceptor(Type interceptorType, string name)
        {
            if (null == interceptorType) throw  new ArgumentNullException(nameof(interceptorType));
            //Guard.TypeIsAssignable(typeof(IInterceptor), interceptorType, "interceptorType");

            _interceptorKey = new NamedTypeBuildKey(interceptorType, name);
        }

        /// <summary>
        /// Construct a new <see cref="DefaultInterceptor"/> that, when
        /// applied to a container, will register the given type as
        /// the default interceptor. 
        /// </summary>
        /// <param name="interceptorType">Type of interceptor.</param>
        public DefaultInterceptor(Type interceptorType)
            : this(interceptorType, null)
        {
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="registeredType">Type of interface being registered. If no interface,
        /// this will be null.</param>
        /// <param name="mappedToType">Type of concrete type being registered.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies<TContext, TPolicySet>(Type registeredType, Type mappedToType, string name, ref TPolicySet policies)
        {
            if (IsInstanceInterceptor)
                AddDefaultInstanceInterceptor(ref policies);
            else
                AddDefaultTypeInterceptor(ref policies);
        }

        private bool IsInstanceInterceptor
        {
            get
            {
                if (_interceptor != null)
                {
                    return _interceptor is IInstanceInterceptor;
                }
                return typeof(IInstanceInterceptor).IsAssignableFrom(_interceptorKey.Type);
            }
        }

        private void AddDefaultInstanceInterceptor<TPolicySet>(ref TPolicySet policies)
            where TPolicySet : IPolicySet
        {
            IInstanceInterceptionPolicy policy;

            if (_interceptor != null)
            {
                policy = new FixedInstanceInterceptionPolicy((IInstanceInterceptor)_interceptor);
            }
            else
            {
                policy = new ResolvedInstanceInterceptionPolicy(_interceptorKey);
            }

            policies.Set(typeof(IInstanceInterceptionPolicy), policy);
        }

        private void AddDefaultTypeInterceptor<TPolicySet>(ref TPolicySet policies)
            where TPolicySet : IPolicySet
        {
            ITypeInterceptionPolicy policy;

            if (_interceptor != null)
            {
                policy = new FixedTypeInterceptionPolicy((ITypeInterceptor)_interceptor);
            }
            else
            {
                policy = new ResolvedTypeInterceptionPolicy(_interceptorKey);
            }

            policies.Set(typeof(ITypeInterceptionPolicy), policy);
        }
    }

    /// <summary>
    /// A generic version of <see cref="DefaultInterceptor"/> so that
    /// you can specify the interceptor type using generics.
    /// </summary>
    /// <typeparam name="TInterceptor"></typeparam>
    internal class DefaultInterceptor<TInterceptor> : DefaultInterceptor
        where TInterceptor : ITypeInterceptor
    {
        /// <summary>
        /// Create a new instance of <see cref="DefaultInterceptor{TInterceptor}"/>.
        /// </summary>
        /// <param name="name">Name to use when resolving interceptor.</param>
        public DefaultInterceptor(string name)
            : base(typeof(TInterceptor), name)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="DefaultInterceptor{TInterceptor}"/>.
        /// </summary>
        public DefaultInterceptor()
            : base(typeof(TInterceptor))
        {
        }
    }
}