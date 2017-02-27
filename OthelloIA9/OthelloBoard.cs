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

        // tableau 2d d'entiers représentant le plateau de jeu
        // -1 : case libre
        //  0 : blanc
        //  1 : noir
        private int[,] tiles;

        int me, ennemi;

        public const int SIZE_GRID = 8;

        // clé: string représentant un mouvement, exemple : "07"
        // valeur: liste des tuiles capturées si ce mouvement est effectué
        private Dictionary<String, List<Tuple<int, int>>> possibleMoves;

        public OthelloBoard()
        {
            // Intialisation
            tiles = new int[SIZE_GRID, SIZE_GRID];
            possibleMoves = new Dictionary<String, List<Tuple<int, int>>>();

            for (int i = 0; i < SIZE_GRID; i++)
            {
                for (int j = 0; j < SIZE_GRID; j++)
                {
                    tiles[i, j] = -1; // -1 : empty tile
                }
            }

            tiles[3, 4] = tiles[4, 3] = 1; // 1 : black
            tiles[3, 3] = tiles[4, 4] = 0; // 0 : white
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public bool IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public bool PlayMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            tiles = game;
            me = whiteTurn ? 0 : 1;
            ennemi = 1 - me;

            ComputePossibleMoves();

            return stringToTuple(possibleMoves.First().Key);

        }

        public int[,] GetBoard()
        {
            return tiles;
        }

        public int GetWhiteScore()
        {
            throw new NotImplementedException();
        }

        public int GetBlackScore()
        {
            throw new NotImplementedException();
        }


        /*
         * OTHELLO ALGO
         */
        /// <summary>
        /// Peuple le dictionnaire possibleMoves avec tous les mouvements possibles pour ce tour,
        /// pour le joueur actuel. Chaque mouvement est associé à une liste de tuples qui indique
        /// les cases capturées si le mouvement est joué.
        /// On refait ce calcul à chaque tour.
        /// </summary>
        public void ComputePossibleMoves()
        {
            possibleMoves.Clear();

            for (int column = 0; column < SIZE_GRID; column++)
            {
                for (int line = 0; line < SIZE_GRID; line++)
                {
                    computeMove(column, line);
                }
            }
        }

        /// <summary>
        /// Si le mouvement donné est possible, il est ajouté au dictionnaire possibleMoves.
        /// Un tuile est jouable si :
        /// - l'une de ses voisines est occupée par un ennemi
        /// - l'autre extrémité est occupée par le joueur
        /// </summary>
        private void computeMove(int column, int line)
        {
            // Retourne si la tuile est déjà occupée
            if (tiles[column, line] != -1)
                return;

            //int ennemi = 1 - currentPlayer;

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
                return;

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
                try
                {
                    // on suit la ligne et on ajoute les cases tant qu'on est sur un ennemi
                    while (tiles[x, y] == ennemi)
                    {
                        temp.Add(new Tuple<int, int>(x, y));
                        // on se déplace selon la direction de la ligne
                        x = x + dx;
                        y = y + dy;
                    }
                    // on sort de la boucle while lorsque la case x,y n'est pas un ennemi,
                    // maintenant, soit c'est une case vide et on ne fait rien, soit c'est une case
                    // du joueur et donc on capture toutes les cases mise en attente dans temp
                    //if (tiles[x, y] == currentPlayer)
                    if (tiles[x, y] == me)
                        catchedTiles.AddRange(temp);
                }
                catch (Exception)
                {
                    // si hors de la grille de jeux --> n'est pas valide, rien à faire
                }
            }

            // si on a rien capturé dans la boucle foreach, le coup n'est pas valide
            if (catchedTiles.Count == 0)
                return;

            // Le coup est valide, on l'ajoute au dictionnaire
            possibleMoves.Add(tupleToString(column, line), catchedTiles);
            return;
        }


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
            int column = (int) (str[0] - '0');
            int line = (int) (str[1] - '0');
            return new Tuple<int, int>(column, line);
        }
    }
}
