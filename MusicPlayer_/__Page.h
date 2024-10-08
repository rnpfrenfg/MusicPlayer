#pragma once

#include "includewindow.h"
#include "Program.h"
#include "Page.h"

WCHAR testItem[][15] = { L"Apple", L"Orange", L"Melon", L"Graph", L"Strawberry" };

class MainPage : public Page
{
public:
	MainPage() {
		Program* program = Program::GetInstance();

		width = program->GetWidth();
		height = program->GetHeight();

		program->MakeButton(PageID::MAIN, L"Test", 200, 0, 100, 100, [](WORD){
			auto* program = Program::GetInstance();
			program->MovePage(PageID::TEST);
		});

		listBox = program->MakeListBox(PageID::MAIN, 1, 100, width - 20, height - 200, [](WORD hi) {
			switch (hi) {
			case LBN_SELCHANGE:
			{
				MainPage* mainPage = (MainPage*) Program::GetInstance()->pages[(int)PageID::MAIN];
				HWND listBox = mainPage->listBox;
				wchar_t str[128] = {0,};
				int i = SendMessage(listBox, LB_GETCURSEL, 0, 0);
				SendMessage(listBox, LB_GETTEXT, i, (LPARAM)str);
				SetWindowText(Program::GetInstance()->handle, str);   
				break;
			}
			}
		});

		auto& musicManager = program->musicManager;
		musicManager.GetDirectoryByPath(L"Music");

		UpdateList();
	}
	void OnKeyDown(WPARAM vkKey) override{
		if (vkKey == VK_DELETE) {
			printf("DD");
		}
	}

	void UpdateList() {
		SendMessage(listBox, LB_RESETCONTENT, 0, 0);

		Program* program = Program::GetInstance();
		program->musicManager.ForEach([](MusicFileManager& manager, MusicData& data, void* val) {
			HWND listBox = (HWND)val;
			SendMessage(listBox, LB_ADDSTRING, 0, (LPARAM)(data.name.c_str()));
		}, listBox);
	}
private:
	HWND listBox;
};

class TestPage : public Page
{
public:
	TestPage() {
		Program* program = Program::GetInstance();

		width = program->GetWidth();
		height = program->GetHeight();

		program->MakeButton(PageID::TEST, L"BackToMenu", 100, 0, 100, 100, [](WORD) {
			auto* program = Program::GetInstance();
			program->MovePage(PageID::MAIN);
		});
	}
};