import { MdModeEditOutline } from "react-icons/md";
import { useNavigate } from "react-router-dom";

type Props = {
    nickname: string;
    className?: string;
}
export default function EditAccountButton({ nickname, className }: Props) {
    const navigate = useNavigate();
    const handleGoToEditPage = (nickname: string) => {
        navigate(`/users/${nickname}/edit`);
    }
    return <button className={`${className}
        text-white hover:scale-110 hover:text-yellow-500 transition-transform duration-300`}
        onClick={() => handleGoToEditPage(nickname)}>

        <MdModeEditOutline />
    </button>;
}