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

namespace MusicPlayer
{
    public enum RepeateMode {
        ONE, FOLDER, ALL, NO
    }
    public enum GetNextMode
    {
        SEQUENTIAL, RANDOM
    }

    internal class MusicPlayer
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
        private void SetPlayList()
        {

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

                    int size = 0;
                    fileManager.ForEach((MusicData a, MusicData folder) => { if (!a.isDirectory) size++; }, targetFolder);
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

        public void ChangeSaveFolder(String newpath)
        {
            if(saveFolder != null)
            {
                saveFolder = "userdata";//TODO
            }

            saveFolder = newpath;
            SaveChange();
        }

        private void SaveChange()
        {
            if (saveFolder == null)
                return;
            return;

            string jsonString = JsonSerializer.Serialize(this);
            using (StreamWriter sw = File.CreateText(saveFolder))
            {
                sw.WriteLine(jsonString);
            }
        }

        public static MusicPlayer Load(string saveFolder)
        {
            string s;
            using (StreamReader sr = File.OpenText(saveFolder))
            {
                s = sr.ReadToEnd();
            }
            return JsonSerializer.Deserialize <MusicPlayer>(s);
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

        private String saveFolder = null;

        private int allSound = 0;
        private int allSpd = 0;

        private RepeateMode playMode;
        private GetNextMode nextMode;
        private MusicFileManager fileManager;
        private MusicData playing = null;
        private MusicData targetFolder = null;
        private MusicData[] playlist = null;
        private int playlistIndex = 0;
        private Queue<MusicData> data;

        MediaPlayer wmp;
    }
}
