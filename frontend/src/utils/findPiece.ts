import type { Chess } from "chess.js";

export class FindPiece {

 static getKingSquare(chess: Chess, color: "w" | "b") {
  const board = chess.board();
  for (let rank = 0; rank < 8; rank++) {
    for (let file = 0; file < 8; file++) {
      const piece = board[rank][file];
      if (piece && piece.type === "k" && piece.color === color) {
        const fileChar = "abcdefgh"[file];
        const rankChar = 8 - rank;
        return `${fileChar}${rankChar}`;
      }
    }
  }
  throw new Error(`King not found for color: ${color}`);
}
}