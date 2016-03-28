echo "Install package r.net"

R CMD REMOVE "%cd%"
R CMD INSTALL "%cd%"

pause