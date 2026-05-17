import torch
import chess


PIECE_VALUE = {
    chess.PAWN: 1, chess.KNIGHT: 3, chess.BISHOP: 3,
    chess.ROOK: 5, chess.QUEEN: 9, chess.KING: 0,
}
def board_to_tensor(board: chess.Board):
    if board.turn == chess.BLACK:
        board = board.mirror()

    state = torch.zeros(22, 8, 8, dtype=torch.float32)

    for sq in chess.SQUARES:
        piece = board.piece_at(sq)
        if piece is not None:
            idx = piece.piece_type - 1
            offset = 0 if piece.color == chess.WHITE else 6
            ch = idx + offset
            r = chess.square_rank(sq)
            c = chess.square_file(sq)
            state[ch, r, c] = 1.0

    if board.has_kingside_castling_rights(chess.WHITE):
        state[12, :, :] = 1.0
    if board.has_queenside_castling_rights(chess.WHITE):
        state[13, :, :] = 1.0
    if board.has_kingside_castling_rights(chess.BLACK):
        state[14, :, :] = 1.0
    if board.has_queenside_castling_rights(chess.BLACK):
        state[15, :, :] = 1.0
    for sq in chess.SQUARES:
        r = chess.square_rank(sq)
        c = chess.square_file(sq)
        state[16, r, c] = len(board.attackers(chess.WHITE, sq))
        state[17, r, c] = len(board.attackers(chess.BLACK, sq))
    pm = board.piece_map().values()
    wm = sum(PIECE_VALUE[p.piece_type] for p in pm if p.color == chess.WHITE)
    bm = sum(PIECE_VALUE[p.piece_type] for p in pm if p.color == chess.BLACK)
    state[18, :, :] = (wm - bm) / 39.0
    state[19, :, :] = board.legal_moves.count() / 40.0
    state[20, :, :] = 1.0 if board.is_check() else 0.0
    state[21, :, :] = board.halfmove_clock / 100.0

    return state


