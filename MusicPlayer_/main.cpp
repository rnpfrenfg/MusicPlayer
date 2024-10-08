#include "includewindow.h"

#include "Program.h"
#include "Image.h"
#include "Page.h"

#include "__Page.h"

void CCreateConsole()
{
	AllocConsole();
	freopen("CONOUT$", "w", stdout);
	freopen("CONIN$", "r", stdin);
}

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int CmdShow)
{
	Program::InitInstance();
	Program* program = Program::GetInstance();

	if (!(program->Init(hInstance, 400, 600, TEXT("ssehc"), TEXT("window"), true, 100)))
		return 0;

	program->pages[0] = new MainPage();
	program->pages[1] = new TestPage();

	CCreateConsole();
	program->Start((int)PageID::MAIN, CmdShow);
	return 0;
}