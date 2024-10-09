#pragma once

#include "includewindow.h"

#include <mmsystem.h>
#include <mmreg.h>
#include <string>
#include <vector>
#include <stack>
#include <iostream>

#include "MMFile.h"

struct MusicData;
using MusicList = MusicData*;

struct MusicData {
	std::wstring name;
	int volume = 50;
	int spd = 50;
	wchar_t path[40000];

	bool isDirectory = false;;
	MusicList dir = nullptr;
	bool isOpened = true;

	MusicData* next = nullptr;

	void Save(FILE* file) {
		if (isDirectory) {
			fwprintf(file, L"1\n%d\n%s\n", isOpened, name.c_str());
		}
		else {
			fwprintf(file, L"0\n%d\n%d\n%s\n%s\n", volume, spd, name.c_str(), path);
		}
	}

private:
};

class MusicFileManager;

using MusicFunction = void(*)(MusicFileManager&, MusicData&, int depth, void*);

class MusicFileManager
{
public:
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
	MusicData* CreateDummyFile(std::wstring name) {
		MusicData* data = new MusicData;
		data->isDirectory = true;
		data->name = name;
		data->path[0] = '\0';
		return data;
	}

	MusicData* GetFileByPath(std::wstring path, WIN32_FIND_DATA* fd) {
		MusicData* data = new MusicData;
		data->isDirectory = false;
		data->name = fd->cFileName;
		int pathLen = GetFullPathNameW(path.c_str(), sizeof(data->path) / sizeof(data->path[0]), data->path, nullptr);
		data->path[pathLen] = L'\\';
		wcscpy(&(data->path[pathLen + 1]), data->name.c_str());
		return data;
	}

	std::wstring GetDirectoryName(std::wstring path) {
		auto name = path;
		int len = name.length();
		auto cstr = name.c_str();
		int last = -1;
		for (int i = 0; i < len; i++) {
			if (cstr[i] == '\\')
				last = i;
		}

		if (last == -1) {
			return path;
		}
		
		return name.substr(last);
	}

	void GetDirectoryByPath(std::wstring path) {
		WIN32_FIND_DATA fd;
		HANDLE file = FindFirstFile(path.c_str(), &fd);
		if (INVALID_HANDLE_VALUE == file)
			return;

		if (fd.dwFileAttributes != FILE_ATTRIBUTE_DIRECTORY) {
			AddDummy(GetFileByPath(path, &fd));
			return;
		}

		auto name = GetDirectoryName(path);
		MusicData* root = CreateDummyDirectory(name);
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

		MusicData* firstMusic = nullptr;
		MusicData* lastMusic = nullptr;

		while (FindNextFile(file, &fd)) {
			if (fd.cFileName[0] == '.')
				continue;
			
			MusicData* temp;
			if (fd.dwFileAttributes == FILE_ATTRIBUTE_DIRECTORY) {
				temp = CreateDummyDirectory(fd.cFileName);
				temp->dir = _GetDirectoryByPath(path + L'\\' + fd.cFileName);
			}
			else {
				temp = GetFileByPath(path, &fd);
			}

			if (lastMusic == nullptr) {
				firstMusic = lastMusic = temp;
			}
			else {
				lastMusic->next = temp;
				lastMusic = temp;
			}
		}
		return firstMusic;
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

				nowLoc++;

				if (now->isDirectory) {
					stack.push(now->next);
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
		int depth = 0;

		SEARCH:
		while (!stack.empty()) {
			now = stack.top();
			stack.pop();

			while (true) {
				if (now == nullptr) break;
				
				func(*this, *now, depth, val);

				if (now->isDirectory) {
					stack.push(now->next);
					stack.push(now->dir);
					depth++;
					goto SEARCH;
				}

				now = now->next;
			}
			depth--;
		}
	}
private:
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
				fwscanf(file, L"%s", data->path);
				data->name = nameBuffer;
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
};

enum class MusicPlayMode {
	ONCE, REPEATE
};

class MusicPlayer {
public:
	void Init(HWND window);

	void Play(int i);
	void Pause();
	void Resume();
	void OnDone();

	MusicFileManager manager;
private:
	MusicData* playNow;
	HWAVEOUT targetDevice;
	WAVEHDR header;

	MMFile musicFile;
	void* musicFileData = nullptr;
};