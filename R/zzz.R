rNetPackageName <- 'RDotNet.RHostClr'

#' r.net .onLoad.
#' 
#' Function called when loading the r.net package with 'library'. 
#' 
#' @param pkgsDir the path to the library from which the package is loaded
#' @param pkgName the name of the package.
#' @rdname dotOnLoad
#' @name dotOnLoad
.onLoad <- function(pkgsDir='~/R', pkgName = 'r.net') {
  
  rNetPkgDir <- path.expand(file.path(pkgsDir, pkgName))
  rNetPkgDir <- system.file(package='r.net')
  rNetLibsDirs <- file.path(rNetPkgDir, 'libs')
  
  nativeLibDir <- file.path(rNetLibsDirs, Sys.getenv('R_ARCH'))
  nativeLibName = rNetPackageName
  nativeLibExt = .Platform$dynlib.ext
  nativeLib <- paste0(file.path(nativeLibDir, nativeLibName), nativeLibExt)
  
  dyn.load(nativeLib)
  
  .C("rStartClr", rNetLibsDirs, PACKAGE = rNetPackageName)
}


