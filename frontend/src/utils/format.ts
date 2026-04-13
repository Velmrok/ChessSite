type GroupedMoveInfo ={
    whiteMove: string;
    blackMove?: string;
    whiteDeltaTime: number;
    blackDeltaTime?: number;
}
export class Format{
     static groupMoves = (moves: Array<MoveInfo>) => {
        const grouped= moves.reduce((acc: Array<GroupedMoveInfo>, move: MoveInfo, index: number) => {
            if (index % 2 === 0) {
                acc.push(({ whiteMove: move.move, whiteDeltaTime: move.deltaTime}));
            } else {
                acc[acc.length - 1] = {
                    ...acc[acc.length - 1],
                    blackMove: move.move,
                    blackDeltaTime: move.deltaTime
                };
            }
            return acc;
        }, []);
        return grouped;
    };
}