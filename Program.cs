using System;
using System.Collections.Generic;
using Raylib_cs;

namespace SnakeGame
{
    public class Program
    {
        static void Main()
        {
            // Window and game settings
            int screenWidth = 800;
            int screenHeight = 600;
            Raylib.InitWindow(screenWidth, screenHeight, "Enhanced Snake Game");

            // Set initial FPS and store it in a variable to adjust later
            int fps = 10;
            Raylib.SetTargetFPS(fps);

            int gridSize = 20;
            int gridWidth = screenWidth / gridSize;
            int gridHeight = screenHeight / gridSize;

            // Custom colors
            Color snakeColor = Color.Blue;
            Color foodColor = Color.Yellow;
            Color backgroundColor = Color.DarkGray;

            // Load sounds
            Raylib.InitAudioDevice();
            Sound eatSound = Raylib.LoadSound("./sound/eat.wav");
            Sound gameOverSound = Raylib.LoadSound("./sound/game_over.wav");

            // Game variables
            List<(int x, int y)> snake = new List<(int, int)> { (gridWidth / 2, gridHeight / 2) };
            int snakeLength = 1;
            (int x, int y) direction = (1, 0);
            Random rand = new Random();
            (int x, int y) food = (rand.Next(0, gridWidth), rand.Next(0, gridHeight));

            int score = 0;
            bool gameOver = false;

            while (!Raylib.WindowShouldClose())
            {
                // Check for game over and handle restart
                if (gameOver)
                {
                    // Display Game Over message
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(backgroundColor);
                    Raylib.DrawText("Game Over! Press ENTER to restart", screenWidth / 2 - 200, screenHeight / 2, 20, Color.White);
                    Raylib.DrawText($"Final Score: {score}", screenWidth / 2 - 60, screenHeight / 2 + 40, 20, Color.White);
                    Raylib.EndDrawing();

                    // Check if Enter key is pressed to restart the game
                    if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                    {
                        // Reset game state
                        snake.Clear();
                        snake.Add((gridWidth / 2, gridHeight / 2));
                        snakeLength = 1;
                        direction = (1, 0);
                        food = (rand.Next(0, gridWidth), rand.Next(0, gridHeight));
                        score = 0;
                        fps = 10;  // Reset FPS
                        Raylib.SetTargetFPS(fps);
                        gameOver = false;
                    }

                    // Skip the rest of the game loop while in game-over state
                    continue;
                }

                // Update direction based on input
                if (Raylib.IsKeyPressed(KeyboardKey.Up) && direction != (0, 1)) direction = (0, -1);
                if (Raylib.IsKeyPressed(KeyboardKey.Down) && direction != (0, -1)) direction = (0, 1);
                if (Raylib.IsKeyPressed(KeyboardKey.Left) && direction != (1, 0)) direction = (-1, 0);
                if (Raylib.IsKeyPressed(KeyboardKey.Right) && direction != (-1, 0)) direction = (1, 0);

                // Move the snake
                (int x, int y) newHead = (snake[0].x + direction.x, snake[0].y + direction.y);

                // Boundary collision
                if (newHead.x < 0 || newHead.x >= gridWidth || newHead.y < 0 || newHead.y >= gridHeight)
                {
                    gameOver = true;
                    Raylib.PlaySound(gameOverSound);
                }

                // Self-collision
                if (snake.Contains(newHead))
                {
                    gameOver = true;
                    Raylib.PlaySound(gameOverSound);
                }

                if (!gameOver)
                {
                    snake.Insert(0, newHead);
                    if (snake.Count > snakeLength)
                    {
                        snake.RemoveAt(snake.Count - 1);
                    }

                    // Check for food collision
                    if (newHead == food)
                    {
                        score++;
                        snakeLength++;
                        food = (rand.Next(0, gridWidth), rand.Next(0, gridHeight));
                        Raylib.PlaySound(eatSound);

                        // Increase speed slightly as the snake grows
                        if (score % 5 == 0)
                        {
                            fps += 2;  // Increase FPS by 2 every 5 points
                            Raylib.SetTargetFPS(fps);
                        }
                    }
                }

                // Drawing
                Raylib.BeginDrawing();
                Raylib.ClearBackground(backgroundColor);

                // Draw food
                Raylib.DrawRectangle(food.x * gridSize, food.y * gridSize, gridSize, gridSize, foodColor);

                // Draw snake
                foreach (var segment in snake)
                {
                    Raylib.DrawRectangle(segment.x * gridSize, segment.y * gridSize, gridSize, gridSize, snakeColor);
                }

                // Draw score
                Raylib.DrawText($"Score: {score}", 10, 10, 20, Color.White);

                Raylib.EndDrawing();
            }

            // Unload sounds and close audio device
            Raylib.UnloadSound(eatSound);
            Raylib.UnloadSound(gameOverSound);
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
}
