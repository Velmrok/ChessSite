import { useEffect } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import useUserStore from '@/stores/useUserStore';
import { authService } from '@/services/authService';

export const OnlyLoggedInRoute = () => {
    const user = useUserStore((state) => state.user);


    useEffect(() => {
        if (!user) {
            authService.login();
        }
    }, [user]);

    if (!user) {
        return null;
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