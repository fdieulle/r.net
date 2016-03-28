#include "RuntimeProxy.h"

mscorlib::_AssemblyPtr proxyAssembly = NULL;
mscorlib::_TypePtr clrProxyType = NULL;

HRESULT loadClrProxy(mscorlib::_AppDomain* appDomain, char* appBaseDir, char** errorMsg) {
	
	bstr_t bstrAssemblyName(L"RDotNet.ClrProxy");

	HRESULT hr = appDomain->Load_2(bstrAssemblyName, &proxyAssembly);
	if(FAILED(hr)) {
		*errorMsg = "Failed to load the RDotNet.ClrProxy assembly";
		return hr;
	}

	bstr_t bstrRuntimeProxyClassName(L"RDotNet.ClrProxy.ClrProxy");
	hr = proxyAssembly->GetType_2(bstrRuntimeProxyClassName, &clrProxyType);
	if(FAILED(hr)) {
		*errorMsg = "Failed to get CSharpProxy.RuntimeProxy type";
		return hr;
	}
	
	return hr;
}

void unloadClrProxy() {
	if(proxyAssembly) {
		proxyAssembly.Release();
		proxyAssembly = NULL;
	}

	if(clrProxyType) {
		clrProxyType.Release();
		clrProxyType = NULL;
	}
}

HRESULT callProxy(PCWSTR methodName, SAFEARRAY* args, VARIANT* result, char** errorMsg) {

	bstr_t bstrMethodName(methodName);

	HRESULT hr = clrProxyType->InvokeMember_3(
		bstrMethodName, 
		static_cast<mscorlib::BindingFlags>(mscorlib::BindingFlags_InvokeMethod | mscorlib::BindingFlags_Static | mscorlib::BindingFlags_Public),
		NULL,
		vtMissing,
		args,
		result);
	
	if(FAILED(hr)) {
		*errorMsg = "Failure in callProxy";
	}

	return hr;
}