type Props = {
    avatarUrl?: string;
    className?: string;
}

export default function Avatar({ avatarUrl, className }: Props) {
    return (
        <img src={avatarUrl || "/default-avatar.webp"} alt="Player Avatar"
            className={className} >
        </img>
    )
}