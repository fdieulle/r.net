#' Gets .Net type from its name or qualified name.
#'
#' The name can be 'Namespace.ClassName'
#' The qualified name can be 'Namespace.ClassName, Assembly.name'
#'
#' @param typeName Defines the type name or qualified name to get the .Net type.
#' @return Returns the .Net type.
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' 
#' type <- netGetType("System.IO.FileOptions")
#' netCall(type, "ToString")
#' }
netGetType <- function(typeName) {
  return (netCallStatic("RDotNet.ClrProxy.ReflectionProxy", "GetType", typeName))
}