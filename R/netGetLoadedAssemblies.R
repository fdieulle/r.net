#' Gets all loaded assemblies in the Common Language Runtime. 
#'
#' Gets all loaded assemblies in the current AppDomain. 
#'
#' @export
#' @examples
#' library(r.net)
#' netGetLoadedAssemblies()
netGetLoadedAssemblies <- function() {
	funcGetName <- function(assembly) { return (netGet(netCall(assembly, "GetName"), "Name")) }
	sapply(netCall(netGetStatic("System.AppDomain", "CurrentDomain"), "GetAssemblies"), funcGetName)
}