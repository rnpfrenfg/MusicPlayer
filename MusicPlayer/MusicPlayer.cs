using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.IO;
using System.Runtime.CompilerServices;
using System.Media;
using System.Data.SqlTypes;

using WMPLib;
using System.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Security.Cryptography.X509Certificates;

namespace MusicPlayer
{
    public enum RepeateMode {
        ONE, FOLDER, ALL, NO
    }
    public enum GetNextMode
    {
        SEQUENTIAL, RANDOM
    }
    public class MusicPlayer
    {
        public MusicPlayer()
        {
            fileManager = new MusicFileManager();
            wmp = new MediaPlayer();

            wmp.MediaEnded += Player_PlayStateChange;
        }
        private void Player_PlayStateChange(Object NewState, EventArgs a)
        {
            Next();
        }
        private void _PlayMusic(MusicData data)
        {
            playing = data;

            _StopMusic();
            wmp.Open(new Uri(data.path, UriKind.Absolute));
            wmp.Play();
            ChangePlayingSetting();
            SaveChange();
            return;
        }
        private void _StopMusic()
        {
            wmp.Stop();
        }
        public void Resume()
        {
            if (playing == null)
            {
                MusicData music = fileManager.FindMusic(0);
                if (music == null) return;
                _PlayMusic(music);
                SaveChange();
                return;
            }
            else
                wmp.Play();
        }
        private int GetIndex(MusicData target)
        {
            for (int i = 0; i < playlist.Length; i++)
            {
                if (playlist[i] == target)
                    return i;
            }
            return 0;
        }
        private void SetFirst(MusicData target)
        {
            if (target.isDirectory) return;
            int targetIdx = GetIndex(target);
            (playlist[targetIdx], playlist[0]) = (playlist[0], playlist[targetIdx]);
        }
        public void PlayNode(TreeNode node)
        {
            var target = SelectItem(node);
            if(target == null) return;
            var folder = SelectItem(node.Parent);

            playing = target;
            switch (playMode)
            {
                case RepeateMode.NO:
                case RepeateMode.ONE:
                    if (target.isDirectory)
                        return;
                    _PlayMusic(target);
                    break;
                case RepeateMode.FOLDER:
                    if (target.isDirectory)
                        folder = target;
                    targetFolder = folder;

                    playlist = GenerateNewPlaylist();
                    SetFirst(target);
                    _PlayMusic(playlist[playlistIndex]);
                    break;
                case RepeateMode.ALL:
                    playlist = GenerateNewPlaylist();
                    playlistIndex = GetIndex(target);
                    _PlayMusic(playlist[playlistIndex]);
                    break;
            }

        }

        public void Stop()
        {
            wmp.Pause();
        }
        private MusicData[] GenerateNewPlaylist()
        {
            playlistIndex = 0;
            MusicData[] ret = null;

            switch (playMode)
            {
                case RepeateMode.NO:
                case RepeateMode.ONE:
                    if (playing == null)
                        return null;

                    ret = new MusicData[1];
                    ret[0] = playing;
                    return ret;
                case RepeateMode.FOLDER:
                case RepeateMode.ALL:
                    if(RepeateMode.ALL == playMode || targetFolder == null)
                    {
                        targetFolder = fileManager.mainForder;
                    }

                    int size = fileManager.GetSize();
                    if(size == 0)
                    {
                        return null;
                    }
                    ret = new MusicData[size];
                    int idx = 0;
                    fileManager.ForEach((MusicData a, MusicData folder) => { if (!a.isDirectory) { ret[idx] = a;idx++; } }, targetFolder);
                    
                    if (nextMode == GetNextMode.RANDOM)
                    {
                        Random random = new Random();
                        int randIdx;
                        MusicData temp;
                        for(int i = 0; i < ret.Length; i++)
                        {
                            randIdx = random.Next(ret.Length);
                            temp = ret[i];
                            ret[i] = ret[randIdx];
                            ret[randIdx] = temp;
                        }
                    }
                    return ret;
            }

            return null;
        }

