#' Sets a static property value on a specified type
#'
#' @param typeName The type name where looking for the static property
#' @param propertyName the static property name.
#' @param value the property value to set
#' @return Property value which can be an external pointer on .Net object or a native R object if the conversion is supported.
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#'
#' dataConverter <- netGetStatic("RDotNet.ClrProxy.ClrProxy", "DataConverter")
#' netSetStatic("RDotNet.ClrProxy.ClrProxy", "DataConverter", dataConverter)
#' 
#' }
netSetStatic <- function(typeName, propertyName, value) {
  invisible(.External("rSetStatic", typeName, propertyName, value, PACKAGE = rNetPackageName))
}