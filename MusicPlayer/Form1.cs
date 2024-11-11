using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WMPLib;

namespace MusicPlayer
{
    public partial class form1 : Form
    {
        StringTable strTable;
        MusicPlayer player;

        TreeNode lastSelectedNode = null;

        void SelectLanguage(StringTable strTable)
        {
            RepeateModeBox.Items.Clear();
            RepeateModeBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_ONE));
            RepeateModeBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_FOLDER));
            RepeateModeBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_ALL));
            RepeateModeBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_NO));

            NextModeBox.Items.Clear();
            NextModeBox.Items.Add(strTable.Format(StringTableIndex.NEXTMODEBOX_SEQUENTIAL));
            NextModeBox.Items.Add(strTable.Format(StringTableIndex.NEXTMODEBOX_RANDOM));

            RepeateModeBox.SelectedIndex = 0;
            NextModeBox.SelectedIndex = 0;

            PlayButton.Text = strTable.Format(StringTableIndex.PLAYER_START);
            NextButton.Text = strTable.Format(StringTableIndex.PLAYER_NEXT);
            BeforeButton.Text = strTable.Format(StringTableIndex.PLAYER_BEFORE);
            StopButton.Text = strTable.Format(StringTableIndex.PLAYER_STOP);

            PlayingMusicTextBox.Text = strTable.Format(StringTableIndex.STATUS_NOWPLAYING);
            playingForderTextBox.Text = strTable.Format(StringTableIndex.STATUS_NOWPLAYINGFORDER);
        }

        void UpdateBox()
        {
            var music = player.playing;
            if (music != null)
            {
                TargetMusicSpdBox.Text = music.spd.ToString();
                TargetMusicSoundBox.Text = music.volume.ToString();
            }
            else
            {
                TargetMusicSpdBox.Text = "0";
                TargetMusicSoundBox.Text = "0";
            }
            AllMusicSoundBox.Text = player.allSound.ToString();
            AllMusicSpdBox.Text = player.allSpd.ToString();
        }

        public void OnMediaOpened(Object NewState, EventArgs a)
        {
            PlayingForderText.Text = player.targetFolder == null ? "ALL" : player.targetFolder.name;
            PlayingMusicText.Text = player.playing == null ? "nothing" : player.playing.name;

            UpdateBox();
        }
        public void OnMediaPosChanged(Object NewState, EventArgs a)
        {
            double val = player.GetLocation();
            trackBar1.Value = (int) (val * trackBar1.Maximum);
        }

        private void AddMusicDataToTree(MusicData folder, TreeNodeCollection nodes)
        {
            var now = folder.dir;
            while (now != null)
            {
                var node = nodes.Add(now.name);
                if (now.isDirectory)
                    AddMusicDataToTree(now, node.Nodes);

                now = now.next;
            }
        }

        void UpdateMusicList()
        {
            MusicListBox.BeginUpdate();
            var manager = player.GetFileManager();
            var now = manager.mainForder;

            var nodes = MusicListBox.Nodes;

            AddMusicDataToTree(now, nodes);
            MusicListBox.EndUpdate();
            MusicListBox.ExpandAll();
        }

        public form1()
        {
            InitializeComponent();

            AllowDrop = true;

            strTable = new EngStringTable();
            SelectLanguage(strTable);

            player = new MusicPlayer("userdata.txt");
            UpdateMusicList();
            ChangeMode();

            player.AddOpenEvent(OnMediaOpened);
            player.AddPositionEvent(OnMediaPosChanged);
            player.AdjustSettingAllMusic(100, 20);
            UpdateBox();
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            player.Resume();
        }

        private void MusicListBox_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Left))
            {
                lastSelectedNode = e.Node;
            }
        }

        void ChangeMode()
        {
            if (player == null) return;

            var str = RepeateModeBox.SelectedItem.ToString();
            RepeateMode mode;
            if (str.Equals(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_ALL)))
                mode = RepeateMode.ALL;
            else if (str.Equals(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_NO)))
                mode = RepeateMode.NO;
            else if (str.Equals(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_FOLDER)))
                mode = RepeateMode.FOLDER;
            else mode = RepeateMode.ONE;

            str = NextModeBox.SelectedItem.ToString();
            GetNextMode nextMode;
            if (str.Equals(strTable.Format(StringTableIndex.NEXTMODEBOX_RANDOM)))
                nextMode = GetNextMode.RANDOM;
            else nextMode = GetNextMode.SEQUENTIAL;

            player.ChangePlayMode(mode, nextMode);
        }
        private void RepeateModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeMode();
        }

        private void NextModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeMode();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            player.Next();
        }

        private void BeforeButton_Click(object sender, EventArgs e)
        {
            player.Before();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            player.Stop();
        }

        private void PlaySelectedButton_Click(object sender, EventArgs e)
        {
            if (lastSelectedNode == null)
                return;
            player.PlayNode(lastSelectedNode);
        }

        private void AllMusicSettingApplyButton_Click(object sender, EventArgs e)
        {
            int spd = int.Parse(AllMusicSpdBox.Text);
            int vol = int.Parse(AllMusicSoundBox.Text);

            player.AdjustSettingAllMusic(spd, vol);
            UpdateBox();
        }

        private void TargetMusicSettingApplyBox_Click(object sender, EventArgs e)
        {
            int spd = int.Parse(TargetMusicSpdBox.Text);
            int vol = int.Parse(TargetMusicSoundBox.Text);

            player.AdjustSettingPlayingMusic(spd, vol);
            UpdateBox();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            var maxi = trackBar1.Maximum;
            Console.WriteLine(trackBar1.Value + "/" + maxi);
            double per = trackBar1.Value / (double)maxi;
            player.ChangeLocation(per);
        }

        private void form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            foreach (var file in files)
            {
                player.AddMusic(file);
            }
            UpdateMusicList();
        }

        private void form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
    }
}