        public void Next()
        {
            switch (playMode)
            {
                case RepeateMode.NO:
                    return;
                case RepeateMode.ONE:
                    _PlayMusic(playing);
                    return;
                case RepeateMode.FOLDER:
                case RepeateMode.ALL:
                    playlistIndex++;
                    if (playlist == null || playlistIndex == playlist.Length)
                    {
                        playlist = GenerateNewPlaylist();
                    }
                    _PlayMusic(playlist[playlistIndex]);
                    return;
            }
        }

        public void Before()
        {
            switch (playMode)
            {
                case RepeateMode.NO:
                    break;
                case RepeateMode.ONE:
                    _StopMusic();
                    _PlayMusic(playing);
                    break;
                case RepeateMode.FOLDER:
                case RepeateMode.ALL:
                    playlistIndex--;
                    if (playlistIndex < 0)
                    {
                        playlist = GenerateNewPlaylist();
                        return;
                    }
                    _PlayMusic(playlist[playlistIndex]);
                    break;
            }
        }

        public MusicData _SelectItem(TreeNode node)
        {
            if (node.Parent == null)
            {
                return fileManager.mainForder.dir.GetNext(node.Index);
            }
            return _SelectItem(node.Parent).dir.GetNext(node.Index);
        }

        public MusicData SelectItem(TreeNode node)
        {
            if (node == null) return null;
            var select = _SelectItem(node);
            return select;
        }

        public void AdjustSettingAllMusic(int spd, int sound)
        {
            this.allSound = sound;
            this.allSpd = spd;

            ChangePlayingSetting();
            SaveChange();
        }

        public void AdjustSettingPlayingMusic(int spd, int volume)
        {
            if (playing == null)
                return;
            playing.volume = volume;
            playing.spd = spd;

            ChangePlayingSetting();
            SaveChange();
        }

        private void ChangePlayingSetting()
        {
            double volume = allSound / 100.0;
            if (playing != null) volume *= playing.volume/100.0;
            wmp.Volume = volume;

            double spd = allSpd / 100.0;
            if (playing != null) spd *= playing.spd / 100.0;

            wmp.SpeedRatio = spd;
        }

        public void ChangePlayMode(RepeateMode mode, GetNextMode nextMode)
        {
            playMode = mode;
            this.nextMode = nextMode;
            SaveChange();
        }

        public void AddMusic(String path)
        {
            fileManager.AddDataByPath(path);
            SaveChange();
        }

        public void ChangeFileTree(MusicData mainData)
        {
            fileManager.ChangeFileTree(mainData);
            SaveChange();
        }

        public MusicFileManager GetFileManager()
        {
            return fileManager;
        }

        public String GetPlayingMusic()
        {
            if (playing != null) return playing.name;
            return "";
        }

        public String GetPlayingTarget()
        {
            if (targetFolder != null) return targetFolder.name;
            return "";
        }

        private void WriteMusicData(MusicData data, StreamWriter sw)
        {
            sw.WriteLine(data.isDirectory);
            sw.WriteLine(data.name);
            if (!data.isDirectory)
            {
                sw.WriteLine(data.path);
                sw.WriteLine(data.spd);
                sw.WriteLine(data.volume);
            }
        }
        private MusicData ReadMusicData(StreamReader sr)
        {
            bool isDir = bool.Parse(sr.ReadLine());
            string name = sr.ReadLine();
            if (isDir)
            {
                return MusicData.CreateFolder(name);
            }
            else
            {
                string path = sr.ReadLine();
                int spd = int.Parse(sr.ReadLine());
                int volume = int.Parse(sr.ReadLine());
                var data = MusicData.CreateMusic(path);
                data.name = name;
                data.spd = spd;
                data.volume = volume;
                return data;
            }
        }
        private void WriteDirectory(MusicData dir, Dictionary<MusicData, int> table, StreamWriter sw, bool isRoot)
        {
            var now = dir.dir;
            if (!isRoot) sw.WriteLine(table[dir]);

            if (now == null)
            {
                sw.WriteLine(0);
                return;
            }
            int size = now.ListLengt();
            sw.WriteLine(size);
            while (now != null)
            {
                sw.WriteLine(table[now]);
                now = now.next;
            }
        }
        private void ReadDirectory(MusicData[] list, StreamReader sr, bool isRoot)
        {
            MusicData folder;
            if (isRoot) folder = fileManager.mainForder;
            else folder = list[int.Parse(sr.ReadLine())];
            int size = int.Parse(sr.ReadLine());
            if(size == 0)
            {
                folder.dir = null;
                return;
            }
            folder.dir = list[int.Parse(sr.ReadLine())];
            var now = folder.dir;
            for (int i = 1; i < size; i++)
            {
                now.next = list[int.Parse(sr.ReadLine())];
                now = now.next;
            }
        }

