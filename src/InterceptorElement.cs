﻿using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Xml;
using Unity;
using Unity.Injection;
using Unity.Interception.Configuration.Properties;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    /// <summary>
    /// Configuration element that lets you configure
    /// what interceptor to use for a type.
    /// </summary>
    public class InterceptorElement : InjectionMemberElement
    {
        private const string TypeNamePropertyName = "type";
        private const string NamePropertyName = "name";
        private const string IsDefaultForTypePropertyName = "isDefaultForType";

        private static int elementCount;
        private readonly int elementNum;

        /// <summary>
        /// Initialize a new <see cref="InterceptorElement"/>.
        /// </summary>
        public InterceptorElement()
        {
            this.elementNum = Interlocked.Increment(ref elementCount);
        }

        /// <summary>
        /// Type name for the interceptor to apply.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName)]
        public string TypeName
        {
            get { return (string)base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Name to use when resolving interceptors from the container.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = false)]
        public string Name
        {
            get { return (string)base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Should this interceptor be registered as the default for the contained
        /// type, or only for this particular type/name pair?
        /// </summary>
        [ConfigurationProperty(IsDefaultForTypePropertyName, IsRequired = false, DefaultValue = false)]
        public bool IsDefaultForType
        {
            get { return (bool)base[IsDefaultForTypePropertyName]; }
            set { base[IsDefaultForTypePropertyName] = value; }
        }

        /// <summary>
        /// Each element must have a unique key, which is generated by the subclasses.
        /// </summary>
        public override string Key
        {
            get { return string.Format(CultureInfo.CurrentCulture, "interceptor:{0}", this.elementNum); }
        }

        /// <summary>
        /// Write the contents of this element to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <remarks>The caller of this method has already written the start element tag before
        /// calling this method, so deriving classes only need to write the element content, not
        /// the start or end tags.</remarks>
        /// <param name="writer">Writer to send XML content to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public override void SerializeContent(XmlWriter writer)
        {
            (writer ?? throw new ArgumentNullException(nameof(writer))).WriteAttributeString(TypeNamePropertyName, this.TypeName);
            writer.WriteAttributeIfNotEmpty(NamePropertyName, this.Name);
            if (this.IsDefaultForType)
            {
                writer.WriteAttributeString(IsDefaultForTypePropertyName, this.IsDefaultForType.ToString());
            }
        }

        /// <summary>
        /// Return the set of <see cref="InjectionMember"/>s that are needed
        /// to configure the container according to this configuration element.
        /// </summary>
        /// <param name="container">Container that is being configured.</param>
        /// <param name="fromType">Type that is being registered.</param>
        /// <param name="toType">Type that <paramref name="fromType"/> is being mapped to.</param>
        /// <param name="name">Name this registration is under.</param>
        /// <returns>One or more <see cref="InjectionMember"/> objects that should be
        /// applied to the container registration.</returns>
        public override IEnumerable<InjectionMember> GetInjectionMembers(IUnityContainer container, Type fromType, Type toType, string name)
        {
            Type interceptorType = this.GuardTypeIsInterceptor(TypeResolver.ResolveType(this.TypeName));

            if (this.IsDefaultForType)
            {
                return new[] { new DefaultInterceptor(interceptorType, this.Name) };
            }
            else
            {
                return new[] { new Interceptor(interceptorType, this.Name) };
            }
        }

        private Type GuardTypeIsInterceptor(Type resolvedType)
        {
            if (!typeof(IInterceptor).IsAssignableFrom(resolvedType))
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.ExceptionResolvedTypeNotCompatible,
                        this.TypeName, resolvedType.FullName, typeof(IInterceptor).FullName));
            }

            return resolvedType;
        }
    }
}
