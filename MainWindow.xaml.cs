using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Chess_4_with_Framework.Pieces;
using Color = System.Windows.Media.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Hollander.Common.Logging;
using Hollander.Common.Logging.Log4net;
using Hollander.IO.Plc;
using Hollander.IO.Plc.OpcUa;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Windows.Documents;
using Opc.Ua;

namespace Chess_4_with_Framework
{
    /// <summary>
    /// Interaction logic foCanvas_OnMouseWheel   /// </summary>
    public partial class MainWindow : Window
    {
        public const int TileSize = 100;
        private Board board = new Board();
        private bool _moving;
        private Point _first;
        private Point _second;

        private bool _robotIsDone = true;
        private bool _robotHasStopped = false;
        private bool _withRobot = false;
        private bool test = true;

        public EColor IsPlaying = EColor.White;
        public bool ComputerIsPlaying = true;

        private static readonly ILog _log;
        public static IController Controller = new OpcUaController("OpcUa.config", "opc.tcp://172.27.8.50:4840 - [None:None:Binary]");
        public static IPlcMonitor Monitor;
        private CancellationTokenSource cts = new CancellationTokenSource();

        #region PlcTags

        #endregion

        static MainWindow()
        {
            LogManager.AdapterFactory = new Log4netAdapterFactory();
            var fileInfo = new FileInfo("log4net.config");
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            _log = LogManager.GetLogger<MainWindow>();
        }

        public MainWindow()
        {
            InitializeComponent();
            if (!test)
            {
                _log.DebugStart();
                if (Controller.Connect())
                {
                    Monitor = new OpcUaMonitor(Controller, 100, true);
                }
            }

            DrawBoard();

            if (ComputerIsPlaying)
            {
                ComputerPlays();
            }
        }

        private void DrawBoard()
        {
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Canvas fieldSquare = new Canvas
                    {
                        Background = (x + y) % 2 == 0 ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.DarkGray),
                        Height = TileSize,
                        Width = TileSize,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(TileSize * x, TileSize * y, 0, 0),
                    };

                    Rectangle border = new Rectangle
                    {
                        Stroke = new SolidColorBrush(Colors.Black),
                        StrokeThickness = 1.5,
                        Height = TileSize,
                        Width = TileSize,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness()
                    };

                    fieldSquare.Children.Add(border);

                    Label label = new Label
                    {
                        Content = $"({x},{y})",
                        Height = TileSize,
                        Width = TileSize,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = 30,
                    };