        private void SaveChange()
        {
            if (saveFolder == null)
                return;

            StreamWriter sw = new StreamWriter(saveFolder);
            sw.WriteLine(allSound);
            sw.WriteLine(allSpd);
            sw.WriteLine((int)playMode);
            sw.WriteLine((int)nextMode);

            int size = fileManager.GetSize();
            sw.WriteLine(size);
            Dictionary<MusicData, int> table = new Dictionary<MusicData, int>();
            int idx = 0;
            fileManager.ForEach((MusicData data, MusicData file) =>{
                table.Add(data, idx);
                WriteMusicData(data, sw);
                idx++;
            }, fileManager.mainForder.dir);

            WriteDirectory(fileManager.mainForder,table,sw, true);
            size = 0;
            fileManager.ForEach((MusicData data, MusicData folder) => {
                if (data.isDirectory)
                {
                    size++;
                }
            }, fileManager.mainForder.dir);
            sw.WriteLine(size);
            fileManager.ForEach((MusicData data, MusicData folder) => {
                if (data.isDirectory)
                {
                    WriteDirectory(data, table, sw, false);
                }
            }, fileManager.mainForder.dir);

            sw.WriteLine(playlistIndex);
            if (playlist == null)
                sw.WriteLine(0);
            else sw.WriteLine(playlist.Length);
            for (int i = 0; i < playlistIndex; i++)
            {
                sw.WriteLine(table[playlist[i]]);
            }
            sw.Close();
        }
        public MusicPlayer(string save) : this()
        {
            if (!File.Exists(save))
            {
                this.saveFolder = save;
                return;
            }

            saveFolder = save;

            StreamReader sr = new StreamReader(saveFolder);
            allSound = int.Parse(sr.ReadLine());
            allSpd = int.Parse(sr.ReadLine());
            playMode = (RepeateMode)int.Parse(sr.ReadLine());
            nextMode = (GetNextMode)int.Parse(sr.ReadLine());

            int size = int.Parse(sr.ReadLine());
            MusicData[] list = new MusicData[size];
            for (int i = 0; i < size; i++)
                list[i] = ReadMusicData(sr);

            ReadDirectory(list,sr,true);
            size = int.Parse(sr.ReadLine());
            for(int i = 0; i < size; i++)
            {
                ReadDirectory(list, sr, false);
            }

            playlistIndex = int.Parse(sr.ReadLine());
            size = int.Parse(sr.ReadLine());
            playlist = new MusicData[size];
            for(int i = 0; i < size; i++)
            {
                playlist[i] = list[int.Parse(sr.ReadLine())];
            }
            sr.Close();
        }
        private String saveFolder = null;

        public MusicFileManager fileManager;

        public int allSound = 0;
        public int allSpd = 0;
        public RepeateMode playMode;
        public GetNextMode nextMode;

        private int playlistIndex = 0;
        public MusicData playing = null;
        public MusicData targetFolder = null;
        private MusicData[] playlist = null;

        MediaPlayer wmp;
    }
}
