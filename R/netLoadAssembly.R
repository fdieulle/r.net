#' @title
#' Loads an assembly.
#'
#' @description
#' Loads an assembly in the Clr (Common Language Runtime). 
#'
#' @param filePath Assembly file. It can be the full file path of the assembly, or a qualified assembly name.
#'
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#'
#' pckPath <- path.package("r.net")
#' f <- file.path(pckPath, "tests", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' }
netLoadAssembly <- function(filePath) {
  .C("rLoadAssembly", filePath, PACKAGE = rNetPackageName)
}