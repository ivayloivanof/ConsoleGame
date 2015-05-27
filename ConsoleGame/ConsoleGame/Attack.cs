﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimerExample;


namespace ConsoleGame
{
    internal class Attack
    {
        //private static double speed = 400.0;
        //private static double acceleration = 0.5;
        private const int playfieldWidth = 50;
        private static int livesCount = Life.LifeCount;
        private static int scoresCount = Score.ScoreCount;
        private static List<Bullet> shots = new List<Bullet>();
        private static List<EnemyInvader> invader = new List<EnemyInvader>();
        private static PlayerShip spaceship = new PlayerShip(5, Console.WindowHeight - 2, "_/|\\_", ConsoleColor.Yellow);
        private static int steps = 0;
        private static int enemiesPause = 5;
        private static Map map = new Map();
        private static Random randomGenerator = new Random();

        public static bool CheckCollision(string invader, string spaceship, int spaceshipX, int spaceshipY, int invaderX, int invaderY)
        {
            bool result = false;
            for (int i = 0; i < invader.Length; i++)
            {
                for (int j = 0; j < spaceship.Length; j++)
                {
                    if (spaceshipY == invaderY && spaceshipX + j == invaderX + i)
                    {
                        
                        result = true;
                    }
                }
            }
            return result;
        }

        public static void UpdateAttack()
        {
           
            while (true)
            {
                //speed += acceleration;
                //if (speed > 200)
                //{
                //    speed = 200;
                //}

                PlayerShipControls();

                if (steps % enemiesPause == 0)
                {
                    GenerateEnemies(randomGenerator);
                    MoveEnemies();
                }
                steps++;
                TakeLivesTillPlayerIsDead();
                CheckEnemyAndShotsCollision();
                Shooting.UpdateShots(shots);
                Console.Clear();
                Visualization.PrintStringAtPosition(spaceship.X, spaceship.Y, spaceship.C, spaceship.Color);

                foreach (var shot in shots)
                {
                    if (shot.Y > 0 && shot.Y < Console.WindowHeight - 3)
                    {
                        Visualization.DrawSymbolAtCoordinates(shot.X, shot.Y, shot.C, shot.Color);
                    }

                }
              

                foreach (EnemyInvader unit in invader)
                {
                    Visualization.PrintStringAtPosition(unit.X, unit.Y, unit.C, unit.Color);
                }
                Visualization.PrintStringAtPosition(70, 2, "LIVES: " + new string('\u2665', livesCount), ConsoleColor.Red);
                Visualization.PrintStringAtPosition(70, 4, "SCORES: " + scoresCount, ConsoleColor.White);

                Visualization.PrintStringAtPosition(70, 6, "TIMER:", ConsoleColor.White); //I have to learn how to implement the Timer class which is built in in .net.
                Visualization.PrintStringAtPosition(60, 20, new string('*', 40), ConsoleColor.White);
                for (int i = 1; i < 20; i++)
                {
                    Visualization.PrintStringAtPosition(60, i, "*", ConsoleColor.White);
                }

                if (scoresCount == 20)
                {
                    //Scoreboard.GenerateScoreboard(scoresCount);
                    Console.Clear();
                    
                    Visualization.PrintStringAtPosition(8, 10, "GAME OVER! YOU WIN!", ConsoleColor.Red);
                    Visualization.PrintStringAtPosition(8, 12, "Press [enter] to exit", ConsoleColor.Red);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Environment.Exit(0);
                  
                }
               
                Thread.Sleep(150);
               
                //Thread.Sleep((int)(600 - speed));
            }
        }

