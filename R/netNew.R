#' @title 
#' Instanciate a .Net object
#' 
#' @description
#' Instanciate a .Net object from its type name.
#'
#' @param typeName The .Net full name type
#' @param ... .Net Constructor arguments.
#' @return Returns a converted .Net instance if a converter is defined, an external pointer otherwise.
#'
#' @details
#' The `typeName` should respect the full type name convention: `Namespace.TypeName`
#' Ellipses `...` has to keep the .net constructor arguments order, the named arguments are not supported yet.
#' If there is many constructors defined for the given .Net type, a score selection is computed from your arguments orders and types to choose the best one. 
#' We consider as a higher priority single value compare to collection of values.
#' 
#' @md
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#'
#' pckPath <- path.package("r.net")
#' f <- file.path(pckPath, "tests", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' 
#' x <- netNew("RDotNet.AssemblyTest.OneCtorData", 21L)
#' netCall(x, "ToString")
#' }
netNew <- function(typeName, ...) {
  result <- .External("rCreateObject", typeName, ..., PACKAGE = rNetPackageName)
  return (result)
}