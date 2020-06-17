using System;

namespace ChessConsole
{
    public struct ColoredChar : IEquatable<ColoredChar>
    {
        public ConsoleColor Foreground;
        public ConsoleColor Background;
   
        public char caracterValue;

        public ColoredChar(char c = ' ', ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Foreground = foreground;
            Background = background;
            caracterValue = c;
        }

        public bool Equals(ColoredChar other)
        {
            if (other.Foreground == Foreground && other.Background == Background)
            {
                if (other.caracterValue == caracterValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator==(ColoredChar lhs, ColoredChar rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(ColoredChar lhs, ColoredChar rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public class ConsoleGraphics
    {
        /// First everything is drawn to the back buffer for double buffering purposes
        private ColoredChar[,] backBuffer;

        /// Current console's colored character buffer
        private ColoredChar[,] frontBuffer;

        public ConsoleGraphics()
        {
            backBuffer = new ColoredChar[Console.BufferWidth, Console.BufferHeight];
            frontBuffer = new ColoredChar[Console.BufferWidth, Console.BufferHeight];
        }

        #region DrawMethods

        public void ClearBackBuffer()
        {
            for (int i = 0; i < backBuffer.GetLength(0); i++)
            {
                for (int j = 0; j < backBuffer.GetLength(1); j++)
                {
                    backBuffer[i, j] = new ColoredChar();
                }
            }
        }

        public void Draw(ColoredChar coloredcharacter, int x, int y)
        {
            backBuffer[x, y] = coloredcharacter;
        }

        public void DrawTransparentBackground(char character, ConsoleColor foreground, int x, int y)
        {
            backBuffer[x, y].caracterValue = character;
            backBuffer[x, y].Foreground = foreground;
        }

        public void DrawAreaColoredCharacters(ColoredChar[,] coloredChars, int x, int y)
        {
            for (int i = 0; i < coloredChars.GetLength(0); i++)
            {
                for (int j = 0; j < coloredChars.GetLength(1); j++)
                {
                    backBuffer[x + i, y + j] = coloredChars[i, j];
                }
            }
        }

        public void DrawText(string textToDraw, ConsoleColor foreground, ConsoleColor background, int startingConsoleCoordX, int startingConsoleCoordY)
        {
            ColoredChar[,] area = new ColoredChar[textToDraw.Length, 1];
            for (int i = 0; i < textToDraw.Length; i++)
            {
                area[i, 0] = new ColoredChar(textToDraw[i], foreground, background);
            }

            DrawAreaColoredCharacters(area, startingConsoleCoordX, startingConsoleCoordY);
        }

        public void DrawTextTrasparentBackground(string text, ConsoleColor foreground, int x, int y)
        {
            ColoredChar[,] area = new ColoredChar[text.Length, 1];
            for (int i = 0; i < text.Length; i++)
            {
                area[i, 0] = new ColoredChar(text[i], foreground, backBuffer[x + i, y].Background);
            }

            DrawAreaColoredCharacters(area, x, y);
        }

        public void FillAreaColoredCharacter(ColoredChar coloredCharacter, int x, int y, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    backBuffer[x + i, y + j] = coloredCharacter;
                }
            }
        }

        public void ClearArea(int x, int y, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    backBuffer[x + i, y + j] = new ColoredChar();
                }
            }
        }

        #endregion

        #region Darken_Lighten

        public void DarkenBackgroundColor(int x, int y)
        {
            switch (backBuffer[x, y].Background)
            {
                case ConsoleColor.Blue:
                    backBuffer[x, y].Background = ConsoleColor.DarkBlue;
                    break;
                case ConsoleColor.Green:
                    backBuffer[x, y].Background = ConsoleColor.DarkGreen;
                    break;
                case ConsoleColor.Yellow:
                    backBuffer[x, y].Background = ConsoleColor.DarkYellow;
                    break;
                case ConsoleColor.Magenta:
                    backBuffer[x, y].Background = ConsoleColor.DarkMagenta;
                    break;
                case ConsoleColor.Gray:
                    backBuffer[x, y].Background = ConsoleColor.DarkGray;
                    break;
                case ConsoleColor.Cyan:
                    backBuffer[x, y].Background = ConsoleColor.DarkCyan;
                    break;
                case ConsoleColor.Red:
                    backBuffer[x, y].Background = ConsoleColor.DarkRed;
                    break;
            }
        }


        public void DarkenForegroundColor(int x, int y)
        {
            switch (backBuffer[x, y].Foreground)
            {
                case ConsoleColor.Blue:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkBlue;
                    break;
                case ConsoleColor.Green:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkGreen;
                    break;
                case ConsoleColor.Yellow:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkYellow;
                    break;
                case ConsoleColor.Magenta:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkMagenta;
                    break;
                case ConsoleColor.Gray:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkGray;
                    break;
                case ConsoleColor.Cyan:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkCyan;
                    break;
                case ConsoleColor.Red:
                    backBuffer[x, y].Foreground = ConsoleColor.DarkRed;
                    break;
            }
        }

        public void LightenBackgroundColor(int x, int y)
        {
            switch (backBuffer[x, y].Background)
            {
                case ConsoleColor.DarkBlue:
                    backBuffer[x, y].Background = ConsoleColor.Blue;
                    break;
                case ConsoleColor.DarkGreen:
                    backBuffer[x, y].Background = ConsoleColor.Green;
                    break;
                case ConsoleColor.DarkYellow:
                    backBuffer[x, y].Background = ConsoleColor.Yellow;
                    break;
                case ConsoleColor.DarkMagenta:
                    backBuffer[x, y].Background = ConsoleColor.Magenta;
                    break;
                case ConsoleColor.DarkGray:
                    backBuffer[x, y].Background = ConsoleColor.Gray;
                    break;
                case ConsoleColor.DarkCyan:
                    backBuffer[x, y].Background = ConsoleColor.Cyan;
                    break;
                case ConsoleColor.DarkRed:
                    backBuffer[x, y].Background = ConsoleColor.Red;
                    break;
            }
        }

        public void LightenForegroundColor(int x, int y)
        {
            switch (backBuffer[x, y].Foreground)
            {
                case ConsoleColor.DarkBlue:
                    backBuffer[x, y].Foreground = ConsoleColor.Blue;
                    break;
                case ConsoleColor.DarkGreen:
                    backBuffer[x, y].Foreground = ConsoleColor.Green;
                    break;
                case ConsoleColor.DarkYellow:
                    backBuffer[x, y].Foreground = ConsoleColor.Yellow;
                    break;
                case ConsoleColor.DarkMagenta:
                    backBuffer[x, y].Foreground = ConsoleColor.Magenta;
                    break;
                case ConsoleColor.DarkGray:
                    backBuffer[x, y].Foreground = ConsoleColor.Gray;
                    break;
                case ConsoleColor.DarkCyan:
                    backBuffer[x, y].Foreground = ConsoleColor.Cyan;
                    break;
                case ConsoleColor.DarkRed:
                    backBuffer[x, y].Foreground = ConsoleColor.Red;
                    break;
            }
        }

        #endregion

        #region Color Getters/Setters

        public void SetBackgroundColor(ConsoleColor color, int x, int y)
        {
            backBuffer[x, y].Background = color;
        }

        public ConsoleColor GetBackgroundColor(int x, int y)
        {
            return backBuffer[x, y].Background;
        }

        public void SetForegroundColor(ConsoleColor color, int x, int y)
        {
            backBuffer[x, y].Foreground = color;
        }

        public ConsoleColor GetForegroundColor(int x, int y)
        {
            return backBuffer[x, y].Foreground;
        }
        #endregion

        public void SwapBuffers()
        {
            for (int i = 0; i < backBuffer.GetLength(0); i++)
            {
                for (int j = 0; j < backBuffer.GetLength(1); j++)
                {
                    if (frontBuffer[i, j] != backBuffer[i, j])
                    {
                        Console.SetCursorPosition(i, j);
                        Console.ForegroundColor = backBuffer[i, j].Foreground;
                        Console.BackgroundColor = backBuffer[i, j].Background;
                        Console.Write(backBuffer[i, j].caracterValue);
                        frontBuffer[i, j] = backBuffer[i, j];
                    }
                }
            }
        }
    }
}
