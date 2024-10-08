#pragma once

#include <Windows.h>

class Image
{
public:
	HDC hdc;

	Image() = default;
	~Image();

	void Initialize(HDC hdc, int sizeX, int sizeY);
	void Initialize(HDC hdc, HBITMAP bitmap, int sizeX, int sizeY);
	//path = MAKEINTRESOURCE(~~)
	void Initialize(HDC hdc, HINSTANCE, LPCWSTR path);

	int width;
	int height;
protected:
	HBITMAP bitmap;
};
