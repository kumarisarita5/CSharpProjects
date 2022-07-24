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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MatchGame
{
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBlock lastTestBlockedClicked;
        bool isFindingMatch = false;

        DispatcherTimer timer = new DispatcherTimer();
        int tenthOfSecondElasped;
        int matchFound;
        float maxScore= 15;


        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += Timer_Tick;

            SetUpGame();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tenthOfSecondElasped++;
            timeTextBlock.Text = (tenthOfSecondElasped / 10F).ToString("0.0s");
            if (matchFound == 8)
            {
                timer.Stop();
                if (maxScore > (tenthOfSecondElasped / 10F))
                {
                    maxScore = (tenthOfSecondElasped / 10F);
                }
                maxScoreBlock.Text = maxScore.ToString() + " s";
                timeTextBlock.Text = timeTextBlock.Text + " - Play again?";
            }
        }

        private void SetUpGame()
        {
            List<string> animalList = new List<string>()
            {
                "🐄","🐒","🐼","🐍","🐘","🐻","🐕","🐈","🦌","🦁","🐇","🐰"
            };

            List<bool> takenAnimal = new List<bool>();

            for(int i = 0;i< animalList.Count; i++)
            {
                takenAnimal.Add(false);
            }

            Random random = new Random();
            List<string> animalEmoji = new List<string>();

            for ( ; animalEmoji.Count < 16;)
            {
                int indexOfAnimal = random.Next(animalList.Count);
                if (!takenAnimal[indexOfAnimal])
                {
                    animalEmoji.Add(animalList[indexOfAnimal]);
                    animalEmoji.Add(animalList[indexOfAnimal]);
                    takenAnimal[indexOfAnimal] = true;
                }
            }

            foreach(TextBlock textBlock in MainGrid.Children.OfType<TextBlock>())
            {
                if(textBlock.Name != "timeTextBlock" && textBlock.Name!="maxScoreBlock")
                {
                    int index = random.Next(animalEmoji.Count());
                    string nextEmoji = animalEmoji[index];
                    textBlock.Text = nextEmoji;
                    textBlock.Visibility = Visibility.Visible;
                    animalEmoji.RemoveAt(index);
                }
            }

            timer.Start();
            tenthOfSecondElasped = 0;
            matchFound = 0;
            maxScoreBlock.Text = maxScore.ToString() + " s";
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock currentBlock = sender as TextBlock;

            if (!isFindingMatch)
            {
                lastTestBlockedClicked = currentBlock;
                currentBlock.Visibility = Visibility.Hidden;
                isFindingMatch = true;
            }
            else if(currentBlock.Text == lastTestBlockedClicked.Text)
            {
                currentBlock.Visibility = Visibility.Hidden;
                matchFound++;
                isFindingMatch = false;
            }
            else
            {
                lastTestBlockedClicked.Visibility = Visibility.Visible;
                isFindingMatch = false;
            }

        }

        private void TimeTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (matchFound == 8)
            {
                SetUpGame();
            }
        }
    }
}
