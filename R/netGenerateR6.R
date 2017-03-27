#' @title 
#' R6 classes generator
#'
#' @description
#' Generate R6 classes from .Net type
#'
#' @param typeNames a list of .Net type names
#' @param filePath File path where generated types will be stored
#' @param appendFile If the specified file already exists we append it.
#' @param withInhritedTypes Defines if you want to generate all inherited types loaded in the clr.
#' 
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#' f <- file.path(pckPath, "src/RDotNet.AssemblyTest/bin/Debug", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' netGenerateR6("RDotNet.AssemblyTest.Model.IData", "AutoGenerate-R6.R", withInheritedTypes = TRUE)
#' source("AutoGenerate-R6.R")
#' s1 <- sample1$new()
#' s5 <- sample5$new("MyName", 1.23)
#' }
netGenerateR6 <- function(typeNames, filePath, appendFile = FALSE, withInheritedTypes = FALSE) {
  invisible (netCallStatic("RDotNet.ClrProxy.R6.R6Generator", "GenerateR6Classes", typeNames, filePath, appendFile, withInheritedTypes))
}