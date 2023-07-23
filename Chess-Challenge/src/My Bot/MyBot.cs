using System;
using ChessChallenge.API;

static class RandomExtensions
{
    public static void Shuffle<T>(this System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}

public class MyBot : IChessBot
{

    readonly int[] pieceRewards = {
        0, // NULL
        100, // pawn
        300, // knight
        300, // bishop
        500, // rook
        900, // queen
        1000, // king
    };

    int moveNr = 0;

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        (int score, int moveNr) result = MiniMax(board, moves, 3, true);
        if (result.moveNr < 0)
        {
            result.moveNr = 0;
        }

        // Console.WriteLine($"-----> [{moveNr++}] - score : {result.score}");

        return moves[result.moveNr];
    }

    (int score, int moveNr) MiniMax(Board board, Move[] moves, int depth, bool maximizing)
    {
        int topScore = (maximizing) ? -9999 : 9999;
        int topMove = -1;

        if (depth == 0)
        {
            topScore = EvaluateBoard(board, maximizing);
        }
        else
        {
            Random random = new();
            random.Shuffle(moves);

            for (int move = 0; move < moves.Length; move++)
            {
                board.MakeMove(moves[move]);
                Move[] newMoves = board.GetLegalMoves();
                (int score, int moveNr) res = MiniMax(board, newMoves, depth - 1, !maximizing);
                board.UndoMove(moves[move]);

                if ((maximizing && res.score > topScore) || (!maximizing && res.score < topScore))
                {
                    topScore = res.score;
                    topMove = move;
                }
            }

        }

        Console.WriteLine($"{new String(' ', depth)}+[{depth}]{new String(' ', 5 - depth)}{topScore}");
        return (topScore, topMove);
    }

    int EvaluateBoard(Board board, bool blackPlayed)
    {
        int score = 0;
        for (int type = 0; type < pieceRewards.Length; type++)
        {
            PieceList opPieceList = board.GetPieceList((PieceType)type, !blackPlayed);
            PieceList myPieceList = board.GetPieceList((PieceType)type, blackPlayed);
            int opLen = (opPieceList != null) ? opPieceList.Count : 0;
            int myLen = (myPieceList != null) ? myPieceList.Count : 0;
            score += pieceRewards[type] * (myLen - opLen);
        }
        return score;
    }
}