using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using RDotNet.ClrProxy.Converters.RDotNet;

namespace RDotNet.ClrProxy.R6
{
    public static class R6Generator
    {
        public static void GenerateR6Classes(string[] typeNames, string file, bool appendFile = false, bool withInheritedTypes = false)
        {
            if (typeNames == null || file == null) return;

            foreach (var typeName in typeNames)
            {
                var type = ReflectionProxy.GetType(typeName);
                if (type == null) continue;
                
                var sb = new StringBuilder();

                var visited = new HashSet<Type>();
                if (withInheritedTypes)
                {
                    var stack = new Stack<Type>();
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (var candidate in assembly.GetTypes().Where(p => p.InheritsFrom(type)))
                        {
                            stack.GetBaseTypes(candidate, type);
                            Type baseType = null;
                            while (stack.Count > 0)
                            {
                                var current = stack.Pop();
                                sb.GenerateR6Class(current, baseType, visited);
                                baseType = current;
                            }
                        }
                    }
                }
                else sb.GenerateR6Class(type, null, visited);

                if (appendFile) File.AppendAllText(file, sb.ToString());
                else File.WriteAllText(file, sb.ToString());
            }
        }

        private static void GetBaseTypes(this Stack<Type> types, Type fromType, Type toType)
        {
            var type = fromType;
            while (type != null)
            {
                types.Push(type);
                if (type == toType) return;
                type = type.BaseType;
            }
        }

        private static void GenerateR6Class(this StringBuilder sb, Type type, Type baseType, HashSet<Type> visited)
        {
            if (visited.Contains(type)) return;
            visited.Add(type);

            const BindingFlags scope = BindingFlags.Public | BindingFlags.Instance;

            var properties = type.GetProperties(scope).Where(p => p.IsBrowsable() && !p.IsSpecialName).ToArray();
            var methods = type.GetMethods(scope).Where(p => !p.IsSpecialName && p.IsBrowsable() && p.DeclaringType != typeof(object)).ToArray();
            var memberUniqueNames = new Dictionary<MemberInfo, string>();

            #region Manage type dependencies
            var names = new Dictionary<string, int> { { "get", 0 }, { "set", 0 }, { "call", 0 } }; // Put NetObject methods
            foreach (var property in properties)
            {
                Type link;
                if(property.PropertyType.TryGetTypeToGenerate(out link))
                    sb.GenerateR6Class(link, null, visited);

                memberUniqueNames[property] = names.GetUniqueName(property.Name);
            }
            foreach (var method in methods)
            {
                Type link;
                if (method.ReturnType.TryGetTypeToGenerate(out link))
                    sb.GenerateR6Class(link, null, visited);
                
                foreach (var parameter in method.GetParameters())
                {
                    if (parameter.ParameterType.TryGetTypeToGenerate(out link))
                        sb.GenerateR6Class(link, null, visited);
                }

                memberUniqueNames[method] = names.GetUniqueName(method.Name);
            }
            #endregion

            sb.GenerateDocumentation(type, baseType, methods, properties, memberUniqueNames);
            sb.AppendFormat("{0} <- R6Class(\"{0}\",\r\n", type.Name);
            sb.AppendFormat("\tinherit = {0},\r\n", baseType == null ? "NetObject" : baseType.Name);

            // Generate methods
            if (properties.Length > 0)
            {
                sb.AppendFormat("\tactive = list(");

                for (var i = 0; i < properties.Length; i++)
                {
                    if (i != 0) sb.Append(",");
                    sb.Generate(properties[i], memberUniqueNames[properties[i]]);
                }

                sb.AppendLine("\t),");
            }

            sb.AppendLine("\tpublic = list(");

            // Generate initializer
            sb.Generate(type, baseType);

            // Generate methods
            foreach (var method in methods)
            {
                sb.Append(",");
                sb.Generate(method, memberUniqueNames[method]);
            }

            sb.AppendLine("\t)");

            sb.AppendLine(")");
            sb.AppendLine();
        }

