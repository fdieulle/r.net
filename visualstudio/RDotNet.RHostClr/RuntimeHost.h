#pragma region Includes and Imports

#include <stdio.h>
#include <windows.h>
#include <metahost.h>
#pragma comment(lib, "mscoree.lib")

// Import mscorlib.tlb (Microsoft Common Language Runtime Class Library).
#import <mscorlib.tlb> raw_interfaces_only				\
	high_property_prefixes("_get","_put","_putref")		\
    rename("ReportEvent", "InteropServices_ReportEvent")

#pragma endregion

HRESULT startClr(const char* clrVersion, char** errorMsg);
void stopClr();

HRESULT loadDomain(const char* name, const char* appBaseDir, const char* appConfigFile, mscorlib::_AppDomain** appDomain, char** errorMsg);
HRESULT unloadDomain(mscorlib::_AppDomain* appDomain, char** errorMsg);

void printHResult(HRESULT hr);
wchar_t* convertToWChar(const char* str);
char* bstrToCString(bstr_t* src);