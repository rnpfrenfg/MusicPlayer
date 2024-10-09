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

		program->MakeButton(PageID::MAIN, L"Test", 200, 0, 100, 100, [](WORD) {
			auto* program = Program::GetInstance();
			program->MovePage(PageID::TEST);
		});

		listBox = program->MakeListBox(PageID::MAIN, 1, 100, width - 20, height - 200, [](WORD hi) {
			switch (hi) {
			case LBN_SELCHANGE:
			{
				auto listBox = ((MainPage*)Program::GetInstance()->pages[(int)PageID::MAIN])->listBox;
				int i = SendMessage(listBox, LB_GETCURSEL, 0, 0);
				Program::GetInstance()->musicPlayer.Play(i);
				break;
			}
			}
		});
		program->MakeButton(PageID::MAIN, L"Resume", 100, height - 100, 50, 50, [](WORD) {
			auto* program = Program::GetInstance();
			program->musicPlayer.Resume();
		});
		program->MakeButton(PageID::MAIN, L"Pause", 200, height - 100, 50, 50, [](WORD) {
			auto* program = Program::GetInstance();
			program->musicPlayer.Pause();
		});

		auto& musicManager = program->musicPlayer.manager;
		musicManager.GetDirectoryByPath(L"Music");

		UpdateList();
	}
	void OnKeyDown(WPARAM vkKey) override{
		if (vkKey == VK_DELETE) {
			printf("DD");
		}
	}

	void OnPageLoad() override {
		UpdateList();
	}

	void UpdateList() {
		SendMessage(listBox, LB_RESETCONTENT, 0, 0);

		Program* program = Program::GetInstance();
		program->musicPlayer.manager.ForEach([](MusicFileManager& manager, MusicData& data, int depth, void* val) {
			HWND listBox = (HWND)val;

			std::wstring temp = L"";
			for (int i = 0; i < depth; i++)
				temp += L"  ";
			if (data.isDirectory)
				temp += L"FOLDER:  ";
			temp += data.name;

			SendMessage(listBox, LB_ADDSTRING, 0, (LPARAM)(temp.c_str()));
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