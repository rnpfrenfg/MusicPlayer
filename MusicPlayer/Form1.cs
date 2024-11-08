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

            strTable = new EngStringTable();
            SelectLanguage(strTable);

            player = new MusicPlayer();
            player.AddMusic("Music");
            UpdateMusicList();
            ChangeMode();
            player.AdjustSettingAllMusic(100, 20);
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
    }
}
