using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Figures.Transformations.Brushes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		Block[,] blocks = new Block[4, 4];
		Block[,] oldBlocks = new Block[4, 4];
		int score, prevScore;
		Button[,] btns = new Button[4, 4];
		DoubleAnimation doubleAnimation;
		Storyboard storyBoard;
		public MainWindow()
        {
            InitializeComponent();
			doubleAnimation = new DoubleAnimation();
			doubleAnimation.From = 80;
			doubleAnimation.To = 40;
			doubleAnimation.Duration = TimeSpan.FromMilliseconds(250);
			//Story board
			storyBoard = new Storyboard();
			storyBoard.Children.Add(doubleAnimation);
			//New game
			NewGame();
		}
		private void NewGame()
		{
			//Default Score
			score = 0;
			//Create new blocks
			Block.InitNewGameBlocks(ref blocks);
			Block.InitBlocks(ref oldBlocks);
			Block.CopyBlock(ref oldBlocks, ref blocks);
			//Display
			//Score.Text = score.ToString();
			DrawNewBlock();
		}
		private void BackGame()
		{
			//Score
			score = prevScore;
			//Chang back to old blocks
			Block.CopyBlock(ref blocks, ref oldBlocks);
			//Update display
			//Score.Text = score.ToString();
			DrawNewBlock();
		}
		private void LoadGame(Block[,] NewBlocks, int NewScore)
		{
			//Load score and blocks
			score = NewScore;
			Block.CopyBlock(ref blocks, ref NewBlocks);
			//Update display
			//Score.Text = NewScore.ToString();
			DrawNewBlock();
		}
		private bool GameOver(Block[,] blocks)
		{
			for (int row = 0; row < 4; row++)
			{
				for (int col = 0; col < 4; col++)
				{
					if (blocks[row, col].num == 0)
						return false;
				}
			}
			for (int row = 0; row < 4; row++)
			{
				for (int col = 0; col < 3; col++)
				{
					if (blocks[row, col].num == blocks[row, col + 1].num)
						return false;
				}
			}
			for (int col = 0; col < 4; col++)
			{
				for (int row = 0; row < 3; row++)
				{
					if (blocks[row + 1, col].num == blocks[row, col].num)
						return false;
				}
			}
			return true;
		}
		/*private void GridClear()
		{
			//Clear all grid except the menu grid column (col = 4)
			for (int i = 0; i < menuGrid.Children.Count; i++)
			{
				if ((Grid.GetColumn(menuGrid.Children[i]) != 4))
				{
					menuGrid.Children.Remove(menuGrid.Children[i]);
				}
			}
		}*/
		private void DrawNewBlock()
		{
			//GridClear();
			for (int row = 0; row < 4; row++)
			{
				for (int col = 0; col < 4; col++)
				{
					//Button settings
					btns[row, col] = new Button();
					btns[row, col].BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));
					btns[row, col].BorderThickness = new Thickness(3);
					btns[row, col].FontStretch = new FontStretch();
					if (blocks[row, col].num != 0) //If have tile (2, 4, 8,...) 
					{
						//Title buttons settings
						btns[row, col].Background = Block.GetTitleBlocksColor(blocks[row, col]);
						btns[row, col].Content = blocks[row, col].num.ToString();
						btns[row, col].FontSize = 40;

						//Animation when new title appear and 2 titles combined
						if (blocks[row, col].combined == true || blocks[row, col].newBlock == true)
						{
							Storyboard.SetTarget(doubleAnimation, btns[row, col]);
							Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(Button.FontSizeProperty));
							storyBoard.Begin(btns[row, col]);
						}
					}
					else
					{
						//None title buttons settings
						btns[row, col].Background = Block.GetUntitledBlockColor();
					}
					//Set it to the grid
					Grid.SetColumn(btns[row, col], col);
					Grid.SetRow(btns[row, col], row);
					//menuGrid.Children.Add(btns[row, col]);
				}
			}
		}
	}
}