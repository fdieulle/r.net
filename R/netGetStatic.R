#' @title 
#' Gets a static property value
#' 
#' @description
#' Gets a static property value from a .Net type name
#'
#' @param typeName Full .Net type name 
#' @param propertyName Property name to get value
#' @return Returns a converted .Net instance if a converter is defined, an external pointer otherwise.
#' 
#' @details
#' Allows you to get a static property value from a .Net type name.
#' The result will be converted if the type mapping is defined. All native C# types are mapped to R types
#' but you can define custom converters in C# for that see `RDotNetDataConverter` class.
#' 
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#'
#' pckPath <- path.package("r.net")
#' f <- file.path(pckPath, "tests", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' 
#' logger <- netGetStatic("RDotNet.ClrProxy.Loggers.Logger", "Instance")
#' }
netGetStatic <- function(typeName, propertyName) {
  result <- .External("rGetStatic", typeName, propertyName, PACKAGE = rNetPackageName)
  return (result)
}