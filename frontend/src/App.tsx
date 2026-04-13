import { Routes, Route } from 'react-router-dom'
import Home from './pages/Home'
import NotFound from './pages/NotFound'
import Navbar from './components/global/Navbar'
import { useState, useEffect, use, useRef } from 'react'
import { getMe } from './services/authService'
import UserProfilePage from './pages/UserProfilePage'
import Loading from './components/global/Loading'
import GamePage from './pages/GamePage'
import AuthPage from './pages/AuthPage'
import useUserStore from './stores/useUserStore'
import ToastContainer from './components/global/ToastContainer'
import FindGame from './pages/FindGame'
import {connectSocket, disconnectSocket} from './services/socket/socketService'
import SearchPage from './pages/SearchPage'
import {OnlyLoggedInRoute,OnlyAdminsRoute} from './components/global/ProtectedRoute'
import { useGlobalSocket } from './hooks/useGlobalSocket'
import { rejoinQueue } from './services/socket/socketGlobalService'
import SearchGamePage from './pages/SearchGamePage'
import { connectToBroker, disconnectFromBroker } from './services/socket/mqttService'
import useGlobalMQTT from './hooks/useGlobalMQTT'
import { connect } from 'http2'
import AddUserPage from './pages/UserActionPage'
import UserActionPage from './pages/UserActionPage'

function App() {
  const [loading, setLoading] = useState(true);
  const user = useUserStore((state) => state.user);
  const setUser = useUserStore((state) => state.setUser);
  const setMqttClient = useUserStore((state) => state.setMqttClient);
  useGlobalSocket();
  useGlobalMQTT();
  useEffect(() => {
    const checkAuth = async () => {
      try {
        const userData = await getMe();
        console.log("Authenticated user:", userData);
        setUser(userData);
        connectSocket();
        setMqttClient(connectToBroker());
        rejoinQueue();
        
      } catch (error) {
        setUser(null);
      }finally {
        setLoading(false);
        
      }
    };
    if(!user)checkAuth();
    else {
      setMqttClient(connectToBroker());
      connectSocket();
     setLoading(false);}
    
    return () => {
      disconnectSocket();
      disconnectFromBroker();
    }
  }, [user]);

  return (
    <>
   

    <Navbar/>
    {loading ? <Loading /> :
      <Routes>
        <Route path="/" element={<Home />} />
            <Route path="/login" element={<AuthPage initialisLogin={true} />} />
            <Route path="/register" element={<AuthPage initialisLogin={false} />} />
            <Route path="/users/:nickname/profile" element={<UserProfilePage />} />
           
            <Route element={<OnlyLoggedInRoute />}>
              <Route path="/find-game" element={<FindGame />} />
              <Route path="/search" element={<SearchPage />} />
               <Route path="/game/:gameId" element={<GamePage />} />
               <Route path ="/games" element={<SearchGamePage/>} />
            
            <Route element={<OnlyAdminsRoute />}>
               <Route path ="/users/create" element={<UserActionPage mode="add"/>} />
                <Route path ="/users/:nickname/edit" element={<UserActionPage mode="edit"/>} />
            </Route>
            </Route>
            <Route path="*" element={<NotFound whatIsMissing='page'/>} />
            
      </Routes>}
     

    <ToastContainer/>
    </>
  )
}

export default App
