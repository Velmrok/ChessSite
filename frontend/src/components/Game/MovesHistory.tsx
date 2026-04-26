import useGameStore from "@/stores/useGameStore";
import { Format } from "@/utils/format";

type Props = {
    moves: Array<MoveInfo>;
    time: number
}

export default function MovesHistory({ moves, time }: Props) {
    const currentMoveIndex = useGameStore((state) => state.currentMoveIndex);
    const isLive = useGameStore((state) => state.game?.status === "active");

    const setAbsoluteCurrentMoveIndex = useGameStore((state) => state.setAbsoluteCurrentMoveIndex);


    return (
        <div className={`w-full   flex flex-col ${!isLive ? "h-[75%]" : "h-[50%]"}`}>
            <div className="w-full h-[100%]  flex items-center justify-center  bg-black/30 rounded-md shadow-md ">
                <div className="w-full  h-full p-2 overflow-y-auto ">
                    <div>
                        {Format.groupMoves(moves)
                            .map((moves, index) => {
                                const proportion = (ms: number) => {

                                    const fraction = ms / (time * 60 * 1000);

                                    const scaled = Math.ceil(Math.pow(fraction, 1.3) * 150);
                                    return scaled < 2 ? 2 : scaled;
                                };
                                const formatTime = (ms: number) => {
                                    const value = ms / 10000;
                                    const rounded = Math.round(value * 100) / 100;
                                    const roundedMinutes = Math.round(value * 100 * 60) / 100;
                                    return rounded > 60 ? `${roundedMinutes}m` : `${rounded}s`;
                                }

                                return (

                                    <div key={index} className={`h-[35px]  text-white ${index % 2 === 0 ? "bg-white/5" : ""} px-2
                 text-sm grid grid-cols-6  items-center font-MyFancyFont`}>
                                        <div className="h-full flex items-center">{index + 1}.</div>
                                        <button onClick={() => setAbsoluteCurrentMoveIndex(index * 2)}
                                            className={`h-full flex hover:scale-105 items-center ${currentMoveIndex === index * 2 ? "text-green-500 " : ""}`}>{moves.whiteMove}</button>
                                        <button onClick={() => setAbsoluteCurrentMoveIndex(index * 2 + 1)}
                                            className={`h-full flex hover:scale-105 items-center ${currentMoveIndex === index * 2 + 1 ? "text-green-500 " : ""}`}>{moves.blackMove}</button>
                                        <div className="col-span-2 text-gray-400 text-sm flex flex-col items-end  w-full h-full">
                                            <div className="h-[50%] pt-1 pb-1 justify-end items-center max-w-full"
                                                style={{ width: moves.whiteDeltaTime ? `${proportion(moves.whiteDeltaTime)}%` : 2 }}>
                                                <div className={`bg-white rounded-[1px] h-[80%]`}></div>

                                            </div>
                                            <div className={`h-[50%] pt-1 pb-1 items-center max-w-full`}
                                                style={{ width: typeof moves.blackDeltaTime === "number" ? `${proportion(moves.blackDeltaTime)}%` : 2 }} >
                                                {typeof moves.blackDeltaTime === "number" && <div className={`bg-gray-500 rounded-[1px] h-[80%]`}></div>
                                                }
                                            </div>
                                        </div>
                                        <div className="">


                                            <div className={`w-full   flex justify-end ${currentMoveIndex === index * 2 ? "text-green-500 text-[12px]" : "text-[11px]"}`}>
                                                {moves.whiteDeltaTime == 0 ? `0.1s` : `${formatTime(moves.whiteDeltaTime)}`}
                                            </div>

                                            <div className={`flex w-full   justify-end ${currentMoveIndex === index * 2 + 1 ? "text-green-500 text-[12px]" : "text-[11px]"}`}>
                                                {typeof moves.blackDeltaTime === "number" && (moves.blackDeltaTime == 0 ? `0.1s` : `${formatTime(moves.blackDeltaTime)}`)}
                                            </div>

                                        </div>
                                    </div>
                                )
                            })}
                    </div>
                </div>

            </div>

        </div>
    );
}