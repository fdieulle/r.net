#' @title 
#' R6 classes generator
#'
#' @description
#' Generate R6 classes from .Net types
#'
#' @param typeNames a list of .Net type names
#' @param filePath File path where generated types will be stored
#' @param appendFile If the specified file already exists we append it.
#' @param withInhritedTypes Defines if you want to generate all inherited types loaded in the clr from your specified types.
#' 
#' @details
#' It can be usefull to use this function to generate R6 classes mapped on .Net types before to build your package.
#' Like that you automatize R6 classes generation and reduce your work to keep consistency between R classes and .Net types.
#' R6 generator generate also the Roxygen2 documentation which will be include in your package to navigate easly in your
#' R6 class graph hierarchy and dependencies. The generator supports type dependencies and type hierarchy. 
#' It can also generate R6 classes for interface, but be carefull because of R6 doesn't support yet multi heritage or interfaces implementation.
#' All generated R6 classes inherits from NetObject class which provides helpers to interact with .Net object instances.
#' 
#' @seealso NetObject
#' @export
#' @examples
#' \dontrun{
#' library(r.net)
#'
#' pckPath <- path.package("r.net")
#' f <- file.path(pckPath, "tests", "RDotNet.AssemblyTest.dll")
#' netLoadAssembly(f)
#' 
#' netGenerateR6("RDotNet.AssemblyTest.Model.IData", "AutoGenerate-R6.R", withInheritedTypes = TRUE)
#' source("AutoGenerate-R6.R")
#' s1 <- Sample1$new()
#' s5 <- Sample5$new("MyName", 1.23)
#' s5$Name
#' }
netGenerateR6 <- function(typeNames, filePath, appendFile = FALSE, withInheritedTypes = FALSE) {
  invisible (netCallStatic("RDotNet.ClrProxy.R6.R6Generator", "GenerateR6Classes", typeNames, filePath, appendFile, withInheritedTypes))
}