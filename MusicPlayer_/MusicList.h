#pragma once

#include "includewindow.h"

#include <string>
#include <vector>
#include <stack>
#include <iostream>

struct MusicData;
using MusicList = MusicData*;

struct MusicData {
	std::wstring name;
	int volume = 50;
	int spd = 50;
	std::wstring path;

	bool isDirectory = false;;
	MusicList dir = nullptr;
	bool isOpened = true;

	MusicData* next = nullptr;

	void Save(FILE* file) {
		if (isDirectory) {
			fwprintf(file, L"1\n%d\n%s\n", isOpened, name.c_str());
		}
		else {
			fwprintf(file, L"0\n%d\n%d\n%s\n%s\n", volume, spd, name.c_str(), path.c_str());
		}
	}

private:
};

class MusicFileManager;

using MusicFunction = void(*)(MusicFileManager&, MusicData&, void*);

class MusicFileManager
{
public:
	void AddDummyMusic(int loc, std::wstring name) {
		MusicData* data = CreateMusicData(name);

		if (musicList == nullptr) {
			musicList = data;
			return;
		}

		if (loc == 0) {
			data->next = musicList;
			musicList = data;
		}
		else {
			MusicData* beforeData = FindMusic(loc - 1);
			MusicData* nextData = beforeData->next;
			beforeData->next = data;
			data->next = nextData;
		}
	}

	void AddDummy(MusicData* data) {
		if (data == nullptr)
			return;

		if (musicList == nullptr) {
			musicList = data;
			return;
		}

		MusicData* lastData = musicList;
		while (lastData->next != nullptr) {
			lastData = lastData->next;
		}
		lastData->next = data;
	}

	MusicData* CreateDummyDirectory(std::wstring name) {
		MusicData* data = new MusicData;
		data->isDirectory = true;
		data->name = name;
		data->dir = nullptr;
		return data;
	}
	MusicData* CreateDummyFile(std::wstring name, std::wstring path) {
		MusicData* data = new MusicData;
		data->isDirectory = true;
		data->name = name;
		data->path = path;
		return data;
	}

	MusicData* GetFileByPath(std::wstring path) {
		MusicData* data = new MusicData;
		data->isDirectory = false;
		data->name = path;
		data->path = path;

		return data;
		//todo
	}

	void GetDirectoryByPath(std::wstring path) {
		WIN32_FIND_DATA fd;
		HANDLE file = FindFirstFile(path.c_str(), &fd);
		if (INVALID_HANDLE_VALUE == file)
			return;

		if (fd.dwFileAttributes != FILE_ATTRIBUTE_DIRECTORY) {
			AddDummy(GetFileByPath(path));
			return;
		}

		MusicData* dir = _GetDirectoryByPath(path);
		AddDummy(dir);
	}

	//TODO stack
	MusicData* _GetDirectoryByPath(std::wstring path) {
		WIN32_FIND_DATA fd;
		auto findStr = path + L"\\*.*";

		HANDLE file = FindFirstFile(findStr.c_str(), &fd);
		if (INVALID_HANDLE_VALUE == file)
			return nullptr;

		MusicData* dir = CreateDummyDirectory(path);
		MusicData* lastMusic = nullptr;

		while (FindNextFile(file, &fd)) {
			if (fd.cFileName[0] == '.')
				continue;
			wprintf(L"%s\n", fd.cFileName);

			MusicData* temp;
			if (fd.dwFileAttributes == FILE_ATTRIBUTE_DIRECTORY) {
				temp = CreateDummyDirectory(path);
				temp->dir = _GetDirectoryByPath(path + L'\\' + fd.cFileName);
			}
			else {
				temp = GetFileByPath(fd.cFileName);
			}

			if (lastMusic == nullptr) {
				lastMusic = temp;
				dir->dir = lastMusic;
			}
			else {
				lastMusic->next = temp;
				lastMusic = temp;
			}
		}
		return dir;
	}

