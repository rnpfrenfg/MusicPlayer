using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicPlayer
{
    public partial class Form1 : Form
    {
        StringTable strTable;
        MusicPlayer player;

        void SelectLanguage(StringTable strTable)
        {
            PlayModeSelectBox.Items.Clear();
            PlayModeSelectBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_ONE));
            PlayModeSelectBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_FOLDER));
            PlayModeSelectBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_ALL));
            PlayModeSelectBox.Items.Add(strTable.Format(StringTableIndex.PLAYMODEBOX_REPEATE_NO));

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

        public Form1()
        {
            InitializeComponent();

            strTable = new KoreanStringTable();
            SelectLanguage(strTable);

            player = new MusicPlayer();
            player.AddMusic("Music");
            UpdateMusicList();

            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PlayModeSelectBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(e.NewValue == CheckState.Checked)
            {
                for(int i=0; i<PlayModeSelectBox.Items.Count; i++)
                {
                    if (e.Index != i) PlayModeSelectBox.SetItemChecked(i, false);

                    PlayMode mode;
                    switch (i)
                    {
                        case 0: mode = PlayMode.ONE; break;
                        case 1: mode = PlayMode.FOLDER; break;
                        case 2: mode = PlayMode.ALL; break;
                        case 3: mode = PlayMode.NO; break;
                        default: mode = PlayMode.ONE; break;
                    }
                    player.ChangePlayMode(mode);
                }
            }
            else
            {
                if (PlayModeSelectBox.SelectedItems.Count == 0)
                {
                }
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            player.Play();
        }
    }
}
