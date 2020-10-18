using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameObjects;

/// <summary>
/// Junlin Yin n9905391
/// Huachen Liu n10073591
/// </summary>
namespace Games {
    /// <summary>
    /// Completely handle the game logic and store the game data
    /// </summary>
    public static class CrazyEights {
        /// <summary>
        /// Declare the following enum
        /// </summary>
        public enum ActionResult {
            /// <summary>
            /// A card was played that won the game
            /// </summary>
            WinningPlay,
            /// <summary>
            /// A valid card was played
            /// </summary>
            ValidPlay,
            /// <summary>
            /// A suit is required to continue play
            /// </summary>
            SuitRequired,
            /// <summary>
            /// Attempted to play an invalid card
            /// </summary>
            InvalidPlay,
            /// <summary>
            /// Attempted to play an invalid card when no cards can be played
            /// </summary>
            InvalidPlayAndMustDraw,
            /// <summary>
            /// A valid card was played, and the other player cannot play
            /// </summary>
            ValidPlayAndExtraTurn,
            /// <summary>
            /// Drew a playable card
            /// </summary>
            DrewPlayableCard,
            /// <summary>
            /// Drew an unplayable card
            /// </summary>
            DrewUnplayableCard,
            /// <summary>
            /// Drew an unplayable card and filled the hand
            /// </summary>
            DrewAndNoMovePossible,
            /// <summary>
            /// Drew an unplayable card and filled the hand, leaving both
            /// players unable to play, so piles were reset so that the
            /// the other player can continue play (rule 9)
            /// </summary>
            DrewAndResetPiles,
            /// <summary>
            /// Attempted to draw a card while moves were still possible
            /// </summary>
            CannotDraw,
            /// <summary>
            /// Flipped the discard pile to use as the new draw pile (rule 10)
            /// </summary>
            FlippedDeck
        }


        private static CardPile _drawPile;
        private static CardPile _discardPile;
        private static Hand _computerhand;
        private static Hand _userhand;
        private static bool _whosturn;
        private static bool _canplay;


        public static Card TopDiscard {
            get { return _discardPile.GetLastCardInPile(); }
        }
        public static bool IsDrawPileEmpty {
            get { return _drawPile.GetCount() == 0; }
        }
        public static Hand ComputerHand {
            get { return _computerhand; }
            private set { _computerhand = value; }
        }
        public static Hand UserHand {
            get { return _userhand; }
            private set { _userhand = value; }
        }
        public static bool IsUserTurn {
            get { return _whosturn; }
            private set { _whosturn = value; }
        }
        public static bool IsPlaying {
            get { return _canplay; }
            private set { _canplay = value; }
        }

        /// <summary>
        /// Sets up a game of Crazy Eights accordint to normal rules
        /// </summary>
        public static void StartGame() {
            _canplay = true;
            IsUserTurn = true;
            _drawPile = new CardPile(true);
            _drawPile.ShufflePile();
            _discardPile = new CardPile();
            UserHand = new Hand(_drawPile.DealCards(8));
            ComputerHand = new Hand(_drawPile.DealCards(8));
            _discardPile.AddCard(_drawPile.DealOneCard());
        }
        /// <summary>
        /// Sets up a game of Crazy Eights using the given hands and card piles.
        /// </summary>
        /// <param name="userHand">given user's hand </param>
        /// <param name="computerHand">given compute's hand</param>
        /// <param name="discardPile">given discardpile</param>
        /// <param name="drawPile">given drawpile</param>
        public static void StartGame(Hand userHand, Hand computerHand, CardPile discardPile, CardPile drawPile) {
            _canplay = true;
            IsUserTurn = true;
            UserHand = userHand;
            ComputerHand = computerHand;
            _discardPile = discardPile;
            _drawPile = drawPile;
        }

        /// <summary>
        /// Sorts the user's hand
        /// </summary>
        public static void SortUserHand() {
            _userhand.SortHand();

        }

