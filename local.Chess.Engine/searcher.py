import chess
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from chess_engine import Evaluator


class Searcher:
    name = "base"
    
    def choose_move(self, board: chess.Board, evaluator: 'Evaluator') -> chess.Move:
        raise NotImplementedError