
import chess

from searcher import Searcher

PIECE_VALUES = {
    chess.PAWN: 1, chess.KNIGHT: 3, chess.BISHOP: 3,
    chess.ROOK: 5, chess.QUEEN: 9, chess.KING: 0,
}
MATE_SCORE = 10.0

class AlphaBetaSearcher(Searcher):
    name = "alphabeta"

    def __init__(self, depth=4, qdepth=4):
        self.depth = depth
        self.qdepth = qdepth   

    def choose_move(self, board, evaluator):
        best_move = None
        best_value = -float("inf")
        alpha, beta = -float("inf"), float("inf")
        for move in self._ordered_moves(board):
            board.push(move)
            value = -self._negamax(board, evaluator, self.depth - 1, -beta, -alpha, 1)
            board.pop()
            if value > best_value:
                best_value = value
                best_move = move
            if value > alpha:
                alpha = value
        return best_move

    def _leaf_eval(self, board, evaluator):
        return evaluator.evaluate(board)

    def _terminal_score(self, board, ply):
        if board.is_checkmate():
            return -(MATE_SCORE - ply)  
        return 0.0                       

    def _negamax(self, board, evaluator, depth, alpha, beta, ply):
        if board.is_game_over(claim_draw=True):
            return self._terminal_score(board, ply)
        if depth == 0:
            return self._quiescence(board, evaluator, alpha, beta, self.qdepth)

        max_value = -float("inf")
        for move in self._ordered_moves(board):
            board.push(move)
            value = -self._negamax(board, evaluator, depth - 1, -beta, -alpha, ply + 1)
            board.pop()
            if value > max_value:
                max_value = value
            if value > alpha:
                alpha = value
            if alpha >= beta:
                break         
        return max_value

    def _quiescence(self, board, evaluator, alpha, beta, qdepth):
        if board.is_game_over(claim_draw=True):
            return self._terminal_score(board, 0)

        stand_pat = self._leaf_eval(board, evaluator)
        if stand_pat >= beta:
            return beta
        if stand_pat > alpha:
            alpha = stand_pat
        if qdepth == 0:
            return alpha

        for move in self._ordered_moves(board, captures_only=True):
            board.push(move)
            score = -self._quiescence(board, evaluator, -beta, -alpha, qdepth - 1)
            board.pop()
            if score >= beta:
                return beta
            if score > alpha:
                alpha = score
        return alpha

    def _ordered_moves(self, board, captures_only=False):
        scored = []
        for move in board.legal_moves:
            cap = board.is_capture(move)
            if captures_only and not cap:
                continue
            s = 0.0
            if cap:
                victim = board.piece_at(move.to_square)
                attacker = board.piece_at(move.from_square)
                victim_val = PIECE_VALUES.get(victim.piece_type, 1) if victim else 1
                attacker_val = PIECE_VALUES.get(attacker.piece_type, 0) if attacker else 0
                s += 10 * victim_val - attacker_val
            if move.promotion:
                s += PIECE_VALUES.get(move.promotion, 0)
            scored.append((-s, move)) 
        scored.sort(key=lambda x: x[0])
        return [m for _, m in scored]