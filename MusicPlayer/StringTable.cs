using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    public static class StringTableIndex
    {
        public const int PLAYMODEBOX_REPEATE_ONE = 1;
        public const int PLAYMODEBOX_REPEATE_FOLDER = 2;
        public const int PLAYMODEBOX_REPEATE_ALL = 3;
        public const int PLAYMODEBOX_REPEATE_NO = 4;

        public const int PLAYER_START = 5;
        public const int PLAYER_STOP = 6;
        public const int PLAYER_BEFORE = 7;
        public const int PLAYER_NEXT = 8;

        public const int STATUS_NOWPLAYING = 9;
        public const int STATUS_NOWPLAYINGFORDER = 10;
    }

    public interface StringTable
    {
        string Format(int i);
    }

    public class KoreanStringTable : StringTable
    {
        public virtual string Format(int i)
        {
            switch (i)
            {
                case StringTableIndex.PLAYMODEBOX_REPEATE_ALL: return "전체반복";
                case StringTableIndex.PLAYMODEBOX_REPEATE_FOLDER: return "폴더반복";
                case StringTableIndex.PLAYMODEBOX_REPEATE_ONE: return "한곡반복";
                case StringTableIndex.PLAYMODEBOX_REPEATE_NO: return "반복없음";
                case StringTableIndex.PLAYER_START: return "시작";
                case StringTableIndex.PLAYER_STOP: return "정지";
                case StringTableIndex.PLAYER_BEFORE: return "이전";
                case StringTableIndex.PLAYER_NEXT: return "다음";
                case StringTableIndex.STATUS_NOWPLAYING: return "현재 노래";
                case StringTableIndex.STATUS_NOWPLAYINGFORDER:return "대상폴더";
            }

            return "ERR";
        }
    }
}
