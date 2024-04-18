using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Figures.Transformations.Brushes
{
    internal class Block
    {
        public int num = 0;
        public bool combined = false;
        public bool newBlock = false;
        public enum Direction { Up = 1, Down = 2, Left = 3, Right = 4 }
        //Color======================================================================
        private static readonly SolidColorBrush tile2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEE4DA"));
        private static readonly SolidColorBrush tile4 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDE0C8"));
        private static readonly SolidColorBrush tile8 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2B179"));
        private static readonly SolidColorBrush tile16 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59563"));
        private static readonly SolidColorBrush tile32 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F67C5F"));
        private static readonly SolidColorBrush tile64 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F65E3B"));
        private static readonly SolidColorBrush tile128 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDCF72"));
        private static readonly SolidColorBrush tile256 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDCC61"));
        private static readonly SolidColorBrush tile512 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDC850"));
        private static readonly SolidColorBrush tile1024 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDC53F"));
        private static readonly SolidColorBrush tile2048 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EDC22E"));
        private static readonly SolidColorBrush tileSuper = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3C3A32"));

        private static readonly SolidColorBrush border = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
        private static readonly SolidColorBrush untitled = new SolidColorBrush((Color)ColorConverter.ConvertFromString("LightGray"));

        public static SolidColorBrush GetTitleBlocksColor(Block block)
        {
            switch (block.num)
            {
                case 2:
                    return tile2;
                case 4:
                    return tile4;
                case 8:
                    return tile8;
                case 16:
                    return tile16;
                case 32:
                    return tile32;
                case 64:
                    return tile64;
                case 128:
                    return tile128;
                case 256:
                    return tile256;
                case 512:
                    return tile512;
                case 1024:
                    return tile1024;
                case 2048:
                    return tile2048;
                default:
                    return tileSuper;
            }
        }

        public static SolidColorBrush GetUntitledBlockColor()
        {
            return untitled;
        }

        private static int CountZeroBlocks(Block[,] blocks)
        {
            int kq = 0;
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    if (blocks[row, col].num == 0)
                        kq++;
            return kq;
        }

        private static List<int> ProcessList(ref List<int> list, ref int score)  //If blocks changed, return the index which combined
        {
            List<int> result = new List<int>();
            int front = 0, end = 1;
            while (end <= 3 && front <= 2)
            {
                if (list[front] == 0)
                {
                    front++;
                    end = front + 1;
                }
                else if (list[end] == 0)
                {
                    end++;
                }
                else
                {
                    if (list[front] == list[end]) //Found something to combine
                    {
                        //Add position that just combined
                        result.Add(front);
                        //Combine and update index
                        list[front] = list[front] * 2;
                        list[end] = 0;
                        score = score + list[front];
                        front++;
                        end = front + 1;
                    }
                    else
                    {
                        front++;
                        end = front + 1;
                    }
                }
            }
            //Process zeros: 0400 -> 4000
            int zeroIndex = -1;
            //Maximun  3 numbers need to put to the top
            for (int i = 0; i < 4; i++)
            {
                if (list[i] == 0)
                {
                    if (zeroIndex == -1)
                        zeroIndex = i;
                }
                else
                {
                    if (zeroIndex != -1)
                    {
                        if (result.Contains(i))
                        {
                            result[result.IndexOf(i)] = zeroIndex;
                        }
                        list[zeroIndex] = list[i];
                        list[i] = 0;
                        zeroIndex = -1;
                        i = zeroIndex + 1;
                        result.Add(-1);
                    }
                }
            }
            return result;
        }

        public static void GenerateABlock(ref Block[,] blocks)
        {
            int k = Block.CountZeroBlocks(blocks);
            var rand = new Random();
            int randPosition = rand.Next(0, k);
            int randNumber = rand.Next(1, 2) * 2;

            int temp_count = -1;
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (blocks[row, col].num == 0)
                    {
                        temp_count++;
                        if (temp_count == randPosition)
                        {
                            blocks[row, col].num = randNumber;
                            blocks[row, col].newBlock = true;
                        }
                    }
                }
            }
        }
        public static void InitBlocks(ref Block[,] blocks)
        {
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    blocks[row, col] = new Block();
        }
        public static void InitNewGameBlocks(ref Block[,] blocks)
        {
            InitBlocks(ref blocks);
            GenerateABlock(ref blocks);
            GenerateABlock(ref blocks);
        }
		public static bool TryUp(ref Block[,] blocks, ref Block[,] oldBlocks, ref int score) //If blocks changed, return true
		{
			//Create a temp blocks to store untouched blks
			Block[,] tempBlocks = new Block[4, 4];
			InitBlocks(ref tempBlocks);
			CopyBlock(ref tempBlocks, ref blocks);

			bool blocksChanged = false;
			for (int col = 0; col < 4; col++)
			{
				List<int> list = new List<int>();
				for (int row = 0; row < 4; row++)
				{
					list.Add(blocks[row, col].num);
					//reset combined and newblock
					blocks[row, col].combined = false;
					blocks[row, col].newBlock = false;
				}
				List<int> changedList = ProcessList(ref list, ref score);
				if (changedList.Count != 0) // If blocks changed
				{
					blocksChanged = true;
					//Tell that block has combined (*Different for each action: up down left right)
					int l_list = changedList.Count;
					for (int i = 0; i < l_list; i++)
					{
						if (changedList[i] != -1)
							blocks[changedList[i], col].combined = true;
					}
					//Update blocks
					int list_k = 0;
					for (int row = 0; row < 4; row++) //(*Different for each action: up down left right)
					{
						blocks[row, col].num = list[list_k];
						list_k++;
					}
					list.Clear();
				}
			}

			if (blocksChanged == true)//If blocks changed then update oldblocks and set every blocks to not new
			{
				CopyBlock(ref oldBlocks, ref tempBlocks);
			}
			return blocksChanged;
		}
		public static bool TryDown(ref Block[,] blocks, ref Block[,] oldBlocks, ref int score) //If blocks changed, return true
		{
			//Create a temp blocks to store Untouched blks
			Block[,] tempBlocks = new Block[4, 4];
			InitBlocks(ref tempBlocks);
			CopyBlock(ref tempBlocks, ref blocks);

			bool blocksChanged = false;
			for (int col = 0; col < 4; col++)
			{
				List<int> list = new List<int>();
				for (int row = 3; row >= 0; row--)
				{
					list.Add(blocks[row, col].num);
					//Set all blocks combined to false
					blocks[row, col].combined = false;
					blocks[row, col].newBlock = false;
				}
				List<int> changedList = ProcessList(ref list, ref score);
				if (changedList.Count != 0) // If blocks changed
				{
					blocksChanged = true;
					//Tell that block has combined (*Different for each action: up down left right)
					int l_list = changedList.Count;
					for (int i = 0; i < l_list; i++)
					{
						if (changedList[i] != -1)
							blocks[3 - changedList[i], col].combined = true;
					}
					int list_k = 0;
					for (int row = 3; row >= 0; row--)
					{
						blocks[row, col].num = list[list_k];
						list_k++;
					}
				}
				list.Clear();
			}

			if (blocksChanged == true)//If blocks changed then update oldblocks and set every blocks to not new
			{
				CopyBlock(ref oldBlocks, ref tempBlocks);
			}
			return blocksChanged;
		}
		public static bool TryLeft(ref Block[,] blocks, ref Block[,] oldBlocks, ref int score) //If blocks changed, return true
		{
			//Create a temp blocks to store Untouched blks
			Block[,] tempBlocks = new Block[4, 4];
			InitBlocks(ref tempBlocks);
			CopyBlock(ref tempBlocks, ref blocks);

			bool blocksChanged = false;
			for (int row = 0; row < 4; row++)
			{
				List<int> list = new List<int>();
				for (int col = 0; col < 4; col++)
				{
					list.Add(blocks[row, col].num);
					//Set all blocks combined to false
					blocks[row, col].combined = false;
					blocks[row, col].newBlock = false;
				}
				List<int> changedList = ProcessList(ref list, ref score);
				if (changedList.Count != 0) // If blocks changed
				{
					blocksChanged = true;
					//Tell that block has combined (*Different for each action: up down left right)
					int l_list = changedList.Count;
					for (int i = 0; i < l_list; i++)
					{
						if (changedList[i] != -1)
							blocks[row, changedList[i]].combined = true;
					}
					int list_k = 0;
					for (int col = 0; col < 4; col++)
					{
						//Set animation if necessary 
						blocks[row, col].num = list[list_k];
						list_k++;
					}
					list.Clear();
				}
			}

			if (blocksChanged == true)//If blocks changed then update oldblocks and set every blocks to not new
			{
				CopyBlock(ref oldBlocks, ref tempBlocks);
			}
			return blocksChanged;
		}
		public static bool TryRight(ref Block[,] blocks, ref Block[,] oldBlocks, ref int score) //If blocks changed, return true
		{
			//Create a temp blocks to store Untouched blks
			Block[,] tempBlocks = new Block[4, 4];
			InitBlocks(ref tempBlocks);
			CopyBlock(ref tempBlocks, ref blocks);

			bool blocksChanged = false;
			for (int row = 0; row < 4; row++)
			{
				List<int> list = new List<int>();
				for (int col = 3; col >= 0; col--)
				{
					list.Add(blocks[row, col].num);
					//Set all blocks combined to false
					blocks[row, col].combined = false;
					blocks[row, col].newBlock = false;
				}
				List<int> changedList = ProcessList(ref list, ref score);
				if (changedList.Count != 0) // If blocks changed
				{
					blocksChanged = true;
					//Tell that block has combined (*Different for each action: up down left right)
					int l_list = changedList.Count;
					for (int i = 0; i < l_list; i++)
					{
						if (changedList[i] != -1)
							blocks[row, 3 - changedList[i]].combined = true;
					}
					int list_k = 0;
					for (int col = 3; col >= 0; col--)
					{
						//Set animation if necessary 
						blocks[row, col].num = list[list_k];
						list_k++;
					}
					list.Clear();
				}
			}

			if (blocksChanged == true)//If blocks changed then update oldblocks and set every blocks to not new
			{
				CopyBlock(ref oldBlocks, ref tempBlocks);
			}
			return blocksChanged;
		}
		public static void CopyBlock(ref Block[,] desBlocks, ref Block[,] souBlocks)
		{
			for (int row = 0; row < 4; row++)
			{
				for (int col = 0; col < 4; col++)
				{
					desBlocks[row, col].num = souBlocks[row, col].num;
					desBlocks[row, col].combined = souBlocks[row, col].combined;
					desBlocks[row, col].newBlock = souBlocks[row, col].newBlock;
				}
			}
		}
	}
}