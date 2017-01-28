# HotelOthello

## Fonctionnalités implemantés
*	Le plateau de jeu est représenté par une interface réalisée en XAML.
*	Les cases jouables sont clairement identifié en rose
*	Le score est affiché et synchronisé automatiquement (databinding)
*	les joueur joue chanqu'un leur tour sur le meme interface. 
*	Une partie peut être sauvegardée et rechargée à tout moment.
*	Les règles du jeu sont respectées et seuls des coups valides seront admis. Si aucun coup valide n'est possible pour le joueur dont c'est    le tour, il passe son tour. Si aucun joueur n'a de coup valide, la partie est terminée
*	Deux horloges affiche le temps de réflexion utilisé par chaque joueur
*	L'interface s'adaptera à des résolutions diverses (minimum 800x600)
* l'utilisateur à la possibilité de revenir en arrière

## Sauvgarde
la sauvgarde se fait avec JsonConvert qui permet de sérializer et déserializer en format json
