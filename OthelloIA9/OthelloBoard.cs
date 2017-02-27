using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPlayable;

namespace OthelloIA9
{
    public class OthelloBoard : IPlayable.IPlayable
    {

        /*
         * matrice d'indication des poids de chaque cases
        500  -150 30 10 10 30 -150 500
        -150 -250 0  0  0  0  -250 -150
        30   0    1  2  2  1  0    30
        10   0    2  16 16 2  0    10
        10   0    2  16 16 2  0    10
        30   0    1  2  2  1  0    30
        -150 -250 0  0  0  0  -250 -150
        500  -150 30 10 10 30 -150 500
        */
        public static readonly int[,] WEIGHT_MATRIX = {
            {500, -150, 30, 10, 10, 30, -150, 500},
            {-150, -250, 0, 0, 0, 0, -250, -150},
            {30, 0, 1, 2, 2, 1, 0, 30},
            {10, 0, 2, 16, 16, 2, 0, 10},
            {10, 0, 2, 16, 16, 2, 0, 10},
            {30, 0, 1, 2, 2, 1, 0, 30},
            {-150, -250, 0, 0, 0, 0, -250, -150},
            {500, -150, 30, 10, 10, 30, -150, 500}
        };

        private int[] scores = { 2, 2 };
        
        public int BlacksScore
        {
            get { return scores[1]; }
            set { scores[1] = value; }
        }

        public int WhitesScore
        {
            get { return scores[0]; }
            set { scores[0] = value; }
        }

        // tableau 2d d'entiers représentant le plateau de jeu
        // -1 : case libre
        //  0 : blanc
        //  1 : noir
        private int[,] tiles;

        int me, ennemi;

        public const int SIZE_GRID = 8;

        // clé: string représentant un mouvement, exemple : "07"
        // valeur: liste des tuiles capturées si ce mouvement est effectué
        //private Dictionary<string, List<Tuple<int, int>>> possibleMoves;

        public OthelloBoard()
        {
            // Intialisation
            tiles = new int[SIZE_GRID, SIZE_GRID];
            //possibleMoves = new Dictionary<string, List<Tuple<int, int>>>();

            for (int i = 0; i < SIZE_GRID; i++)
            {
                for (int j = 0; j < SIZE_GRID; j++)
                {
                    tiles[i, j] = -1; // -1 : empty tile
                }
            }

            tiles[3, 4] = tiles[4, 3] = 1; // 1 : black
            tiles[3, 3] = tiles[4, 4] = 0; // 0 : white

            //ComputePossibleMoves(1);
        }
        
        #region IPlayable implementation
        public string GetName()
        {
            return "IA9 PerezRamseyer";
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            int currentPlayer = isWhite ? 0 : 1;
            List<Tuple<int, int>> catched = computeMove(column, line, currentPlayer);
            if (catched == null)
                return false;

            // pose une pièce sur la case jouée
            tiles[column, line] = currentPlayer;
            incrementScore(currentPlayer, 1);

            // retourne les cases ennemies capturées par ce mouvement
            foreach (Tuple<int, int> item in catched)
            {
                tiles[item.Item1, item.Item2] = currentPlayer;
                // incrémente le score du joueur
                incrementScore(currentPlayer, 1);
                // décrémente le score de l'ennemi
                incrementScore(1 - currentPlayer, -1);
            }

            // Calcul les coups possibles pour le prochain tour
            //ComputePossibleMoves(1 - currentPlayer);
            return true;
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            tiles = (int[,])game.Clone();
            me = whiteTurn ? 0 : 1;
            ennemi = 1 - me;

            var possibleMoves = ComputePossibleMoves(me);

            if (possibleMoves.Count() > 0)
                return minmax(tiles, level);

            string color = whiteTurn ? "whites" : "blacks";
            Console.WriteLine($"{color} can't move");
            // on ne peut pas jouer, on revoie (-1,-1)
            // on doit calculer les mouvements de l'ennemi parce que PlayMove ne sera pas appelée
            //ComputePossibleMoves(ennemi);
            return new Tuple<int, int>(-1, -1);
        }

        public int[,] GetBoard()
        {
            return tiles;
        }

        public int GetWhiteScore()
        {
            return WhitesScore;
        }

        public int GetBlackScore()
        {
            return BlacksScore;
        }

        private void incrementScore(int player, int delta)
        {
            if (player == 1)
                BlacksScore += delta;
            else if (player == 0)
                WhitesScore += delta;
        }
        #endregion
        
        #region othello algo

        /// <summary>
        /// Peuple le dictionnaire possibleMoves avec tous les mouvements possibles pour ce tour,
        /// pour le joueur actuel. Chaque mouvement est associé à une liste de tuples qui indique
        /// les cases capturées si le mouvement est joué.
        /// On refait ce calcul à chaque tour.
        /// </summary>
        public Dictionary<string, List<Tuple<int, int>>> ComputePossibleMoves(int player)
        {
            //possibleMoves.Clear();
            Dictionary<string, List<Tuple<int, int>>> possibleMoves = new Dictionary<string, List<Tuple<int, int>>>();

            for (int column = 0; column < SIZE_GRID; column++)
            {
                for (int line = 0; line < SIZE_GRID; line++)
                {
                    List<Tuple<int, int>> catched = computeMove(column, line, player);
                    if (catched != null)
                        possibleMoves.Add(tupleToString(column, line), catched);
                }
            }

            return possibleMoves;
        }

