#' Shuts down the current runtime.
#'
#' Shuts down the current runtime.
#'
#' @return nothing is returned by this function
#' @export
netShutdown <- function() { 
  result <- .C("rShutdownClr", PACKAGE = rNetPackageName)
}