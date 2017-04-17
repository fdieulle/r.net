#' @title 
#' Gets all loaded assemblies
#' 
#' @description
#' Gets all loaded assemblies name
#'
#' @export
#' @examples
#' \dontrun{
#' 	library(r.net)
#' 	netGetLoadedAssemblies()
#' }
netGetLoadedAssemblies <- function() {
	funcGetName <- function(assembly) { return (netGet(netCall(assembly, "GetName"), "Name")) }
	sapply(netCall(netGetStatic("System.AppDomain", "CurrentDomain"), "GetAssemblies"), funcGetName)
}