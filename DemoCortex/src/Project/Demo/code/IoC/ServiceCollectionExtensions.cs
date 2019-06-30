using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Project.Demo.IoC
{
    public static class ServiceCollectionExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void AddMvcControllersInCurrentAssembly(this IServiceCollection serviceCollection)
        {
            AddMvcControllers(serviceCollection, Assembly.GetCallingAssembly());
        }

        public static void AddMvcControllers(this IServiceCollection serviceCollection, params string[] assemblyFilters)
        {
            var assemblyNames = new HashSet<string>(assemblyFilters.Where(filter => !filter.Contains('*')));
            var wildcardNames = assemblyFilters.Where(filter => filter.Contains('*')).ToArray();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly =>
            {
                var nameToMatch = assembly.GetName().Name;
                if (assemblyNames.Contains(nameToMatch)) return true;

                return wildcardNames.Any(wildcard => IsWildcardMatch(nameToMatch, wildcard));
            })
                .ToArray();

            AddMvcControllers(serviceCollection, assemblies);
        }

        public static void AddMvcControllers(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            var controllers = GetTypesImplementing<IController>(assemblies)
                .Where(controller => controller.Name.EndsWith("Controller", StringComparison.Ordinal));

            foreach (var controller in controllers)
            {
                serviceCollection.AddTransient(controller);
            }

            var apiControllers = GetTypesImplementing<ApiController>(assemblies)
                .Where(controller => controller.Name.EndsWith("Controller", StringComparison.Ordinal));
            foreach (var apiController in apiControllers)
            {
                serviceCollection.AddTransient(apiController);
            }
        }

        public static Type[] GetTypesImplementing<T>(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                return new Type[0];
            }

            var targetType = typeof(T);

            return assemblies
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(GetExportedTypes)
                .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition && targetType.IsAssignableFrom(type))
                .ToArray();
        }

        private static IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (NotSupportedException)
            {
                // A type load exception would typically happen on an Anonymously Hosted DynamicMethods
                // Assembly and it would be safe to skip this exception.
                return Type.EmptyTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Return the types that could be loaded. Types can contain null values.
                return ex.Types.Where(type => type != null);
            }
            catch (Exception ex)
            {
                // Throw a more descriptive message containing the name of the assembly.
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Unable to load types from assembly {0}. {1}", assembly.FullName, ex.Message), ex);
            }
        }

        /// <summary>
        /// Checks if a string matches a wildcard argument (using regex)
        /// </summary>
        private static bool IsWildcardMatch(string input, string wildcards)
        {
            return Regex.IsMatch(input, "^" + Regex.Escape(wildcards).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase);
        }


        //  HTTP Extentions



        private const string DefaultControllerFilter = "*Controller";

        private static Assembly[] GetAssemblies(IEnumerable<string> assemblyFilters)
        {
            var assemblies = new List<Assembly>();
            foreach (var assemblyFilter in assemblyFilters)
            {
                assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(assembly => IsWildcardMatch(assembly.GetName().Name, assemblyFilter)).ToArray());
            }
            return assemblies.ToArray();
        }

        private static IEnumerable<Type> GetTypesImplementing(Type implementsType, IEnumerable<Assembly> assemblies, params string[] classFilter)
        {
            var types = GetTypesImplementing(implementsType, assemblies.ToArray());
            if (classFilter != null && classFilter.Any())
            {
                types = types.Where(type => classFilter.Any(filter => IsWildcardMatch(type.FullName, filter)));
            }
            return types;
        }

        private static IEnumerable<Type> GetTypesImplementing(Type implementsType, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                return new Type[0];
            }

            var targetType = implementsType;

            return assemblies
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(GetExportedTypes)
                .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition && targetType.IsAssignableFrom(type))
                .ToArray();
        }

        public static void AddApiControllers(this IServiceCollection serviceCollection, params string[] assemblyFilters)
        {
            serviceCollection.AddApiControllers(GetAssemblies(assemblyFilters));
        }

        public static void AddApiControllers(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            serviceCollection.AddApiControllers(assemblies, new[] { DefaultControllerFilter });
        }
        private static void AddApiControllers(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies, string[] classFilters)
        {
            var controllers = GetTypesImplementing(typeof(IHttpController), assemblies, classFilters);

            foreach (var controller in controllers)
            {
                serviceCollection.AddTransient(controller);
            }
        }
    }
}