#' Loads an assembly in the Common Language Runtime. 
#'
#' Loads an assembly. 
#'
#' @param name a character vector of length one. It can be the full file name of the assembly to load, or a fully qualified assembly name, or as a last resort a partial name.
#' @seealso \code{\link{.C}} which this function wraps
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' }
netLoadAssembly <- function(filePath) {
  .C("rLoadAssembly", filePath, PACKAGE = rNetPackageName)
}