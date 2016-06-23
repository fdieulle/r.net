#' Set log level verbosity. By default the value is Info.
#'
#' Log level supported: 
#'	Debug
#'	Info
#'	Warn
#'	Error
#'
#' @param level Log level
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#'
#' netSetLogLevel("Debug")
#' netSetLogLevel("Info")
#' netSetLogLevel("Warn")
#' netSetLogLevel("Error")
#' }
netSetLogLevel <- function(level) {
	logger <- netGetStatic("RDotNet.ClrProxy.Loggers.Logger", "Instance")
	netSet(logger, "Level", level)
}