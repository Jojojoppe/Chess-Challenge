using ChessChallenge.API;
using System.Diagnostics;

static class RandomExtensions
{
    public static void Shuffle<T> (this System.Random rng, T[] array)
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

    int[] pieceRewards = {
        0, // NULL
        100, // pawn
        300, // knight
        300, // bishop
        500, // rook
        900, // queen
        1000, // king
    };

    int I = 0;

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        System.Random random = new();
        Move topMove = moves[random.Next(moves.Length)];
        int topMoveScore = -9999;
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = MiniMax(board, timer, 2, false);
            // Check if mate
            if (board.IsInCheckmate())
            {
                score *= 1000; // Avoid/play always
            };
            Debug.WriteLine($"{I}.xx - score {score} : {move.ToString()}");
            // int score = evaluateBoard(board);
            board.UndoMove(move);
            if (score >= topMoveScore)
            {
                topMoveScore = score;
                topMove = move;
            }
        }
        Debug.WriteLine($"{I++} - Best move score {topMoveScore}");
        return topMove;
    }

    int MiniMax(Board board, Timer timer, int depth, bool isPlayer)
    {
        if (depth == 0)
        {
            return evaluateBoard(board);
        }

        Move[] moves = board.GetLegalMoves();
        System.Random random = new();
        random.Shuffle(moves);
        int topMoveScore = (isPlayer) ? -9999 : 9999;

        if (moves.Length == 0)
        {
            return topMoveScore;
        }

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = MiniMax(board, timer, depth - 1, !isPlayer);
            board.UndoMove(move);
            if ((score >= topMoveScore && isPlayer) || (score <= topMoveScore && !isPlayer))
            {
                topMoveScore = score;
            }
        }

        return topMoveScore;
    }

    int evaluateBoard(Board board)
    {
        int score = 0;
        for (int type = 0; type < pieceRewards.Length; type++)
        {
            PieceList opPieceList = board.GetPieceList((PieceType)type, board.IsWhiteToMove);
            PieceList myPieceList = board.GetPieceList((PieceType)type, !board.IsWhiteToMove);
            int opLen = (opPieceList != null) ? opPieceList.Count : 0;
            int myLen = (myPieceList != null) ? myPieceList.Count : 0;
            score += pieceRewards[type] * (myLen - opLen);
        }
        return score;
    }
}