#include "Image.h"

Image::~Image()
{
	DeleteDC(hdc);
	DeleteObject(bitmap);
}

void Image::Initialize(HDC hdc, int sizeX, int sizeY)
{
	width = sizeX;
	height = sizeY;

	this->hdc = CreateCompatibleDC(hdc);
	bitmap = CreateCompatibleBitmap(hdc, sizeX, sizeY);
	SelectObject(this->hdc, bitmap);
}

void Image::Initialize(HDC hdc, HBITMAP bitmap, int sizeX, int sizeY)
{
	width = sizeX;
	height = sizeY;

	this->hdc = CreateCompatibleDC(hdc);
	this->bitmap = bitmap;
	SelectObject(this->hdc, this->bitmap);
}

void Image::Initialize(HDC hdc, HINSTANCE ins, LPCWSTR path)
{
	BITMAP bitInfo;

	bitmap = LoadBitmap(ins, path);
	GetObject(bitmap, sizeof(BITMAP), &bitInfo);
	width = bitInfo.bmWidth;
	height = bitInfo.bmHeight;

	Initialize(hdc, bitmap, width, height);
}