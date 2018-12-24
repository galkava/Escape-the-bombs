using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;

namespace EscapeTheBombs
{
    public partial class MainWindow : Window
    {
        public string imagePath;
        public int bombArraySize = 10;
        public int jumpsCounter = 3;
        public int score;
        public int bombsDestroyed = 0;
        public Direction direction;

        public bool gameOver;
        public bool isRunning;
        public bool isOnPause;
        public bool newGame;
        public bool howToPlayIsShown;

        System.Text.UTF7Encoding encoding = new System.Text.UTF7Encoding();

        Random ran = new Random();

        public Player tank;
        public Bomb[] bombArray;
        public Explosion explosion;
        public int[] bombSpeedArray;

        DispatcherTimer countDownTimer;
        TimeSpan time = TimeSpan.FromSeconds(60);

        public MainWindow()
        {
            InitializeComponent();

            KeyDown += new KeyEventHandler(OnKeyDownHandler);

            DispatcherTimer bombTimer = new DispatcherTimer();
            bombTimer.Tick += Bombs_Timer_Tick;
            bombTimer.Interval = TimeSpan.FromSeconds(0.05);
            bombTimer.Start();

            countDownTimer = new DispatcherTimer(new TimeSpan(00, 0, 1), DispatcherPriority.Normal, delegate
            {
                //bombArraySizeTextBlock.Text = bombArraySize.ToString() + "\nisRunning: " + isRunning.ToString() + "\nisOnPause: " + isOnPause.ToString();

                if (explosion != null)
                {
                    RemoveExplosionImage();
                }

                if (!howToPlayIsShown)
                {
                    MessageBoxResult result = ShowHowToPlayPopUp();
                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            howToPlayIsShown = true;
                            break;
                    }

                }
                if (isRunning && !gameOver && !isOnPause)
                {
                    time = time.Add(TimeSpan.FromSeconds(-1));

                    if (time == TimeSpan.Zero)
                    {
                        gameOver = true;
                        MessagesTxtBlock.Text = "GAME OVER!\nThe time expired";
                        time = TimeSpan.FromSeconds(60);
                    }

                    TimeRemainingTxtBlock.Text = time.ToString();

                    if (explosion != null)
                    {
                        RemoveExplosionImage();
                    }
                }
            }, Application.Current.Dispatcher);

            countDownTimer.Start();

            bombSpeedArray = new int[bombArraySize];
            Random randomSpeed = new Random();
            for (int i = 0; i < bombArraySize; i++)
            {
                bombSpeedArray[i] = randomSpeed.Next(2, 4);
            }

            CreateTank();
            CreateBombs(bombArraySize);
        }

        private void RemoveExplosionImage() => playArea.Children.Remove(explosion.PlayerImage);

        private MessageBoxResult ShowHowToPlayPopUp() => MessageBox.Show(File.ReadAllText("..\\..\\Files\\HowToPlay.txt", encoding), "How to play", MessageBoxButton.OK);

        public void CreateTank()
        {
            tank = new TankLeft(ran.Next(5, (int)playArea.Width - 40), ran.Next(5, (int)playArea.Height - 40), imagePath);
            Canvas.SetTop(tank.PlayerImage, tank._y);
            Canvas.SetLeft(tank.PlayerImage, tank._x);
            playArea.Children.Add(tank.PlayerImage);
        }

        public void CreateBombs(int bombArraySize)
        {
            if (newGame)
            {
                foreach (Player itemPlayer in bombArray)
                {
                    playArea.Children.Remove(itemPlayer.PlayerImage);
                }
            }

            bombArray = new Bomb[bombArraySize];
            for (int i = 0; i < bombArraySize; i++)
            {
                bombArray[i] = new Bomb(ran.Next(5, (int)playArea.Width - 40), ran.Next(5, (int)playArea.Height - 40), imagePath);
                bombArray[i].CheckWhetherWithinTheCanvas();
            }

            foreach (Player itemPlayer in bombArray)
            {
                Canvas.SetTop(itemPlayer.PlayerImage, itemPlayer._y);
                Canvas.SetLeft(itemPlayer.PlayerImage, itemPlayer._x);
                playArea.Children.Add(itemPlayer.PlayerImage);
            }
        }

        public void CreateExplosion(int x, int y)
        {
            explosion = new Explosion(x, y, imagePath);
            Canvas.SetTop(explosion.PlayerImage, y);
            Canvas.SetLeft(explosion.PlayerImage, x);
            playArea.Children.Add(explosion.PlayerImage);
        }