	void Save(std::wstring path) {
		if (nullptr == musicList)
			return;

		file = _wfopen(path.c_str(), L"w");
		_Save(musicList);
	}

	void Read(std::wstring path) {
		file = _wfopen(path.c_str(), L"r");
		if (file == NULL)
			return;

		musicList = _ReadDirectory();
	}

	MusicData* FindMusic(int target) {
		if (nullptr == musicList)
			return nullptr;

		std::stack<MusicData*> stack;
		stack.push(musicList);
		int nowLoc = 0;
		MusicData* now;
		while (!stack.empty()) {
			now = stack.top();
			stack.pop();

			while (true) {
				if (now == nullptr) break;

				if (nowLoc == target) {
					return now;
				}

				if (now->isDirectory) {
					stack.push(now);
					stack.push(now->dir);
					break;
				}

				now = now->next;
			}
		}

		return nullptr;
	}

	void ForEach(MusicFunction func, void* val) {
		if (nullptr == musicList)
			return;

		std::stack<MusicData*> stack;
		stack.push(musicList);
		MusicData* now;
		while (!stack.empty()) {
			now = stack.top();
			stack.pop();

			while (true) {
				if (now == nullptr) break;
				
				func(*this, *now, val);

				if (now->isDirectory) {
					stack.push(now->next);
					stack.push(now->dir);
					break;
				}

				now = now->next;
			}
		}
	}
private:
	MusicData* CreateMusicData(std::wstring path) {
		MusicData* data = new MusicData;
		data->next = nullptr;
		data->path = path;
		data->isDirectory = false;

		int finalTok = 0;
		for (int i = 0; i < path.length(); i++) {
			if (path[i] == '\n') {
				finalTok = i;
			}
		}
		data->name = path.substr(finalTok, path.length());
		if (data->name.length() == 0) {
			data->name = L"NoNamed";
		}
	}
	void _Save(MusicList dir) {
		std::stack<MusicData*> stack;
		stack.push(musicList);
		MusicData* now;
		while (!stack.empty()) {
			now = stack.top();
			stack.pop();

			while (true) {
				if (now == nullptr) break;

				now->Save(file);

				if (now->isDirectory) {
					stack.push(now);
					stack.push(now->dir);
					break;
				}

				now = now->next;
			}
			fwprintf(file, L"2\n");
		}
	}

	MusicData* _ReadDirectory() {
		MusicData* now = nullptr;
		MusicData* first = nullptr;
		int type;
		while (true) {
			fwscanf(file, L"%d", &type);
			if (type == 2) {
				return first;
			}

			MusicData* data = new MusicData;
			if (type == 1) {
				fwscanf(file, L"%d", &(data->isOpened));
				fwscanf(file, L"%s", nameBuffer);
				data->name = nameBuffer;
				data->dir = _ReadDirectory();
			}
			else if (type == 0) {
				MusicData* data = new MusicData;
				fwscanf(file, L"%d", &(data->volume));
				fwscanf(file, L"%d", &(data->spd));
				fwscanf(file, L"%s", nameBuffer);
				fwscanf(file, L"%s", pathBuffer);
				data->name = nameBuffer;
				data->path = pathBuffer;
			}
			else {
				printf("File error\n");
				return first;
			}

			if (now == nullptr) {
				now = first = data;
			}
			else {
				now->next = data;
				now = now->next;
			}
		}
	}

private:
	FILE* file;
	MusicList musicList = nullptr;

	wchar_t nameBuffer[100000];
	wchar_t pathBuffer[100000];
};

enum class MusicPlayMode {
	ONCE, REPEATE
};

class MusicPlayer {
public:
	void Play(int i) {
		MusicData* data = manager.FindMusic(i);

		if (data->isDirectory) {
			printf("DIR. %s", data->name.c_str());
		}
		printf("loc:%d\npath:%s\nname:%s\n", i, data->path.c_str(), data->name.c_str());

		playNow = data;
	}

	MusicFileManager manager;
private:
	MusicData* playNow;
};