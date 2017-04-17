#' @title 
#' Call a method
#'
#' @description
#' Call a method member for a given .Net object pointer.
#'
#' @param x External pointer on a .Net object
#' @param methodName Method name to call
#' @param ... Method arguments
#' @return Returns a converted .Net instance if a converter is defined, an external pointer otherwise.
#' 
#' @details
#' Call a method member for a given .Net object instance which has to be an external pointer.
#' Ellipses has to keep the .net arguments method order, the named arguments are not yet supported.
#' If there is conflicts with a method name (many definition in .Net), the best matched one will be chose.
#' A score is computed from your arguments orders and types. We consider as higher priority single value compare to collection of values.
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
netCall <- function(x, methodName, ...) {
  result <- .External("rCall", x, methodName, ..., PACKAGE = rNetPackageName)
  return (result)
}