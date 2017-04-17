#' @title 
#' Sets a static property value
#' 
#' @description
#' Sets a static property value from a .Net type name
#'
#' @param typeName Full .Net type name 
#' @param propertyName Property name to set value
#' @param value value to set.
#' 
#' @details
#' Allows you to set a property value from .Net type name.
#' The input value will be converted from R type to a .Net type. 
#' If the property value isn't a native C# type or a mapped conversion type you have to use an external pointer on .Net object.
#' You can define custom converters in C# for that see `RDotNetDataConverter` class.
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
#' dataConverter <- netGetStatic("RDotNet.ClrProxy.ClrProxy", "DataConverter")
#' netSetStatic("RDotNet.ClrProxy.ClrProxy", "DataConverter", dataConverter)
#' 
#' }
netSetStatic <- function(typeName, propertyName, value) {
  invisible(.External("rSetStatic", typeName, propertyName, value, PACKAGE = rNetPackageName))
}