        /// <summary>
        /// Judge if the hand has a playable card
        /// </summary>
        /// <param name="hand">computer's hand  or user's hand</param>
        /// <returns>return true if hand has playable card, false if not</returns>
        private static bool IsHandPlayable(Hand hand) {
            if (TopDiscard.GetFaceValue() == FaceValue.Eight && _discardPile.GetCount() == 1) {
                return true;
            }
            foreach (Card card in hand) {
                if (card.GetFaceValue() == TopDiscard.GetFaceValue()) {
                    return true;
                } else if (card.GetSuit() == TopDiscard.GetSuit()) {
                    return true;
                } else if (card.GetFaceValue() == FaceValue.Eight) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Judge if the drew card is playable
        /// </summary>
        /// <param name="card">drew card</param>
        /// <returns>return true if the drew card playable, false if not</returns>
        private static bool IsCardPlayable(Card card) {
            if (card.GetFaceValue() == TopDiscard.GetFaceValue()) {
                return true;
            } else if (card.GetSuit() == TopDiscard.GetSuit()) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Attempt to draw a card for the user
        /// </summary>
        /// <returns>actions according to the rules</returns>
        public static ActionResult UserDrawCard() {
            if (IsPlaying == false) {
                throw new System.ArgumentException("This game is not start!");
            }
            if (IsUserTurn == false) {
                throw new System.ArgumentException("This is not your turn!");
            }


            if (IsHandPlayable(UserHand) == true) {
                return ActionResult.CannotDraw;
            }


            if (_drawPile.GetCount() == 0) {
                _discardPile.Reverse();
                while (_discardPile.GetCount() > 1) {
                    _drawPile.AddCard(_discardPile.DealOneCard());
                }
                return ActionResult.FlippedDeck;
            }

            Card drawCard = _drawPile.DealOneCard();
            UserHand.AddCard(drawCard);

            if (IsCardPlayable(drawCard) == true) {
                return ActionResult.DrewPlayableCard;
            } else {
                if (UserHand.GetCount() == 13) {
                    IsUserTurn = false;
                    if (ComputerHand.GetCount() == 13) {
                        return ActionResult.DrewAndResetPiles;
                    } else {
                        return ActionResult.DrewAndNoMovePossible;
                    }
                } else {
                    return ActionResult.DrewUnplayableCard;
                }
            }


        }


        /// <summary>
        /// Attempt to play the chosen card for the user 
        /// </summary>
        /// <param name="cardNum">the card has been chosen</param>
        /// <param name="chosenSuit">change the suit for eight</param>
        /// <returns>action according to rules</returns>
        public static ActionResult UserPlayCard(int cardNum, Suit? chosenSuit = null) {
            if (IsPlaying == false) {
                throw new System.ArgumentException("This game is not start!");
            }
            if (IsUserTurn == false) {
                throw new System.ArgumentException("This is not your turn!");
            }

            Card card = _userhand.GetCard(cardNum);

            if (_discardPile.GetCount() == 1 && TopDiscard.GetFaceValue() == FaceValue.Eight) {
                IsUserTurn = false;
                _userhand.RemoveCardAt(cardNum);
                _discardPile.AddCard(card);
                return ActionResult.ValidPlay;
            }

            if (card.GetFaceValue() == FaceValue.Eight) {
                if (!chosenSuit.HasValue) {
                    return ActionResult.SuitRequired;
                }

                IsUserTurn = false;
                _userhand.RemoveCardAt(cardNum);
                _discardPile.AddCard(new Card(chosenSuit, FaceValue.Eight));
                return ActionResult.ValidPlay;
            }

            if (card.GetFaceValue() == TopDiscard.GetFaceValue() || card.GetSuit() == TopDiscard.GetSuit()) {
                _userhand.RemoveCardAt(cardNum);
                _discardPile.AddCard(card);

                if (UserHand.GetCount() == 0) {
                    IsPlaying = false;
                    return ActionResult.WinningPlay;
                }

                if (ComputerHand.GetCount() == 13) {
                    foreach (Card i in ComputerHand) {
                        if (i.GetFaceValue() == TopDiscard.GetFaceValue() || i.GetSuit() == TopDiscard.GetSuit()) {
                            IsUserTurn = false;
                            return ActionResult.ValidPlay;
                        }
                    }
                    return ActionResult.ValidPlayAndExtraTurn;
                }

                IsUserTurn = false;
                return ActionResult.ValidPlay;
            }

            // Invalid Play

            foreach (Card j in UserHand) {
                if (j.GetFaceValue() == TopDiscard.GetFaceValue() || j.GetSuit() == TopDiscard.GetSuit() || j.GetFaceValue() == FaceValue.Eight) {
                    return ActionResult.InvalidPlay;
                }
            }

            return ActionResult.InvalidPlayAndMustDraw;
        }


        /// <summary>
        /// Perform action according to the comuter's rules
        /// </summary>
        /// <returns>action results according to the rule</returns>
        public static ActionResult ComputerAction() {
            if (IsPlaying == false) {
                throw new System.ArgumentException("This game is not start!");
            }
            if (IsUserTurn == true) {
                throw new System.ArgumentException("This is not your turn!");
            }


            if (TopDiscard.GetFaceValue() == FaceValue.Eight && _discardPile.GetCount() == 1) {
                IsUserTurn = true;
                _discardPile.AddCard(ComputerHand.GetCard(0));
                ComputerHand.RemoveCardAt(0);
                return ActionResult.ValidPlay;
            }

            for (int j = 0; j < 3; ++j) // j = 0, match face, j = 1, match suit, j = 2, match eight
            {
                for (int i = ComputerHand.GetCount() - 1; i >= 0; --i) {
                    Card k = ComputerHand.GetCard(i);
                    if ((k.GetFaceValue() == TopDiscard.GetFaceValue() && j == 0) || (k.GetSuit() == TopDiscard.GetSuit() && j == 1) || (k.GetFaceValue() == FaceValue.Eight && j == 2)) {
                        IsUserTurn = false;
                        ComputerHand.RemoveCardAt(i);
                        _discardPile.AddCard(k);

                        if (ComputerHand.GetCount() == 0) {
                            IsPlaying = false;
                            return ActionResult.WinningPlay;
                        }

                        if (UserHand.GetCount() == 13) {
                            foreach (Card card in UserHand) {
                                if (card.GetFaceValue() == TopDiscard.GetFaceValue() || card.GetSuit() == TopDiscard.GetSuit()) {
                                    IsUserTurn = true;
                                    return ActionResult.ValidPlay;
                                }
                            }

                            IsUserTurn = false;
                            return ActionResult.ValidPlayAndExtraTurn;
                        }

                        IsUserTurn = true;
                        return ActionResult.ValidPlay;
                    }
                }
            }

            if (_drawPile.GetCount() == 0) {
                _discardPile.Reverse();
                while (_discardPile.GetCount() > 1) {
                    _drawPile.AddCard(_discardPile.DealOneCard());
                }
                return ActionResult.FlippedDeck;
            }

            Card drawCard = _drawPile.DealOneCard();
            ComputerHand.AddCard(drawCard);

            if (IsCardPlayable(drawCard) == true) {
                return ActionResult.DrewPlayableCard;
            } else {
                if (ComputerHand.GetCount() == 13) {
                    IsUserTurn = true;
                    if (UserHand.GetCount() == 13) {
                        return ActionResult.DrewAndResetPiles;
                    } else {
                        return ActionResult.DrewAndNoMovePossible;
                    }
                } else {
                    return ActionResult.DrewUnplayableCard;
                }
            }
        }
    }
}
