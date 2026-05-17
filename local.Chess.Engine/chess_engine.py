import chess
import random
import torch
from data_loader import board_to_tensor
from alpha_beta_searcher import AlphaBetaSearcher
from searcher import Searcher

class Evaluator:
    def __init__(self, model, device):
        self.model = model
        self.device = device
        self.model.eval()

    @torch.no_grad()
    def evaluate(self, board: chess.Board) -> float:
        if board.is_checkmate():
            return -1.0
        if board.is_stalemate() or board.is_insufficient_material():
            return 0.0
        tensor = board_to_tensor(board).unsqueeze(0).to(self.device)
        return self.model(tensor).item()


def load_evaluator(checkpoint_path, model_class, device):
    model = model_class(in_channels=22).to(device)
    state = torch.load(checkpoint_path, map_location=device)

    uncompiled_state = {}
    for key, value in state.items():
        if key.startswith('_orig_mod.'):
            new_key = key[10:]
        else:
            new_key = key
        uncompiled_state[new_key] = value
    model.load_state_dict(uncompiled_state)
    return Evaluator(model, device)


SEARCHERS = {
    "alphabeta": AlphaBetaSearcher,
    # "mcts": MCTSSearcher, 
}


def make_searcher(name: str, **kwargs) -> Searcher:
    if name not in SEARCHERS:
        raise ValueError(f"Nieznany searcher: {name}. Dostepne: {list(SEARCHERS)}")
    return SEARCHERS[name](**kwargs)

class ChessEngine:
    def __init__(self, evaluator: Evaluator, searcher: Searcher, name="Engine"):
        self.evaluator = evaluator
        self.searcher = searcher
        self.name = name

    def choose_move(self, board: chess.Board) -> chess.Move:
        return self.searcher.choose_move(board, self.evaluator)

    def evaluate(self, board: chess.Board) -> float:
        return self.evaluator.evaluate(board)


def build_engine(checkpoint_path, model_class, searcher: Searcher, device, name=None):
    evaluator = load_evaluator(checkpoint_path, model_class, device)
    if name is None:
        base = checkpoint_path.replace("\\", "/").split("/")[-1].replace(".pth", "")
        name = f"{base}-{searcher.name}"
    return ChessEngine(evaluator, searcher, name=name)
