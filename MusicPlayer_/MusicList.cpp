#include "MusicList.h"

#include <dshow.h>

bool MusicPlayer::Init(HWND handle) {
	int devices = waveOutGetNumDevs();
	WAVEOUTCAPS cap;
	char devname[128];
	for (UINT i = 0; i < devices; i++) {
		waveOutGetDevCaps(i, &cap, sizeof(WAVEOUTCAPS));
		WideCharToMultiByte(CP_ACP, 0, cap.szPname, -1, devname, 128, NULL, NULL);

		printf("%d번 : %d 채널,지원 포맷=%x,기능=%x,이름=%s\n",
			i, cap.wChannels, cap.dwFormats, cap.dwSupport, devname);

	}

	HRESULT err = CoInitialize(NULL);
	if (FAILED(err))
		return false;

	control = NULL;
	graph = NULL;
	event = NULL;
	audio = NULL;
	seek = NULL;

	playNow = NULL;

	return true;
}

void MusicPlayer::Clear() {
	if (control) {
		control->Stop();
		control->Release();
		control = NULL;
	}
	if (graph) {
		graph->Release();
		graph = NULL;
	}
	if (event) {
		event->Release();
		event = NULL;
	}
	if (audio) {
		audio->Release();
		audio = NULL;
	}
	if (seek){
		seek->Release();
		seek = NULL;
	}
}

void MusicPlayer::Play(int loc) {
	auto music = manager.FindMusic(loc);
	if (music->isDirectory) {
		wprintf(L"%s is forder\n", music->name.c_str());
		return;
	}
	wprintf(L"%s\n", music->path);

	if (playNow == music) {
		control->Run();
		return;
	}

	this->Clear();
	playNow = music;

	HRESULT err = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, IID_IGraphBuilder, (void**)&graph);
	if (FAILED(err))
		return;

	graph->QueryInterface(IID_IMediaControl, (void**)&control);
	graph->QueryInterface(IID_IMediaEvent, (void**)&event);
	graph->QueryInterface(IID_IBasicAudio, (void**)&audio);
	graph->QueryInterface(IID_IMediaSeeking, (void**)&seek);

	err = graph->RenderFile(music->path, NULL);
	if (SUCCEEDED(err))
	{
		err = control->Run();
		if (FAILED(err))
		{
			std::wcout << "FAILED TO READ : " << music->path << '\n';
		}
		else {
			seek->GetDuration(&duration);
			Volume(-500);
			std::wcout << Volume()<<' '<<Duration() << '\n';
		}
	}
}

__int64 MusicPlayer::Duration() {
	return duration;
}

long MusicPlayer::Volume() {
	long volume;
	
	HRESULT err = audio->get_Volume(&volume);
	DxThrowIfFailed(err);

	return volume;
}

void MusicPlayer::Volume(long volume) {
	HRESULT err = audio->put_Volume(volume);
}

__int64 MusicPlayer::GetPosition() {
	if (!seek)
		return 0;

	__int64 pos;
	seek->GetCurrentPosition(&pos);
	return pos;
}

void MusicPlayer::SetPosition(__int64* cur, bool absolute) {
	if (seek) {
		DWORD flag;
		if (absolute)
			flag = AM_SEEKING_AbsolutePositioning | AM_SEEKING_SeekToKeyFrame;
		else
			flag = AM_SEEKING_RelativePositioning | AM_SEEKING_SeekToKeyFrame;
		seek->SetPositions(cur, flag, NULL, AM_SEEKING_NoPositioning);
	}
}

void MusicPlayer::OnDone() {
	if(control)
	control->Run();
}
void MusicPlayer::Pause() {
	if (control)
	control->Pause();
}
void MusicPlayer::Resume() {
	if (control)
	control->Run();
}