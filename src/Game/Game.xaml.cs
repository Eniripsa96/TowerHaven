using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using TowerHaven.AI;
using WPFEditor;

namespace TowerHaven
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        # region constants

        /// <summary>
        /// level sprite directory
        /// </summary>
        private readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\temp\\";

        # endregion constants

        # region fields

        /// <summary>
        /// Menu being displayed for a tile that was clicked on
        /// </summary>
        private GameTileMenu tileMenu;

        /// <summary>
        /// Active enemy list
        /// </summary>
        private SimpleList<Enemy> activeEnemies;

        /// <summary>
        /// List of canvases for displaying tower ranges
        /// </summary>
        private SimpleList<Canvas> rangeCanvases;

        /// <summary>
        /// Pathfinding object
        /// </summary>
        private Pathfinder pathing;

        /// <summary>
        /// Level data object
        /// </summary>
        private BasicLevel level;

        /// <summary>
        /// Current wave object
        /// </summary>
        private Wave currentWave;

        /// <summary>
        /// Currently selected tower name
        /// </summary>
        private string tower;

        /// <summary>
        /// Current wave id
        /// </summary>
        private int wave;

        /// <summary>
        /// Wave speed
        /// </summary>
        private int speed;

        /// <summary>
        /// Wave tick timer
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// Wave moves
        /// </summary>
        private int[][][] moves;

        /// <summary>
        /// Wave spawn id
        /// </summary>
        private int[] spawn;

        /// <summary>
        /// Wave tick count
        /// </summary>
        private int[] ticks;

        # endregion fields

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">level data</param>
        public Game(BasicLevel level)
        {
            InitializeComponent();

            if (Properties.Settings.Default.GridLines)
                tileGrid.Margin = new Thickness(tileGrid.Margin.Left + 1, tileGrid.Margin.Top + 1, tileGrid.Margin.Right - 1, tileGrid.Margin.Bottom - 1);

            // Initialize fields
            speed = 100;
            currentWave = level.GetWave(0);
            activeEnemies = new SimpleList<Enemy>();
            rangeCanvases = new SimpleList<Canvas>();
            pathing = new Pathfinder(level);
            this.level = level;
            tileMenu = null;
            wave = 1;

            // Draw the map
            for (int x = 0; x < level.HorizontalSize; ++x)
                for (int y = 0; y < level.VerticalSize; ++y)
                    DrawTile(x, y);

            // Arrange the GUI
            levelLabel.Content = level.MapName;
            creatorLabel.Content = level.Creator;
            tower = level.GetTowerName(0);
            speedSlider.Value = 2;
            UpdateLabels(false);
            LoadMoves();
        }

        /// <summary>
        /// Loads moves for each wave
        /// </summary>
        private void LoadMoves()
        {
            List<Wave> waves = level.Waves;

            // Count the number of spawns
            int moveCount = 0;
            foreach (Wave w in waves)
                foreach (SpawnPoint p in w.spawns)
                    moveCount++;

            // Set the moves for each spawn if it has not been used yet
            moves = new int[moveCount][][];
            int index = 0;
            foreach (Wave w in waves)
                foreach (SpawnPoint s in w.spawns)
                    if (index < wave - 1)
                    {
                        index++;
                        continue;
                    }
                    else
                        moves[index++] = pathing.GetMoves(s.Start, s.End);
        }

        /// <summary>
        /// Draws the tile sprite at the given coordinates
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        private void DrawTile(int x, int y)
        {
            Canvas tile = ControlManager.CreateCanvas(directory + "Tile" + level.GetTileName(x, y), 16 * x, 16 * y, 16);
            tile.MouseLeftButtonUp += tile_Click;
            tileGrid.Children.Add(tile);
        }

        /// <summary>
        /// Actions when a tile is clicked (display a mini-menu)
        /// </summary>
        /// <param name="sender">tile clicked</param>
        /// <param name="e">not used</param>
        private void tile_Click(object sender, RoutedEventArgs e)
        {
            // Get the position
            int x = ((int)(sender as Canvas).Margin.Left) / 16;
            int y = ((int)(sender as Canvas).Margin.Top) / 16;

            // Clear any previous menu
            CloseTileMenu();
            tileMenu = null;

            // Use the tower over the tile if one is there
            if (level.IsTower(x, y))
            {
                Tower t = level.GetTower(level.GetTileName(x, y));

                // Show the tower's range
                for (int i = 0; i < level.HorizontalSize; ++i)
                    for (int j = 0; j < level.VerticalSize; ++j)
                        if (i == x && j == y)
                            continue;
                        else if (Math.Sqrt((i - x) * (i - x) + (j - y) * (j - y)) <= t.range)
                        {
                            Canvas c = CreateRangeCanvas(i, j);
                            tileGrid.Children.Add(c);
                            rangeCanvases.Add(c);
                        }

                // Create the menu with the tower's name
                tileMenu = new GameTileMenu(level, x, y);
            }

            // Use the tile when there is no tower
            else
            {
                try
                {
                    // Don't allow building if a spawn point or end point was clicked
                    foreach (Wave w in level.Waves)
                        foreach (SpawnPoint s in w.spawns)
                            if ((x == s.xStart && y == s.yStart) || (x == s.xEnd && y == s.yEnd))
                                tileMenu = new GameTileMenu(level, x, y, false);

                    if (tileMenu == null)
                    {
                        // If the tile is on a wave path location, make sure the enemies
                        // can get from the prvious spot to the next spot
                        for (int j = 0; j < moves.Length; ++j)
                        {
                            if (moves[j] == null)
                                continue;
                            int[][] moveSet = moves[j];
                            for (int i = 0; i < moveSet.Length; ++i)
                                if (moveSet[i][0] == x && moveSet[i][1] == y)
                                {
                                    int waveId = 0;
                                    int temp = j;
                                    while (temp >= level.Waves[waveId].spawns.Count)
                                    {
                                        temp -= level.Waves[waveId].spawns.Count;
                                        waveId++;
                                    }
                                    if (i == 0)
                                        pathing.GetMoves(level.Waves[waveId].spawns[temp].Start, level.Waves[waveId].spawns[temp].End, x, y);
                                    else
                                        pathing.GetMoves(moveSet[i - 1], moveSet[i + 1], x, y);
                                }
                        }

                        // If in an enemy path during a wave, don't allow building
                        int moveIndex = -1;
                        if (timer != null)
                            if (timer.IsEnabled)
                                foreach (SpawnPoint p in level.Waves[wave - 1].spawns)
                                    if (moves[++moveIndex] != null)
                                        foreach (int[] spot in moves[moveIndex])
                                            if (x == spot[0] && y == spot[1])
                                                tileMenu = new GameTileMenu(level, x, y, false);

                        // If none of the restrictions occurred, allow building
                        if (tileMenu == null)
                            tileMenu = new GameTileMenu(level, x, y);
                    }
                }
                catch (Exception)
                {
                    // Don't allow building when a path wasn't found
                    tileMenu = new GameTileMenu(level, x, y, false);
                }
            }
            
            // Show the tile menu
            tileMenu.Owner = this;
            tileMenu.Closed += new EventHandler(tileMenu_Closed);
            tileMenu.Left = Left + (sender as Canvas).Margin.Left + 240;
            tileMenu.Top = Top + (sender as Canvas).Margin.Top + 76;
            tileMenu.Show();
        }

        /// <summary>
        /// OnClosed override
        /// closes the tile menu before closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            if (timer != null)
                if (timer.IsEnabled)
                    timer.Stop();
            CloseTileMenu();
            base.OnClosed(e);
        }

        /// <summary>
        /// Tile menu was closed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void tileMenu_Closed(object sender, EventArgs e)
        {
            foreach (Canvas c in rangeCanvases)
                tileGrid.Children.Remove(c);
            rangeCanvases.Clear();
        }

        /// <summary>
        /// Closes the tile menu if open
        /// </summary>
        private void CloseTileMenu()
        {
            tileMenu_Closed(null, null);
            try
            {
                tileMenu.Close();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Updates the labels in the GUI
        /// </summary>
        /// <param name="duringWave">whether or not updating during a wave</param>
        private void UpdateLabels(Boolean duringWave)
        {
            // Stat labels
            MoneyLabel.Content = level.Money.ToString();
            healthLabel.Content = level.Health.ToString();
            waveLabel.Content = wave.ToString();
            leftBlackGrid.Children.Clear();
            int index = 0;

            // Get maximum coordinate for spawn labels
            int yMax = 16 * level.VerticalSize - 125;
            if (255 > yMax)
                yMax = 255;

            // Add spawn labels
            int additional = 0;
            int lastY = 0;
            foreach (SpawnPoint p in currentWave.spawns)
                foreach (EnemyInstance e in p.enemies)
                {
                    int y = 18 * index++ + 24;
                    if (y > yMax)
                    {
                        if (lastY == 0)
                            lastY = y;
                        additional += e.quantity;
                        continue;
                    }
                    Label enemyLabel = ControlManager.CreateLabel(e.name + " (x" + e.quantity + ")", 6, y);
                    leftBlackGrid.Children.Add(enemyLabel);
                }

            // If too many spawn labels, condense the rest into a label
            if (additional > 0)
            {
                Label additionalLabel = ControlManager.CreateLabel("... (" + additional + " more) ...", 6, lastY);
                leftBlackGrid.Children.Add(additionalLabel);
            }

            // Display closest enemy during a wave
            if (duringWave)
            {
                if (activeEnemies.Empty())
                    detailLabel.Content = "";
                else
                    detailLabel.Content = "Closest Enemy: " + activeEnemies.Peek.Name + " - HP: " + activeEnemies[0].Health;
            }

            // Display the number of remaining waves when not in a wave
            else
                detailLabel.Content = "Remaining Waves: " + level.GetRemainingWaves(wave - 1);
        }

        /// <summary>
        /// Draws a tower at the given coordinates
        /// </summary>
        /// <param name="name">tower name</param>
        /// <param name="x">Horizontal Coordinate</param>
        /// <param name="y">Vertical Coordinate</param>
        public void DrawTower(string name, int x, int y)
        {
            Canvas tower = ControlManager.CreateCanvas(directory + "Tower" + name, 0, 0, 16);
            x *= 16;
            y *= 16;
            foreach (Canvas c in tileGrid.Children)
                if (c.Margin.Top == y && c.Margin.Left == x)
                    c.Children.Add(tower);

            UpdateLabels(false);
            LoadMoves();
        }

        /// <summary>
        /// Deletes a tower from the display at the given coordinates
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        public void DeleteTower(int x, int y)
        {
            x *= 16;
            y *= 16;
            foreach (Canvas tile in tileGrid.Children)
                if (tile.Margin.Left == x && tile.Margin.Top == y)
                    tile.Children.Clear();

            UpdateLabels(false);
            LoadMoves();
        }

        /// <summary>
        /// Runs the next wave in the level
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void waveButton_Click(object sender, RoutedEventArgs e)
        {
            // Close any tile menus
            CloseTileMenu();

            // Don't start a new wave if one is in progress
            if (timer != null)
                if (timer.IsEnabled)
                    return;
            
            // Initialize counters for the current spawn and the number of ticks since the last spawn
            spawn = new int[currentWave.spawns.Count];
            ticks = new int[currentWave.spawns.Count];
            for (int i = 0; i < currentWave.spawns.Count; ++i)
            {
                spawn[i] = 0;
                ticks[i] = 0;
            }

            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10000 / speed);
            timer.Tick += Wave_Tick;
            timer.Start();

            // Set up a name scope for animation
            NameScope.SetNameScope(tileGrid, new NameScope());
        }

        /// <summary>
        /// Performs actions for the wave each tick of the timer
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Wave_Tick(object sender, EventArgs e)
        {
            # region end of wave
            // Wave ends when there are no active enemies and all enemies in the wave have been spawned
            if  (activeEnemies.Size == 0)
            {
                Boolean isDone = true;
                int index = 0;
                foreach (SpawnPoint p in level.GetWave(wave - 1).spawns)
                    if (spawn[index++] < p.enemies.Count)
                        isDone = false;
                
                // Reward the player and stop the timer at the end of the wave
                if (isDone)
                {
                    foreach (TowerInstance t in level.Towers)
                        level.Money += (level.GetTower(t.Name)).moneyPerWave;

                    timer.Stop();
                    level.Money += currentWave.reward;
                    wave++;
                    if (level.GetRemainingWaves(wave - 1).Equals("0"))
                    {
                        MessageBox.Show("You beat the level!");
                        Close();
                        return;
                    }
                    currentWave = level.GetWave(wave - 1);
                    UpdateLabels(false);
                    return;
                }
            }
            # endregion end of wave
            # region enemies
            // Move each enemy, applying DOTs if applicable
            foreach (Enemy enemy in activeEnemies)
            {
                int fear = enemy.Fear;

                if (enemy.CanMove)
                {
                    enemy.Ticks = 0;
                    
                    // Flying enemy movement
                    // Moves towards the end point, ignoring all objects in the way
                    if (enemy.flying)
                    {
                        // Get target coordinates
                        int x = (int)enemy.sprite.Margin.Left;
                        int y = (int)enemy.sprite.Margin.Top;
                        int xEnd = 16 * currentWave.spawns[enemy.spawn].xEnd;
                        int yEnd = 16 * currentWave.spawns[enemy.spawn].yEnd;
                        if (x > xEnd)
                            x -= 16 * fear;
                        if (x < xEnd)
                            x += 16 * fear;
                        if (Math.Abs(x - xEnd) < 16)
                            x = xEnd;
                        if (y > yEnd)
                            y -= 16 * fear;
                        if (y < yEnd)
                            y += 16 * fear;
                        if (Math.Abs(y - yEnd) < 16)
                            y = yEnd;

                        // Animate the enemy to the target coordinates
                        Animate(enemy, x, y);

                        // Enemy reaches the end when they're margin matches the end location
                        if (enemy.sprite.Margin.Left == xEnd && enemy.sprite.Margin.Top == yEnd)
                        {
                            // Remove the enemy and deal damage
                            activeEnemies.Remove(enemy);
                            tileGrid.Children.Remove(enemy.sprite);
                            level.Health -= enemy.damage;
                            if (level.Health <= 0)
                            {
                                MessageBox.Show("You lost...");
                                
                                Close();
                            }
                            continue;
                        }
                    }

                    // Ground enemy movement
                    // Move according to pathfinding paths
                    else
                    {
                        // Get the next move index
                        int moveId = enemy.spawn;
                        for (int i = 0; i < wave - 1; ++i)
                            moveId += level.Waves[i].spawns.Count;
                       
                        // Enemy reached the end when they have no more moves left
                        if (enemy.move == moves[moveId].Length)
                        {
                            // Remove enemy and deal damage
                            activeEnemies.Remove(enemy);
                            tileGrid.Children.Remove(enemy.sprite);
                            level.Health -= enemy.damage;
                            if (level.Health <= 0)
                            {
                                MessageBox.Show("You lost...");
                                Close();
                            }
                            continue;
                        }

                        // Get the next move
                        int[] moveSpot;
                        if (enemy.move > 1 || fear == 1)
                            moveSpot = moves[moveId][moves[moveId].Length - fear - enemy.move];
                        else 
                            moveSpot = currentWave.spawns[enemy.spawn].Start;

                        // Animate to the next spot
                        Animate(enemy, 16 * moveSpot[0], 16 * moveSpot[1]);

                        enemy.move += fear;
                        if (enemy.move < 0)
                            enemy.move = 0;
                    }
                }
                
                // Apply per-frame actions to the enemy
                enemy.Tick();
            }
            # endregion enemies
            # region spawning
            // Check spawns for each spawn point
            for (int i = 0; i < ticks.Length; ++i)
            {
                ticks[i]++;

                // If all enemies haven't been spawned and it is time to spawn the next enemy, spawn the next enemy
                if (spawn[i] < currentWave.spawns[i].enemies.Count)
                    if (ticks[i] >= currentWave.spawns[i].enemies[spawn[i]].delay)
                    {
                        // Reset the tick count
                        ticks[i] = 0;

                        // Create the new enemy
                        Enemy newEnemy = level.GetEnemy(currentWave.spawns[i].enemies[spawn[i]].name).Clone();
                        newEnemy.Damaged += Enemy_Damaged;
                        newEnemy.spawn = i;
                        activeEnemies.Add(newEnemy);

                        // Draw the enemy
                        Canvas enemyCanvas = ControlManager.CreateCanvas(directory + "Enemy" + newEnemy.Name, 16 * currentWave.spawns[i].xStart, 16 * currentWave.spawns[i].yStart, 16);
                        newEnemy.sprite = enemyCanvas;
                        tileGrid.Children.Add(enemyCanvas);

                        // Prepare the enemy for animation
                        enemyCanvas.Name = "Enemy_" + i + "_" + spawn[i] + "_" + currentWave.spawns[i].enemies[spawn[i]].quantity; ;
                        tileGrid.RegisterName(enemyCanvas.Name, enemyCanvas);

                        // Deduct the spawn counter, moving to the next enemy if this was the last one
                        currentWave.spawns[i].enemies[spawn[i]].quantity--;
                        if (currentWave.spawns[i].enemies[spawn[i]].quantity == 0)
                            spawn[i]++;
                    }
            }

            // Sort the enemy list if there are more than one active
            if (activeEnemies.Size > 1)
                Sort(activeEnemies);

            # endregion spawning
            # region towers
            // Check each tower if it can attack
            foreach (TowerInstance t in level.Towers)
            {
                Tower tower = level.GetTower(t.Name);
                level.Money += tower.moneyPerTick;

                // If the tower can attack, see if it is range
                if (t.ticks >= tower.delay)
                {
                    foreach (Enemy enemy in activeEnemies)
                    {
                        int x = (int)enemy.sprite.Margin.Left / 16;
                        int y = (int)enemy.sprite.Margin.Top / 16;
                        int distance = t.GetDistance(x, y);
                        
                        // If the tower is in range, attack the enemy
                        if (distance <= tower.range)
                        {
                            // Add bonus money for attacking
                            level.Money += tower.moneyPerAttack;

                            // Apply an AOE attack if applicable
                            if (tower.aoeRadius > 0)
                            {
                                // Damage all enemies within the AOE radius of the target enemy
                                for (int i = 0; i < activeEnemies.Size; ++i)
                                    if (!activeEnemies[i].Equals(enemy))
                                    {
                                        int ii = (int)activeEnemies[i].sprite.Margin.Left / 16;
                                        int jj = (int)activeEnemies[i].sprite.Margin.Top / 16;
                                        if (Math.Sqrt((ii - x) * (ii - x) + (jj - y) * (jj - y)) <= tower.aoeRadius)
                                        {
                                            activeEnemies[i].Damage(tower.damage * tower.aoeDamage / 100);
                                            if (activeEnemies[i].Health <= 0)
                                                level.Money += tower.moneyPerKill;

                                            // Apply the status to each enemy if the tower has an AOE status effect
                                            if (tower.aoeStatus && !tower.status.Equals("None"))
                                                activeEnemies[i].Inflict(level.GetStatus(tower.status).Clone());
                                        }
                                    }
                            }

                            // Reset the tower's attack cooldown (ticks)
                            t.ticks = 0;

                            // Apply damage and the status effect to the enemy
                            enemy.Damage(tower.damage);
                            if (enemy.Health <= 0)
                                level.Money += tower.moneyPerKill;
                            if (!tower.status.Equals("None"))
                                enemy.Inflict(level.GetStatus(tower.status).Clone());

                            // Beep if the option is enabled
                            if (Properties.Settings.Default.TowerBeep)
                                Beep.shouldBeep = true;

                            // Don't attack more than once
                            break;
                        }
                    }
                }
                else
                    t.ticks++;
            }
            # endregion towers

            UpdateLabels(true);
        }

        /// <summary>
        /// Checks if a damaged enemy was defeated
        /// </summary>
        /// <param name="sender">enemy affected</param>
        /// <param name="e">not used</param>
        private void Enemy_Damaged(object sender, EventArgs e)
        {
            Enemy enemy = sender as Enemy;
            if (enemy.Health < 1)
            {
                activeEnemies.Remove(enemy);
                level.Money += enemy.Reward;
                tileGrid.Children.Remove(enemy.sprite);
            }
        }

        /// <summary>
        /// Animates the enemy's margin to the given coordinates
        /// </summary>
        /// <param name="animatedEnemy">enemy to animate</param>
        /// <param name="toHorizontalCoordinate">target horizontal coordinate</param>
        /// <param name="toVerticalCoordinate">target vertical coordinate</param>
        private void Animate(Enemy animatedEnemy, int toHorizontalCoordinate, int toVerticalCoordinate)
        {
            // Animate the enemy margin from its current location to the target location
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = animatedEnemy.sprite.Margin;
            animation.To = new Thickness(toHorizontalCoordinate, toVerticalCoordinate, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromMilliseconds((animatedEnemy.Speed + 1) * 10000 / speed));

            // Apply the animation via storyboard
            Storyboard.SetTargetName(animation, animatedEnemy.sprite.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.MarginProperty));
            Storyboard storyBoard = new Storyboard();
            storyBoard.Children.Add(animation);
            storyBoard.Begin(animatedEnemy.sprite);
        }

        /// <summary>
        /// Sorts the list of enemies using merge sort
        /// </summary>
        /// <param name="enemies">list of enemies</param>
        private static void Sort(SimpleList<Enemy> enemies)
        {
            if (enemies.Size <= 1)
                return;

            int middle = enemies.Size / 2;

            SimpleList<Enemy> firstHalf = new SimpleList<Enemy>();
            SimpleList<Enemy> secondHalf = new SimpleList<Enemy>();

            // Split the list in half
            int index = 0;
            while (enemies.Size > 0)
                if (index++ < middle)
                    firstHalf.Add(enemies.Pop);
                else
                    secondHalf.Add(enemies.Pop);

            // Sort each half
            Sort(firstHalf);
            Sort(secondHalf);

            // Fill the array in order
            enemies.Clear();
            while (firstHalf.Size > 0 || secondHalf.Size > 0)
                if (firstHalf.Size > 0 && secondHalf.Size > 0)
                    enemies.Add(firstHalf.Peek.CompareTo(secondHalf.Peek) >= 0 ? firstHalf.Pop : secondHalf.Pop);
                else
                    enemies.Add(firstHalf.Size == 0 ? secondHalf.Pop : firstHalf.Pop);
        }

        /// <summary>
        /// Returns to the level select menu
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void levelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Creates an indication canvas for a tower's range
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <returns>canvas</returns>
        private Canvas CreateRangeCanvas(int x, int y)
        {
            x *= 16;
            y *= 16;
            Canvas canvas = new Canvas();
            canvas.Background = new SolidColorBrush(Color.FromRgb((byte)0, (byte)0, (byte)0));
            canvas.Opacity = 0.5;
            canvas.Margin = new Thickness(x, y, 0, 0);
            canvas.HorizontalAlignment = HorizontalAlignment.Left;
            canvas.VerticalAlignment = VerticalAlignment.Top;
            canvas.Width = 16;
            canvas.Height = 16;
            return canvas;
        }

        /// <summary>
        /// Update speed when the slider is changed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void speedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speed = 25 * (int)Math.Pow(2, speedSlider.Value);
            speedLabel.Content = "Speed: " + (speed / 100.0) + (speed < 100 ? "x" : ".0x");
            if (timer != null)
                timer.Interval = new TimeSpan(0, 0, 0, 0, 10000 / speed);
        }
    }
}
