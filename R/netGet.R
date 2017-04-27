#' @title 
#' Gets a property value
#' 
#' @description
#' Gets property value from an external pointer of .Net object.
#'
#' @param x External pointer on a .Net object
#' @param propertyName Property name to get value
#' @return Returns a converted .Net instance if a converter is defined, an external pointer otherwise.
#' 
#' @details
#' Allows you to get a property value from an external pointer on .Net object. 
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
#' obj <- netNew("RDotNet.AssemblyTest.DefaultCtorData")
#' netSet(obj, "Name", "Test")
#' netGet(obj, "Name")
#' }
netGet <- function(x, propertyName) {
  result <- .External("rGet", x, propertyName, PACKAGE = rNetPackageName)
  return (result)
}