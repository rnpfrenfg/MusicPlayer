#include "MusicList.h"

#include <mmsystem.h>

void MusicPlayer::Init() {
}

void MusicPlayer::Play(int loc) {
	auto music = manager.FindMusic(loc);
	if (music->isDirectory) {
		wprintf(L"%s is forder\n", music->name.c_str());
		return;
	}
	wprintf(L"%s\n", music->path);

	PlaySound(music->path, NULL, SND_FILENAME | SND_ASYNC);
}