using System;
using System.Collections.Generic;
using System.Linq;

namespace OthelloIA9
{
    public class OthelloBoard : IPlayable.IPlayable
    {
        public const int SIZE_GRID = 8;

        // matrice d'indication des poids de chaque cases
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

        // tableau contenant le score des deux joueurs 
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

        // sera définit au premier appel de GetNextMove
        private int me, ennemi;

        public OthelloBoard()
        {
            // Intialisation
            tiles = new int[SIZE_GRID, SIZE_GRID];

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

        #region IPlayable implementation

        /// <summary>
        /// Returns the IA's name
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return "09_Perez_Ramseyer";
        }

        /// <summary>
        /// Returns true if the move is valid for specified color
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            // si computeMove retourne null, c'est que le mouvement n'est pas jouable
            return computeMove(tiles, column, line, isWhite ? 0 : 1) == null;
        }

        /// <summary>
        /// Will update the board status if the move is valid and return true
        /// Will return false otherwise (board is unchanged)
        /// </summary>
        /// <param name="column">value between 0 and 7</param>
        /// <param name="line">value between 0 and 7</param>
        /// <param name="isWhite">true for white move, false for black move</param>
        /// <returns></returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            int currentPlayer = isWhite ? 0 : 1;
            // computeMove retourne une liste des cases retournées par ce coup
            List<Tuple<int, int>> catched = computeMove(tiles, column, line, currentPlayer);
            if (catched == null)
                return false;

            // pose une pièce sur la case jouée
            tiles[column, line] = currentPlayer;
            incrementScore(currentPlayer, 1);

            // retourne les cases ennemies capturées par ce mouvement
            foreach (Tuple<int, int> tile in catched)
            {
                tiles[tile.Item1, tile.Item2] = currentPlayer;
                // incrémente le score du joueur
                incrementScore(currentPlayer, 1);
                // décrémente le score de l'ennemi
                incrementScore(1 - currentPlayer, -1);
            }
            
