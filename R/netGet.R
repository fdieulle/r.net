#' Gets the property value of an object
#'
#' @param x an external pointer on a .Net object
#' @param propertyName the property name of the object
#' @return Property value which can be an external pointer on .Net object or a native R object if the conversion is supported.
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' type <- "RDotNet.AssemblyTest.DefaultCtorData";
#' testObj <- netNew(type)
#' netGet(testObj, "Name")
#' }
netGet <- function(x, propertyName) {
  result <- .External("rGet", x, propertyName, PACKAGE = rNetPackageName)
  return (result)
}