        private void Bombs_Timer_Tick(object sender, object e)
        {
            if (isRunning && !gameOver && !isOnPause)
            {
                for (int i = 0; i < bombArraySize; i++)
                {
                    if (bombArray[i]._x > tank._x)
                        bombArray[i]._x -= bombSpeedArray[i];
                    else if (bombArray[i]._x < tank._x)
                        bombArray[i]._x += bombSpeedArray[i];

                    if (bombArray[i]._y > tank._y)
                        bombArray[i]._y -= bombSpeedArray[i];
                    else if (bombArray[i]._y < tank._y)
                        bombArray[i]._y += bombSpeedArray[i];

                    bombArray[i].CheckWhetherWithinTheCanvas();

                    Canvas.SetTop(bombArray[i].PlayerImage, bombArray[i]._y);
                    Canvas.SetLeft(bombArray[i].PlayerImage, bombArray[i]._x);

                    CheckTankVSBombPosition();
                    CheckBombsCollision();
                }
            }
        }

        private void CheckBombsCollision()
        {
            int checkXbomb;
            int checkYbomb;

            for (int i = 0; i < bombArraySize; i++)
            {
                for (int j = i + 1; j < bombArraySize; j++)
                {
                    if (bombArray[i]._x <= bombArray[j]._x)
                    {
                        checkXbomb = bombArray[j]._x - bombArray[i]._x;
                    }

                    else
                    {
                        checkXbomb = bombArray[i]._x - bombArray[j]._x;
                    }

                    if (bombArray[i]._y <= bombArray[j]._y)
                    {
                        checkYbomb = bombArray[j]._y - bombArray[i]._y;
                    }

                    else
                    {
                        checkYbomb = bombArray[i]._y - bombArray[j]._y;
                    }

                    if ((checkXbomb < 21) && (checkYbomb < 21))
                    {
                        score += 100;
                        ScoreTxtBlock.Text = score.ToString();
                        bombsDestroyed++;
                        BombsDestroedTxtBlock.Text = bombsDestroyed.ToString();

                        CreateExplosion(bombArray[j]._x, bombArray[j]._y);
                        playArea.Children.Remove(bombArray[j].PlayerImage);

                        var bombArrayList = bombArray.ToList();
                        bombArrayList.Remove(bombArray[j]);
                        bombArraySize--;
                        bombArray = bombArrayList.ToArray();

                        if (bombArray.Length == 1)
                        {
                            gameOver = true;
                            MessagesTxtBlock.Text = "Congratulations!\nYou won!\nYour score is: " + score.ToString();
                            time = TimeSpan.FromSeconds(60);
                        }
                    }
                }
            }
        }

        private void CheckTankVSBombPosition()
        {
            int checkX;
            int checkY;

            for (int i = 0; i < bombArraySize; i++)
            {
                if (tank._x <= bombArray[i]._x)
                {
                    checkX = bombArray[i]._x - tank._x;
                }

                else
                {
                    checkX = tank._x - bombArray[i]._x;
                }

                if (tank._y <= bombArray[i]._y)
                {
                    checkY = bombArray[i]._y - tank._y;
                }

                else
                {
                    checkY = tank._y - bombArray[i]._y;
                }

                if ((checkX < 31) && (checkY < 31))
                {
                    playArea.Children.Remove(bombArray[i].PlayerImage);
                    DestroyTheTank();
                }
            }
        }

        private void DestroyTheTank()
        {
            playArea.Children.Remove(tank.PlayerImage);
            CreateExplosion(tank._x, tank._y);
            gameOver = true;
            time = TimeSpan.FromSeconds(60);
            MessagesTxtBlock.Text = "GAME OVER!\nYou lost";
        }

