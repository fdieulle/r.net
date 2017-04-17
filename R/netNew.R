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