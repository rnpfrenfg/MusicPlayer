#pragma once

#define WIN32_LEAN_AND_MEAN
#define _CRT_SECURE_NO_WARNINGS

#pragma comment (lib,"ws2_32.lib")
#pragma comment (lib,"winmm.lib")

#include <WinSock2.h>
#include <Ws2tcpip.h>
#include <DirectXPackedVector.h>
#include <Windows.h>
#include <windowsx.h>

#define CDEBUG TRUE

#if CDEBUG
#include <iostream>
#define DxThrowIfFailed(x) if(FAILED(x)){std::cout<<"[Error::"<<std::hex<<x<<std::dec<<"] At " <<__FILE__<<" :: "<<__LINE__ <<'\n';while(true);}
#else
#define DxThrowIfFailed(x) if(FAILED(x)){exit(0);}
#endif