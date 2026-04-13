import { Navigate, Outlet } from 'react-router-dom';
import useUserStore from '@/stores/useUserStore';

export const OnlyLoggedInRoute = () => {
    const user = useUserStore((state) => state.user);
    
    
    if (!user) {
        
        return <Navigate to="/login" replace />;
    }

    
    return <Outlet />;
};

export const OnlyAdminsRoute = () => {
    const user = useUserStore((state) => state.user);
    if (!user || user.role !== 'admin') {
        return <Navigate to="/" replace />;
    }
    return <Outlet />;
}