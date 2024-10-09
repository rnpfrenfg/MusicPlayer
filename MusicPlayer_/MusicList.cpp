#include "MusicList.h"


void MusicPlayer::Init(HWND handle) {
	int devices = waveOutGetNumDevs();
	WAVEOUTCAPS cap;
	char devname[128];
	for (UINT i = 0; i < devices; i++) {
		waveOutGetDevCaps(i, &cap, sizeof(WAVEOUTCAPS));
		WideCharToMultiByte(CP_ACP, 0, cap.szPname, -1, devname, 128, NULL, NULL);

		printf("%d번 : %d 채널,지원 포맷=%x,기능=%x,이름=%s\n",
			i, cap.wChannels, cap.dwFormats, cap.dwSupport, devname);

	}

	//WAVEFORMATEXTENSIBLE format;
	WAVEFORMATEX format;
	format.wFormatTag = WAVE_FORMAT_PCM;
	format.nChannels = 2;
	format.nSamplesPerSec = 44100;
	format.wBitsPerSample = 16;
	format.nBlockAlign = format.nChannels * format.wBitsPerSample / 8;
	format.nAvgBytesPerSec = format.nSamplesPerSec * format.nBlockAlign;
	format.cbSize = 0;

	HRESULT err = waveOutOpen(&targetDevice, WAVE_MAPPER, &format, (DWORD_PTR)handle, 0, CALLBACK_WINDOW);
	if (err != MMSYSERR_NOERROR) {
		std::wcout << err;
		return;
	}

	std::wcout << "SUCCCESSSS\n";
}

void MusicPlayer::Play(int loc) {
	auto music = manager.FindMusic(loc);
	if (music->isDirectory) {
		wprintf(L"%s is forder\n", music->name.c_str());
		return;
	}
	wprintf(L"%s\n", music->path);

	if(musicFileData != nullptr)
		waveOutUnprepareHeader(targetDevice, &header, sizeof(header));

	musicFile.Close();
	musicFileData = musicFile.Read(music->path);

	header.lpData = (char*)musicFileData + 44;
	header.dwBufferLength = musicFile.fileLen - 44;

	waveOutPrepareHeader(targetDevice, &header, sizeof(WAVEHDR));
	waveOutWrite(targetDevice, &header, sizeof(WAVEHDR));
}

void MusicPlayer::OnDone() {
	waveOutWrite(targetDevice, &header, sizeof(WAVEHDR));
}
void MusicPlayer::Pause() {
	waveOutPause(targetDevice);
}
void MusicPlayer::Resume() {
	waveOutRestart(targetDevice);
}