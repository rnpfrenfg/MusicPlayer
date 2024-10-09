#pragma once

#include "includewindow.h"

#include <thread>
#include <time.h>

#include "Image.h"
#include "Page.h"
#include "MusicList.h"

enum { IdStartAt = 20000};

LRESULT CALLBACK winProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

typedef void (*OnHwndClicked)(WORD hiword);

class Program
{
	friend LRESULT CALLBACK winProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
public:
	static void InitInstance();
	static Program* GetInstance();

	bool Init(HINSTANCE instance, int maxWidth, int maxHeight, LPCWSTR classname, LPCWSTR windowname, bool noSameProgram, int pages);
	void (*OnQuit)() = nullptr;

	void Start(int id, int cmdShow);
	void ReDraw();

	int GetWidth() { return width; }
	int GetHeight() { return height; }

	HWND handle;

	Page** pages = nullptr;

	HDC hdc;
	HINSTANCE hInstance;
	
	MusicPlayer musicPlayer;
private:
	const int maxButtons = 100;

	Program() = default;
	static Program* instance;

	void MessageLoop();

	LPCWSTR classname;
	LPCWSTR windowname;

	Image buffer;
	int width;
	int height;
	int maxWidth;
	int maxHeight;

	Page* page;
	int pageID;

	int lastHwndID = 0;
	// pageHwnds[i][j] = j'th button in i'th page
	HWND** pageHwnds;
	//pageHwndLen[i] = buttons in i'th page
	int* pageHwndLen;
	OnHwndClicked onHwnd[5000];

	void LoadButtons(int pageID);

private:
	int AddHwnd(PageID pageID, OnHwndClicked func) {
		onHwnd[lastHwndID] = func;
		int id = IdStartAt + lastHwndID;
		lastHwndID++;
		return id;
	}
public:
	template<typename T>
	void MovePage(T id)
	{
		page->OnPageChange();

		auto list = pageHwnds[pageID];
		for (int i = 0; i < pageHwndLen[pageID]; i++)
			ShowWindow(list[i], SW_HIDE);

		pageID = (int)id;
		page = pages[pageID];
		page->OnPageLoad();

		LoadButtons(pageID);

		ReDraw();
	}

	template <typename T>
	HWND MakeButton(T pageID, int x, int y, int width, int height, OnHwndClicked func, DWORD style = BS_PUSHBUTTON)
	{
		return MakeButton((int)pageID, L"Button", x, y, width, height, func, style);
	}

	template <typename T>
	HWND MakeImageButton(T pageID, int x, int y, OnHwndClicked func, HBITMAP bitmap)
	{
		BITMAP bitInfo;
		GetObject(bitmap, sizeof(BITMAP), &bitInfo);
		int width = bitInfo.bmWidth;
		int height = bitInfo.bmHeight;

		HWND handle = MakeButton((int)pageID, L"button", x, y, width, height, func, BS_BITMAP);
		SendMessage(handle, BM_SETIMAGE, (WPARAM)IMAGE_BITMAP, (LPARAM)bitmap);
		return handle;
	}

	template <typename T>
	HWND MakeButton(T pageID, LPCWSTR text, int x, int y, int width, int height, OnHwndClicked func, DWORD style = 0)
	{
		int id = AddHwnd(pageID, func);

		style |= WS_CHILD;
		style |= BS_PUSHBUTTON;
		style &= ~WS_VISIBLE;
		HWND buttonHandle = CreateWindow(L"button", text, style, x, y, width, height, handle, (HMENU)id, hInstance, NULL);

		int& len = pageHwndLen[(int)pageID];
		pageHwnds[(int)pageID][len] = buttonHandle;
		len++;
		return buttonHandle;
	}

	template <typename T>
	HWND MakeListBox(T pageID, int x, int y, int width, int height, OnHwndClicked func, DWORD style = 0) {
		int id = AddHwnd(pageID, func);

		style |= WS_CHILD;
		style |= LBS_NOTIFY | WS_VSCROLL | WS_BORDER;
		style &= ~WS_VISIBLE;
		HWND buttonHandle = CreateWindow(L"listbox", NULL, style, x, y, width, height, handle, (HMENU)id, hInstance, NULL);

		int& len = pageHwndLen[(int)pageID];
		pageHwnds[(int)pageID][len] = buttonHandle;
		len++;
		return buttonHandle;
	}
};