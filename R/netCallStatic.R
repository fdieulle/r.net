#' Call a static method from a .Net type
#'
#' @param typename The .Net full name type 
#' @param methodName the static method name on the type
#' @param ... additional method arguments passed to .External
#' @return an object returned by the call. Can be an external pointer on .Net object or a native R object if the conversion is supported.
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' netCallStatic("RDotNet.AssemblyTest.StaticClass", "CallWithInteger", 2L)
#' }
netCallStatic <- function(typename, methodName, ...) {
  result <- .External("rCallStaticMethod", typename, methodName, ..., PACKAGE = rNetPackageName)
  return (result)
}