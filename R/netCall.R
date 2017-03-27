#' @title 
#' Call a method
#'
#' @description
#' Call a method member for a given .Net object pointer.
#'
#' @param x an external pointer on a .Net object
#' @param methodName the method name of the object
#' @param ... additional method arguments passed to .External
#' @return an object returned by the call. Can be an external pointer on .Net object or a native R object if the conversion is supported.
#' 
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' x <- netNew("RDotNet.AssemblyTest.OneCtorData", 21L)
#' netCall(x, "ToString")
#' }
netCall <- function(x, methodName, ...) {
  result <- .External("rCall", x, methodName, ..., PACKAGE = rNetPackageName)
  return (result)
}