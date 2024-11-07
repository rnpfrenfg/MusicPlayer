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
using System.Media;

using WMPLib;

namespace MusicPlayer
{
    public enum PlayMode {
        ONE, FOLDER, ALL, NO
    }

    internal class MusicPlayer
    {
        public MusicPlayer()
        {
            playlist = new List<MusicData>();
            fileManager = new MusicFileManager();
            wmp = new WindowsMediaPlayer();
        }
        private void _PlayMusic(MusicData data)
        {
            wmp.URL = data.path;
            wmp.controls.play();
        }
        public void Play()
        {
            if(playing == null)
            {
                MusicData music = fileManager.FindMusic(0);
                if (music == null) return;
                _PlayMusic(music);
                SaveChange();
                return;
            }

            _PlayMusic(playing);
        }

        public void Stop()
        {

        }

        public void Next()
        {
            if(playlist.Count() == playlistIndex)
            {

            }
            switch (playMode)
            {
                case PlayMode.NO:
                case PlayMode.ONE:

                    break;
                case PlayMode.FOLDER:
                    break;
            }

            SaveChange();
        }

        public void Before()
        {

            SaveChange();
        }

        public void AdjustSettingAllMusic(int spd, int sound)
        {
            this.allSound = sound;
            this.allSpd = spd;

            SaveChange();
        }

        public void AdjustSettingTargetMusic(int spd, int sound)
        {

            SaveChange();
        }

        public void ChangePlayMode(PlayMode mode)
        {
            playMode = mode;
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

        private String saveFolder = null;

        private int allSound = 0;
        private int allSpd = 0;

        private PlayMode playMode;
        private MusicFileManager fileManager;
        private MusicData playing = null;
        private MusicData targetFolder = null;
        private List<MusicData> playlist = null;
        private int playlistIndex = 0;
        private Queue<MusicData> data;

        WindowsMediaPlayer wmp;
    }
}
