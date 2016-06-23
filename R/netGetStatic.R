#' Gets a static property value from .Net type
#'
#' @param typeName The type name where looking for the static property
#' @param propertyName the static property name.
#' @return Property value which can be an external pointer on .Net object or a native R object if the conversion is supported.
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#'
#' logger <- netGetStatic("RDotNet.ClrProxy.Loggers.Logger", "Instance")
#' }
netGetStatic <- function(typeName, propertyName) {
  result <- .External("rGetStatic", typeName, propertyName, PACKAGE = rNetPackageName)
  return (result)
}