                    //fieldSquare.Children.Add(label);
                    Canvas.Children.Add(fieldSquare);
                }
            }

            Rectangle leftStorage = new Rectangle
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 4,
                Height = TileSize * 8 - 2,
                Width = TileSize * 2 - 2,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0)
            };
            Canvas.Children.Add(leftStorage);

            Rectangle rightStorage = new Rectangle
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 4,
                Height = TileSize * 8 - 2,
                Width = TileSize * 2 - 2,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(TileSize * 10, 0, 0, 0)
            };
            Canvas.Children.Add(rightStorage);

            board.Show().ForEach(label => Canvas.Children.Add(label));
            DrawSides();

            Label turnLabel = new Label
            {
                FontSize = 50,
                Content = board.IsDone() ? "" : IsPlaying == EColor.White ? "Whites turn" : "Blacks turn",
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(TileSize * 4.5, TileSize * 8.5, 0, 0)
            };
            Canvas.Children.Add(turnLabel);
        }

        private void DrawSides()
        {
            //for (int x = 2; x < 10; x++)
            for (int x = 0; x < 12; x++)
            {
                Label xAxis = new Label
                {
                    Content = x,
                    //Content = ((char) (95 + x)).ToString(),
                    FontSize = 40,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Height = TileSize,
                    Width = TileSize,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(TileSize * x, TileSize * 8, 0, 0)
                };

                Canvas.Children.Add(xAxis);
            }

            for (int y = 0; y < 8; y++)
            {
                Label yAxis = new Label
                {
                    Content = y,// + 1,
                    FontSize = 40,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Height = TileSize,
                    Width = 85,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(TileSize * 12 + 15, TileSize * y, 0, 0)
                };
                Canvas.Children.Add(yAxis);
            }
        }

        private void DrawHighLight(Piece piece, Point target)
        {
            Color color = new Color();

            if (board.GetPieceAt(target) == null)
            {
                color = Colors.Green;
            }
            else if (board.GetPieceAt(target) == piece)
            {
                color = Colors.Blue;
            }
            else if (board.GetPieceAt(target) != null)
            {
                color = Colors.Red;
            }

            Rectangle rectangle = new Rectangle
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 4,
                Height = TileSize - 10,
                Width = TileSize - 10,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(TileSize * target.X + 5, TileSize * target.Y + 5, 0, 0)
            };

            Canvas.Children.Add(rectangle);
        }

        private async void Canvas_OnLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_robotIsDone && !ComputerIsPlaying)
            {
                if (!board.Filled)
                {
                    if (!_withRobot)
                    {
                        await FillBoardWithPieces();
                    }
                    else
                    {
                        board.FillBoard();
                    }
                    
                    DrawBoard();
                    return;
                }

                var mousePoint = e.GetPosition(this);

                if (_first.IsEmpty && !_moving)
                {
                    Reset();
                    _first.X = (int) Math.Floor(mousePoint.X / TileSize);
                    _first.Y = (int) Math.Floor(mousePoint.Y / TileSize);
                    Piece piece = board.GetPieceAt(_first);

                    if (piece != null && piece.Color == IsPlaying)
                    {
                        List<Point> moves = piece.GenerateMoves(board);
                        _moving = moves.Count != 1;
                        moves.ForEach(move => DrawHighLight(piece, move));
                    }
                    else
                    {
                        Reset();
                    }
                }
                else
                {
                    _second = new Point((int) Math.Floor(mousePoint.X / TileSize), (int) Math.Floor(mousePoint.Y / TileSize));
                    Piece piece = board.GetPieceAt(_first);
                    List<Point> moves = piece.GenerateMoves(board);
                    foreach (var move in moves.Where(move => move.X == _second.X && move.Y == _second.Y))
                    {
                        if (_second.X != _first.X || _second.Y != _first.Y)
                        {
                            if (!_withRobot) //change!!!!!!!!!!!!!!!!!!!!!!!!!
                            {
                                var attacking = board.GetPieceAt(_second);
                                if (attacking != null)
                                {
                                    await BlockUser(attacking.Pos, attacking.ReturnToStorage(attacking, board, true));
                                }

                                await BlockUser(_first, _second);

                                //if (_robotHasStopped)
                                //{
                                //    Reset();
                                //    break;
                                //}
                            }

                            board.Move(_first, _second);
                            Console.WriteLine("??");
                            IsPlaying = IsPlaying == EColor.White ? EColor.Black : EColor.White;
                            Reset();
                            if (board.IsDone())
                            {
                                await Endgame();
                            }
                        }
                    }
                }
            }
        }

        private async Task Endgame()
        {
            WaitingCanvas.Visibility = Visibility.Visible;
            string text = IsPlaying == EColor.Black ? "White" : "Black";
            TextBox.Text = $"{text} has won";
            Reset();
            await Task.Run(() => Thread.Sleep(3000));
            await Task.Run(Console.ReadLine);

            TextBox.Text = $"Removing remaining pieces off the board\n Please wait";

            List<Piece> pieces = board.PiecesOnTheBoard();
            foreach (var piece in pieces)
            {
                await SendPoints(piece.Pos, piece.ReturnToStorage(piece, board, true), cts.Token);
                board.Move(piece.Pos, piece.ReturnToStorage(piece, board, false));
                Reset();
            }

            board = new Board();
            WaitingCanvas.Visibility = Visibility.Collapsed;
            IsPlaying = EColor.White;
            board.Filled = false;
            Reset();
            if (ComputerIsPlaying)
            {
                ComputerPlays();
            }
        }

        private void Reset()
        {
            _moving = false;
            _first = new Point();
            _second = new Point();
            Canvas.Children.Clear();
            DrawBoard();
        }

        private void Canvas_OnRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_robotIsDone && !ComputerIsPlaying)
            {
                Reset();
            }
        }

        private async Task FillBoardWithPieces()
        {
            foreach (var piece in board.WhitePieces)
            {
                await BlockUser(piece.Pos, new Point(piece.Pos.X + 2, piece.Pos.Y));
                board.Move(piece.Pos, new Point(piece.Pos.X + 2, piece.Pos.Y));
                Reset();
            }

            foreach (var piece in board.BlackPieces)
            {
                await BlockUser(piece.Pos, new Point(piece.Pos.X - 2, piece.Pos.Y));
                board.Move(piece.Pos, new Point(piece.Pos.X - 2, piece.Pos.Y));
                Reset();
            }
            
            foreach (var storage in board.StorageList)
                storage.Taken = false;
            board.Filled = true;
        }

        private async Task BlockUser(Point first, Point second)
        {
            TextBox.Text = $"Moving robot \n Please Wait";
            WaitingCanvas.Visibility = Visibility.Visible;
            await Task.Run(() => SendPoints(first, second, cts.Token));
            WaitingCanvas.Visibility = Visibility.Collapsed;
        }

        private async Task SendPoints(Point first, Point second, CancellationToken ct)
        {
            if (test)
            {
                await Task.Run(() => Thread.Sleep(500));
                return;
            }

            #region PLCTags

            PlcTag<short> tagFirstPointX = new PlcTag<short>("OpcUaServer;Application.GVL_Chess.G_lrFirstPointX", Controller, Monitor);
            PlcTag<short> tagFirstPointY = new PlcTag<short>("OpcUaServer;Application.GVL_Chess.G_lrFirstPointY", Controller, Monitor);
            PlcTag<short> tagSecondPointX = new PlcTag<short>("OpcUaServer;Application.GVL_Chess.G_lrSecondPointX", Controller, Monitor);
            PlcTag<short> tagSecondPointY = new PlcTag<short>("OpcUaServer;Application.GVL_Chess.G_lrSecondPointY", Controller, Monitor);
            PlcTag<bool> tagRobotIsDone = new PlcTag<bool>("OpcUaServer;Application.GVL_Chess.G_xRobotIsDone", Controller, Monitor);
            PlcTag<bool> tagRobotHasStopped = new PlcTag<bool>("OpcUaServer;Application.GVL_Chess.G_xRobotHasStopped", Controller, Monitor);
            PlcTag<short> tagWatchDog = new PlcTag<short>("OpcUaServer;Application.GVL_Chess.G_iWatchdog", Controller, Monitor);
            PlcTag<bool> tagGenLocation = new PlcTag<bool>("OpcUaServer;Application.GVL_Chess.G_xGenPosition", Controller, Monitor);

            #endregion

            _robotIsDone = false;

            List<Task<bool>> tasks = new List<Task<bool>>()
            {
                tagFirstPointX.WriteValueAsync((short) first.X),
                tagFirstPointY.WriteValueAsync((short) first.Y),
                tagSecondPointX.WriteValueAsync((short) second.X),
                tagSecondPointY.WriteValueAsync((short) second.Y),
            };

            await Task.WhenAll(tasks);

            short resultFx;
            short resultFy;
            short resultSx;
            short resultSy;

            do
            {
                (_, resultFx) = await tagFirstPointX.ReadValueAsync();
                (_, resultFy) = await tagFirstPointY.ReadValueAsync();
                (_, resultSx) = await tagSecondPointX.ReadValueAsync();
                (_, resultSy) = await tagSecondPointY.ReadValueAsync();
            } while (resultFx != first.X || resultFy != first.Y || resultSx != second.X || resultSy != second.Y);

            await tagGenLocation.WriteValueAsync(true);

            tagRobotIsDone.ValueChanged += TagRobotIsDoneOnValueChanged;

            int i = 0;
            while (!_robotIsDone)
            {
                Console.WriteLine(i);
                i++;
                Thread.Sleep(1000);

                /*tagRobotHasStopped.ValueChanged += (sender, e) =>
                {
                    _robotHasStopped = e.NewValue;
                    IsPlaying = IsPlaying == EColor.White ? EColor.Black : EColor.White;
                    Console.WriteLine("Broken");
                };*/

                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("?");
                    break;
                }
            }

            List<Task<bool>> tasks2 = new List<Task<bool>>()
            {
                tagFirstPointX.WriteValueAsync(0),
                tagFirstPointY.WriteValueAsync(0),
                tagSecondPointX.WriteValueAsync(0),
                tagSecondPointY.WriteValueAsync(0),
                tagGenLocation.WriteValueAsync(false),
            };

            await Task.WhenAll(tasks2);
            Console.WriteLine(@"Tasks completed");
        }

        private void TagRobotIsDoneOnValueChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                _robotIsDone = true;
            }

            Console.WriteLine("hello");
        }

        private async void ComputerPlays()
        {
            await Task.Run(Console.ReadLine);
            if (!board.Filled)
            {
                if (!_withRobot)
                {
                    await FillBoardWithPieces();
                }
                else
                {
                    board.FillBoard();
                }
                
                DrawBoard();
                await Task.Run(() => Thread.Sleep(500));
            }

            Random r = new Random();
            while (!board.IsDone() && !_robotHasStopped)
            {
                var move = GetComputerPoints(IsPlaying);
                Console.Write(IsPlaying + " ");
                Console.WriteLine(move);

                if (!_withRobot) //change!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    var attacking = board.GetPieceAt(move.Item2);
                    if (attacking != null)
                    {
                        await BlockUser(attacking.Pos, attacking.ReturnToStorage(attacking, board, true));
                    }

                    await BlockUser(move.Item1, move.Item2);

                    //if (_robotHasStopped)
                    //{
                    //    Reset();
                    //    break;
                    //}
                }

                board.Move(move.Item1, move.Item2);
                IsPlaying = IsPlaying == EColor.White ? EColor.Black : EColor.White;
                Reset();
                await Task.Run(() => Thread.Sleep(50));
            }

            await Endgame();
        }

        private Tuple<Point, Point> GetComputerPoints(EColor color)
        {
            Random r = new Random();
            List<Piece> pieces = new List<Piece>();
            pieces.AddRange(color == EColor.White ? board.WhitePieces.Where(piece => !piece.Taken) : board.BlackPieces.Where(piece => !piece.Taken));
            List<Tuple<Piece, Point, int>> results = new List<Tuple<Piece, Point, int>>();
            foreach (var piece in pieces)
            {
                List<Point> moves = piece.GenerateMoves(board);
                foreach (var point in moves)
                {
                    if (piece.Pos != point)
                    {
                        results.Add(board.GetPieceAt(point) == null ? new Tuple<Piece, Point, int>(piece, point, 0) : new Tuple<Piece, Point, int>(piece, point, board.GetPieceAt(point).Value));
                    }
                }
            }

            var maxResults = results.FindAll(tuple => tuple.Item3 == results.Max(tuple1 => tuple1.Item3));
            int index = r.Next(0, maxResults.Count);

            var result = new Tuple<Point, Point>(maxResults[index].Item1.Pos, maxResults[index].Item2);

            return result;
        }
    }
}