        private static void Generate(this StringBuilder sb, PropertyInfo property, string name)
        {
            sb.AppendLine();
            sb.AppendFormat("\t\t{0} = function(value) ", name);
            sb.AppendLine("{");

            sb.AppendLine("\t\t\tif(missing(value)) {");
            if (property.CanRead)
            {
                sb.AppendFormat("\t\t\t\tresult <- self$get(\"{0}\")\r\n", property.Name);
                sb.WrapToNetObject(property.PropertyType, "\t\t\t\t");
            }
            else sb.AppendLine("\t\t\t\treturn(NA)");
            sb.Append("\t\t\t}");

            if (property.CanWrite)
            {
                sb.AppendLine(" else {");
                sb.AppendFormat("\t\t\t\tinvisible(self$set(\"{0}\", value))\r\n", property.Name); // self$set function already supports unwrap NetObject recursivly
                sb.AppendLine("\t\t\t}");
            }
            else sb.AppendLine();
                
            sb.Append("\t\t}");
        }

        private static void WrapToNetObject(this StringBuilder sb, Type targetType, string tabs)
        {
            Type subType;
            if (targetType.TryGetTypeToGenerate(out subType))
            {
                if (targetType.InheritsFrom(typeof(IEnumerable)))
                {
                    sb.Append(tabs);
                    sb.AppendLine("if(is.list(result)) {");

                    // Wrap all items in a NetObject and keep the list
                    sb.Append(tabs);
                    sb.AppendLine("\tfor(i in 1:length(result)) {");
                    sb.AppendFormat("{0}\t\tresult[[i]] <- ", tabs);
                    sb.NewNetObjectFromPtr(subType, "result[[i]]");
                    sb.AppendLine();
                    sb.Append(tabs);
                    sb.AppendLine("\t}");

                    sb.Append(tabs);
                    sb.AppendLine("}");
                }
                else
                {
                    sb.Append("\t\t\t\tresult <- ");
                    sb.NewNetObjectFromPtr(subType, "result");
                    sb.AppendLine();
                }

                sb.AppendLine("\t\t\t\treturn(result)");
            }
            else sb.AppendFormat("\t\t\t\treturn(result)\r\n");
        }

        private static void Generate(this StringBuilder sb, Type type, Type baseType)
        {
            sb.Append("\t\tinitialize = function(");
            var parameters = type.GetCtorParameters();
            foreach (var parameter in parameters)
                sb.AppendFormat("{0}, ", parameter.Name);
            sb.AppendLine("...) {");

            sb.AppendLine("\t\t\tprivate$getPtr(...)");

            if (!type.IsAbstract && !type.IsInterface)
            {
                sb.AppendFormat("\t\t\tif(is.null(private$ptr)) private$ptr = netNew(\"{0}\"", type.FullName);
                foreach (var parameter in parameters)
                    sb.AppendFormat(", self$unwrap({0})", parameter.Name);
                sb.AppendLine(")");
            }

            sb.Append("\t\t\tsuper$initialize(");
            foreach (var parameter in baseType.GetCtorParameters())
                sb.AppendFormat("{0}, ", parameter.Name);
            sb.AppendLine("...)");

            sb.Append("\t\t}");
        }

        private static void Generate(this StringBuilder sb, MethodInfo method, string name)
        {
            sb.AppendLine();

            // Generate function signature
            sb.AppendFormat("\t\t{0} = function(", name);
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                if(i != 0) sb.Append(", ");
                sb.Append(parameters[i].Name);
            }
            sb.AppendLine(") {");

            // Generate function body
            if (method.ReturnType == typeof (void))
            {
                sb.AppendFormat("\t\t\tinvisible(self$call(\"{0}\"", method.Name);
                foreach (var parameter in parameters)
                    sb.AppendFormat(", {0}", parameter.Name);
                sb.AppendLine("))");
            }
            else
            {
                sb.AppendFormat("\t\t\tresult <- self$call(\"{0}\"", method.Name);
                foreach (var parameter in parameters)
                    sb.AppendFormat(", {0}", parameter.Name);
                sb.AppendLine(")");

                sb.WrapToNetObject(method.ReturnType, "\t\t\t");
            }
            
