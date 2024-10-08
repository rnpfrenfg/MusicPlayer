#pragma once

#include "includewindow.h"

enum class PageID : int
{
	MAIN = 0,
	TEST
};

class Page
{
public:
	virtual ~Page() = default;

	int width = 0;
	int height = 0;

	virtual void Draw(HDC hdc);

	virtual void OnPageLoad();
	virtual void OnPageChange();

	virtual void OnMouseMove(int x, int y);
	virtual void OnLButtonDown(int x, int y);
	virtual void OnLButtonUp(int x, int y);
	virtual void OnRButtonDown(int x, int y);
	virtual void OnRButtonUp(int x, int y);

	virtual void OnKeyDown(WPARAM vkKey);
	virtual void OnKeyUp(WPARAM vkKey);
};