using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Timers;

Random random = new Random();
Console.CursorVisible = false;
int height = Console.WindowHeight - 1;
int width = Console.WindowWidth - 5;
bool shouldExit = false;
bool consumed = false;

// Console position of the player
int playerX = 0;
int playerY = 0;

// Console position of the food
int foodX = 0;
int foodY = 0;

// Available player and food strings
string[] states = { "('-')", "(^-^)", "(X_X)" };
string[] foods = { "@@@@@", "$$$$$", "#####" };

// Current player string displayed in the Console
string player = states[0];
int playerWidth = player.Length;

// Index of the current food
int food = 0;
int foodWidth = 0;

ConsoleKeyInfo input;

InitializeGame();
while (!shouldExit)
{
    if (!TerminalResized())
    {
        input = Console.ReadKey(true);

        if (CheckStateOfPlayer(player) == "Healthy")
        {
            Move(3, input.Key.ToString());
        }
        else
            Move(pressedKey: input.Key.ToString());
    }
    else
        shouldExit = !shouldExit;
}

Console.Clear();
Console.WriteLine("Console was resized. Program exiting.");

// Returns true if the Terminal was resized 
bool TerminalResized()
{
    return height != Console.WindowHeight - 1 || width != Console.WindowWidth - 5;
}

// Displays random food at a random location
void ShowFood()
{
    // Update food to a random index
    food = random.Next(0, foods.Length);

    // Update food position to a random location
    foodX = random.Next(0, width - player.Length);
    foodY = random.Next(0, height - 1);

    // Display the food at the location
    Console.SetCursorPosition(foodX, foodY);
    Console.Write(foods[food]);
    foodWidth = foods[food].Length;
}

// Changes the player to match the food consumed
void ChangePlayer()
{
    player = states[food];
    Console.SetCursorPosition(playerX, playerY);
    Console.Write(player);
}

// Temporarily stops the player from moving
void FreezePlayer()
{
    System.Threading.Thread.Sleep(1000);
    player = states[0];
}

// Reads directional input from the Console and moves the player
void Move(int speed = 1, string pressedKey = "")
{
    int lastX = playerX;
    int lastY = playerY;

    switch (pressedKey)
    {
        case "UpArrow":
            playerY--;
            break;
        case "DownArrow":
            playerY++;
            break;
        case "LeftArrow":
            playerX = playerX - speed;
            break;
        case "RightArrow":
            playerX = playerX + speed;
            break;
        case "Escape":
        default:
            shouldExit = true;
            break;
    }

    // Clear the characters at the previous position
    Console.SetCursorPosition(lastX, lastY);
    for (int i = 0; i < player.Length; i++)
    {
        Console.Write(" ");
    }

    // Keep player position within the bounds of the Terminal window
    playerX = (playerX < 0) ? 0 : (playerX >= width ? width : playerX);
    playerY = (playerY < 0) ? 0 : (playerY >= height ? height : playerY);

    // Draw the player at the new location
    Console.SetCursorPosition(playerX, playerY);
    Console.Write(player);

    consumed = FoodConsumed(playerX, playerY);
    if (consumed)
    {
        ChangePlayer();
        ShowFood();
        consumed = !consumed;
    }
}

bool FoodConsumed(int playerPositionX, int playerPositionY)
{
    if (playerPositionX + playerWidth >= foodX && playerPositionX <= foodX + foodWidth && playerPositionY == foodY)
        return true;
    return false;
}

string CheckStateOfPlayer(string s)
{
    if (s == states[1])
    {
        return "Healthy";
    }
    else if (s == states[0])
    {
        return "Hurt";
    }
    else
        FreezePlayer();
    return "Dead";
}

// Clears the console, displays the food and player
void InitializeGame()
{
    Console.Clear();
    ShowFood();
    Console.SetCursorPosition(0, 0);
    Console.Write(player);
}