using System;
using System.Reflection;
using System.Text;

namespace RDotNet.ClrProxy
{
    public static class R6Generator
    {
        public static void GenerateR6Class(this StringBuilder sb, Type type)
        {
            sb.AppendFormat("{0} <- R6Class(\"{0}\",", type.Name);
            sb.AppendLine();

            // Generate private list which container the externalPtr of .net object
            sb.AppendLine("\tprivate = list(");
            sb.AppendLine("\t\tclrObj = NULL");
            sb.AppendLine("\t),");

            // Generate properties as active elements
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var length = properties.Length;
            if (length > 0)
            {
                sb.Append("\tactive = list(");
                for (var i = 0; i < length; i++)
                {
                    sb.Generate(properties[i]);
                    if (i < length - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
                sb.AppendLine("\t),");
            }

            sb.AppendLine("\tpublic = list(");

            // Generate initializer
            sb.Generate(type);

            // Generate methods
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            length = methods.Length;

            for (var i = 0; i < length; i++)
            {
                if(methods[i].IsSpecialName) continue;
                sb.Append(",");
                sb.Generate(methods[i]);
            }

            sb.AppendLine("\t)");

            sb.AppendLine(")");
        }

        private static void Generate(this StringBuilder sb, PropertyInfo property)
        {
            sb.AppendLine();
            sb.AppendFormat("\t\t{0} = function(value) ", property.Name);
            sb.Append('{');
            sb.AppendLine();
            if (property.CanRead)
            {
                sb.AppendFormat("\t\t\tif (missing(value)) return(netGet(private$clrObj, \"{0}\"))", property.Name);
                sb.AppendLine();
            }
            if (property.CanWrite)
            {
                sb.Append("\t\t\t");
                if (property.CanRead) sb.Append("else ");
                sb.AppendFormat("invisible(netSet(private$clrObj, \"{0}\", value))", property.Name);
            }
            sb.AppendLine();
            sb.Append("\t\t}");
        }

        private static void Generate(this StringBuilder sb, Type type)
        {
            sb.AppendLine("\t\tinitialize = function(clrObj = NULL, ...) {");
            sb.AppendFormat("\t\t\tif(is.null(clrObj)) private$clrObj = netNew(\"{0}\")", type.FullName);
            sb.AppendLine();
            sb.AppendLine("\t\t\telse private$clrObj = clrObj");

            // Manage ellipsis as property named setter
            sb.AppendLine("\t\t\tparameters = list(...)");
            sb.AppendLine("\t\t\tfor(name in names(...)) {");
            sb.AppendLine("\t\t\t\tnetSet(private$clrObj, name, parameters[name])");
            sb.AppendLine("\t\t\t}");

            sb.Append("\t\t}");
        }

        private static void Generate(this StringBuilder sb, MethodInfo method)
        {
            sb.AppendLine();

            // Generate function signature
            sb.AppendFormat("\t\t{0} = function(", method.Name);
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[i].Name);
                if (i < parameters.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(") {");
            sb.AppendLine();

            // Generate function body
            sb.Append("\t\t\t");
            sb.Append(method.ReturnType == typeof (void) ? "invisible(" : "return(");
            
            sb.AppendFormat("netCall(private$clrObj, \"{0}\"", method.Name);
            foreach (var parameter in parameters)
                sb.AppendFormat(", {0}", parameter.Name);
            sb.Append(')');

            sb.Append(')'); // Ends Return or invisible function
            
            sb.AppendLine();
            sb.Append("\t\t}");
        }
    }
}
