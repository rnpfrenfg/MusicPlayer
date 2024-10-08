#include "Program.h"

Program* Program::instance = nullptr;

void Program::InitInstance()
{
	instance = new Program();
}

Program* Program::GetInstance()
{
	return instance;
}

LRESULT CALLBACK winProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	Program* program = Program::GetInstance();
	HWND handle = program->handle;
	Image* buffer = &program->buffer;
	Page* page = program->page;

	int width = program->width;
	int height = program->height;

	switch (msg)
	{
	case WM_CREATE:
		return 0;
	case WM_PAINT:
	{
		PAINTSTRUCT ps;
		BeginPaint(handle, &ps);

		page->Draw(buffer->hdc);

		StretchBlt(ps.hdc, 0, 0, width, height, buffer->hdc, 0, 0, page->width, page->height, SRCCOPY);
		EndPaint(handle, &ps);
		return 0;
	}
	case WM_DESTROY:
		ReleaseDC(hwnd, program->hdc);
		if (program->OnQuit != nullptr)
			program->OnQuit();
		PostQuitMessage(0);
		exit(0);
		return 0;
	case WM_COMMAND:
	{
		int id = LOWORD(wParam) - IdStartAt;

		if (nullptr != (program->onHwnd[id]))
			program->onHwnd[id](HIWORD(wParam));
		SetFocus(hwnd);
		return DefWindowProc(hwnd, msg, wParam, lParam);
	}
	case WM_LBUTTONDOWN:
		page->OnLButtonDown(LOWORD(lParam), HIWORD(lParam));
		return 0;
	case WM_LBUTTONUP:
		page->OnLButtonUp(LOWORD(lParam), HIWORD(lParam));
		return 0;
	case WM_RBUTTONDOWN:
		page->OnRButtonDown(LOWORD(lParam), HIWORD(lParam));
		return 0;
	case WM_RBUTTONUP:
		page->OnLButtonUp(LOWORD(lParam), HIWORD(lParam));
		return 0;
	case WM_KEYDOWN:
		page->OnKeyDown(wParam);
		return 0;
	case WM_KEYUP:
		page->OnKeyUp(wParam);
		return 0;
	case WM_MOUSEMOVE:
		page->OnMouseMove(LOWORD(lParam), HIWORD(lParam));
		return 0;
	}
	return DefWindowProc(hwnd, msg, wParam, lParam);
}

bool Program::Init(HINSTANCE instance, int maxWidth, int maxHeight, LPCWSTR classname, LPCWSTR windowname, bool noSameProgram, int pages)
{
	if (noSameProgram) {
		CreateMutex(0, 0, L"TestProgram");
		if (ERROR_ALREADY_EXISTS == GetLastError()) {
			CreateMutex(0, 0, L"__TestProgram_SameExistMsg");
			if (ERROR_ALREADY_EXISTS == GetLastError())
				return false;
			MessageBox(NULL, L"Running", L"Notice", MB_OK);
			return false;
		}
	}

	this->hInstance = instance;

	WNDCLASSEX winc;
	winc.cbSize = sizeof(winc);
	winc.style = 0;
	winc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	winc.hInstance = instance;
	winc.hCursor = LoadCursor(NULL, IDC_ARROW);
	winc.cbClsExtra = 0;
	winc.cbWndExtra = 0;
	winc.hbrBackground = NULL;
	winc.lpszMenuName = windowname;
	winc.lpszClassName = classname;
	winc.hIconSm = LoadIcon(NULL, IDI_APPLICATION);
	winc.lpfnWndProc = winProc;

	if (!(RegisterClassEx(&winc)))
	{
		return false;
	}

	this->classname = classname;
	this->windowname = windowname;

	handle = CreateWindowEx(NULL, classname, windowname, WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX, 0, 0,
		maxWidth, maxHeight, NULL, NULL, instance, NULL);
	hdc = GetDC(handle);
	buffer.Initialize(hdc, maxWidth, maxHeight);

	width = maxWidth;
	height = maxHeight;
	this->maxWidth = maxWidth;
	this->maxHeight = maxHeight;

	this->pages = new Page * [pages];

	pageHwnds = new HWND * [pages];
	pageHwndLen = new int[pages];
	for (int i = 0; i < pages; i++)
	{
		pageHwnds[i] = new HWND[maxButtons];
		memset(pageHwndLen, 0, pages * sizeof(int));
	}

	return true;
}

void Program::Start(int id, int cmdShow)
{
	this->page = pages[id];
	pageID = id;

	ShowWindow(handle, cmdShow);
	UpdateWindow(handle);

	MovePage(id);

	MessageLoop();
	UnregisterClass(classname, hInstance);
}

void Program::MessageLoop()
{
	MSG msg;

	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (msg.message == WM_QUIT)
		{
			PostQuitMessage(0);
			return;
		}
		TranslateMessage(&msg);
		DispatchMessage(&msg);
		continue;
	}
}

void Program::ReDraw()
{
	InvalidateRect(handle, NULL, true);
}

void Program::LoadButtons(int pageID)
{
	auto list = pageHwnds[pageID];
	for (int i = 0; i < pageHwndLen[pageID]; i++)
		ShowWindow(list[i], SW_SHOWNOACTIVATE);
}