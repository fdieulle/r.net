#include "RuntimeHost.h"

HRESULT loadClrProxy(mscorlib::_AppDomain* appDomain, char* appBaseDir, char** errorMsg);
void unloadClrProxy();
HRESULT callProxy(PCWSTR methodName, SAFEARRAY* args, VARIANT* result, char** errorMsg);