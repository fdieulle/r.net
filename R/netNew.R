#' Create a .Net object from a type name. 
#' 
#' Call the constructor which match the arguments list will be called to create a .Net object instance.
#'
#' @param typename The .Net full name type
#' @return Can be an external pointer on .Net object or a native R object if the conversion is supported.
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' x <- netNew("RDotNet.AssemblyTest.OneCtorData", 21L)
#' netCall(x, "ToString")
#' }
netNew <- function(typename, ...) {
  result <- .External("rCreateObject", typename, ..., PACKAGE = rNetPackageName)
  return (result)
}