        private static void CheckEnemyAndShotsCollision()
        {
            for (int i = 0; i < shots.Count; i++)
            {
                for (int j = 0; j < invader.Count; j++)
                {
                    if (CheckCollision(invader[j].C, shots[i].C, shots[i].X, shots[i].Y, invader[j].X, invader[j].Y))
                    {
                        scoresCount++;
                        if (invader.Count > 1)
                        {
                            invader.Remove(invader[j]);
                        }
                        //else
                        //{
                        //    invader.Add(new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "\\\\|//", ConsoleColor.Green));
                        //}

                        if (shots.Count > 1)
                        {
                            shots.Remove(shots[i]);
                        }

                    }
                }

            }
        }

        private static void TakeLivesTillPlayerIsDead()
        {
            for (int enemy = 0; enemy < invader.Count; enemy++)
            {
                if (CheckCollision(invader[enemy].C, spaceship.C, spaceship.X, spaceship.Y, invader[enemy].X, invader[enemy].Y) || invader[enemy].Y == Console.WindowHeight - 4)
                {
                    livesCount--;

                    if (invader.Count > 1)
                    {
                        invader.Remove(invader[enemy]);
                    }

                    Visualization.PrintOnPosition(spaceship.X, spaceship.Y, 'X', ConsoleColor.Red);
                    if (livesCount <= 0)
                    {
                        Console.Clear();
                        Visualization.PrintStringAtPosition(8, 10, "GAME OVER! YOU LOSE!", ConsoleColor.Red);
                        Visualization.PrintStringAtPosition(8, 12, "Press [enter] to exit", ConsoleColor.Red);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Environment.Exit(0);
                    }
                }
            }
        }

        private static void MoveEnemies()
        {
            List<EnemyInvader> newList = new List<EnemyInvader>();
            for (int i = 0; i < invader.Count; i++)
            {

                EnemyInvader oldInvader = invader[i];
                EnemyInvader newInvader = new EnemyInvader();
                newInvader.X = oldInvader.X;
                newInvader.Y = oldInvader.Y + 1;
                newInvader.C = oldInvader.C;
                newInvader.Color = oldInvader.Color;

                if (newInvader.Y < Console.WindowHeight - 1)
                {
                    newList.Add(newInvader);
                }
            }
            invader = newList;
        }

        private static void PlayerShipControls()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo pressedKey = Console.ReadKey(true);
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
                if (pressedKey.Key == ConsoleKey.LeftArrow)
                {
                    if (spaceship.X - 1 >= 0)
                    {
                        spaceship.X = spaceship.X - 1;
                    }

                }
                else if (pressedKey.Key == ConsoleKey.RightArrow)
                {
                    if (spaceship.X + 1 < playfieldWidth)
                    {
                        spaceship.X = spaceship.X + 1;
                    }

                }

                if (pressedKey.Key == ConsoleKey.Spacebar)
                {
                    Shooting.Shoot(spaceship, shots);
                }
            }
        }

        private static void GenerateEnemies(Random randomGenerator)
        {
            int chance = randomGenerator.Next(0, 100);

            if (chance < 2)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "\\\\|//", ConsoleColor.Green);
                invader.Add(newInvader);
            }
            else if (chance < 5)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "\\\\|//", ConsoleColor.Red);

                invader.Add(newInvader);
            }
            else if (chance < 8)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "//|\\\\", ConsoleColor.Green);

                invader.Add(newInvader);
            }
            else if (chance < 10)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "\\\\|//", ConsoleColor.Cyan);
                invader.Add(newInvader);
            }
            else if (chance < 13)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "//|\\\\", ConsoleColor.Magenta);
                invader.Add(newInvader);
            }
            else if (chance < 15)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "\\\\|//", ConsoleColor.Magenta);
                invader.Add(newInvader);

            }
            else if (chance < 18)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "//|\\\\", ConsoleColor.Cyan);
                invader.Add(newInvader);
            }
            else if (chance < 20)
            {
                EnemyInvader newInvader = new EnemyInvader(randomGenerator.Next(0, playfieldWidth), 0, "\\\\|//", ConsoleColor.White);
                invader.Add(newInvader);
            }
        }
    }
}
