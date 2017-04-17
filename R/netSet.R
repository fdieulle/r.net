#' @title 
#' Sets a property value
#' 
#' @description
#' Sets property value from a .Net object pointer
#'
#' @param x External pointer on a .Net object
#' @param propertyName Property name to set value
#' @param value value to set.
#' 
#' @details
#' Allows you to set a property value from an external pointer on .Net object.
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
#' obj <- netNew("RDotNet.AssemblyTest.DefaultCtorData")
#' netSet(obj, "Name", "Test")
#' netGet(obj, "Name")
#' }
netSet <- function(x, propertyName, value) {
  invisible(.External("rSet", x, propertyName, value, PACKAGE = rNetPackageName))
}