            return true;
        }

        /// <summary>
        /// Asks the game engine next (valid) move given a game position
        /// The board assumes following standard move notation:
        /// 
        ///             A B C D E F G H
        ///             0 1 2 3 4 5 6 7     (first index)
        ///          0
        ///          1
        ///          2        X
        ///          3            X
        ///          4
        ///          5
        ///          6
        ///          7
        ///       
        ///          Column Line
        ///  E.g.:    D3, F4 game notation will map to {3,2} resp. {5,3}
        /// </summary>
        /// <param name="game">a 2D board with integer values: 0 for white 1 for black and -1 for empty tiles. First index for the column, second index for the line</param>
        /// <param name="level">an integer value to set the level of the IA, 5 normally</param>
        /// <param name="whiteTurn">true if white players turn, false otherwise</param>
        /// <returns>The column and line indices. Will return {-1,-1} as PASS if no possible move </returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            // Clone l'état du plateau de jeu pour éviter de travailler directement sur l'attribut de classe
            tiles = (int[,])game.Clone();
            // C'est GetNextMove qui définit quelle couleur l'IA doit jouer
            me = whiteTurn ? 0 : 1;
            ennemi = 1 - me;

            if (level == 0)
            {
                // si on appelle GetNextMove avec une profondeur de 0, renvoie un mouvement pioché aléatoirement dans la liste des coups possibles
                var possibleMoves = ComputePossibleMoves(game, me);
                if(possibleMoves.Count > 0)
                    return stringToTuple(possibleMoves.ElementAt(new Random().Next(0, possibleMoves.Count)).Key);
                return new Tuple<int, int>(-1, -1);
            }

            // Appelle l'algorithme récursif. Cette méthode retourne 2 valeurs, l'Item2 est le coup à jouer
            return minmax(tiles, level, me, int.MaxValue, 1).Item2;
        }

        /// <summary>
        /// Returns a reference to a 2D array with the board status
        /// </summary>
        /// <returns>The 8x8 tiles status</returns>
        public int[,] GetBoard()
        {
            return tiles;
        }

        /// <summary>
        /// Returns the number of white tiles on the board
        /// </summary>
        /// <returns></returns>
        public int GetWhiteScore()
        {
            return WhitesScore;
        }

        /// <summary>
        /// Returns the number of black tiles
        /// </summary>
        /// <returns></returns>
        public int GetBlackScore()
        {
            return BlacksScore;
        }
        #endregion

        #region othello algo

        /// <summary>
        /// Peuple et retourne un dictionnaire avec tous les mouvements possibles pour ce tour,
        /// pour le joueur donné. Chaque mouvement est associé à une liste de tuples qui indique
        /// les cases capturées si le mouvement est joué.
        /// On refait ce calcul à chaque tour.
        /// </summary>
        public Dictionary<string, List<Tuple<int, int>>> ComputePossibleMoves(int[,] board, int player)
        {
            /// Le dictionnaire à comme clé un mouvement sous forme de string
            /// (parce que string est hashable, contrairement au Tuple),
            /// et comme valeur une liste des cases capturées par ce mouvement
            var possibleMoves = new Dictionary<string, List<Tuple<int, int>>>();
            
            // itère sur chaque tuile et peuple le dictionnaire avec les coups possibles
            for (int column = 0; column < SIZE_GRID; column++)
            {
                for (int line = 0; line < SIZE_GRID; line++)
                {
                    List<Tuple<int, int>> catched = computeMove(board, column, line, player);
                    if (catched != null)
                        possibleMoves.Add(tupleToString(column, line), catched);
                }
            }

            return possibleMoves;
        }

        /// <summary>
        /// Si le coup n'est pas valide, retourn null,
        /// sinon, retourne une liste des tuiles capturées par ce coup
        /// Un tuile est jouable si :
        /// - l'une de ses voisines est occupée par un ennemi
        /// - l'autre extrémité est occupée par le joueur
        /// </summary>
        private List<Tuple<int, int>> computeMove(int[,] board, int column, int line, int player)
        {
            // Retourne si la tuile est déjà occupée
            if (board[column, line] != -1)
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
                    if (!outside && board[i, j] == ennemi)
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
                while (!outside && board[x, y] == ennemi)
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
                if (!outside && board[x, y] == player)
                    catchedTiles.AddRange(temp);
            }

            // si on n'a rien capturé dans la boucle foreach, le coup n'est pas valide
            if (catchedTiles.Count == 0)
                return null;

            // Le coup est valide, on retourne les tuiles capturées
            return catchedTiles;
        }
        
        // ajoute delta au score de ce joueur
        private void incrementScore(int player, int delta)
        {
            scores[player] += delta;
        }
        #endregion

        #region tools
        // Tuple(1, 2) --> "12"
        private string tupleToString(Tuple<int, int> tuple)
        {
            return tupleToString(tuple.Item1, tuple.Item2);
        }

        // 1, 2 --> "12"
        private string tupleToString(int x, int y)
        {
            return $"{x}{y}";
        }

        // "12" --> Tuple(1, 2)
        private Tuple<int, int> stringToTuple(string str)
        {
            int column = str[0] - '0';
            int line = str[1] - '0';
            return new Tuple<int, int>(column, line);
        }
        #endregion
        
        #region IA
        /// <summary>
        /// Algorithme récursif min-max avec élagage alpha-beta.
        /// Nous ne fonctionnons pas avec une structure de données arborescante pour l'arbre de décision,
        /// C'est la récursion qui forme l'arbre dans la méméoire.
        /// Une amélioration serait de calculer seulement deux étages de plus que le coup d'avant,
        /// en gardant en mémoire les possibilités de l'opposant et tous les enfants engendrés par chaque possibilité.
        /// Cela améliorerait grandement les performance de l'algorithme, mais ça nécessiterait une structure de données
        /// personnalisée et un algorithme itératif.
        /// Cela est compliqué à mettre en place dans le temps imparti.
        /// Nous avons donc fait le choix de recalculer l'arbre en entier à chaque fois.
        /// L'élagage alpha-beta est déjà un gain de temps non négligeable, l'arbre final est bien plus maigre.
        /// </summary>
        /// <param name="board">Tableau 2D d'entier représentant l'état du jeu sur lequel appliquer l'algorithme</param>
        /// <param name="depth">La profondeur qu'il reste à descendre dans l'arbre de décision</param>
        /// <param name="player">0 pour blanc, 1 pour noir, le joueur dont c'est le tour dans l'état actuel (commute à chaque niveau de profondeur)</param>
        /// <param name="parentValue">La plus grande/petite valeur dans le niveau supérieur (maximum dans un étage min, minumum dans un étage max)</param>
        /// <param name="minOrMax">1 pour un étage max (il faut maximiser), -1 pour un étage min</param>
        /// <returns>la valeur optimale (maximale si on maximise, minimale si on minimise) des enfants et l'opérateur (le mouvement) pour atteindre cette valeur optimale</returns>
        private Tuple<int, Tuple<int, int>> minmax(int[,] board, int depth, int player, int parentValue, int minOrMax)
        {
            // Si on atteint une feuille, on calcule et retourne le score de cet état de jeu
            if (depth == 0)
                // On est dans une feuille, il n'y a donc pas de mouvement enfant à retourner --> null
                return new Tuple<int, Tuple<int, int>>(score(board), null);
            // calcule les opérateurs (mouvements) applicables à cet état
            var possibleMoves = ComputePossibleMoves(board, player);
            if (possibleMoves.Count == 0)
                // Si il n'y a pas de mouvement possible, la valeur (-1, -1) doit être retournée (cf. doc de IPlayable)
                return new Tuple<int, Tuple<int, int>>(score(board), new Tuple<int, int>(-1, -1));

            // optVal contiendra la valeur max si on maximise, on l'initialise à la pire valeur pour s'assurer qu'elle change
            int optVal = -minOrMax * Int32.MaxValue;
            // L'opérateur (mouvement) optimal
            Tuple<int, int> optOp = null;
            // chaque mouvement possible va créer un état enfant
            foreach (var move in possibleMoves)
            {
                // on récupère le nouvel état
                int[,] newBoard = apply(board, move, player);
                // et on rappelle la fonciton récursive en diminuant la profondeur, changeant le joueur et le type de niveau (max ou min)
                int val = minmax(newBoard, depth - 1, 1 - player, optVal, -minOrMax).Item1;
                // si la valeur de cet enfant est meilleure que la précédente meilleure, on la sauvegarde
                if (val * minOrMax > optVal * minOrMax)
                {
                    optVal = val;
                    optOp = stringToTuple(move.Key); // la clé est une string, il faut juste la convertir en un mouvement
                    // Alpha-beta :
                    if (optVal * minOrMax > parentValue * minOrMax)
                        // on coupe les prochaines branches de l'arbre si la valeur est déjà meilleure que celle de l'étage parent
                        // c'est-à-dire on arrête la boucle, on ne prend pas en considération lesprochains mouvements frères
                        break;
                }
            }
            return new Tuple<int, Tuple<int, int>>(optVal, optOp);
        }
        
        /// <summary>
        /// Applique le mouvement donné à l'état donné et retourne un nouvel état
        /// </summary>
        /// <param name="board">L'état du plateau sur lequel appliquer l'opérateur</param>
        /// <param name="move">L'opérateur (le mouvement) : un élément de dictionnaire avec clé: mouvement en forme de string, valeur: les tuiles capturées par ce mouvement</param>
        /// <param name="player">0 ou 1, le joueur dont c'est le tour à ce moment</param>
        /// <returns>Un nouvel état du plateau de jeu</returns>
        private int[,] apply(int[,] board, KeyValuePair<string, List<Tuple<int, int>>> move, int player)
        {
            // Il faut cloner l'état, le tableau étant de type reference-type, on ne peut pas partager le même
            // état dans différents noeuds de l'arbre
            int[,] newBoard = (int[,]) board.Clone();

            // récupère le mouvement joué
            var tile = stringToTuple(move.Key);
            // capture cette case
            newBoard[tile.Item1, tile.Item2] = player;

            // affecte les cases ennemies capturées par ce mouvement au joueur
            foreach (Tuple<int, int> item in move.Value)
            {
                newBoard[item.Item1, item.Item2] = player;
            }
            return newBoard;
        }

        /// <summary>
        /// La fonction score qui évalue un état spécifique.
        /// Nous utilisons une matrice de poids trouvée sur internet pour l'évaluation.
        /// A chaque case est associée un poids, l'addition des poids des cases affectées au joueur
        /// définit son score. L'évaluation finale est la différence entre le score du joueur et celui
        /// de l'opposant.
        /// </summary>
        /// <param name="board">L'état qu'il faut évaluer</param>
        /// <returns>La note attribuée à cet état</returns>
        private int score(int[,] board)
        {
            // tableau contenant le score des blancs [0] et celui des noirs [1]
            int[] scores = { 0, 0 };
            // itère sur toutes les cases du plateau de jeu
            for (int y = 0; y < SIZE_GRID; y++)
            {
                for (int x = 0; x < SIZE_GRID; x++)
                {
                    // si la case n'est pas vide
                    if (board[x, y] != -1)
                        // Ajoute le poids de cette case au score du joueur qui la détient
                        scores[board[x, y]] += WEIGHT_MATRIX[x,y];
                }
            }
            // mon score moins le score de l'autre
            // on prend en considération les deux scores pour être plus juste dans l'évaluation
            // (un état est meilleur pour un joueur si il est pire pour son opposant) 
            return scores[me] - scores[1 - me];
        }

        #endregion
    }
}
