using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace Tent
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class Scenario_2048 : Page
    {
        public const int LEFT = 0;
        public const int UP = 1;
        public const int RIGHT = 2;
        public const int DOWN = 3;
        Scenario_2048_Tile[,] tiles;
        Point start;
        int[,] num;
        int nth = 0;
        bool won = false;

        public Scenario_2048()
        {
            this.InitializeComponent();
            bigmain.ManipulationStarted += _ManipulationStarted;
            bigmain.ManipulationDelta += _ManipulationDelta;
            bigmain.ManipulationCompleted += _ManipulationCompleted;
            this.Loaded += MainPage_Loaded;
        }

        private void _ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) { start = e.Position; }
        private void _ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Point end = e.Position;
            e.Complete();
            if (Math.Abs(end.X - start.X) < 5 && Math.Abs(end.Y - start.Y) < 5)
            {
                return;
            }

            if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
            {
                if (end.X - start.X > 0) { Move(RIGHT); }
                else { Move(LEFT); }
            }
            else
            {
                if (end.Y - start.Y > 0) { Move(DOWN); }
                else { Move(UP); }
            }
        }
        private void _ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) { }

        private void NewGameClicked(object sender, RoutedEventArgs e)
        {
            popup.Visibility = Visibility.Collapsed;
            popup.IsOpen = false;
            NewGame(nth++, false);
        }

        private void NewGameEasyClicked(object sender, RoutedEventArgs e)
        {
            popup.Visibility = Visibility.Collapsed;
            popup.IsOpen = false;
            NewGame(nth++, true);
        }

        private void ReturnClicked(object sender, RoutedEventArgs e)
        {
            popup.Visibility = Visibility.Collapsed;
            popup.IsOpen = false;
        }
        
        private void IfGameOver()
        {
            if (isGameOver())
            {
                Debug.WriteLine("Game Over");

                popupBorder.Width = main.ActualWidth * 0.8;
                popupBorder.Height = main.ActualHeight * 0.8;

                popupImgKelian.Visibility = Visibility.Visible;
                popupImgLaipi.Visibility = Visibility.Collapsed;
                popupImgKelian.MaxWidth = popupBorder.Width;
                popupImgKelian.MaxHeight = popupBorder.Height * 0.8;
                popupText.Text = "已无法合并更多数字\n您的得分为:" + score.Text + ",最高分为:" + best.Text + ".";

                popup.HorizontalOffset = (main.ActualWidth - popupBorder.Width) / 2;
                popup.VerticalOffset = (main.ActualHeight - popupBorder.Height) / 2;

                popup.Visibility = Visibility.Visible;
                popup.IsOpen = true;
            }

        }
        private bool isGameOver()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    Debug.WriteLine(i + " " + j);
                    if (tiles[i, j].Number == 0) return false;
                    if (i - 1 >= 0)
                        if (tiles[i, j].Number == tiles[i - 1, j].Number)
                            return false;
                    if (j - 1 >= 0)
                        if (tiles[i, j].Number == tiles[i, j - 1].Number) return false;
                    if (j + 1 < 4)
                        if (tiles[i, j].Number == tiles[i, j + 1].Number) return false;
                    if (i + 1 < 4)
                        if (tiles[i, j].Number == tiles[i + 1, j].Number) return false;
                }
            return true;
        }

        private void IfWon()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (!won && (tiles[i, j].Number == 2048))
                    {
                        won = true;

                        Debug.WriteLine("Game Won");

                        popupBorder.Width = main.ActualWidth * 0.8;
                        popupBorder.Height = main.ActualHeight * 0.8;

                        popupImgKelian.Visibility = Visibility.Collapsed;
                        popupImgLaipi.Visibility = Visibility.Visible;
                        popupImgLaipi.MaxWidth = popupBorder.Width;
                        popupImgLaipi.MaxHeight = popupBorder.Height * 0.8;
                        popupText.Text =
                            "目标达成！乖乖沈老师！\n您的得分为:" + score.Text + ",最高分为:" + best.Text + ".";          

                        popup.HorizontalOffset = (main.ActualWidth - popupBorder.Width) / 2;
                        popup.VerticalOffset = (main.ActualHeight - popupBorder.Height) / 2;

                        popup.Visibility = Visibility.Visible;
                        popup.IsOpen = true;
                    }
                }
            }
        }

        private void Move(int direction)
        {
            IfGameOver();
            Debug.WriteLine(direction);

            // Initialize tiles
            num = new int[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    num[i, j] = 0;

            // Stats
            bool hasBlankMove = ClearBlank(direction);
            int i_score = int.Parse(score.Text);
            bool hasAddMove = AddNumber(direction, ref i_score);
            score.Text = i_score.ToString();
            if (i_score > int.Parse(best.Text)) { best.Text = i_score.ToString(); }

            if (hasAddMove | hasBlankMove)
            {
                // Generate a new number
                Random random = new Random();
                int a = (random.Next(15) == 0) ? 4 : 2;

                // Generate the coordinate for the new tile
                int x = 0, y = 0;
                do
                {
                    x = random.Next(4);
                    y = random.Next(4);
                } while (tiles[x, y].Number != 0);
                tiles[x, y].Number = a;

                // Animate the tile
                tiles[x, y].Appear();
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Debug.Write(tiles[i, j].Number + " ");

                    // Update for saving
                    UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("score = " + score.Text + " best = " + best.Text);
            UserData.CurrentInstance.Score = score.Text;
            UserData.CurrentInstance.Best = best.Text;

            IfWon();
        }

        private bool ClearBlank(int o)
        {
            bool hasBlankMove = false;
            if (o == LEFT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 0;
                    for (int j = 0; j < 4; j++)
                        if (0 != tiles[i, j].Number)
                            num[i, t++] = tiles[i, j].Number;
                }
            }

            if (o == RIGHT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 3;
                    for (int j = 3; j >= 0; j--)
                        if (0 != tiles[i, j].Number)
                            num[i, t--] = tiles[i, j].Number;

                }
            }

            if (o == UP)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                        if (0 != tiles[i, j].Number)
                            num[t++, j] = tiles[i, j].Number;
                }
            }

            if (o == DOWN)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 3;
                    for (int i = 3; i >= 0; i--)
                        if (0 != tiles[i, j].Number)
                            num[t--, j] = tiles[i, j].Number;
                }
            }

            // Update numbers
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // The number differs before and after the move, something has moved.
                    if (tiles[i, j].Number != num[i, j])
                        hasBlankMove = true;
                    tiles[i, j].Number = num[i, j];
                }
            }

            return hasBlankMove;
        }

        private bool AddNumber(int o, ref int s)
        {
            bool hasAddMove = false;
            if (o == LEFT)
            {
                Debug.WriteLine("←");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (tiles[i, j].Number == tiles[i, j + 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            // Animate zoom
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j + 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j + 1; t < 3; t++)
                            {
                                tiles[i, t].Number = tiles[i, t + 1].Number;
                            }
                            tiles[i, 3].Number = 0;
                        }
                    }
                }
            }

            if (o == RIGHT)
            {
                Debug.WriteLine("→");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        if (tiles[i, j].Number == tiles[i, j - 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j - 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j - 1; t > 0; t--)
                            {
                                tiles[i, t].Number = tiles[i, t - 1].Number;
                            }
                            tiles[i, 0].Number = 0;
                        }
                    }
                }
            }

            if (o == UP)
            {
                Debug.WriteLine("↑");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (tiles[i, j].Number == tiles[i + 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i + 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i + 1; t < 3; t++)
                            {
                                tiles[t, j].Number = tiles[t + 1, j].Number;
                            }
                            tiles[3, j].Number = 0;
                        }
                    }
                }
            }

            if (o == DOWN)
            {
                Debug.WriteLine("↓");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 3; i > 0; i--)
                    {
                        if (tiles[i, j].Number == tiles[i - 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i - 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i - 1; t > 0; t--)
                            {
                                tiles[t, j].Number = tiles[t - 1, j].Number;
                            }
                            tiles[0, j].Number = 0;
                        }
                    }
                }
            }

            return hasAddMove;
        }

        private void MoveAnimate(Scenario_2048_Tile tile, int direction, int distance)
        {
            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = 0;
            moveTransform.Y = 0;

            tile.RenderTransform = moveTransform;
            Duration duration = new Duration(TimeSpan.FromSeconds(1));
            
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            DoubleAnimation myDoubleAnimationY = new DoubleAnimation();
            
            myDoubleAnimationX.Duration = duration;
            myDoubleAnimationY.Duration = duration;
            
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            
            moveStoryboard.Children.Add(myDoubleAnimationX);
            moveStoryboard.Children.Add(myDoubleAnimationY);
            
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            Storyboard.SetTarget(myDoubleAnimationY, moveTransform);
            
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            Storyboard.SetTargetProperty(myDoubleAnimationY, "Y");
            
            switch (direction)
            {
                case LEFT: myDoubleAnimationX.To = -tile.ActualWidth * distance; break;
                case UP: myDoubleAnimationY.To = -tile.ActualHeight * distance; break;
                case RIGHT: myDoubleAnimationX.To = tile.ActualWidth * distance; break;
                case DOWN: myDoubleAnimationY.To = tile.ActualHeight * distance; break;
            }
            

            tile.SetValue(Canvas.ZIndexProperty, 999);
            moveStoryboard.Begin();
        }

        private void NewGame(int times, bool easyMode)
        {
            if (times == 0)
            {
                score.Text = UserData.CurrentInstance.Score;
                best.Text = UserData.CurrentInstance.Best;
                bool all0 = true;
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        tiles[i, j].Number = UserData.CurrentInstance.Num[i, j];
                        if (tiles[i, j].Number != 0) all0 = false;
                    }
                if (all0) Init(easyMode);
            }
            else
            {
                Init(easyMode);
            }
        }

        private void Init(bool easyMode)
        {
            won = false;

            foreach (Scenario_2048_Tile t in tiles)
                t.Number = 0;
            Random random = new Random();

            int a;
            if (easyMode)
            {
                a = 1024;
            }
            else
            {
                a = (random.Next(0, 10) != 0) ? 2 : 4;
            }
            
            int x1 = random.Next(0, 4),
                y1 = random.Next(0, 4);
            int x2, y2;
            do
            {
                x2 = random.Next(0, 4);
                y2 = random.Next(0, 4);
            } while (x1 == x2 && y1 == y2);

            tiles[x1, y1].Number = 2;
            tiles[x2, y2].Number = a;
            tiles[x1, y1].Appear();
            tiles[x2, y2].Appear();
            score.Text = "0";

            // TODO: remove
            /*tiles[0, 0].Number = 1024;
            tiles[0, 1].Number = 1024;
            tiles[0, 0].Appear();
            tiles[0, 1].Appear();*/
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double height = root.ActualHeight;
            double width = root.ActualWidth;
            
            double up = 0.25, down = 0.75, left = 0.4, right = 0.6, logomargin = 10;

            if (ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Portrait)
            {
                root.Orientation = Orientation.Vertical;
                header.Orientation = Orientation.Horizontal;

                header.Height = height * up;
                footer.Height = height * down;
                header.Width = width;
                footer.Width = width;

                logo.Width = width * left - logomargin;
                logo.Height = height * up - logomargin;
                logo.Margin = new Thickness(logomargin);

                headerRight.Width = width * right;
                headerRight.Height = height * up;

                bigmain.Width = (width > height * down) ? (height * down) : (width);
                bigmain.Height = (width > height * down) ? (height * down) : (width);
                bigmain.Margin = new Thickness((width - bigmain.Width) / 2,
                    (height * down - bigmain.Height) / 2,
                    (width - bigmain.Width) / 2,
                    (height * down - bigmain.Height) / 2);
            }
            else
            {
                root.Orientation = Orientation.Horizontal;
                header.Orientation = Orientation.Vertical;

                header.Width = width * up;
                footer.Width = width * down;
                header.Height = height;
                footer.Height = height;

                logo.Height = height * left - logomargin;
                logo.Width = width * up - logomargin;
                logo.Margin = new Thickness(logomargin);

                headerRight.Height = height * right;
                headerRight.Width = width * up;

                bigmain.Height = (height > width * down) ? (width * down) : (height);
                bigmain.Width = (height > width * down) ? (width * down) : (height);
                bigmain.Margin = new Thickness((width * down - bigmain.Width) / 2,
                    (height - bigmain.Height) / 2,
                    (width * down - bigmain.Width) / 2,
                    (height - bigmain.Height) / 2);
            }
            headergrid.Width = headerRight.Width > headerRight.Height ? headerRight.Height : headerRight.Width;
            headergrid.Height = headerRight.Width > headerRight.Height ? headerRight.Height : headerRight.Width;
            headergrid.Margin = new Thickness((headerRight.Width - headergrid.Width) / 2,
                (headerRight.Height - headergrid.Height) / 2,
                (headerRight.Width - headergrid.Width) / 2,
                (headerRight.Height - headergrid.Height) / 2);
        }

        private void easymodegrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            easymodetxt.Foreground = new SolidColorBrush(Colors.White);
            
            Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
            easymodegrid.PointerExited += (o, e1) =>
            {
                try { easymodetxt.Foreground = Application.Current.Resources["txt"] as SolidColorBrush; }
                catch (Exception) { easymodetxt.Foreground = new SolidColorBrush(Colors.Black); }
                CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            };
        }
        private void newgrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            newtxt.Foreground = new SolidColorBrush(Colors.White);
            CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
            newgrid.PointerExited += (o, e1) =>
            {
                try { newtxt.Foreground = Application.Current.Resources["txt"] as SolidColorBrush; }
                catch (Exception) { newtxt.Foreground = new SolidColorBrush(Colors.Black); }
                CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            };
        }

        private void easymodegrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NewGame(nth++, true);
        }

        private void newgrid_Tapped(object sender, TappedRoutedEventArgs e) { NewGame(nth++, false); }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyUp;

            // Set focus to the root element
            easymodetxt.Focus(FocusState.Keyboard);
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            Debug.WriteLine(args.VirtualKey);

            if (args.VirtualKey == Windows.System.VirtualKey.A || args.VirtualKey == Windows.System.VirtualKey.Left || args.VirtualKey == Windows.System.VirtualKey.H)
            {
                Move(LEFT);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.W || args.VirtualKey == Windows.System.VirtualKey.Up || args.VirtualKey == Windows.System.VirtualKey.K)
            {
                Move(UP);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.D || args.VirtualKey == Windows.System.VirtualKey.Right || args.VirtualKey == Windows.System.VirtualKey.L)
            {
                Move(RIGHT);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.S || args.VirtualKey == Windows.System.VirtualKey.Down || args.VirtualKey == Windows.System.VirtualKey.J)
            {
                Move(DOWN);
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            tiles = new Scenario_2048_Tile[4, 4];
            tiles[0, 0] = b00;
            tiles[0, 1] = b01;
            tiles[0, 2] = b02;
            tiles[0, 3] = b03;
            tiles[1, 0] = b10;
            tiles[1, 1] = b11;
            tiles[1, 2] = b12;
            tiles[1, 3] = b13;
            tiles[2, 0] = b20;
            tiles[2, 1] = b21;
            tiles[2, 2] = b22;
            tiles[2, 3] = b23;
            tiles[3, 0] = b30;
            tiles[3, 1] = b31;
            tiles[3, 2] = b32;
            tiles[3, 3] = b33;
            NewGame(nth++, false);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;

            Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;

            base.OnNavigatedFrom(e);
        }
    }

    public class UserData
    {
        private static UserData _userData = null;
        static UserData() { _userData = new UserData(); }
        public static UserData CurrentInstance { get { return _userData; } }

        string score = "0", best = "0"; int[,] num = new int[4, 4];
        public string Score { get { return score; } set { score = value; } }
        public string Best { get { return best; } set { best = value; } }
        public int[,] Num { get { return num; } set { num = value; } }

        byte[,] accent = new byte[2, 4];
        public byte[,] AccentAndBg { get { return accent; } set { accent = value; } }
        public int Nth { get; set; }

        public static void Load()
        {
            Debug.WriteLine("正加载数据...");
            var rs = ApplicationData.Current.RoamingSettings;
            object v = null;
            if (rs.Values.TryGetValue("score", out v)) { CurrentInstance.Score = (string)v; } else { CurrentInstance.Score = "0"; }
            if (rs.Values.TryGetValue("best", out v)) { CurrentInstance.Best = (string)v; } else { CurrentInstance.Best = "0"; }
            if (rs.Values.TryGetValue("nth", out v)) { CurrentInstance.Nth = (int)v; } else { CurrentInstance.Nth = 0; }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (rs.Values.TryGetValue(i + "" + j, out v)) { CurrentInstance.Num[i, j] = (int)v; }
                    else { CurrentInstance.Num[i, j] = 0; }
                    if (i < 2)
                    {
                        if (rs.Values.TryGetValue(i + "a" + j, out v)) { CurrentInstance.AccentAndBg[i, j] = (byte)v; }

                    }
                }
            }

          (Application.Current.Resources["accent"] as SolidColorBrush).Color = Color.FromArgb(
                CurrentInstance.AccentAndBg[0, 0],
                CurrentInstance.AccentAndBg[0, 1],
                CurrentInstance.AccentAndBg[0, 2],
                CurrentInstance.AccentAndBg[0, 3]);
            (Application.Current.Resources["bg"] as SolidColorBrush).Color = Color.FromArgb(
                CurrentInstance.AccentAndBg[1, 0],
                CurrentInstance.AccentAndBg[1, 1],
                CurrentInstance.AccentAndBg[1, 2],
                CurrentInstance.AccentAndBg[1, 3]);

            if (!rs.Values.TryGetValue("0a0", out v))
            {
                (Application.Current.Resources["accent"] as SolidColorBrush).Color = (Color)Application.Current.Resources["SystemAccentColor"];
                (Application.Current.Resources["accent"] as SolidColorBrush).Color = Colors.Gray;
            }

            byte txtr = 0, txtg = 0, txtb = 0;
            if (rs.Values.TryGetValue("txtr", out v)) { txtr = (byte)v; }
            if (rs.Values.TryGetValue("txtg", out v)) { txtg = (byte)v; }
            if (rs.Values.TryGetValue("txtb", out v)) { txtb = (byte)v; }
            (Application.Current.Resources["txt"] as SolidColorBrush).Color = Color.FromArgb(255, txtr, txtg, txtb);

        }

        public static void Save()
        {
            Debug.WriteLine("正保存数据");
            var rs = ApplicationData.Current.RoamingSettings;
            rs.Values["score"] = CurrentInstance.Score;
            rs.Values["best"] = CurrentInstance.best;
            CurrentInstance.AccentAndBg[0, 0] = (Application.Current.Resources["accent"] as SolidColorBrush).Color.A;
            CurrentInstance.AccentAndBg[0, 1] = (Application.Current.Resources["accent"] as SolidColorBrush).Color.R;
            CurrentInstance.AccentAndBg[0, 2] = (Application.Current.Resources["accent"] as SolidColorBrush).Color.G;
            CurrentInstance.AccentAndBg[0, 3] = (Application.Current.Resources["accent"] as SolidColorBrush).Color.B;
            CurrentInstance.AccentAndBg[1, 0] = (Application.Current.Resources["bg"] as SolidColorBrush).Color.A;
            CurrentInstance.AccentAndBg[1, 1] = (Application.Current.Resources["bg"] as SolidColorBrush).Color.R;
            CurrentInstance.AccentAndBg[1, 2] = (Application.Current.Resources["bg"] as SolidColorBrush).Color.G;
            CurrentInstance.AccentAndBg[1, 3] = (Application.Current.Resources["bg"] as SolidColorBrush).Color.B;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rs.Values[i + "" + j] = CurrentInstance.Num[i, j];
                    if (i < 2)
                    {
                        rs.Values[i + "a" + j] = CurrentInstance.AccentAndBg[i, j];
                    }
                }
            }
            rs.Values["txtr"] = (Application.Current.Resources["txt"] as SolidColorBrush).Color.R;
            rs.Values["txtg"] = (Application.Current.Resources["txt"] as SolidColorBrush).Color.G;
            rs.Values["txtb"] = (Application.Current.Resources["txt"] as SolidColorBrush).Color.B;
            rs.Values["nth"] = CurrentInstance.Nth;
        }
    }

}
