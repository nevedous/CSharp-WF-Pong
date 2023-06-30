using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

class Program : Form {
    public static readonly int WIDTH = 800;
    public static readonly int HEIGHT = 600;
    public static readonly int FPS = 60;
    public static readonly int DELAY = 1000 / (FPS);
    public static readonly int PADDLE_WIDTH = 34;
    public static readonly int PADDLE_HEIGHT = 100;
    public static readonly int BALL_SIZE = 27;

    private static readonly SolidBrush blueBrush = new SolidBrush(Color.Blue);
    private static readonly SolidBrush whiteBrush = new SolidBrush(Color.White);
    private static readonly SolidBrush redBrush = new SolidBrush(Color.Red);

    private static int ballX = (WIDTH - BALL_SIZE) / 2, ballY = (HEIGHT - BALL_SIZE) / 2, ballDX = -3, ballDY = 2;
    private static int leftX = 0, leftY = (HEIGHT - PADDLE_HEIGHT) / 2, leftScore = 0;
    private static int rightX = (WIDTH - PADDLE_WIDTH), rightY = (HEIGHT - PADDLE_HEIGHT) / 2, rightScore = 0;

    private static Thread gameThread;

    private bool isTouched(int x1, int y1, int w1, int h1,
                           int x2, int y2, int w2, int h2) {
        return w1 > 0 && h1 > 0 && w2 > 0 && h2 > 0
            && x2 < x1 + w1 && x2 + w2 > x1
            && y2 < y1 + h1 && y2 + h2 > y1;
    }

    private void run() {
        while (gameThread != null) {
            ballX += ballDX;
            ballY += ballDY;

            //if (ballX < 0 || ballX > (WIDTH - BALL_SIZE)) ballDX *= -1;
            if (ballY < 0 || ballY > (HEIGHT - BALL_SIZE)) ballDY *= -1;

            if (isTouched(rightX, rightY, PADDLE_WIDTH, PADDLE_HEIGHT,
                           ballX,  ballY,    BALL_SIZE,     BALL_SIZE)) {
                ballDX = -Math.Abs(ballDX) - 1;
                ballDY += (new Random().Next(1, 3) == 2) ? 1 : -1;
            }

            if (isTouched(leftX, leftY, PADDLE_WIDTH, PADDLE_HEIGHT,
                           ballX,  ballY,    BALL_SIZE,     BALL_SIZE)) {
                ballDX = Math.Abs(ballDX) + 1;
                ballDY += (new Random().Next(1, 3) == 2) ? 1 : -1;
            }

            if (ballX < 0) {
                rightScore++;
                ballX = (WIDTH - BALL_SIZE) / 2;
                ballY = (HEIGHT - BALL_SIZE) / 2;
                ballDX = new Random().Next(2) == 1 ? -3 : 3;
                ballDY = new Random().Next(2) == 1 ? -2 : 2;
            }

            if (ballX > (WIDTH - BALL_SIZE)) {
                leftScore++;
                ballX = (WIDTH - BALL_SIZE) / 2;
                ballY = (HEIGHT - BALL_SIZE) / 2;
                ballDX = new Random().Next(2) == 1 ? -3 : 3;
                ballDY = new Random().Next(2) == 1 ? -2 : 2;
            }

            Thread.Sleep(DELAY);
        }
    }

    protected override void OnPaint(PaintEventArgs e) {
        var g = e.Graphics;

        g.FillEllipse(whiteBrush, ballX, ballY, BALL_SIZE, BALL_SIZE);
        g.FillRectangle(blueBrush, leftX, leftY, PADDLE_WIDTH, PADDLE_HEIGHT);
        g.FillRectangle(redBrush, rightX, rightY, PADDLE_WIDTH, PADDLE_HEIGHT);

        g.FillRectangle(whiteBrush, ((WIDTH - 1) / 2), 0, 1, HEIGHT);

        g.DrawString("LEFT " + leftScore, new Font("Arial", 24), whiteBrush, 5, 5);
        g.DrawString("RIGHT " + rightScore, new Font("Arial", 24), whiteBrush, WIDTH - 160, 5);

        Invalidate();
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        if (e.KeyCode == Keys.W) leftY -= 16;
        if (e.KeyCode == Keys.S) leftY += 16;

        if (e.KeyCode == Keys.Up) rightY -= 16;
        if (e.KeyCode == Keys.Down) rightY += 16;
    }

    Program() {
        Text = "Pong";
        ClientSize = new Size(WIDTH, HEIGHT);
        DoubleBuffered = true;
        BackColor = Color.Black;
        CenterToScreen();

        gameThread = new Thread(new ThreadStart(run));      // Active the "run" function that we created
        gameThread.Start();
    }

    static void Main() {
        Application.Run(new Program());
        gameThread.Abort();
    }
}