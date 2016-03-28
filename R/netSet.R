#' Sets the property value of an object
#'
#' @param x an external pointer on a .Net object
#' @param propertyName the property name of the object
#' @param value the property value to set
#' @return Property value which can be an external pointer on .Net object or a native R object if the conversion is supported.
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' type <- "RDotNet.AssemblyTest.DefaultCtorData";
#' testObj <- netNew(type)
#' netSet(testObj, "Name", "Test")
#' netGet(testObj, "Name")
#' }
netSet <- function(x, propertyName, value) {
  invisible(.External("rSet", x, propertyName, value, PACKAGE = rNetPackageName))
}