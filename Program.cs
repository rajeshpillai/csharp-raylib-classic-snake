using System;
using System.Collections.Generic;
using Raylib_cs;

namespace SnakeGame
{
    public class Program
    {
        static void Main()
        {
            // Window dimensions and settings
            int screenWidth = 800;
            int screenHeight = 600;
            Raylib.InitWindow(screenWidth, screenHeight, "Classic Snake Game");
            Raylib.SetTargetFPS(10);

            // Game settings
            int gridSize = 20;
            int gridWidth = screenWidth / gridSize;
            int gridHeight = screenHeight / gridSize;

            // Snake initialization
            List<(int x, int y)> snake = new List<(int, int)>
            {
                (gridWidth / 2, gridHeight / 2)
            };
            int snakeLength = 1;
            (int x, int y) direction = (1, 0);

            // Food initialization
            Random rand = new Random();
            (int x, int y) food = (rand.Next(0, gridWidth), rand.Next(0, gridHeight));

            bool gameOver = false;

            while (!Raylib.WindowShouldClose())
            {
                // Game over condition and restart
                if (gameOver)
                {
                    if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                    {
                        // Reset game
                        snake.Clear();
                        snake.Add((gridWidth / 2, gridHeight / 2));
                        snakeLength = 1;
                        direction = (1, 0);
                        food = (rand.Next(0, gridWidth), rand.Next(0, gridHeight));
                        gameOver = false;
                    }
                    continue;
                }

                // Control direction
                if (Raylib.IsKeyPressed(KeyboardKey.Up) && direction != (0, 1)) direction = (0, -1);
                if (Raylib.IsKeyPressed(KeyboardKey.Down) && direction != (0, -1)) direction = (0, 1);
                if (Raylib.IsKeyPressed(KeyboardKey.Left) && direction != (1, 0)) direction = (-1, 0);
                if (Raylib.IsKeyPressed(KeyboardKey.Right) && direction != (-1, 0)) direction = (1, 0);

                // Update snake position
                (int x, int y) newHead = (snake[0].x + direction.x, snake[0].y + direction.y);

                // Wrap around screen edges
                if (newHead.x < 0) newHead.x = gridWidth - 1;
                if (newHead.x >= gridWidth) newHead.x = 0;
                if (newHead.y < 0) newHead.y = gridHeight - 1;
                if (newHead.y >= gridHeight) newHead.y = 0;

                // Check self-collision
                if (snake.Contains(newHead))
                {
                    gameOver = true;
                }
                else
                {
                    snake.Insert(0, newHead);
                    if (snake.Count > snakeLength)
                        snake.RemoveAt(snake.Count - 1);

                    // Check food collision
                    if (newHead == food)
                    {
                        snakeLength++;
                        food = (rand.Next(0, gridWidth), rand.Next(0, gridHeight));
                    }
                }

                // Draw everything
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Draw food
                Raylib.DrawRectangle(food.x * gridSize, food.y * gridSize, gridSize, gridSize, Color.Red);

                // Draw snake
                foreach (var segment in snake)
                {
                    Raylib.DrawRectangle(segment.x * gridSize, segment.y * gridSize, gridSize, gridSize, Color.Green);
                }

                // Draw game over text
                if (gameOver)
                {
                    Raylib.DrawText("Game Over! Press ENTER to restart", screenWidth / 2 - 200, screenHeight / 2, 20, Color.White);
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}