        // KEY EVENTS

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (!gameOver && !isOnPause)
            {
                if (e.Key == Key.Up)
                {
                    if (direction != Direction.Up)
                    {
                        if (direction == Direction.Right)
                        {
                            playArea.Children.Remove(tank.PlayerImage);
                            tank = new TankUpFromRight(tank._x, tank._y, imagePath);
                        }
                        else
                        {
                            playArea.Children.Remove(tank.PlayerImage);
                            tank = new TankUp(tank._x, tank._y, imagePath);
                        }

                        direction = Direction.Up;
                        playArea.Children.Remove(tank.PlayerImage);
                        tank._y = tank.Move(Direction.Up);

                        Canvas.SetTop(tank.PlayerImage, tank._y);
                        Canvas.SetLeft(tank.PlayerImage, tank._x);
                        playArea.Children.Add(tank.PlayerImage);
                    }
                    else
                    {
                        tank._y = tank.Move(Direction.Up);
                        Canvas.SetTop(tank.PlayerImage, tank._y);
                    }
                }

                else if (e.Key == Key.Down)
                {
                    if (direction != Direction.Down)
                    {
                        if (direction == Direction.Left)
                        {
                            playArea.Children.Remove(tank.PlayerImage);
                            tank = new TankDownFromLeft(tank._x, tank._y, imagePath);
                        }
                        else
                        {
                            playArea.Children.Remove(tank.PlayerImage);
                            tank = new TankDown(tank._x, tank._y, imagePath);
                        }

                        direction = Direction.Down;

                        tank._y = tank.Move(Direction.Down);

                        Canvas.SetTop(tank.PlayerImage, tank._y);
                        Canvas.SetLeft(tank.PlayerImage, tank._x);
                        playArea.Children.Add(tank.PlayerImage);
                    }
                    else
                    {
                        tank._y = tank.Move(Direction.Down);
                        Canvas.SetTop(tank.PlayerImage, tank._y);
                    }
                }

                else if (e.Key == Key.Left)
                {
                    if (direction != Direction.Left)
                    {
                        direction = Direction.Left;
                        playArea.Children.Remove(tank.PlayerImage);
                        tank = new TankLeft(tank._x, tank._y, imagePath);
                        tank._x = tank.Move(Direction.Left);

                        Canvas.SetTop(tank.PlayerImage, tank._y);
                        Canvas.SetLeft(tank.PlayerImage, tank._x);
                        playArea.Children.Add(tank.PlayerImage);

                    }
                    else
                    {
                        tank._x = tank.Move(Direction.Left);
                        Canvas.SetLeft(tank.PlayerImage, tank._x);
                    }
                }

                else if (e.Key == Key.Right)
                {
                    if (direction != Direction.Right)
                    {
                        direction = Direction.Right;
                        playArea.Children.Remove(tank.PlayerImage);
                        tank = new TankRight(tank._x, tank._y, imagePath);
                        tank._x = tank.Move(Direction.Right);

                        Canvas.SetTop(tank.PlayerImage, tank._y);
                        Canvas.SetLeft(tank.PlayerImage, tank._x);
                        playArea.Children.Add(tank.PlayerImage);
                    }
                    else
                    {
                        tank._x = tank.Move(Direction.Right);
                        Canvas.SetLeft(tank.PlayerImage, tank._x);
                    }
                }

                else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                {
                    if (jumpsCounter != 0 && isRunning)
                    {
                        tank._x = tank.Escape('x');
                        tank._y = tank.Escape('y');
                        Canvas.SetTop(tank.PlayerImage, tank._y);
                        Canvas.SetLeft(tank.PlayerImage, tank._x);
                        jumpsCounter -= 1;
                        JumpsRemainingTxtBlock.Text = jumpsCounter.ToString();

                        score -= 20;
                        ScoreTxtBlock.Text = score.ToString();
                    }
                }
            }
        }

        // CLICK EVENTS

        private void StartPauseResumeButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonState = StartPauseResumeButton.Content.ToString();

            switch (buttonState)
            {
                case "START":
                    isRunning = true;
                    isOnPause = false;
                    gameOver = false;
                    StartPauseResumeButton.Content = "PAUSE";
                    break;

                case "PAUSE":
                    isRunning = false;
                    isOnPause = true;
                    MessagesTxtBlock.Text = "Paused.\nClick RESUME to continue the game";
                    StartPauseResumeButton.Content = "RESUME";
                    break;

                case "RESUME":
                    isRunning = true;
                    isOnPause = false;
                    MessagesTxtBlock.Text = "";
                    StartPauseResumeButton.Content = "PAUSE";
                    break;

                default:
                    break;
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            isOnPause = true;
            isRunning = false;
            StartPauseResumeButton.Content = "Waiting...";

            if (gameOver)
            {
                StartNewGame(10, 3, 0, 0);
                gameOver = false;
            }

            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure? This will delete your current game", "New game", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        bombArraySize = 10;
                        StartNewGame(bombArraySize, 3, 0, 0);
                        time = TimeSpan.FromSeconds(60);
                        break;
                    case MessageBoxResult.No:
                        isOnPause = false;
                        isRunning = true;
                        StartPauseResumeButton.Content = "PAUSE";
                        break;
                }
            }  
        }

        private void StartNewGame(int bombArraySize, int jumpsCounter, int bombsDestroyed, int score)
        {
            isOnPause = false;
            gameOver = false;
            newGame = true;
            playArea.Children.Clear();
            ScoreTxtBlock.Text = score.ToString();
            JumpsRemainingTxtBlock.Text = jumpsCounter.ToString();
            BombsDestroedTxtBlock.Text = bombsDestroyed.ToString();
            MessagesTxtBlock.Text = "";
            TimeRemainingTxtBlock.Text = "00:01:00";
            StartPauseResumeButton.Content = "START";
            CreateTank();
            CreateBombs(bombArraySize);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            isOnPause = true;
            MessageBoxResult result = MessageBox.Show(File.ReadAllText("..\\..\\Files\\About.txt", encoding), "About", MessageBoxButton.OK);

            switch (result)
            {
                case MessageBoxResult.OK:
                    isOnPause = false;
                    break;
            }

        }

        private void HowToPlayButton_Click(object sender, RoutedEventArgs e)
        {
            isOnPause = true;

            MessageBoxResult result = ShowHowToPlayPopUp();

            switch (result)
            {
                case MessageBoxResult.OK:
                    isOnPause = false;
                    break;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                isOnPause = true;

                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit? This will delete your current game", "", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        System.Windows.Application.Current.Shutdown();
                        break;
                    case MessageBoxResult.No:
                        isOnPause = false;
                        break;
                }
            }

            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        System.Windows.Application.Current.Shutdown();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
    }
}