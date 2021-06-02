using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Threading;

namespace Endless_Rattlesnake
{
    
    public partial class MainWindow : Window
    {
        // Instance of the class DispatcherTimer, an integrated timer integrated into the Dispatcher class
        DispatcherTimer gameTimer = new DispatcherTimer();

        // Declaration of sprite hitboxes
        Rect playerHitBox;
        Rect groundHitBox;
        Rect obstacleHitBox;

        // Declaration of the jumping boolean
        bool jumping;

        int force = 20;
        int speed = 5;

        // Instance of the Random class, used for randomization
        Random rnd = new Random();

        // Declaration of the game over boolean
        bool gameOver;

        // Declaration of the sprite index, meaning currently displayed sprite
        double spriteIndex = 0;

        // Instances of the ImageBrush class, used for defining content as an image
        ImageBrush playerSprite = new ImageBrush();
        ImageBrush backgroundSprite = new ImageBrush();
        ImageBrush obstacleSprite = new ImageBrush();

        // Integer array used for determining where new obstacles spawn
        int[] obstaclePosition = { 320, 310, 300, 305, 315 };

        // Declarartion of the game score
        int score = 0;



        public MainWindow()
        {
            InitializeComponent();

            // Focuses the keyboard interaction to the game window
            MyCanvas.Focus();

            // Manages time based on the DispatcherTimer class
            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            // Uses the linked image as the background
            backgroundSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/background.gif"));

            // Fills the rectangles in the XAML file with the background images
            background.Fill = backgroundSprite;
            background2.Fill = backgroundSprite;

            // Runs the method StartGame()
            StartGame();

        }

        private void GameEngine(object sender, EventArgs e)
        {
            // Sets the scrolling speed of the background
            Canvas.SetLeft(background, Canvas.GetLeft(background) -6);
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) -6);

            // Enables paralax scrolling
            if (Canvas.GetLeft(background) < -1262)
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background2) + background2.Width);
            }

            if (Canvas.GetLeft(background2) < -1262)
            {
                Canvas.SetLeft(background2, Canvas.GetLeft(background) + background2.Width);
            }

            // Determines the speed of the player and the obstacle sprites
            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 12);

            // Assigns the score variable to the scoreText Label
            scoreText.Content = "Score: " + score;

            // Determines the hitboxes of the sprites
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 15, player.Height);
            obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);

            // Enables the player to run on the ground
            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                speed = 0;

                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);

                jumping = false;

                // Determines the speed of the running animation
                spriteIndex += .5;

                // Restarts the running animation
                if (spriteIndex > 8)
                {
                    spriteIndex = 1;
                }

                // Passes through spriteIndex into RunSprite method to determine which sprite should be displayed
                RunSprite(spriteIndex);
            }

            // Determines the behaviour of the jump action
            if (jumping == true)
            {
                speed = -9;

                force -= 1;
            }

            else
            {
                speed = 12;
            }

            // Makes sure the player doesn't fly of into outer space
            if (force < 0)
            {
                jumping = false;
            }

            // Increases the score by 1 for each obstacles passed
            if (Canvas.GetLeft(obstacle) < -50)
            {
                Canvas.SetLeft(obstacle, 950);

                Canvas.SetTop(obstacle, obstaclePosition[rnd.Next(0, obstaclePosition.Length)]);

                score += 1;
            }

            // Determines what happens when the player and the obstacle sprite intersect
            if (playerHitBox.IntersectsWith(obstacleHitBox))
            {
                gameOver = true;

                gameTimer.Stop();
            }

            // Determines what happens when the game is over
            if (gameOver == true)
            {
                obstacle.Stroke = Brushes.Black;
                obstacle.StrokeThickness = 1;

                player.Stroke = Brushes.Red;
                player.StrokeThickness = 1;

                scoreText.Content = "Score: " + score + " Press Enter to play again!";

            }
            else
            {
                player.StrokeThickness = 0;
                obstacle.StrokeThickness = 0;
            }

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
            /// <summary>
            /// Restarts the game when the enter key is pressed and the game is over
            /// </summary>
            /// <param name="e"> Currently pressed keyboard key </param>
        {
            if (e.Key == Key.Enter && gameOver == true)
            {
                StartGame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
            /// <summary> 
            /// Links the spacebar with the jump action 
            /// </summary>
            /// <param name="e"> Currently pressed keyboard key </param>
        {
            // Triggers the jump action when spacebars is pressed and released
            if (e.Key == Key.Space && jumping == false && Canvas.GetTop(player) > 260)
            {
                // Assigns values to variables when the jump action is triggerd
                jumping = true;
                force = 15;
                speed = -12;

                // Uses the second player sprite when the jump action is triggerd
                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_02.gif"));
            }
        }

        private void StartGame()
            /// <summary>
            /// Decides how to game should act when it first runs
            /// </summary>
        {
            // Default locations of all the sprites
            Canvas.SetLeft(background, 0);
            Canvas.SetLeft(background2, 1262);
            
            Canvas.SetLeft(player, 110);
            Canvas.SetTop(player, 140);

            Canvas.SetLeft(obstacle, 950);
            Canvas.SetTop(obstacle, 310);

            // Uses the first frame of the run animation described in the Method RunSprite
            RunSprite(1);

            // Uses the linked image as the obstacles
            obstacleSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/obstacle.png"));
            obstacle.Fill = obstacleSprite;

            // Assigns the values for variables at the start of the game
            jumping = false;
            gameOver = false;
            score = 0;

            // Assigns the text of the Label scoreText
            scoreText.Content = "Score: " + score;

            // Starts the gameTimer
            gameTimer.Start();
        }

        private void RunSprite(double i)
            /// <summary>
            /// Decides what image the running animation should use
            /// </summary>
            /// <param name="i"> A double deciding witch frame should be displayed </param>
        {

            switch (i)
            {
                case 1:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_01.gif"));
                    break;

                case 2:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_02.gif"));
                    break;

                case 3:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_03.gif"));
                    break;

                case 4:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_04.gif"));
                    break;

                case 5:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_05.gif"));
                    break;

                case 6:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_06.gif"));
                    break;

                case 7:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_07.gif"));
                    break;

                case 8:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images2/newRunner_08.gif"));
                    break;
            }

            // Fills the player sprite with the linked image of the current case
            player.Fill = playerSprite;

        }
    }
}