            sb.Append("\t\t}");
        }

        private static void NewNetObjectFromPtr(this StringBuilder sb, Type type, string ptrVarName)
        {
            sb.AppendFormat("{0}$new(", type.Name);
            var count = type.GetCtorParameters().Length;
            for (var i = 0; i < count; i++)
                sb.Append("NA, ");
            sb.AppendFormat("ptr = {0})", ptrVarName);
        }

        private static ParameterInfo[] GetCtorParameters(this Type type)
        {
            if(type == null) return new ParameterInfo[0];

            var ctors = type.GetConstructors();

            var candidate = (ctors.FirstOrDefault(p => p.GetCustomAttribute<R6CtorAttribute>(false) != null) 
                ?? ctors.FirstOrDefault(p => p.GetParameters().Length == 0)) 
                ?? ctors.FirstOrDefault();

            return candidate != null ? candidate.GetParameters() : new ParameterInfo[0];
        }

        private static bool ShouldGenerateR6(this Type type)
        {
            Type t;
            return type.TryGetTypeToGenerate(out t);
        }

        private static bool TryGetTypeToGenerate(this Type type, out Type t)
        {
            t = type;
            if (type.IsValueType
                || type == typeof(void)
                || type.IsByRef // out and ref keyword aren't supported yet
                || type.IsEnum
                || type == typeof(object)
                || type == typeof(string)
                || !type.IsBrowsable()
                || RDotNetDataConverter.Instance.IsDefined(type))
                return false;

            // Manage array
            if (type.IsArray)
            {
                t = type.GetElementType();
                return t.ShouldGenerateR6();
            }

            // Manage IEnumerable<>
            if (type.GetInterfaces().Any(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof (IEnumerable<>)))
            {
                t = type.GetGenericArguments()[0];
                return t.ShouldGenerateR6();
            }

            return !type.IsGenericType // Not yet supported
                && !type.InheritsFrom(typeof (IEnumerable));
        }

        private static bool IsBrowsable(this MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<BrowsableAttribute>(true);
            return attribute == null || attribute.Browsable;
        }

        private static string GetUniqueName(this Dictionary<string, int> names, string name)
        {
            while (true)
            {
                int count;
                if (!names.TryGetValue(name, out count))
                {
                    names.Add(name, 0);
                    break;
                }

                names[name] = ++count;
                name = name + count;
            }

            return name;
        }

        private static bool InheritsFrom(this Type type, Type baseType)
        {
            if (type == null || baseType == null) return false;

            if (baseType.IsInterface)
                return type.GetInterfaces().Any(p => p == baseType);

            while (type != null)
            {
                if (type == baseType) return true;
                type = type.BaseType;
            }

            return false;
        }

        #region Generate documentation

        public static void GenerateDocumentation(this StringBuilder sb, Type type, Type baseType, MethodInfo[] methods, PropertyInfo[] properties, Dictionary<MemberInfo, string> unique)
        {
            sb.AppendLine("#' @title");
            sb.AppendFormat("#' {0}\r\n", type.GetName());
            sb.AppendLine("#' ");

            sb.AppendLine("#' @description");
            sb.AppendFormat("#' {0}\r\n", type.GetDescription());
            
            if (properties.Length > 0)
            {
                sb.AppendLine("#' ");
                sb.AppendLine("#' @section Properties:");
                sb.AppendLine("#' ");
                foreach (var pair in properties.GroupByCategory())
                {
                    if (pair.Key != null)
                    {
                        sb.Append(@"#' \subsection{");
                        sb.Append(pair.Key);
                        sb.AppendLine("}{");
                    }

                    foreach (var property in pair.Value)
                    {
                        sb.AppendFormat("#' * `{0}` (", unique[property]);
                        sb.AppendResultType(property.PropertyType);
                        sb.AppendFormat("): {0}", property.GetDescription());

                        if (property.DeclaringType != null && property.DeclaringType != type &&
                            property.DeclaringType != typeof (object))
                        {
                            sb.Append(@". Inherited from \code{\link{");
                            sb.Append(property.DeclaringType.Name);
                            sb.Append(@"}}");
                        }
                        sb.AppendLine();
                    }

                    if (pair.Key != null)
                    {
                        sb.AppendLine("#' }");
                        sb.AppendLine("#' ");
                    }
                }
            }

            if (methods.Length > 0)
            {
                sb.AppendLine("#' ");
                sb.AppendLine("#' @section Methods:");
                sb.AppendLine("#' ");
                foreach (var pair in methods.GroupByCategory())
                {
                    if (pair.Key != null)
                    {
                        sb.Append(@"#' \subsection{");
                        sb.Append(pair.Key);
                        sb.AppendLine("}{");
                    }

                    foreach (var method in pair.Value)
                    {
                        sb.AppendFormat("#' * `{0}(", unique[method]);
                        var parameters = method.GetParameters();
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            if (i > 0) sb.Append(", ");
                            sb.Append(parameters[i].Name);
                        }
                        sb.Append("): ");
                        sb.AppendResultType(method.ReturnType);
                        sb.AppendFormat("`: {0}", method.GetDescription());

                        if (method.DeclaringType != null && method.DeclaringType != type)
                        {
                            sb.Append(@". Inherited from ");
                            sb.AppendLink(method.DeclaringType.Name);
                        }
                        sb.AppendLine();
                    }

                    if (pair.Key != null)
                    {
                        sb.AppendLine("#' }");
                        sb.AppendLine("#' ");
                    }
                }
            }

            sb.AppendLine("#' ");
            if(baseType != null)
                sb.AppendFormat("#' @family {0}\r\n", baseType.Name);
            sb.AppendFormat("#' @family {0}\r\n", type.Name);

            sb.AppendLine("#' @md");
            sb.AppendLine("#' @export");
        }

        private static string GetName(this MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<DisplayNameAttribute>();
            return attribute == null || string.IsNullOrEmpty(attribute.DisplayName)
                       ? member.Name
                       : attribute.DisplayName;
        }

        private static string GetDescription(this MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null || string.IsNullOrEmpty(attribute.Description)
                       ? member.Name
                       : attribute.Description;
        }

        private static IEnumerable<KeyValuePair<string, List<T>>> GroupByCategory<T>(this T[] members) where T : MemberInfo
        {
            const string misc = "Misc";
            var result = new Dictionary<string, List<T>>
            {
                { misc, new List<T>() }
            };
            
            foreach (var member in members)
            {
                var attribute = member.GetCustomAttribute<CategoryAttribute>();
                var category = attribute == null || string.IsNullOrEmpty(attribute.Category) ? misc : attribute.Category;
                List<T> list;
                if(!result.TryGetValue(category, out list))
                    result.Add(category, list = new List<T>());
                list.Add(member);
            }

            if (result.Count == 1)
                yield return new KeyValuePair<string, List<T>>(null, result[misc]);
            else
            {
                foreach (var pair in result.Where(p => p.Value.Count > 0))
                    yield return pair;
            }
        }

        private static void AppendLink(this StringBuilder sb, string value)
        {
            sb.Append(@"\code{\link{");
            sb.Append(value);
            sb.Append("}}");
        }

        private static void AppendResultType(this StringBuilder sb, Type type)
        {
            var isArray = type.IsArray;
            var isList = type.IsGenericType && type.InheritsFrom(typeof(IEnumerable));

            if (isList)
                sb.Append("list(");

            Type link;
            if (type.TryGetTypeToGenerate(out link))
            {
                sb.AppendLink(link.Name);
                if (isArray)
                    sb.Append("[]");
            }
            else
            {
                link = type;
                if (isList)
                    link = type.GetGenericArguments()[0];

                sb.AppendFormat("`{0}`", link.Name);
            }

            if (isList) sb.Append(")");
        }

        #endregion
    }
}
