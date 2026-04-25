type GroupedMoveInfo ={
    whiteMove: string;
    blackMove?: string;
    whiteDeltaTime: number;
    blackDeltaTime?: number;
}
export class Format {
 
    static groupMoves(moves: Array<MoveInfo | string>): Array<GroupedMoveInfo> {
        return moves.reduce((acc: Array<GroupedMoveInfo>, move, index) => {
            const moveStr = typeof move === 'string' ? move : move.move;
            const deltaTime = typeof move === 'string' ? 0 : move.deltaTime;

            if (index % 2 === 0) {
                acc.push({ whiteMove: moveStr, whiteDeltaTime: deltaTime });
            } else {
                acc[acc.length - 1] = {
                    ...acc[acc.length - 1],
                    blackMove: moveStr,
                    blackDeltaTime: deltaTime
                };
            }
            return acc;
        }, []);
    }
}