#' Restart the current AppDomain.
#'
#' Restart the current AppDomain.
#'
#' @return nothing is returned by this function
#' @export
netRestartAppDomain <- function() { 
  result <- .C("rRestartAppDomain", PACKAGE = rNetPackageName)
}