import os
import time

import chess
import torch
from flask import Flask, request, jsonify

from chess_engine import load_evaluator
from alpha_beta_searcher import AlphaBetaSearcher
from chess_model_cnn import ChessCNN
from chess_model_chessresnet import ChessResNet

MODEL_PATH = os.environ.get("MODEL_PATH", "models/best_model.pth")
MAX_DEPTH = int(os.environ.get("MAX_DEPTH", "20"))   
MODEL_CLASS = ChessResNet

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
evaluator = load_evaluator(MODEL_PATH, MODEL_CLASS, device)

app = Flask(__name__)


def choose_move_timed(board, time_ms):
    deadline = time.monotonic() + time_ms / 1000.0
    best_move = None
    for depth in range(1, MAX_DEPTH + 1):
        move = AlphaBetaSearcher(depth=depth).choose_move(board, evaluator)
        if move is not None:
            best_move = move
        if time.monotonic() >= deadline:
            break
    return best_move


@app.route("/health")
def health():
    return jsonify({"status": "ok"})


@app.route("/move", methods=["POST"])
def move():
    data = request.get_json(force=True) or {}
    fen = data.get("fen")
    time_ms = int(data.get("timems", 1000))

    if not fen:
        return jsonify({"error": "missing fen"}), 400
    try:
        board = chess.Board(fen)
    except ValueError:
        return jsonify({"error": "invalid fen"}), 400

    if board.is_game_over(claim_draw=True):
        return jsonify({"san": None, "reason": "game_over"})

    mv = choose_move_timed(board, time_ms)
    return jsonify({"san": board.san(mv) if mv is not None else None})


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, threaded=False)
