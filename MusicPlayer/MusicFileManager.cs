using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public class MusicData
    {
        public string path;
        public string name;

        public int volume;
        public int spd;

        public bool isDirectory;
        public MusicData dir = null;
        public MusicData next = null;
        public bool isOpened = false;

        public MusicData GetFinalNext()
        {
            MusicData ret = this;
            while (ret.next != null) ret = ret.next;
            return ret;
        }

        public MusicData GetNext(int idx)
        {
            int now = 0;
            MusicData data = this;
            while(now != idx && data.next != null)
            {
                data = data.next;
                now++;
            }
            return data;
        }

        private static String GetPathName(string path)
        {
            var l = path.Split('\\');
            return l[l.Length - 1].Split('.')[0];
        }
        private static MusicData CreateFolder(String path)
        {
            MusicData folder = new MusicData();
            folder.name = GetPathName(path);
            folder.isDirectory = true;
            return folder;
        }
        private static MusicData CreateMusic(string path)
        {
            MusicData file = new MusicData();
            file.name = GetPathName(path);
            file.path = path;
            file.isDirectory = false;
            file.volume = 100;
            file.spd = 100;
            return file;
        }
        public static MusicData ReadMusicData(String path)
        {
            if (!File.Exists(path) && !Directory.Exists(path)) return null;


            FileInfo info = new FileInfo(path);
            path = info.FullName;

            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                if (dirs.Length + files.Length == 0)
                    return null;

                MusicData first = null;
                MusicData last = null;
                foreach (String s in dirs)
                {
                    MusicData newFolder = CreateFolder(s);
                    newFolder.dir = ReadMusicData(s);

                    if (first == null) first = last = newFolder;
                    else
                    {
                        last.next = newFolder;
                        last = newFolder;
                    }
                }
                foreach (String s in files)
                {
                    MusicData newMusic = CreateMusic(s);

                    if (first == null) first = last = newMusic;
                    else
                    {
                        last.next = newMusic;
                        last = newMusic;
                    }
                }

                return first;
            }
            else
            {
                MusicData newMusic = CreateMusic(path);
                return newMusic;
            }
        }
    }

    public delegate void MusicListForeach(MusicData data, MusicData parent);

    internal class MusicFileManager
    {
        public MusicData mainForder = null;

        public MusicFileManager()
        {
            mainForder = new MusicData();
            mainForder.name = "";
            mainForder.path = "";
            mainForder.isDirectory = true;
            mainForder.dir = null;
        }

        public MusicData FindData(int target)
        {
            Stack<MusicData> stack = new Stack<MusicData>();
            stack.Push(mainForder);
            int nowLoc = 0;
            MusicData now = null;
            while (stack.Count > 0)
            {
                now = stack.Pop();
                while (true)
                {
                    if (now == null) break;
                    if (nowLoc == target) return now;
                    nowLoc++;
                    if (now.isDirectory)
                    {
                        stack.Push(now.next);
                        stack.Push(now.dir);
                        break;
                    }
                    now = now.next;
                }
            }

            return null;
        }

        public MusicData FindMusic(int target)
        {
            if (null == mainForder) return null;

            Stack<MusicData> stack = new Stack<MusicData>();
            stack.Push(mainForder);
            int nowLoc = 0;
            MusicData now = null;
            while (stack.Count > 0)
            {
                now = stack.Pop();
                while (true)
                {
                    if (now == null) break;
                    if (!now.isDirectory)
                    {
                        if (nowLoc == target) return now;
                        nowLoc++;
                    }

                    if (now.isDirectory)
                    {
                        stack.Push(now.next);
                        stack.Push(now.dir);
                        break;
                    }
                    now = now.next;
                }
            }

            return null;
        }

        public MusicData FindDataByName(String path)
        {
            if (null == mainForder) return null;

            Stack<MusicData> stack = new Stack<MusicData>();
            stack.Push(mainForder);
            MusicData now = null;
            while (stack.Count > 0)
            {
                now = stack.Pop();
                while (true)
                {
                    if (now == null) break;

                    if (now.name == path) return now;

                    if (now.isDirectory)
                    {
                        stack.Push(now.next);
                        stack.Push(now.dir);
                        break;
                    }
                    now = now.next;
                }
            }

            return null;
        }

        public MusicData GetNextMusic(MusicData folder, MusicData target)
        {
            if (folder == null) folder = mainForder;
            if (folder == null) return null;
            if (target.name == null) return null;

            Stack<MusicData> stack = new Stack<MusicData>();
            stack.Push(folder);
            MusicData now = null;
            bool FIND = false;
            while (stack.Count > 0)
            {
                now = stack.Pop();
                while (true)
                {
                    if (now == null) break;
                    if (!now.isDirectory)
                    {
                        if (FIND) return now;
                        if (target.name.Equals(folder.name))
                            FIND = true;
                    }

                    if (now.isDirectory)
                    {
                        stack.Push(now.next);
                        stack.Push(now.dir);
                        break;
                    }
                    now = now.next;
                }
            }

            return null;
        }

        //Action<music, folder>
        public void ForEach(Action<MusicData, MusicData> act, MusicData targetFolder)
        {
            if (null == targetFolder) return;

            Stack<MusicData> stack = new Stack<MusicData>();
            stack.Push(targetFolder);
            MusicData now = null;
            while (stack.Count > 0)
            {
                now = stack.Pop();
                while (true)
                {
                    if (now == null) break;

                    if(stack.Count == 0)
                        act(now, null);
                    else act(now, stack.Peek());

                    if (now.isDirectory)
                    {
                        stack.Push(now.next);
                        stack.Push(now.dir);
                        break;
                    }
                    now = now.next;
                }
            }
        }

        public void ChangeFileTree(MusicData mainData)
        {
            if (!mainData.isDirectory)
                return;
            this.mainForder = mainData;
        }

        public void AddData(MusicData data)
        {
            if (mainForder.dir != null)
                mainForder.dir.GetFinalNext().next = data;
            else
                mainForder.dir = data;
        }

        public void AddDataByPath(string path)
        {
            AddData(MusicData.ReadMusicData(path));
        }
    }
}