        /// <summary>
        /// Si le mouvement donné est possible, il est ajouté au dictionnaire possibleMoves.
        /// Un tuile est jouable si :
        /// - l'une de ses voisines est occupée par un ennemi
        /// - l'autre extrémité est occupée par le joueur
        /// </summary>
        private List<Tuple<int, int>> computeMove(int column, int line, int player)
        {
            // Retourne si la tuile est déjà occupée
            if (tiles[column, line] != -1)
                return null;

            int ennemi = 1 - player;

            // Liste contenant les coordonéées des cases voisines qui sont occupées par un ennemis 
            List<Tuple<int, int>> voisins = new List<Tuple<int, int>>();

            // Parcoure les tuiles autour de cette tuile, ajoute celles qui sont occupées par un ennemi
            // à la liste des voisins
            for (int i = column - 1; i <= column + 1; i++)
            {
                for (int j = line - 1; j <= line + 1; j++)
                {
                    bool outside = i < 0 || i >= SIZE_GRID || j < 0 || j >= SIZE_GRID;
                    if (!outside && tiles[i, j] == ennemi)
                        voisins.Add(new Tuple<int, int>(i, j));
                }
            }

            // aucune des cases voisines n'est occupée par l'ennemi, cette tuile n'est pas jouable
            if (voisins.Count == 0)
                return null;

            // Cette liste contiendra toutes les cases capturées par ce mouvement
            List<Tuple<int, int>> catchedTiles = new List<Tuple<int, int>>();

            // Pour chaque voisin ennemi
            foreach (Tuple<int, int> voisin in voisins)
            {
                int x = voisin.Item1;
                int y = voisin.Item2;
                // dx et dy indiquent la direction de la ligne sur laquelles les cases ennemies seront captuées 
                int dx = x - column;
                int dy = y - line;

                // liste des cases potentiellement capturées
                // (on ne sait pas encore si l'autre extrémité de la ligne est occupée par le joueur)
                List<Tuple<int, int>> temp = new List<Tuple<int, int>>();

                bool outside = false;
                // on suit la ligne et on ajoute les cases tant qu'on est sur un ennemi
                while (!outside && tiles[x, y] == ennemi)
                {
                    temp.Add(new Tuple<int, int>(x, y));
                    // on se déplace selon la direction de la ligne
                    x = x + dx;
                    y = y + dy;
                    // si hors de la grille de jeux --> n'est pas valide, rien à faire
                    outside = x < 0 || x >= SIZE_GRID || y < 0 || y >= SIZE_GRID;
                }
                // on sort de la boucle while lorsque la case x,y n'est pas un ennemi,
                // maintenant, soit c'est une case vide et on ne fait rien, soit c'est une case
                // du joueur et donc on capture toutes les cases mise en attente dans temp
                //if (tiles[x, y] == currentPlayer)
                if (!outside && tiles[x, y] == player)
                    catchedTiles.AddRange(temp);
            }

            // si on a rien capturé dans la boucle foreach, le coup n'est pas valide
            if (catchedTiles.Count == 0)
                return null;

            // Le coup est valide, on l'ajoute au dictionnaire
            return catchedTiles;
        }
        #endregion
        
        #region tools
        private string tupleToString(Tuple<int, int> tuple)
        {
            return tupleToString(tuple.Item1, tuple.Item2);
        }

        private string tupleToString(int x, int y)
        {
            return $"{x}{y}";
        }

        private Tuple<int, int> stringToTuple(string str)
        {
            int column = (int)(str[0] - '0');
            int line = (int)(str[1] - '0');
            return new Tuple<int, int>(column, line);
        }
        #endregion
        
        #region IA
        private Tuple<int, int> minmax(int[,] board, int depth)
        {
            if (depth == 0)
            {
                var possibleMoves = ComputePossibleMoves(me);
                return stringToTuple(possibleMoves.ElementAt(new Random().Next(0, possibleMoves.Count)).Key);
            }

            Tuple<int, int> op;
            minOrMax(board, depth, me, out op, 1);

            return op;
        }
        
        private int minOrMax(int[,] board, int depth, int player, out Tuple<int,int> op, int maximize)
        {
            var possibleMoves = ComputePossibleMoves(player);
            if (depth == 0 || possibleMoves.Count == 0)
            {
                op = new Tuple<int, int>(-1, -1);
                return maximize * score(board, player);
            }
            int maxVal = Int32.MinValue;
            Tuple<int, int> maxOp = null;
            foreach (var move in possibleMoves)
            {
                int[,] newBoard = apply(board, move, player);
                int val = minOrMax(newBoard, depth - 1, 1 - player, out op, -maximize);
                if (val > maxVal)
                {
                    maxVal = val;
                    maxOp = stringToTuple(move.Key);
                }
            }
            op = maxOp;
            return maxVal;
        }

        private int[,] apply(int[,] board, KeyValuePair<string, List<Tuple<int, int>>> move, int player)
        {
            int[,] newBoard = (int[,]) board.Clone();

            var tile = stringToTuple(move.Key);
            newBoard[tile.Item1, tile.Item2] = player;

            // retourne les cases ennemies capturées par ce mouvement
            foreach (Tuple<int, int> item in move.Value)
            {
                newBoard[item.Item1, item.Item2] = player;
            }
            return newBoard;
        }

        private int score(int[,] tiles, int player)
        {
            int[] scores = { 0, 0 };
            for (int y = 0; y < SIZE_GRID; y++)
            {
                for (int x = 0; x < SIZE_GRID; x++)
                {
                    if (tiles[x, y] != -1)
                        scores[tiles[x, y]]++;
                }
            }
            //return scores[player];
            // mon score moins le score de l'autre
            return scores[player] - scores[1 - player];
        }

        #endregion
    }
}
